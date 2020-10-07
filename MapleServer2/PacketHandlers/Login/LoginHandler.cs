﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using MaplePacketLib2.Tools;
using MapleServer2.Constants;
using MapleServer2.Data;
using MapleServer2.Data.Static;
using MapleServer2.Extensions;
using MapleServer2.Packets;
using MapleServer2.Servers.Login;
using MapleServer2.Types;
using Microsoft.Extensions.Logging;

namespace MapleServer2.PacketHandlers.Login {
    // ReSharper disable once ClassNeverInstantiated.Global
    public class LoginHandler : LoginPacketHandler {
        public override ushort OpCode => RecvOp.RESPONSE_LOGIN;

        private readonly IAccountStorage accountStorage;

        // TODO: This data needs to be dynamic
        private readonly ImmutableList<IPEndPoint> serverIps;
        private readonly string serverName;

        public LoginHandler(IAccountStorage accountStorage, ILogger<LoginHandler> logger) : base(logger) {
            this.accountStorage = accountStorage;

            var builder = ImmutableList.CreateBuilder<IPEndPoint>();
            builder.Add(new IPEndPoint(IPAddress.Loopback, LoginServer.PORT));

            this.serverIps = builder.ToImmutable();
            this.serverName = "Paperwood";
        }

        public override void Handle(LoginSession session, PacketReader packet) {
            byte mode = packet.ReadByte();
            string user = packet.ReadUnicodeString();
            string pass = packet.ReadUnicodeString();
            logger.Debug($"Logging in with user:{user} pass:{pass}");
            // TODO: From this user/pass lookup we should be able to find the accountId
            if (string.IsNullOrEmpty(user) && string.IsNullOrEmpty(pass)) {
                session.AccountId = StaticAccountStorage.DEFAULT_ACCOUNT;
            } else {
                session.AccountId = StaticAccountStorage.SECONDARY_ACCOUNT;
            }

            switch (mode) {
                case 1:
                    session.Send(PacketWriter.Of(SendOp.NPS_INFO).WriteLong().WriteUnicodeString(""));
                    session.Send(BannerListPacket.SetBanner());
                    session.Send(ServerListPacket.SetServers(serverName, serverIps));
                    break;
                case 2:
                    List<PlayerData> characters = new List<PlayerData>();
                    foreach (long characterId in accountStorage.ListCharacters(session.AccountId)) {
                        characters.Add(accountStorage.GetCharacter(characterId));
                    }

                    Console.WriteLine("Initializing loginc with " + session.AccountId);
                    session.Send(LoginResultPacket.InitLogin(session.AccountId));
                    session.Send(UgcPacket.SetEndpoint("http://127.0.0.1/ws.asmx?wsdl", "http://127.0.0.1"));
                    session.Send(CharacterListPacket.SetMax(2, 3));
                    session.Send(CharacterListPacket.StartList());
                    // Send each character data
                    session.Send(CharacterListPacket.AddEntries(characters));
                    session.Send(CharacterListPacket.EndList());
                    break;
            }
        }
    }
}
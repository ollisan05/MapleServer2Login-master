using System;
using System.Net;
using MaplePacketLib2.Tools;
using MapleServer2.Constants;
using MapleServer2.Data;
using MapleServer2.Enums;
using MapleServer2.Extensions;
using MapleServer2.Packets;
using MapleServer2.Servers.Game;
using MapleServer2.Servers.Login;
using MapleServer2.Types;
using Microsoft.Extensions.Logging;

namespace MapleServer2.PacketHandlers.Login {
    public class CharacterManagementHandler : LoginPacketHandler {
        public override ushort OpCode => RecvOp.CHARACTER_MANAGEMENT;

        public CharacterManagementHandler(ILogger<CharacterManagementHandler> logger) : base(logger) {
        }

        public override void Handle(LoginSession session, PacketReader packet) {
            byte mode = packet.ReadByte();
            switch (mode) {
                case 0: // Login
                    long charId = packet.ReadLong();
                    packet.ReadShort(); // 01 00
                    logger.Info($"Logging in to game with charId:{charId}");

                    var endpoint = new IPEndPoint(IPAddress.Loopback, GameServer.PORT);
                    var authData = new AuthData {
                        TokenA = session.GetToken(),
                        TokenB = session.GetToken(),
                        CharacterId = charId,
                    };
                    // Write AuthData to storage shared with GameServer
                    AuthStorage.SetData(session.AccountId, authData);

                    session.Send(MigrationPacket.LoginToGame(endpoint, authData));
                    //LoginPacket.LoginError("message?");
                    break;
                case 1: // Create
                    byte gender = packet.ReadByte();
                    packet.ReadShort(); // const?
                    string name = packet.ReadUnicodeString();
                    var skinColor = packet.Read<SSkinColor>();
                    packet.ReadShort(); // const?

                    logger.Info($"Creating character:{name}, gender:{gender}, skinColor:{skinColor}");
                    int equipCount = packet.ReadByte();
                    for (int i = 0; i < equipCount; i++) {
                        uint id = packet.ReadUInt();
                        string typeStr = packet.ReadUnicodeString();
                        if (!Enum.TryParse(typeStr, out EquipSlotType type)) {
                            throw new ArgumentException($"Unknown equip type: {typeStr}");
                        }
                        var equipColor = packet.Read<SEquipColor>();
                        int colorIndex = packet.ReadInt();
                        packet.Skip(4);

                        switch (type) {
                            case EquipSlotType.HR: // Hair
                                packet.Skip(28); // Hair Position
                                packet.Skip(28); // Hair Position
                                break;
                            case EquipSlotType.FD: // Face Decoration
                                packet.Skip(16);
                                break;
                            case EquipSlotType.CP:
                                packet.Skip(52);
                                break;
                        }
                        logger.Info($" > {type} - id:{id}, color:{equipColor}, colorIndex:{colorIndex}");
                    }

                    packet.ReadInt(); // const? (4)

                    // OnFailure, forcing failure here while debugging
                    session.Send(ResponseCharCreatePacket.NameTaken());

                    // OnSuccess
                    //SendOp.CHAR_MAX_COUNT
                    //session.Send(CharacterListPacket.SetMax(2, 3));
                    //SendOp.CHARACTER_LIST (New char only. This will append)
                    //session.Send(CharacterListPacket.AddEntry());
                    break;
                case 2: // Delete
                    long deleteCharId = packet.ReadLong();
                    logger.Info($"Deleting {deleteCharId}");
                    break;
                default:
                    throw new ArgumentException($"Invalid Char select mode {mode}");
            }
        }
    }
}
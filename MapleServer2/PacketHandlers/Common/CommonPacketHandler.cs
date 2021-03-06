﻿using MaplePacketLib2.Tools;
using MapleServer2.Network;
using MapleServer2.Servers.Game;
using MapleServer2.Servers.Login;
using Microsoft.Extensions.Logging;

namespace MapleServer2.PacketHandlers.Common {
    public abstract class CommonPacketHandler : IPacketHandler<LoginSession>, IPacketHandler<GameSession> {
        public abstract ushort OpCode { get; }

        protected readonly ILogger logger;

        protected CommonPacketHandler(ILogger<CommonPacketHandler> logger) {
            this.logger = logger;
        }

        public virtual void Handle(GameSession session, PacketReader packet) {
            HandleCommon(session, packet);
        }

        public virtual void Handle(LoginSession session, PacketReader packet) {
            HandleCommon(session, packet);
        }

        protected abstract void HandleCommon(Session session, PacketReader packet);

        public override string ToString() => $"[0x{OpCode:X4}] Common.{GetType().Name}";
    }
}
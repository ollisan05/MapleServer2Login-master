using MaplePacketLib2.Tools;
using MapleServer2.Constants;
using MapleServer2.Packets;
using MapleServer2.Servers.Game;
using MapleServer2.Types;
using Microsoft.Extensions.Logging;

namespace MapleServer2.PacketHandlers.Game {
    public class FieldEnterHandler : GamePacketHandler {
        public override ushort OpCode => RecvOp.RESPONSE_FIELD_ENTER;

        public FieldEnterHandler(ILogger<FieldEnterHandler> logger) : base(logger) { }

        public override void Handle(GameSession session, PacketReader packet) {
            packet.ReadInt(); // ?

            // Liftable: 00 00 00 00 00
            // SendBreakable
            // Self
            session.EnterField(session.Player.MapId);
            session.Send(EmotePacket.LoadEmotes());
        }
    }
}
using MaplePacketLib2.Tools;
using MapleServer2.Constants;
using MapleServer2.Types;

namespace MapleServer2.Packets {
    public static class ChatPacket {
        public static Packet All(PlayerData player, string message) {
            return PacketWriter.Of(SendOp.USER_CHAT)
                .WriteLong(player.AccountId)
                .WriteLong(player.CharacterId)
                .WriteUnicodeString(player.Name)
                .WriteByte()
                .WriteUnicodeString(message)
                .WriteInt()
                .WriteByte()
                .WriteInt(1)
                .WriteByte();
        }
    }
}
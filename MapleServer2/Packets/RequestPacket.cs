using System;
using MaplePacketLib2.Tools;
using MapleServer2.Constants;

namespace MapleServer2.Packets {
    public static class RequestPacket {
        public static Packet Login() {
            return PacketWriter.Of(SendOp.REQUEST_LOGIN);
        }

        public static Packet Key() {
            return PacketWriter.Of(SendOp.REQUEST_KEY);
        }

        public static Packet Heartbeat() {
            return PacketWriter.Of(SendOp.REQUEST_HEARTBEAT)
                .WriteInt(Environment.TickCount);
        }

        public static Packet TickSync() {
            return PacketWriter.Of(SendOp.REQUEST_CLIENTTICK_SYNC)
                .WriteInt(Environment.TickCount);
        }
    }
}
using System;
using MaplePacketLib2.Tools;
using MapleServer2.Constants;
using MapleServer2.Extensions;
using MapleServer2.Network;
using Microsoft.Extensions.Logging;

namespace MapleServer2.PacketHandlers.Common {
    // Note: socket_exception debug offset includes +6 bytes from encrypted header
    public class SendLogHandler : CommonPacketHandler {
        public override ushort OpCode => RecvOp.LOG_SEND;

        public SendLogHandler(ILogger<SendLogHandler> logger) : base(logger) { }

        protected override void HandleCommon(Session session, PacketReader packet) {
            packet.Skip(2);
            try {
                while (packet.Available > 2) {
                    string message = packet.ReadUnicodeString();
                    if (message.Contains("exception")) {
                        // Read remaining string
                        string debug = packet.ReadUnicodeString(packet.Available / 2);
                        logger.Error($"[{message}] {debug}");

                        session.OnError?.Invoke(session, debug);
                    } else {
                        //logger.Debug(message);
                    }
                }
            } catch (Exception ex) {
                logger.Error($"Error parsing DEBUG_MSG packet:{packet}", ex);
            }
        }
    }
}
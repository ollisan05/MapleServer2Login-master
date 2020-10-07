using System;
using MaplePacketLib2.Tools;
using MapleServer2.Constants;
using MapleServer2.Enums;
using MapleServer2.Packets;
using MapleServer2.Servers.Game;
using MapleServer2.Types;
using Microsoft.Extensions.Logging;

// User Chat
// >00 00 00 00 05 00 48 00 65 00 6C 00 6C 00 6F 00 00 00 00 00 00 00 00 00 00 00
// <B2 79 72 04 3E 95 3C 26 EE 1A AD 2C 56 95 3B 26 08 00 42 00 6C 00 75 00 65 00 52 00 6F 00 70 00 65 00 00 05 00 48 00 65 00 6C 00 6C 00 6F 00 00 00 00 00 00 02 00 00 00 00
// Whisper
// >04 00 00 00 05 00 48 00 65 00 6C 00 6C 00 6F 00 0B 00 43 00 61 00 6E 00 74 00 42 00 65 00 6C 00 69 00 65 00 76 00 65 00 00 00 00 00 00 00 00 00
// <00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0B 00 43 00 61 00 6E 00 74 00 42 00 65 00 6C 00 69 00 65 00 76 00 65 00 00 05 00 48 00 65 00 6C 00 6C 00 6F 00 04 00 00 00 00 02 00 00 00 00
// Party
// >07 00 00 00 05 00 48 00 65 00 6C 00 6C 00 6F 00 00 00 00 00 00 00 00 00 00 00
// <B2 79 72 04 3E 95 3C 26 EE 1A AD 2C 56 95 3B 26 08 00 42 00 6C 00 75 00 65 00 52 00 6F 00 70 00 65 00 00 05 00 48 00 65 00 6C 00 6C 00 6F 00 07 00 00 00 00 02 00 00 00 00
// Guild
// >08 00 00 00 05 00 48 00 65 00 6C 00 6C 00 6F 00 00 00 00 00 00 00 00 00 00 00
// <B2 79 72 04 3E 95 3C 26 EE 1A AD 2C 56 95 3B 26 08 00 42 00 6C 00 75 00 65 00 52 00 6F 00 70 00 65 00 00 05 00 48 00 65 00 6C 00 6C 00 6F 00 08 00 00 00 00 02 00 00 00 00
// Channel
// >0C 00 00 00 05 00 48 00 65 00 6C 00 6C 00 6F 00 00 00 00 00 00 00 00 00 00 00
// <B2 79 72 04 3E 95 3C 26 EE 1A AD 2C 56 95 3B 26 08 00 42 00 6C 00 75 00 65 00 52 00 6F 00 70 00 65 00 00 05 00 48 00 65 00 6C 00 6C 00 6F 00 0C 00 00 00 00 02 00 00 00 00
// Channel
// >0B 00 00 00 05 00 48 00 65 00 6C 00 6C 00 6F 00 00 00 00 00 00 00 00 00 00 00
// <B2 79 72 04 3E 95 3C 26 EE 1A AD 2C 56 95 3B 26 08 00 42 00 6C 00 75 00 65 00 52 00 6F 00 70 00 65 00 00 05 00 48 00 65 00 6C 00 6C 00 6F 00 0C 00 00 00 00 02 00 00 00 00
namespace MapleServer2.PacketHandlers.Game {
    public class UserChatHandler : GamePacketHandler {
        public override ushort OpCode => RecvOp.USER_CHAT;

        public UserChatHandler(ILogger<GamePacketHandler> logger) : base(logger) { }

        public override void Handle(GameSession session, PacketReader packet) {
            int function = packet.ReadInt();
            string message = packet.ReadUnicodeString();
            string recipient = packet.ReadUnicodeString();
            packet.ReadLong();

            Console.WriteLine($"[{function}] {message}");
            string[] splits = message.Split(" ");
            if (splits[0] == "add") {
                long uid = Environment.TickCount64;
                int flag = 0;
                if (splits.Length > 1) {
                    int.TryParse(splits[1], out flag);
                }

                var item = new Item {
                    Id = 20000027,
                    Uid = uid,
                    Rarity = 5,
                    CreationTime = 1584567649,
                    Amount = 100,
                    Stats = new ItemStatData {
                        Enchants = 10,
                        TotalSockets = 3,
                    },
                    TransferFlag = (TransferFlag) flag,
                };

                // Simulate looting item
                if (session.Inventory.Add(item)) {
                    session.Send(ItemInventoryPacket.Add(item));
                    session.Send(ItemInventoryPacket.AddItem(item));
                }
            }

            if (function == 0) {
                session.FieldManager.SendChat(session.Player, message);
            }
        }
    }
}
/*
packet.ReadLong(); // accountid
packet.ReadLong(); // characterid
string name = packet.ReadUnicodeString();
packet.ReadByte();
string message = packet.ReadUnicodeString();
packet.Skip(8); // Unknown
Console.WriteLine($"[CH] {name}: {message}");
 */
// Party invite
// 01 09 00 42 00 75 00 62 00 62 00 6C 00 65 00 47 00 75 00 6E 00
using System;
using MaplePacketLib2.Tools;
using MapleServer2.Constants;
using MapleServer2.Enums;
using MapleServer2.Packets;
using MapleServer2.Servers.Game;
using MapleServer2.Types;
using Microsoft.Extensions.Logging;

namespace MapleServer2.PacketHandlers.Game {
    public class RequestItemInventoryHandler : GamePacketHandler {
        public override ushort OpCode => RecvOp.REQUEST_ITEM_INVENTORY;

        public RequestItemInventoryHandler(ILogger<GamePacketHandler> logger) : base(logger) { }

        public override void Handle(GameSession session, PacketReader packet) {
            byte function = packet.ReadByte();

            Inventory inventory = session.Inventory;
            switch (function) {
                case 0: {
                    break;
                }
                case 3: { // Move
                    long uid = packet.ReadLong();
                    short dstSlot = packet.ReadShort();
                    Tuple<long, short> result = inventory.Move(uid, dstSlot);
                    if (result == null) {
                        break;
                    }

                    // send updated move
                    session.Send(ItemInventoryPacket.Move(result.Item1, result.Item2, uid, dstSlot));
                    break;
                }
                case 4: { // Drop
                    // TODO: Make sure items are tradable?
                    long uid = packet.ReadLong();
                    int amount = packet.ReadInt();
                    int remaining = inventory.Drop(uid, out Item droppedItem, amount);
                    if (remaining < 0) {
                        break; // Drop failed
                    }

                    session.Send(remaining > 0
                        ? ItemInventoryPacket.Update(uid, remaining)
                        : ItemInventoryPacket.DropAll(uid));
                    session.FieldManager.AddItem(session, droppedItem);
                    break;
                }
                case 5: { // Drop Bound
                    long uid = packet.ReadLong();
                    if (inventory.Drop(uid, out Item droppedItem) != 0) {
                        break; // Drop failed
                    }

                    session.Send(ItemInventoryPacket.DropAll(uid));
                    // Allow dropping bound items for now
                    session.FieldManager.AddItem(session, droppedItem);
                    break;
                }
                case 10: { // Sort
                    var tab = (InventoryTab) packet.ReadShort();
                    inventory.Sort(tab);
                    session.Send(ItemInventoryPacket.ResetTab(tab));
                    session.Send(ItemInventoryPacket.LoadItemsToTab(tab, inventory.GetItems(tab)));
                    break;
                }
            }
        }
    }
}
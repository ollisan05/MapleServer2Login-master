using System.Collections.Generic;
using MaplePacketLib2.Tools;
using MapleServer2.Enums;
using MapleServer2.Types;

namespace MapleServer2.Packets.Helpers {
    public static class ItemPacketHelper {
        public static PacketWriter WriteItem(this PacketWriter pWriter, Item item) {
            pWriter.WriteInt(item.Amount)
                .WriteInt()
                .WriteInt(-1)
                .WriteLong(item.CreationTime)
                .WriteLong(item.ExpiryTime)
                .WriteLong()
                .WriteInt(item.TimesAttributesChanged)
                .WriteInt()
                .WriteBool(item.IsLocked)
                .WriteLong(item.UnlockTime)
                .WriteShort(item.RemainingGlamorForges)
                .WriteByte()
                .WriteInt()
                .WriteAppearance(item)
                .WriteStats(item.Stats)
                .WriteInt()
                .WriteByte()
                .WriteLong()
                .WriteInt()
                .WriteInt()
                .WriteByte(1)
                .WriteInt()
                .WriteStatDiff(item.Stats, item.Stats)
                .WriteInt((int)item.TransferFlag)
                .WriteByte()
                .WriteInt()
                .WriteInt()
                .WriteByte()
                .WriteByte(1);


            bool flag = false;
            pWriter.WriteBool(flag);
            if (flag) {
                pWriter.WriteLong(); // owner char Id?
                pWriter.WriteUnicodeString(item.OwnerName); // ownerName
            }

            pWriter.WriteByte();

            pWriter.WriteSockets(item.Stats);

            pWriter.WriteLong(item.PairedCharacterId);
            if (item.PairedCharacterId != 0) {
                pWriter.WriteUnicodeString(item.PairedCharacterName);
            }

            // Bound to character
            return pWriter.WriteLong()
                .WriteUnicodeString("");
        }

        private static PacketWriter WriteAppearance(this PacketWriter pWriter, Item item) {
            pWriter.Write<SEquipColor>(item.Color);
            pWriter.WriteInt(item.ColorIndex); // ColorIndex
            pWriter.WriteInt(item.AppearanceFlag);
            // Positioning Data
            switch (item.EquipSlotType) {
                case EquipSlotType.CP:
                    for (int i = 0; i < 13; i++) {
                        pWriter.Write<float>(0);
                    }
                    break;
                case EquipSlotType.HR:
                    pWriter.Write<float>(0.3f);
                    pWriter.WriteZero(24);
                    pWriter.Write<float>(0.3f);
                    pWriter.WriteZero(24);
                    break;
                case EquipSlotType.FD:
                    for (int i = 0; i < 4; i++) {
                        pWriter.Write<float>(0);
                    }
                    break;
            }

            return pWriter.WriteByte();
        }

        // 9 Blocks of stats, Only handling Basic and Bonus attributes for now
        private static PacketWriter WriteStats(this PacketWriter pWriter, ItemStatData stats) {
            List<StatData> basicAttributes = stats.BasicAttributes;
            pWriter.WriteShort((short)basicAttributes.Count);
            foreach (StatData stat in basicAttributes) {
                pWriter.Write<StatData>(stat);
            }
            pWriter.WriteShort().WriteInt();

            // Another basic attributes block
            pWriter.WriteShort().WriteShort().WriteInt();

            List<BonusStatData> bonusAttributes = stats.BonusAttributes;
            pWriter.WriteShort((short)bonusAttributes.Count);
            foreach (BonusStatData stat in bonusAttributes) {
                pWriter.Write<BonusStatData>(stat);
            }
            pWriter.WriteShort().WriteInt();

            // Ignore other attributes
            pWriter.WriteShort().WriteShort().WriteInt();
            pWriter.WriteShort().WriteShort().WriteInt();
            pWriter.WriteShort().WriteShort().WriteInt();
            pWriter.WriteShort().WriteShort().WriteInt();
            pWriter.WriteShort().WriteShort().WriteInt();
            pWriter.WriteShort().WriteShort().WriteInt();

            return pWriter.WriteInt(stats.Enchants);
        }

        private static PacketWriter WriteStatDiff(this PacketWriter pWriter, ItemStatData old, ItemStatData @new) {
            // TODO: Find stat diffs (low priority)
            List<StatData> generalStatDiff = new List<StatData>();
            pWriter.WriteByte((byte)generalStatDiff.Count);
            foreach (StatData stat in generalStatDiff) {
                pWriter.Write<StatData>(stat);
            }

            pWriter.WriteInt(); // ???

            List<StatData> statDiff = new List<StatData>();
            pWriter.WriteInt(statDiff.Count);
            foreach (StatData stat in statDiff) {
                pWriter.Write<StatData>(stat);
            }

            List<BonusStatData> bonusStatDiff = new List<BonusStatData>();
            pWriter.WriteInt(bonusStatDiff.Count);
            foreach (BonusStatData stat in bonusStatDiff) {
                pWriter.Write<BonusStatData>(stat);
            }

            return pWriter;
        }

        private static PacketWriter WriteSockets(this PacketWriter pWriter, ItemStatData stats) {
            pWriter.WriteByte(stats.TotalSockets);
            for (int i = 0; i < stats.TotalSockets; i++) {
                if (i >= stats.Gemstones.Count) {
                    pWriter.WriteBool(false); // Locked
                    continue;
                }

                pWriter.WriteBool(true); // Unlocked
                GemstoneData gem = stats.Gemstones[i];
                pWriter.WriteInt(gem.Id);
                pWriter.WriteBool(gem.OwnerId != 0);
                if (gem.OwnerId != 0) {
                    pWriter.WriteLong(gem.OwnerId)
                        .WriteUnicodeString(gem.OwnerName);
                }

                pWriter.WriteBool(gem.Unknown != 0);
                if (gem.Unknown != 0) {
                    pWriter.WriteByte()
                        .WriteLong(gem.Unknown);
                }
            }

            return pWriter;
        }
    }
}
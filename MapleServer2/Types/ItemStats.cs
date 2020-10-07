using System.Collections.Generic;
using MapleServer2.Enums;

namespace MapleServer2.Types {
    public struct StatData {
        public Attribute Type { get; private set; }
        public int Value { get; private set; }
        public float Unknown { get; private set; }

        public static StatData Of(Attribute type, int value) {
            return new StatData {
                Type = type,
                Value = value,
                Unknown = 0,
            };
        }
    }

    public struct BonusStatData {
        public BonusAttribute Type { get; private set; }
        public float Value { get; private set; }
        public float Unknown { get; private set; }

        public static BonusStatData Of(BonusAttribute type, float value) {
            return new BonusStatData {
                Type = type,
                Value = value,
                Unknown = 0,
            };
        }
    }

    public class GemstoneData {
        public readonly int Id;

        // Used if bound
        public readonly long OwnerId = 0;
        public readonly string OwnerName = "";

        public readonly long Unknown = 0;

        public GemstoneData(int id) {
            this.Id = id;
        }
    }

    public class ItemStatData {
        public int Enchants;

        public readonly List<StatData> BasicAttributes;
        public readonly List<BonusStatData> BonusAttributes;

        public byte TotalSockets;
        public readonly List<GemstoneData> Gemstones;

        public ItemStatData() {
            this.BasicAttributes = new List<StatData>();
            this.BonusAttributes = new List<BonusStatData>();
            this.Gemstones = new List<GemstoneData>();
        }

        public ItemStatData(ItemStatData other) {
            Enchants = other.Enchants;
            BasicAttributes = new List<StatData>(other.BasicAttributes);
            BonusAttributes = new List<BonusStatData>(other.BonusAttributes);
            TotalSockets = other.TotalSockets;
            Gemstones = new List<GemstoneData>(other.Gemstones);
        }
    }
}
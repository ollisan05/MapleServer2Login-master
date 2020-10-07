using System;
using MapleServer2.Enums;

namespace MapleServer2.Types {
    public class Item {
        public InventoryTab InventoryType = InventoryTab.Consumables;

        public int Id;
        public long Uid;
        public short Slot;
        public int Amount;
        public int Rarity;
        public EquipSlotType EquipSlotType;

        public long CreationTime;
        public long ExpiryTime;

        public int TimesAttributesChanged;
        public bool IsLocked;
        public long UnlockTime;
        public short RemainingGlamorForges;
        public int Bound;
        public TransferFlag TransferFlag;
        public int RemainingTrades;

        // For friendship badges
        public long PairedCharacterId;
        public string PairedCharacterName;

        public string OwnerName;

        public SEquipColor Color;
        public int ColorIndex;
        public byte AppearanceFlag;

        public ItemStatData Stats;

        public Item() {
            Slot = -1;
            Amount = 1;
            ColorIndex = -1;
        }

        public Item(Item other) {
            Id = other.Id;
            Uid = Environment.TickCount64; // TODO: proper uid generation
            Slot = other.Slot;
            Amount = other.Amount;
            Rarity = other.Rarity;
            EquipSlotType = other.EquipSlotType;
            CreationTime = other.CreationTime;
            ExpiryTime = other.ExpiryTime;
            TimesAttributesChanged = other.TimesAttributesChanged;
            IsLocked = other.IsLocked;
            UnlockTime = other.UnlockTime;
            RemainingGlamorForges = other.RemainingGlamorForges;
            Bound = other.Bound;
            RemainingTrades = other.RemainingTrades;
            PairedCharacterId = other.PairedCharacterId;
            PairedCharacterName = other.PairedCharacterName;
            OwnerName = other.OwnerName;
            Color = other.Color;
            Stats = new ItemStatData(other.Stats);
        }

        public static Item Ear() {
            return new Item {
                Id = 10500002,
                Uid = 2754959794416496488,
                EquipSlotType = EquipSlotType.ER,
                CreationTime = 1558494660,
                Color = new SEquipColor(),
                Stats = new ItemStatData(),
            };
        }

        public static Item Hair() {
            return new Item {
                Id = 10200012,
                Uid = 2867972925711604442,
                EquipSlotType = EquipSlotType.HR,
                CreationTime = 1565575851,
                Color = new SEquipColor {
                    Primary = SColor.Argb(0xFF, 0x7E, 0xCC, 0xF7),
                    Secondary = SColor.Argb(0xFF, 0x4C, 0x85, 0xDB),
                    Tertiary = SColor.Argb(0xFF, 0x48, 0x5E, 0xA8)
                },
                ColorIndex = 15,
                AppearanceFlag = 2,
                Stats = new ItemStatData(),
            };
        }

        public static Item Face() {
            return new Item {
                Id = 10300005,
                Uid = 2754959794416496483,
                EquipSlotType = EquipSlotType.FA,
                CreationTime = 1558494660,
                Color = new SEquipColor {
                    Primary = SColor.Argb(0xFF, 0xB5, 0x24, 0x29),
                    Secondary = SColor.Argb(0xFF, 0xF7, 0xE3, 0xE3),
                    Tertiary = SColor.Argb(0xFF, 0x14, 0x07, 0x02)
                },
                ColorIndex = 0,
                AppearanceFlag = 3,
                Stats = new ItemStatData(),
            };
        }

        public static Item FaceDecoration() {
            return new Item {
                Id = 10400001,
                Uid = 2754959794416496484,
                EquipSlotType = EquipSlotType.FD,
                CreationTime = 1558494660,
                Color = new SEquipColor(),
                Stats = new ItemStatData(),
            };
        }

        public bool TrySplit(int amount, out Item splitItem) {
            if (this.Amount <= amount) {
                splitItem = null;
                return false;
            }

            splitItem = new Item(this);
            this.Amount -= amount;
            splitItem.Amount = amount;
            splitItem.Slot = -1;
            splitItem.Uid = Environment.TickCount64;
            return true;
        }
    }
}

/*
// No easy way to determine outfit vs weapon :(
Hair = 102x
Face = 103x
Cosmetic = 104x
Ear = 105x
Face Accessory = 110x
Eyewear = 111x
Emote = 202x

Curio = 200x
Consumable = 203x, 300x

Gemstone = 402x
Fishing = 320x
Music = 350x
CustomMusic = 351x
BasicInstrument = 191x
Instrument = 340x

Storybook = 390x

Hat = 113x
Top = 114x
Bottom = 115x
Gloves = 116x
Shoes = 117x
Cape = 118x
Suit = 122x

Earring = 112x
Pendant = 119x
Ring = 120x
Belt = 121x

Bludgeon  = 130x
Dagger    = 131x
Longsword = 132x
Scepter   = 133x
Throwing Star = 134x
Codex = 140x
Shield = 141x
Sword = 150x
Bow = 151x
Staff = 152x
Cannon = 153x
Blade = 154x
Knuckle = 155x
Orb = 156x

Gatherable Ids
Mining = 300x
Foraging = 400x
Ranching = 500x
Farming = 600x
Smithing = 700x
Handicraft = 800x
Alchemy = 900x
Cooking = 100x
*/
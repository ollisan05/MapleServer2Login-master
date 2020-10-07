using System;
using System.Collections.Generic;
using MapleServer2.Data.Static;

namespace MapleServer2.Types {
    public class PlayerData {
        // Bypass Key is constant PER ACCOUNT, unsure how it is validated
        // Seems like as long as it's valid, it doesn't matter though
        public readonly long UnknownId = 0x01EF80C2; //0x01CC3721;

        // Constant Values
        public long AccountId { get; private set; }
        public long CharacterId { get; private set; }
        public long CreationTime { get; private set; }
        public string Name { get; private set; }
        public int JobId { get; private set; }

        // Mutable Values
        public int ObjectId;
        public int MapId;

        public SCoordF Coord;
        public SCoordF UnknownCoord;

        // Appearance
        public SSkinColor SkinColor;

        public string ProfileUrl = ""; // profile/e2/5a/2755104031905685000/637207943431921205.png

        public List<Item> Equips = new List<Item>();
        public List<Item> Badges = new List<Item>();

        public static PlayerData Default(long accountId, long characterId) {
            return new PlayerData {
                MapId = 2000063,
                AccountId = accountId,
                CharacterId = characterId,
                Name = "Heroic",
                Coord = SCoordF.From(2000, 1000, 2000),
                JobId = 104,
                SkinColor = new SSkinColor(),
                CreationTime = DateTimeOffset.Now.ToUnixTimeSeconds(),
                Equips = new List<Item> {
                    Item.Ear(),
                    Item.Hair(),
                    Item.Face(),
                    Item.FaceDecoration()
                }
            };
        }
    }
}
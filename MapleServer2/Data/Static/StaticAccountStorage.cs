using System.Collections.Generic;
using MapleServer2.Types;

namespace MapleServer2.Data.Static {
    public class StaticAccountStorage : IAccountStorage {
        private readonly Dictionary<long, List<long>> accountCharacters = new Dictionary<long, List<long>>();
        private readonly Dictionary<long, PlayerData> characters = new Dictionary<long, PlayerData>();

        public const long DEFAULT_ACCOUNT = 0x1111111111111111;
        public const long DEFAULT_CHARACTER = 0x7777777777777777;

        public const long SECONDARY_ACCOUNT = 0x2222222222222222;
        public const long SECONDARY_CHARACTER1 = 0x5555555555555555;
        public const long SECONDARY_CHARACTER2 = 0x6666666666666666;

        public StaticAccountStorage() {
            // Default Account
            accountCharacters.Add(DEFAULT_ACCOUNT, new List<long> { DEFAULT_CHARACTER });
            characters[DEFAULT_CHARACTER] = PlayerData.Default(DEFAULT_ACCOUNT, DEFAULT_CHARACTER);

            // Secondary Account, has a character at tutorial
            accountCharacters.Add(SECONDARY_ACCOUNT, new List<long> { SECONDARY_CHARACTER1, SECONDARY_CHARACTER2 });
            characters[SECONDARY_CHARACTER1] = PlayerData.Default(SECONDARY_ACCOUNT, SECONDARY_CHARACTER1);
            PlayerData tutorialChar = PlayerData.Default(SECONDARY_ACCOUNT, SECONDARY_CHARACTER2);
            tutorialChar.MapId = 52000065; // Tutorial
            tutorialChar.Coord = SCoordF.From(-675, 525, 600);
            characters[SECONDARY_CHARACTER2] = tutorialChar;
        }

        public List<long> ListCharacters(long accountId) {
            return accountCharacters.GetValueOrDefault(accountId, new List<long>());
        }

        public PlayerData GetCharacter(long characterId) {
            return characters.GetValueOrDefault(characterId);
        }

        // Cannot save to static storage
        public bool SaveCharacter(long characterId, PlayerData data) {
            return false;
        }
    }
}
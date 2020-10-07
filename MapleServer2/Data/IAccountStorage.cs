﻿using System.Collections.Generic;
using MapleServer2.Types;

namespace MapleServer2.Data {
    // Interface for retrieving account data
    public interface IAccountStorage {
        // Retrieves a list of character ids for an account
        public List<long> ListCharacters(long accountId);

        // Retrieves a specific character for an account
        public PlayerData GetCharacter(long characterId);

        public bool SaveCharacter(long characterId, PlayerData data);
    }
}
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace MapleServer2.Types {
    // TODO: This class should probably hold references to all sessions in the field
    // so that it can propagate actions to all users.
    // All operations on this class should be thread safe
    public class FieldState {
        private int counter;

        public IReadOnlyDictionary<int, FieldObject<Item>> Items => items;
        public IReadOnlyDictionary<int, PlayerData> Players => players;

        private readonly ConcurrentDictionary<int, FieldObject<Item>> items;
        private readonly ConcurrentDictionary<int, PlayerData> players;

        public FieldState() {
            this.counter = 10000000;
            this.items = new ConcurrentDictionary<int, FieldObject<Item>>();
            this.players = new ConcurrentDictionary<int, PlayerData>();
        }

        public bool TryGetItem(int objectId, out FieldObject<Item> item) {
            return items.TryGetValue(objectId, out item);
        }

        public FieldObject<Item> AddItem(Item item) {
            int objectId = Interlocked.Increment(ref counter);
            items[objectId] = new FieldObject<Item>(objectId, item);

            return items[objectId];
        }

        public bool RemoveItem(int objectId, out Item item) {
            bool result = items.Remove(objectId, out FieldObject<Item> fieldItem);
            item = fieldItem?.Value;

            return result;
        }

        public int AddPlayer(PlayerData player) {
            int objectId = Interlocked.Increment(ref counter);
            player.ObjectId = objectId;
            players[objectId] = player;

            return objectId;
        }

        public bool RemovePlayer(int objectId) {
            return players.Remove(objectId, out PlayerData player);
        }
    }
}
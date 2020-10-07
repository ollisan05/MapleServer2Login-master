using System.Collections.Generic;

namespace MapleServer2.Tools {
    public class ManagerFactory<T> where T : new() {
        private readonly Dictionary<int, CacheItem> managers;

        public ManagerFactory() {
            this.managers = new Dictionary<int, CacheItem>();
        }

        public T GetManager(int key) {
            lock (managers) {
                if (!managers.TryGetValue(key, out CacheItem item)) {
                    item = new CacheItem(new T());
                    managers[key] = item;
                }

                item.Pin();
                return item.Value;
            }
        }

        public bool Release(int key) {
            lock (managers) {
                if (!managers.TryGetValue(key, out CacheItem manager) || manager.Release() > 0) {
                    return false;
                }

                return managers.Remove(key);
            }
        }

        private class CacheItem {
            public readonly T Value;
            private int count;

            public CacheItem(T value) {
                this.Value = value;
                this.count = 0;
            }

            public int Pin() {
                return ++count;
            }

            public int Release() {
                return --count;
            }
        }
    }
}
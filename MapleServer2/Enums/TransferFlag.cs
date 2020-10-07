using System;

namespace MapleServer2.Enums {
    [Flags]
    public enum TransferFlag {
        None = 0,
        Unknown1 = 1,
        Splitable = 2,
        Tradeable = 4,
        Binds = 8,
    }
}
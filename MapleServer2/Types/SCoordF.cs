namespace MapleServer2.Types {
    // TODO: CONFIRM ORDERING
    public struct SCoordF {
        public float X;
        public float Y;
        public float Z;

        public static SCoordF From(float x, float y, float z) {
            return new SCoordF {
                X = x,
                Y = y,
                Z = z,
            };
        }
    }
}
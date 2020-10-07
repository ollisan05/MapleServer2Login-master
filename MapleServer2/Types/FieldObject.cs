namespace MapleServer2.Types {
    public class FieldObject<T> {
        public readonly int ObjectId;
        public readonly T Value;

        public FieldObject(int objectId, T value) {
            this.ObjectId = objectId;
            this.Value = value;
        }
    }
}
namespace MapleServer2.Types {
    public struct SColor {
        public byte Blue { get; private set; }
        public byte Green { get; private set; }
        public byte Red { get; private set; }
        public byte Alpha { get; private set; }

        public static SColor Argb(byte alpha, byte red, byte green, byte blue) {
            return new SColor {
                Alpha = alpha,
                Red = red,
                Green = green,
                Blue = blue
            };
        }

        public override string ToString() => $"RGBA({Red:X2}, {Green:X2}, {Blue:X2}, {Alpha:X2})";
    }

    public struct SSkinColor {
        public SColor Primary;
        public SColor Secondary;

        public static SSkinColor Argb(SColor color) {
            return new SSkinColor {
                Primary = color,
                Secondary = color
            };
        }

        public override string ToString() => $"Primary:{Primary}|Secondary:{Secondary}";
    }

    public struct SEquipColor {
        public SColor Primary;
        public SColor Secondary;
        public SColor Tertiary;

        public static SEquipColor Argb(SColor color) {
            return new SEquipColor {
                Primary = color,
                Secondary = color,
                Tertiary = color
            };
        }

        public override string ToString() => $"Primary:{Primary}|Secondary:{Secondary}|Tertiary:{Tertiary}";
    }
}
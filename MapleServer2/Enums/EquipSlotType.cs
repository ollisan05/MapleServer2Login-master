using System.ComponentModel;

// NOTE: These enum MUST be capitalized
namespace MapleServer2.Enums {
    public enum EquipSlotType {
        [Description("None")]
        NONE = 0,
        [Description("Hair")]
        HR,
        [Description("Face")]
        FA,
        [Description("FaceDecoration")]
        FD,
        [Description("LeftHand")]
        LH,
        [Description("RightHand")]
        RH,
        [Description("Cap")]
        CP,
        [Description("Mantle")]
        MT,
        [Description("Clothes")]
        CL,
        [Description("Pants")]
        PA,
        [Description("Gloves")]
        GL,
        [Description("Shoes")]
        SH,
        [Description("NO_IDEA")]
        FH,
        [Description("Eyewear")]
        EY,
        [Description("Earring")]
        EA,
        [Description("Pendant")]
        PD,
        [Description("Ring")]
        RI,
        [Description("Belt")]
        BE,
        [Description("Ear")]
        ER,
    }
}
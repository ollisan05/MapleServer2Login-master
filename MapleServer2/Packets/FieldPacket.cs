using System;
using MaplePacketLib2.Tools;
using MapleServer2.Constants;
using MapleServer2.Extensions;
using MapleServer2.Packets.Helpers;
using MapleServer2.Types;

namespace MapleServer2.Packets {
    public static class FieldPacket {
        public static Packet RequestEnter(PlayerData player) {
            return PacketWriter.Of(SendOp.REQUEST_FIELD_ENTER)
                .WriteByte(0x00)
                .WriteInt(player.MapId)
                .WriteByte()
                .WriteByte(1)
                .WriteInt(1000074)
                .WriteInt()
                .Write<SCoordF>(player.Coord)
                .Write<SCoordF>(player.UnknownCoord)
                .WriteInt(71510);
        }

        public static Packet AddUser(int sessionId, PlayerData player) {
            var pWriter = PacketWriter.Of(SendOp.FIELD_ADD_USER);
            pWriter.WriteInt(sessionId);
            CharacterListPacket.WriteCharacter(player, pWriter);

            // Possible skill related?
            pWriter.WriteInt(player.JobId); // Job?
            pWriter.WriteByte(1);
            pWriter.WriteInt(player.JobId / 10);
            WriteSkills(pWriter);

            pWriter.WriteByte();
            pWriter.WriteByte();
            pWriter.Write<SCoordF>(player.Coord);
            pWriter.Write<SCoordF>(player.UnknownCoord);
            pWriter.WriteByte();

            // STATS (0x23) 35 stats?
            // idk if this is count or something related to stats
            /*pWriter.WriteByte(0x23);
            for (int i = 0; i < 18; i++) {
                pWriter.WriteInt();
            }*/
            pWriter.Write(
                "23 3D 00 00 00 00 00 00 00 64 00 00 00 64 00 00 00 64 00 00 00 64 00 00 00 3D 00 00 00 00 00 00 00 64 00 00 00 64 00 00 00 64 00 00 00 64 00 00 00 3D 00 00 00 00 00 00 00 64 00 00 00 64 00 00 00 64 00 00 00 64 00 00 00"
                    .ToByteArray());
            pWriter.WriteByte();
            pWriter.WriteByte();
            pWriter.WriteInt();
            pWriter.WriteLong();
            pWriter.WriteLong();

            // ???
            bool flagA = false;
            pWriter.WriteBool(flagA);
            if (flagA) {
                pWriter.WriteLong();
                pWriter.WriteUnicodeString("");
                pWriter.WriteUnicodeString("");
                pWriter.WriteByte();
                pWriter.WriteInt();
                pWriter.WriteLong();
                pWriter.WriteLong();
                pWriter.WriteUnicodeString("");
                pWriter.WriteLong();
                pWriter.WriteUnicodeString("");
                pWriter.WriteByte();
            }

            pWriter.WriteInt(1);
            pWriter.Write<SSkinColor>(player.SkinColor);
            pWriter.WriteUnicodeString(player.ProfileUrl); // Profile URL

            // ???
            bool flagB = false;
            pWriter.WriteBool(flagB);
            if (flagB) {
                pWriter.WriteByte();
                pWriter.WriteInt();
                pWriter.WriteInt();
                pWriter.WriteByte();
            }
            pWriter.WriteInt();
            pWriter.WriteLong(DateTimeOffset.UtcNow.ToUnixTimeSeconds()); // some timestamp
            pWriter.WriteInt();
            pWriter.WriteInt();

            // This seems to be character appearance encoded as a blob
            pWriter.WriteBool(true);
            if (true) {
                var appearanceBuffer = new PacketWriter();
                appearanceBuffer.WriteByte((byte)player.Equips.Count); // num equips
                foreach (Item equip in player.Equips) {
                    CharacterListPacket.WriteEquip(equip, appearanceBuffer);
                }

                appearanceBuffer.WriteByte(1)
                    .WriteLong()
                    .WriteLong()
                    .WriteByte();

                pWriter.WriteDeflated(appearanceBuffer.Buffer, 0, appearanceBuffer.Length);
                pWriter.WriteByte(); // Separator?
                pWriter.WriteDeflated(new byte[1], 0, 1); // Unknown
                pWriter.WriteByte(); // Separator?
                pWriter.WriteDeflated(new byte[1], 0, 1); // Badge appearances

                WritePassiveSkills(pWriter);

                pWriter.WriteInt();
                pWriter.WriteInt();
                pWriter.WriteByte();
                pWriter.WriteInt();
                pWriter.WriteByte();
                pWriter.WriteByte();
                pWriter.WriteInt(); // TitleId
                pWriter.WriteShort();
                pWriter.WriteByte();
                pWriter.WriteInt();
                pWriter.WriteByte();
                pWriter.WriteLong(); // Another timestamp
                pWriter.WriteInt(int.MaxValue);
                pWriter.WriteByte();
                pWriter.WriteInt();
                pWriter.WriteInt();
                pWriter.WriteShort();
            } else {
                pWriter.WriteInt();
            }

            return pWriter;
        }

        private static void WriteSkills(PacketWriter pWriter) {
            pWriter.Write(
                "23 00 00 05 38 A0 00 01 00 00 00 00 00 01 38 38 A0 00 01 00 00 00 00 00 00 7D 38 A0 00 01 00 00 00 00 00 00 D3 37 A0 00 01 00 00 00 00 00 01 39 38 A0 00 01 00 00 00 00 00 00 4B 38 A0 00 01 00 00 00 00 00 01 A1 37 A0 00 01 00 00 00 00 00 00 C3 38 A0 00 01 00 00 00 00 00 00 4C 38 A0 00 01 00 00 00 00 00 00 91 38 A0 00 01 00 00 00 00 00 01 0B 2D 31 01 01 00 00 00 00 00 00 4D 38 A0 00 01 00 00 00 00 00 00 5F 38 A0 00 01 00 00 00 00 00 00 B5 37 A0 00 01 00 00 00 00 00 00 4E 38 A0 00 01 00 00 00 00 00 00 2D 38 A0 00 01 00 00 00 00 00 00 60 38 A0 00 01 00 00 00 00 00 00 93 38 A0 00 01 00 00 00 00 00 00 FB 37 A0 00 01 00 00 00 00 00 00 61 38 A0 00 01 00 00 00 00 00 00 A5 38 A0 00 01 00 00 00 00 00 00 C9 37 A0 00 01 00 00 00 00 00 00 73 38 A0 00 01 00 00 00 00 00 00 FD 37 A0 00 01 00 00 00 00 00 00 30 38 A0 00 01 00 00 00 00 00 00 B9 38 A0 00 01 00 00 00 00 00 00 87 38 A0 00 01 00 00 00 00 00 01 01 2D 31 01 01 00 00 00 00 00 00 DD 37 A0 00 01 00 00 00 00 00 00 55 38 A0 00 01 00 00 00 00 00 01 AB 37 A0 00 01 00 00 00 00 00 00 F1 37 A0 00 01 00 00 00 00 00 00 BF 37 A0 00 01 00 00 00 00 00 00 E1 37 A0 00 01 00 00 00 00 00 01 37 38 A0 00 01 00 00 00 00 0E 00 00 E3 37 A0 00 01 00 00 00 00 00 00 AF 38 A0 00 01 00 00 00 00 00 00 19 38 A0 00 01 00 00 00 00 00 00 C4 38 A0 00 01 00 00 00 00 00 00 E7 37 A0 00 01 00 00 00 00 00 00 C5 38 A0 00 01 00 00 00 00 00 00 41 38 A0 00 01 00 00 00 00 00 01 0F 38 A0 00 01 00 00 00 00 00 00 88 38 A0 00 01 00 00 00 00 00 00 23 38 A0 00 01 00 00 00 00 00 00 DF 37 A0 00 01 00 00 00 00 00 00 9B 38 A0 00 01 00 00 00 00 00 00 E0 37 A0 00 01 00 00 00 00 00 00 69 38 A0 00 01 00 00 00 00"
                    .ToByteArray());
            // Normal + Awakening skills (2)
            /*for (int i = 0; i < 2; i++) {
                byte count = 0;
                pWriter.WriteByte(count);
                for (int j = 0; j < count; j++) {
                    pWriter.WriteShort();
                    pWriter.WriteInt();
                    pWriter.WriteByte();
                    pWriter.WriteInt();
                }
            }*/
        }

        private static void WritePassiveSkills(PacketWriter pWriter) {
            pWriter.Write(
                "01 00 3E FF 5A 00 A4 63 12 02 3E FF 5A 00 D0 71 85 28 D0 71 85 28 0F 38 A0 00 01 00 01 00 00 00 01 00 00 00 00 00 00 00 00"
                    .ToByteArray());
            /*short count = 0;
            pWriter.WriteShort(count);
            for (int i = 0; i < count; i++) {
                pWriter.WriteInt();
                pWriter.WriteInt();
                pWriter.WriteInt();
                pWriter.WriteInt();
                pWriter.WriteInt();
                pWriter.WriteInt();
                pWriter.WriteShort();
                pWriter.WriteInt();
                pWriter.WriteByte();
                pWriter.WriteLong();
            }*/
        }

        public static Packet AddItem(FieldObject<Item> item, int sessionId) {
            var pWriter = PacketWriter.Of(SendOp.FIELD_ADD_ITEM)
                .Write(item.ObjectId) // object id
                .Write(item.Value.Id)
                .Write(item.Value.Amount);

            bool flag = true;
            pWriter.WriteBool(flag);
            if (flag) {
                pWriter.WriteLong();
            }

            return pWriter.Write<SCoordF>(SCoordF.From(2850, 2550, 1800)) // drop location?
                .WriteInt(sessionId)
                .WriteInt()
                .WriteByte(2)
                .WriteInt(item.Value.Rarity)
                .WriteShort(1005)
                .WriteByte(1)
                .WriteByte(1)
                .WriteItem(item.Value);
        }

        public static Packet PickupItem(int objectId, int sessionId) {
            return PacketWriter.Of(SendOp.FIELD_PICKUP_ITEM)
                .WriteByte(0x01)
                .WriteInt(objectId)
                .WriteInt(sessionId);
        }

        public static Packet RemoveItem(int objectId) {
            return PacketWriter.Of(SendOp.FIELD_REMOVE_ITEM)
                .WriteInt(objectId);
        }
    }
}
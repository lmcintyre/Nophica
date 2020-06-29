using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SaintCoinach;
using SaintCoinach.Graphics;
using SaintCoinach.Xiv;
using SaintCoinach.Xiv.Items;

namespace Nophica
{
    public static class Data
    {
        public enum EquipSlotKey
        {
            Empty = 0,
            Mainhand,
            Offhand,
            Head,
            Body,
            Hands,
            Waist,
            Legs,
            Feet,
            Ears,
            Neck,
            Wrist,
            Finger,
            Twohanded,
            OneHanded, // unused
            BodyNoHead,
            BodyNoHandsLegsFeet,
            JobStone,
            LegsNoFeet,
            BodyNoHeadHandsLegsFeet,
            BodyNoHandsLegs,
            BodyNoLegsFeet,
            None
        }

        public enum EquipRestrict {
            Unequippable = 0,
            NoRestriction,
            Male,
            Female,
            HyurMale,
            HyurFemale,
            ElezenMale,
            ElezenFemale,
            LalaMale,
            LalaFemale,
            MiqoteMale,
            MiqoteFemale,
            RoeMale,
            RoeFemale,
            AuraMale,
            AuraFemale,
            Hrothgar,
            Viera
        }

        public static short GetKeyForCharacter(Tribe t, Sex s)
        {
            return s == Sex.Male ? t.MaleModelTypeKey : t.FemaleModelTypeKey;
        }

        public static bool CharaHasTail(Tribe t, Sex s) {
            return CharaHasTail(GetKeyForCharacter(t, s));
        }

        public static bool CharaHasTail(short charaCode) {
            switch (charaCode) {
                case 701:
                case 801:
                case 1301:
                case 1401:
                case 1501:
                    return true;
                default:
                    return false;
            }
        }

        public static bool CharaCanEquip(Tribe t, Sex s, Equipment e) {
            short charaCode = GetKeyForCharacter(t, s);
            return CharaCanEquip(charaCode, e);
        }

        public static bool CharaIsMale(short charaCode) {
            return charaCode / 100 % 2 == 1;
        }

        public static bool CharaIsChild(short charaCode) {
            return charaCode % 10 == 4;
        }

        public static bool CharaCanEquip(short charaCode, Equipment e) {
            EquipRestrict er = (EquipRestrict) e.EquipRestriction;
            return CharaCanEquip(charaCode, er);
        }

        public static bool CharaCanEquip(short charaCode, EquipRestrict er) {
            //TODO contemplate support for kids
            switch (er)
            {
                case EquipRestrict.Unequippable:
                    return false;
                case EquipRestrict.NoRestriction:
                    return true;
                case EquipRestrict.Male:
                    return CharaIsMale(charaCode);
                case EquipRestrict.Female:
                    return !CharaIsMale(charaCode);
                case EquipRestrict.HyurMale:
                    return charaCode == 101 || charaCode == 301;
                case EquipRestrict.HyurFemale:
                    return charaCode == 201 || charaCode == 401;
                case EquipRestrict.ElezenMale:
                    return charaCode == 501;
                case EquipRestrict.ElezenFemale:
                    return charaCode == 601;
                case EquipRestrict.LalaMale:
                    return charaCode == 1101;
                case EquipRestrict.LalaFemale:
                    return charaCode == 1201;
                case EquipRestrict.MiqoteMale:
                    return charaCode == 701;
                case EquipRestrict.MiqoteFemale:
                    return charaCode == 801;
                case EquipRestrict.RoeMale:
                    return charaCode == 901;
                case EquipRestrict.RoeFemale:
                    return charaCode == 1001;
                case EquipRestrict.AuraMale:
                    return charaCode == 1301;
                case EquipRestrict.AuraFemale:
                    return charaCode == 1401;
                case EquipRestrict.Hrothgar:
                    return charaCode == 1501;
                case EquipRestrict.Viera:
                    return charaCode == 1801;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static bool IsAWeapon(EquipSlotKey key) {
            return
                key == EquipSlotKey.Mainhand
                || key == EquipSlotKey.Offhand
                || key == EquipSlotKey.Twohanded
                || key == EquipSlotKey.OneHanded;
        }

        public static bool IsAnEquipment(EquipSlotKey key) {
            return
                key == EquipSlotKey.Head
                || key == EquipSlotKey.Body
                || key == EquipSlotKey.Hands
                || key == EquipSlotKey.Legs
                || key == EquipSlotKey.Feet
                || key == EquipSlotKey.BodyNoHead
                || key == EquipSlotKey.BodyNoHandsLegsFeet
                || key == EquipSlotKey.LegsNoFeet
                || key == EquipSlotKey.BodyNoHeadHandsLegsFeet
                || key == EquipSlotKey.BodyNoHandsLegs
                || key == EquipSlotKey.BodyNoLegsFeet;
        }

        public static bool IsAnAccessory(EquipSlotKey key) {
            return
                key == EquipSlotKey.Ears
                || key == EquipSlotKey.Neck
                || key == EquipSlotKey.Wrist
                || key == EquipSlotKey.Finger;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CharaMakeTypeStruct
        {
            CharaMakeTypeLooksStruct[] Looks; //28
            CharaMakeTypeVoiceStruct Voice;
            CharaMakeTypeFaceOptionStruct[] FaceOption; //8
            // 4 bytes padding
            CharaMakeTypeEquipStruct[] Equip; //3
            int Race;
            int Tribe;
            sbyte Gender;

            public static CharaMakeTypeStruct Read(byte[] buffer)
            {
                int offset = 0;

                CharaMakeTypeStruct cmt = new CharaMakeTypeStruct();

                cmt.Looks = new CharaMakeTypeLooksStruct[28];
                for (int i = 0; i < 28; i++)
                    cmt.Looks[i] = CharaMakeTypeLooksStruct.Read(buffer, ref offset);

                cmt.Voice = CharaMakeTypeVoiceStruct.Read(buffer, ref offset);

                cmt.FaceOption = new CharaMakeTypeFaceOptionStruct[8];
                for (int i = 0; i < 8; i++)
                    cmt.FaceOption[i] = CharaMakeTypeFaceOptionStruct.Read(buffer, ref offset);

                offset += 4;

                cmt.Equip = new CharaMakeTypeEquipStruct[3];
                for (int i = 0; i < 3; i++)
                    cmt.Equip[i] = CharaMakeTypeEquipStruct.Read(buffer, ref offset);

                cmt.Race = OrderedBitConverter.ToInt32(buffer, offset, true);
                offset += 4;
                cmt.Tribe = OrderedBitConverter.ToInt32(buffer, offset, true);
                offset += 4;
                cmt.Gender = (sbyte)buffer[offset];

                return cmt;
            }
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct CharaMakeTypeLooksStruct
        {
            uint Menu;
            uint SubMenuMask;
            uint Customize;
            uint[] SubMenuParam; //100
            byte InitVal;
            byte SubMenuType;
            byte SubMenuNum;
            byte LookAt;
            byte[] SubMenuGraphic; //10
            byte[] padding; //2?

            public static CharaMakeTypeLooksStruct Read(byte[] buffer, ref int offset)
            {
                CharaMakeTypeLooksStruct cmtl = new CharaMakeTypeLooksStruct();

                cmtl.Menu = OrderedBitConverter.ToUInt32(buffer, offset, true);
                offset += 4;
                cmtl.SubMenuMask = OrderedBitConverter.ToUInt32(buffer, offset, true);
                offset += 4;
                cmtl.Customize = OrderedBitConverter.ToUInt32(buffer, offset, true);
                offset += 4;
                cmtl.SubMenuParam = new uint[100];

                for (int i = 0; i < 100; i++)
                {
                    cmtl.SubMenuParam[i] = OrderedBitConverter.ToUInt32(buffer, offset, true);
                    offset += 4;
                }

                cmtl.InitVal = buffer[offset];
                cmtl.SubMenuType = buffer[offset + 1];
                cmtl.SubMenuNum = buffer[offset + 2];
                cmtl.LookAt = buffer[offset + 3];
                offset += 4;

                cmtl.SubMenuGraphic = new byte[10];
                for (int i = 0; i < 10; i++)
                {
                    cmtl.SubMenuGraphic[i] = buffer[offset];
                    offset += 1;
                }

                offset += 2;
                
                return cmtl;
            }
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct CharaMakeTypeFaceOptionStruct
        {
            int[] Option; //7

            public static CharaMakeTypeFaceOptionStruct Read(byte[] buffer, ref int offset)
            {
                CharaMakeTypeFaceOptionStruct cmtf = new CharaMakeTypeFaceOptionStruct();

                cmtf.Option = new int[7];
                for (int i = 0; i < 7; i++)
                {
                    cmtf.Option[i] = OrderedBitConverter.ToInt32(buffer, offset, true);
                    offset += 4;
                }
                return cmtf;
            }
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct CharaMakeTypeVoiceStruct
        {
            byte[] SEPackId; //12

            public static CharaMakeTypeVoiceStruct Read(byte[] buffer, ref int offset) {
                CharaMakeTypeVoiceStruct cmtv = new CharaMakeTypeVoiceStruct();

                cmtv.SEPackId = new byte[12];
                for (int i = 0; i < 12; i++) {
                    cmtv.SEPackId[i] = buffer[offset];
                    offset += 1;
                }
                return cmtv;
            }
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct CharaMakeTypeEquipStruct
        {
            ulong Helmet;
            ulong Top;
            ulong Glove;
            ulong Down;
            ulong Shoes;
            ulong Weapon;
            ulong SubWeapon;

            public static CharaMakeTypeEquipStruct Read(byte[] buffer, ref int offset)
            {
                CharaMakeTypeEquipStruct cmte = new CharaMakeTypeEquipStruct();
                cmte.Helmet = OrderedBitConverter.ToUInt64(buffer, offset, true);
                offset += 8;
                cmte.Top = OrderedBitConverter.ToUInt64(buffer, offset, true);
                offset += 8;
                cmte.Glove = OrderedBitConverter.ToUInt64(buffer, offset, true);
                offset += 8;
                cmte.Down = OrderedBitConverter.ToUInt64(buffer, offset, true);
                offset += 8;
                cmte.Shoes = OrderedBitConverter.ToUInt64(buffer, offset, true);
                offset += 8;
                cmte.Weapon = OrderedBitConverter.ToUInt64(buffer, offset, true);
                offset += 8;
                cmte.SubWeapon = OrderedBitConverter.ToUInt64(buffer, offset, true);
                offset += 8;
                return cmte;
            }
        };

        public static CharaMakeTypeStruct[] LoadCharaMakeType(ARealmReversed realm) {
            
            string sheetFile = "exd/charamaketype_0_en.exd";
            byte[] fileData = realm.Packs.GetFile(sheetFile).GetData();
            uint rows = MiniExdReader.GetNumRows(fileData);

            CharaMakeTypeStruct[] sheet = new CharaMakeTypeStruct[rows];

            for (int i = 0; i < rows; i++) {
                byte[] row = MiniExdReader.GetRow(fileData, i);
                sheet[i] = CharaMakeTypeStruct.Read(row);
            }

            return sheet;
        }

        // This will have to be greatly expanded on, with support for
        // all gear's EquipmentParameters.......... kill me
        public static Mesh LoadMeshWithVariant(Model m, ImcVariant variant) {
            var ranges = new List<Range>();
            var hiddenAttrs = variant.GetHiddenMeshParts();

            Mesh[] meshes = m.Meshes;

            foreach (var mesh in meshes)
            {
                foreach (var part in mesh.Parts)
                {
                    if (part.Attributes.Length == 0)
                        ranges.Add(new Range(part.IndexOffset, part.IndexCount));
                    if (part.ShouldDisplay(hiddenAttrs))
                        ranges.Add(new Range(part.IndexOffset, part.IndexCount));
                }
            }

            List<Vertex> realVertices = new List<Vertex>();
            List<ushort> tmpIndices = new List<ushort>();
            List<ushort> realIndices = new List<ushort>();

            ushort vOffset = 0;
            for (int i = 0; i < meshes.Length; i++)
            {
                if (i != 0)
                    vOffset += (ushort)meshes[i - 1].Vertices.Length;

                while (tmpIndices.Count != meshes[i].Header.IndexBufferOffset)
                    tmpIndices.Add(0);

                foreach (Vertex t in meshes[i].Vertices)
                    realVertices.Add(t);

                foreach (ushort t in meshes[i].Indices)
                    tmpIndices.Add((ushort)(t + vOffset));
            }

            foreach (Range r in ranges)
                for (int j = r.Start; j < r.End; j++)
                    realIndices.Add(tmpIndices[j]);

            return new Mesh(m, 0, realVertices.ToArray(), realIndices.ToArray());
        }

        public static Mesh LoadMesh(Model m) {

            Mesh[] meshes = m.Meshes;

            List<Vertex> realVertices = new List<Vertex>();
            List<ushort> realIndices = new List<ushort>();

            ushort vOffset = 0;
            for (int i = 0; i < meshes.Length; i++)
            {
                if (i != 0)
                    vOffset += (ushort)meshes[i - 1].Vertices.Length;

                while (realIndices.Count != meshes[i].Header.IndexBufferOffset)
                    realIndices.Add(0);

                foreach (Vertex t in meshes[i].Vertices)
                    realVertices.Add(t);

                foreach (ushort t in meshes[i].Indices)
                    realIndices.Add((ushort)(t + vOffset));
            }

            return new Mesh(m, 0, realVertices.ToArray(), realIndices.ToArray());
        }

        public struct Sex {
            public static readonly Sex Male = new Sex {value = 0, name = "Male"};
            public static readonly Sex Female = new Sex {value = 1, name = "Female"};

            public sbyte value;
            public string name;

            public override string ToString() {
                return name;
            }

            public override bool Equals(object obj) {
                if (obj is Sex s)
                    return value == s.value;
                return false;
            }

            public static bool operator ==(Sex a, Sex b) {
                return a.value == b.value;
            }

            public static bool operator !=(Sex a, Sex b) {
                return a.value != b.value;
            }
        }

    }
}

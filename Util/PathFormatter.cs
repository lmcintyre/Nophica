using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using SaintCoinach;
using SaintCoinach.Graphics;
using static Nophica.Data;
using SaintCoinach.Xiv;
using SaintCoinach.Xiv.Collections;
using SaintCoinach.Xiv.Items;
using File = SaintCoinach.IO.File;

namespace Nophica.Util
{
    public class PathFormatter {
        private static PathFormatter _Instance;
        private static ARealmReversed Realm;

        public static PathFormatter Instance {
            get {
                if (_Instance != null)
                    return _Instance;
                throw new Exception("PathFormatter was not initialized.");
            }
        }

        public static void Init(ARealmReversed realm) {
            _Instance = new PathFormatter();
            Realm = realm;
        }

        private readonly Dictionary<int, string> SlotAbbreviationDictionary = new Dictionary<int, string> {
            {0, ""},
            {3, "met"},
            {4, "top"},
            {5, "glv"},
            {7, "dwn"},
            {8, "sho"},
            {9, "ear"},
            {10, "nek"},
            {11, "wrs"},
            {12, "rir"},
            {15, "top"},
            {16, "top"},
            {18, "dwn"},
            {19, "top"},
            {20, "top"},
            {21, "top"},

            // {XivStrings.Face, "fac"},
            // {XivStrings.Iris, "iri"},
            // {XivStrings.Etc, "etc"},
            // {XivStrings.Accessory, "acc"},
            // {XivStrings.Hair, "hir"},
            // {XivStrings.Tail, "til"}
        };

        private static Dictionary<EquipSlotKey, int> SlotOffsetDictionary = new Dictionary<EquipSlotKey, int> {
            {EquipSlotKey.Mainhand, 0},
            {EquipSlotKey.Offhand, 0},
            {EquipSlotKey.Twohanded, 0},
            // {EquipSlotKey.Main_Off, 0},  //?
            {EquipSlotKey.Head, 0},
            {EquipSlotKey.Body, 1},
            {EquipSlotKey.Hands, 2},
            {EquipSlotKey.Legs, 3},
            {EquipSlotKey.Feet, 4},
            {EquipSlotKey.Ears, 0},
            {EquipSlotKey.Neck, 1},
            {EquipSlotKey.Wrist, 2},
            {EquipSlotKey.Finger, 3},
            {EquipSlotKey.BodyNoHead, 1},
            {EquipSlotKey.BodyNoHandsLegsFeet, 1},
            {EquipSlotKey.BodyNoHandsLegs, 1},
            {EquipSlotKey.BodyNoLegsFeet, 1},
            {EquipSlotKey.BodyNoHeadHandsLegsFeet, 1},
            {EquipSlotKey.LegsNoFeet, 3},
            {EquipSlotKey.Empty, 1}, //?
        };

        private const short DEFAULT_MALE_CODE = 101;
        private const short DEFAULT_FEMALE_CODE = 201;
        private const short DEFAULT_CHILD_CODE = 101;

        private const string AccessoryImcFormat = "chara/accessory/a{0:D4}/a{0:D4}.imc";
        private const string EquipmentImcFormat = "chara/equipment/e{0:D4}/e{0:D4}.imc";
        private const string WeaponImcFormat = "chara/weapon/w{0:D4}/obj/body/b{1:D4}/b{1:D4}.imc";

        private const string AccessoryModelFormat = "chara/accessory/a{0:D4}/model/c{1:D4}a{0:D4}_{2}.mdl";
        private const string EquipmentModelFormat = "chara/equipment/e{0:D4}/model/c{1:D4}e{0:D4}_{2}.mdl";
        private const string WeaponModelFormat = "chara/weapon/w{0:D4}/obj/body/b{1:D4}/model/w{0:D4}b{1:D4}.mdl";

        // _top only?
        private const string BodyModelFormat = "chara/human/c{0:D4}/obj/body/b{1:D4}/model/c{0:D4}b{1:D4}_top.mdl";
        private const string FaceModelFormat = "chara/human/c{0:D4}/obj/face/f{1:D4}/model/c{0:D4}f{1:D4}_fac.mdl";
        private const string HairModelFormat = "chara/human/c{0:D4}/obj/hair/h{1:D4}/model/c{0:D4}h{1:D4}_hir.mdl";
        // viera only
        private const string EarModelFormat = "chara/human/c1801/obj/zear/z{0:D4}/model/c1801z{0:D4}_zer.mdl";
        private const string TailModelFormat = "chara/human/c{0:D4}/obj/tail/t{1:D4}/model/c{0:D4}t{1:D4}_til.mdl";

        private const string WeaponSkeleFormat = "chara/weapon/w{0:D4}/skeleton/base/b0001/skl_w{0:D4}b0001.sklb";

        private const string MetSkeleFormat = "chara/human/c{0:D4}/skeleton/met/m{1:D4}/skl_c{0:D4}m{1:D4}.sklb";
        private const string TopSkeleFormat = "chara/human/c{0:D4}/skeleton/top/t{1:D4}/skl_c{0:D4}t{1:D4}.sklb";
        
        private const string BodySkeleFormat = "chara/human/c{0:D4}/skeleton/base/b0001/skl_c{0:D4}b0001.sklb"; //need skp?
        private const string FaceSkeleFormat = "chara/human/c{0:D4}/skeleton/face/f{1:D4}/skl_c{0:D4}f{1:D4}.sklb";
        private const string HairSkeleFormat = "chara/human/c{0:D4}/skeleton/hair/h{1:D4}/skl_c{0:D4}h{1:D4}.sklb";

        public ImcVariant GetVariant(Quad q, EquipSlotKey slotKey) {
            ImcFile imc = new ImcFile(Realm.Packs.GetFile(GetImcPath(q, slotKey)));
            int variantPart = SlotOffsetDictionary[slotKey];
            int variantKey = IsAWeapon(slotKey) ? q.Value3 : q.Value2;
            return imc.Parts.ElementAt(variantPart).Variants[variantKey];
        }
        
        // gear
        public string GetImcPath(Equipment e) {
            return GetImcPath(e.PrimaryModelKey, (EquipSlotKey) e.EquipSlotCategory.Key);
        }

        public string GetImcPath(Quad q, EquipSlotKey slotKey) {

            string imcPath = "";
            if (IsAWeapon(slotKey))
                imcPath = string.Format(WeaponImcFormat, q.Value1, q.Value2);
            else if (IsAnEquipment(slotKey))
                imcPath = string.Format(EquipmentImcFormat, q.Value1);
            else if (IsAnAccessory(slotKey))
                imcPath = string.Format(AccessoryImcFormat, q.Value1);

            return Realm.Packs.FileExists(imcPath) ? imcPath : "";
        }

        public string GetEquipmentModelPath(Tribe t, Sex s, Equipment e) {
            return GetEquipmentModelPath(t, s, e.PrimaryModelKey, (EquipSlotKey) e.EquipSlotCategory.Key);
        }

        public string GetEquipmentModelPath(Tribe t, Sex s, Quad q, EquipSlotKey slotKey) {
            short charaCode = GetKeyForCharacter(t, s);
            return GetEquipmentModelPath(charaCode, q, slotKey);
        }

        public string GetEquipmentModelPath(short charaCode, Quad q, EquipSlotKey slotKey) {
            string equipCode = SlotAbbreviationDictionary[(int) slotKey];

            string modelPath = string.Format(EquipmentModelFormat, q.Value1, charaCode, equipCode);

            if (Realm.Packs.FileExists(modelPath))
                return modelPath;

            if (CharaIsChild(charaCode))
                modelPath = string.Format(EquipmentModelFormat, q.Value1, DEFAULT_CHILD_CODE, equipCode);
            if (CharaIsMale(charaCode))
                modelPath = string.Format(EquipmentModelFormat, q.Value1, DEFAULT_MALE_CODE, equipCode);
            else if (!CharaIsMale(charaCode)) {
                modelPath = string.Format(EquipmentModelFormat, q.Value1, DEFAULT_FEMALE_CODE, equipCode);
                if (Realm.Packs.FileExists(modelPath))
                    return modelPath;

                modelPath = string.Format(EquipmentModelFormat, q.Value1, DEFAULT_MALE_CODE, equipCode);
            }

            if (Realm.Packs.FileExists(modelPath))
                return modelPath;

            return "";
        }

        public string GetAccessoryModelPath(Tribe t, Sex s, Equipment e) {
            return GetAccessoryModelPath(t, s, e.ModelMain, (EquipSlotKey) e.EquipSlotCategory.Key);
        }

        public string GetAccessoryModelPath(Tribe t, Sex s, Quad q, EquipSlotKey slotKey) {
            short charaCode = GetKeyForCharacter(t, s);
            return GetAccessoryModelPath(charaCode, q, slotKey);
        }

        public string GetAccessoryModelPath(short charaCode, Quad q, EquipSlotKey slotKey) {
            string equipCode = SlotAbbreviationDictionary[(int) slotKey];

            string modelPath = string.Format(AccessoryModelFormat, q.Value1, charaCode, equipCode);

            if (Realm.Packs.FileExists(modelPath))
                return modelPath;

            if (CharaIsChild(charaCode))
                modelPath = string.Format(AccessoryModelFormat, q.Value1, DEFAULT_CHILD_CODE, equipCode);
            if (CharaIsMale(charaCode))
                modelPath = string.Format(AccessoryModelFormat, q.Value1, DEFAULT_MALE_CODE, equipCode);
            else if (!CharaIsMale(charaCode)) {
                modelPath = string.Format(AccessoryModelFormat, q.Value1, DEFAULT_FEMALE_CODE, equipCode);
                if (Realm.Packs.FileExists(modelPath))
                    return modelPath;

                modelPath = string.Format(AccessoryModelFormat, q.Value1, DEFAULT_MALE_CODE, equipCode);
            }

            if (Realm.Packs.FileExists(modelPath))
                return modelPath;

            return "";
        }

        public string[] GetWeaponModelPath(Equipment e) {
            return GetWeaponModelPath(e.ModelMain, e.ModelSub, (EquipSlotKey) e.EquipSlotCategory.Key);
        }

        public string[] GetWeaponModelPath(Quad main, Quad sub, EquipSlotKey slotKey) {
            string[] paths = new string[2];

            string tmp = string.Format(WeaponModelFormat, main.Value1, main.Value2);

            if (Realm.Packs.FileExists(tmp))
                paths[0] = tmp;

            if (sub.Value1 != 0) {
                tmp = string.Format(WeaponModelFormat, sub.Value1, sub.Value2);
                if (Realm.Packs.FileExists(tmp))
                    paths[1] = tmp;
            }

            return paths;
        }


        // chara parts
        public string GetBodyModelPath(Tribe t, Sex s, int body) {
            return GetBodyModelPath(GetKeyForCharacter(t, s), body);
        }

        public string GetBodyModelPath(short charaCode, int body) {
            return ReturnIfExists(string.Format(BodyModelFormat, charaCode, body));
        }

        public string GetFaceModelPath(Tribe t, Sex s, int face) {
            return GetFaceModelPath(GetKeyForCharacter(t, s), face);
        }

        public string GetFaceModelPath(short charaCode, int face) {
            return ReturnIfExists(string.Format(FaceModelFormat, charaCode, face));
        }
        /* ?????????????
        chara/human/c%04d/obj/face/f%04d/material/mt_c%04df%04d_etc_a.mtrl
        chara/human/c%04d/obj/face/f%04d/material/mt_c%04df%04d_fac_a.mtrl
        chara/human/c%04d/obj/face/f%04d/material/mt_c%04df%04d_iri_a.mtrl
         */

        public string GetHairModelPath(Tribe t, Sex s, int hair) {
            return GetHairModelPath(GetKeyForCharacter(t, s), hair);
        }

        public string GetHairModelPath(short charaCode, int hair) {
            return ReturnIfExists(string.Format(HairModelFormat, charaCode, hair));
        }

        // viera only
        public string GetEarModelPath(int ear) {
            return ReturnIfExists(string.Format(EarModelFormat, ear));
        }

        public string GetTailModelPath(Tribe t, Sex s, int tail) {
            return GetTailModelPath(GetKeyForCharacter(t, s), tail);
        }

        public string GetTailModelPath(short charaCode, int tail) {
            if (!CharaHasTail(charaCode))
                return "";

            return ReturnIfExists(string.Format(TailModelFormat, charaCode, tail));
        }
        
        //skeletons
        // TODO: phyb
        public string[] GetWeaponSkeletonPath(Equipment e) {
            return GetWeaponSkeletonPath(e.ModelMain, e.ModelSub, (EquipSlotKey) e.EquipSlotCategory.Key);
        }

        public string GetMetSkeletonPath(Tribe t, Sex s, Equipment e)
        {
            return GetMetSkeletonPath(GetKeyForCharacter(t, s), e.ModelMain);
        }

        public string GetTopSkeletonPath(Tribe t, Sex s, Equipment e) {
            return GetTopSkeletonPath(GetKeyForCharacter(t, s), e.ModelMain);
        }

        public string GetBodySkeletonPath(Tribe t, Sex s) {
            return GetBodySkeletonPath(GetKeyForCharacter(t, s));
        }

        public string GetFaceSkeletonPath(Tribe t, Sex s, int face) {
            return GetFaceSkeletonPath(GetKeyForCharacter(t, s), face);
        }

        public string GetHairSkeletonPath(Tribe t, Sex s, int hair) {
            return GetHairSkeletonPath(GetKeyForCharacter(t, s), hair);
        }

        public string[] GetWeaponSkeletonPath(Quad main, Quad sub, EquipSlotKey slotKey) {
            string[] ret = new string[2];
            ret[0] = "";
            ret[1] = "";
            
            ret[0] = string.Format(WeaponSkeleFormat, main.Value1);

            if (sub.Value1 != 0)
                ret[1] = string.Format(WeaponSkeleFormat, sub.Value1);

            if (Realm.Packs.FileExists(ret[0]))
                return ret;
            return new[] {"", ""};
        }

        public string GetMetSkeletonPath(short charaCode, Quad met) {
            string skelePath = string.Format(MetSkeleFormat, charaCode, met.Value1);

            if (Realm.Packs.FileExists(skelePath))
                return skelePath;

            // Don't know if met/top fall back?
            System.Diagnostics.Debug.WriteLine($"Fellback on {charaCode} met {met}");

            if (CharaIsChild(charaCode))
                skelePath = string.Format(MetSkeleFormat, met.Value1, DEFAULT_CHILD_CODE);
            if (CharaIsMale(charaCode))
                skelePath = string.Format(MetSkeleFormat, met.Value1, DEFAULT_MALE_CODE);
            else if (!CharaIsMale(charaCode)) {
                skelePath = string.Format(MetSkeleFormat, met.Value1, DEFAULT_FEMALE_CODE);
                if (Realm.Packs.FileExists(skelePath))
                    return skelePath;

                skelePath = string.Format(MetSkeleFormat, met.Value1, DEFAULT_MALE_CODE);
            }

            return ReturnIfExists(skelePath);
        }

        public string GetTopSkeletonPath(short charaCode, Quad top)
        {
            string skelePath = string.Format(TopSkeleFormat, charaCode, top.Value1);

            if (Realm.Packs.FileExists(skelePath))
                return skelePath;

            // Don't know if met/top fall back?
            System.Diagnostics.Debug.WriteLine($"Fellback on {charaCode} top {top}");

            if (CharaIsChild(charaCode))
                skelePath = string.Format(TopSkeleFormat, top.Value1, DEFAULT_CHILD_CODE);
            if (CharaIsMale(charaCode))
                skelePath = string.Format(TopSkeleFormat, top.Value1, DEFAULT_MALE_CODE);
            else if (!CharaIsMale(charaCode))
            {
                skelePath = string.Format(TopSkeleFormat, top.Value1, DEFAULT_FEMALE_CODE);
                if (Realm.Packs.FileExists(skelePath))
                    return skelePath;

                skelePath = string.Format(TopSkeleFormat, top.Value1, DEFAULT_MALE_CODE);
            }

            return ReturnIfExists(skelePath);
        }

        public string GetBodySkeletonPath(short charaCode) {
            return ReturnIfExists(string.Format(BodySkeleFormat, charaCode));
        }

        public string GetFaceSkeletonPath(short charaCode, int face) {
            return ReturnIfExists(string.Format(FaceSkeleFormat, charaCode, face));
        }

        public string GetHairSkeletonPath(short charaCode, int hair) {
            return ReturnIfExists(string.Format(HairSkeleFormat, charaCode, hair));
        }





        private string ReturnIfExists(string path) {
            return Realm.Packs.FileExists(path) ? path : "";
        }

















    }
}

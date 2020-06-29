using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Nophica.Data;
using Nophica.Util;
using SaintCoinach;
using SaintCoinach.Ex;
using SaintCoinach.Xiv;
using SaintCoinach.Xiv.Items;

namespace Nophica.Tests
{
    [TestClass]
    public class PathTest {

        private static string GameDirectory = @"C:\Program Files (x86)\SquareEnix\FINAL FANTASY XIV - A Realm Reborn\";
        private static IXivSheet<Item> ItemSheet;

        private short[] charaCodes = new short[]
            {101, 104, 201, 204, 301, 401, 501, 601, 701, 801, 804, 901, 1001, 1101, 1201, 1301, 1401, 1501, 1801};

        [ClassInitialize]
        public static void Init(TestContext context) {
            ARealmReversed Realm = new ARealmReversed(GameDirectory, Language.English);
            ItemSheet = Realm.GameData.GetSheet<Item>();
            PathFormatter.Init(Realm);
        }

        [TestMethod]
        public void TestImcPathsAgainstItemSheet()
        {
            foreach (Item item in ItemSheet)
            {
                EquipSlotKey eq = (EquipSlotKey)item.EquipSlotCategory.Key;
                if (IsAWeapon(eq) || IsAnEquipment(eq) || IsAnAccessory(eq))
                {
                    string path = PathFormatter.Instance.GetImcPath(item.ModelMain, eq);
                    Assert.AreNotEqual(path, "", $"{item} IMC failed");
                }
            }
        }

        [TestMethod]
        public void TestEquipmentModelPathsAgainstItemSheet() {
            foreach (Item item in ItemSheet) {
                if (IsAnEquipment((EquipSlotKey) item.EquipSlotCategory.Key)) {
                    foreach (short code in charaCodes) {
                        if (!CharaCanEquip(code, (EquipRestrict) item.EquipRestriction))
                            continue;
                        string path = PathFormatter.Instance.GetEquipmentModelPath(code, item.ModelMain, (EquipSlotKey) item.EquipSlotCategory.Key);
                        Assert.AreNotEqual(path, "", $"{item} model failed for chara {code}");
                    }
                }
            }
        }

        [TestMethod]
        public void TestWeaponModelPathsAgainstItemSheet() {
            foreach (Item item in ItemSheet) {
                if (IsAWeapon((EquipSlotKey)item.EquipSlotCategory.Key)) {
                    string[] path = PathFormatter.Instance.GetWeaponModelPath(item.ModelMain, item.ModelSub,
                        (EquipSlotKey) item.EquipSlotCategory.Key);
                    Assert.IsNotNull(path);
                    Assert.AreNotEqual(path[0], "", $"{item} main failed!");
                    
                    if (item.ModelSub.Value1 != 0)
                        Assert.AreNotEqual(path[1], "", $"{item} sub failed!");
                }
            }
        }

        [TestMethod]
        public void TestAccessoryModelPathsAgainstItemSheet()
        {
            foreach (Item item in ItemSheet)
            {
                if (IsAnAccessory((EquipSlotKey)item.EquipSlotCategory.Key))
                {
                    foreach (short code in charaCodes)
                    {
                        if (!CharaCanEquip(code, (EquipRestrict)item.EquipRestriction))
                            continue;
                        string path = PathFormatter.Instance.GetAccessoryModelPath(code, item.ModelMain, (EquipSlotKey)item.EquipSlotCategory.Key);
                        Assert.AreNotEqual(path, "", $"{item} model failed for chara {code}");
                    }
                }
            }
        }













    }
}

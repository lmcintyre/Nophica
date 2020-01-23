using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nophica.ViewModels;
using SaintCoinach.Xiv;
using SaintCoinach.Xiv.Items;

namespace Nophica
{
    static class Extensions
    {
        public static bool HasSubModel(this Equipment eq) {
            return eq.ModelSub.Value1 != 0 ||
                   eq.ModelSub.Value2 != 0 ||
                   eq.ModelSub.Value3 != 0 ||
                   eq.ModelSub.Value4 != 0;
        }

        public static bool IsMainhand(this Equipment eq) {
            return eq.EquipSlotCategory.PossibleSlots.All(s => s.Key == (int) Slot.Mainhand);
        }

        public static bool IsTwoHanded(this Equipment eq) {

            // eq.EquipSlotCategory 
            return false;
        }
    }
}

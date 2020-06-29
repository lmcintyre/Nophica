using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Nophica.ViewModels;
using SaintCoinach;
using SaintCoinach.Graphics;
using SaintCoinach.Xiv;
using SaintCoinach.Xiv.Items;

namespace Nophica
{
    public static class Extensions
    {
        public static string[] GetVisibleMeshParts(this ImcVariant variant) {
            ushort mask = variant.PartVisibilityMask;
            const char start = 'a';
            
            List<string> visibleParts = new List<string>();

            for (byte i = 0; i < 10; i++) {
                if (((mask >> i) & 1) == 1)
                    visibleParts.Add( "_" + (char) (start + i));
            }

            return visibleParts.ToArray();
        }

        public static string[] GetHiddenMeshParts(this ImcVariant variant) {
            ushort mask = variant.PartVisibilityMask;
            const char start = 'a';

            List<string> hiddenParts = new List<string>();

            for (byte i = 0; i < 10; i++) {
                if (((mask >> i) & 1) == 0)
                    hiddenParts.Add("_" + (char)(start + i));
            }
            
            return hiddenParts.ToArray();
        }

        public static bool IsHiddenForVariant(this ModelAttribute attr, ImcVariant variant) {
            return IsHiddenForVariant(attr, GetHiddenMeshParts(variant));
        }

        public static bool IsHiddenForVariant(this ModelAttribute attr, string[] invisMeshAttrs) {
            foreach (string t in invisMeshAttrs)
                if (attr.Name.EndsWith(t))
                    return true;
            return false;
        }

        public static bool ShouldDisplay(this MeshPart part, string[] invisMeshAttrs)
        {
            bool display = true;

            foreach (ModelAttribute attr in part.Attributes)
                foreach (string visible in invisMeshAttrs)
                    if (attr.Name.EndsWith(visible))
                        display = false;

            return display;
        }

        public static bool Contains(this List<Range> ranges, int index) {
            foreach (Range range in ranges) {
                if (range.Contains(index))
                    return true;
            }

            return false;
        }

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

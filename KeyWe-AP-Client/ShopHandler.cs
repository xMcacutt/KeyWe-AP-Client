using System;
using HarmonyLib;
using Object = UnityEngine.Object;

namespace KeyWe_AP_Client;

public class ShopHandler
{
    [HarmonyPatch(typeof(Wardrobe))]
    public class Wardrobe_Patch
    {
        [HarmonyPatch("ShowDisplayHalves")]
        [HarmonyPostfix]
        public static void OnShowDisplayHalves(Wardrobe __instance)
        {
            __instance.itemDisplays[1].SetActive(false);
        }
    }

    [HarmonyPatch(typeof(PlayerItemDisplay))]
    public class PlayerItemDisplay_Patch
    {
        public static void AddCategory(PlayerItemDisplay __instance, string categoryName, Customizables.Categories category, int index)
        {
            var addOnSelected    = AccessTools.Method(typeof(CategoryDisplay), "add_OnSelected");
            var addOnHighlighted = AccessTools.Method(typeof(CategoryDisplay), "add_OnHighlighted");

            var cat = Object.Instantiate(__instance.categoryDisplayPrefab, __instance.categoryParent);
            __instance.categories[index] = cat;

            cat.Init(categoryName, category);

            var selectedHandler = Delegate.CreateDelegate(
                addOnSelected.GetParameters()[0].ParameterType,
                __instance,
                nameof(PlayerItemDisplay.Category_OnSelected));

            var highlightedHandler = Delegate.CreateDelegate(
                addOnHighlighted.GetParameters()[0].ParameterType,
                __instance,
                nameof(PlayerItemDisplay.Category_OnHighlighted));
            
            addOnSelected.Invoke(cat, [selectedHandler]);
            addOnHighlighted.Invoke(cat, [highlightedHandler]);
        }
        
        [HarmonyPatch("PopulateCategoryUI")]
        [HarmonyPrefix]
        public static bool OnPopulateCategoryUI(PlayerItemDisplay __instance)
        {
            __instance.categories = new CategoryDisplay[2];
            AddCategory(__instance, "Stamp Shop", Customizables.Categories.Face, 0);
            AddCategory(__instance, "Coin Shop", Customizables.Categories.Hat, 1);
            return false;
        }
    }

    [HarmonyPatch(typeof(CategoryDisplay))]
    public class CategoryDisplay_Patch
    {
        [HarmonyPatch("Init")]
        [HarmonyPrefix]
        public static bool Init(CategoryDisplay __instance, string text, Customizables.Categories category)
        {
            if (category is not Customizables.Categories.Face and not Customizables.Categories.Hat)
            {
                __instance.SetActive(false);
                return false;
            }
            __instance.CategoryName = category ==  Customizables.Categories.Hat ? "Coin Shop" : "Stamp Shop";
            __instance.Category = Customizables.Categories.None;
            __instance.textMesh.text = __instance.CategoryName;
            return false;
        }
        
    }
}
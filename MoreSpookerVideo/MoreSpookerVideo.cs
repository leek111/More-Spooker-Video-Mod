using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MoreSpookerVideo
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class MoreSpookerVideo : BaseUnityPlugin
    {
        public static MoreSpookerVideo Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger { get; private set; } = null!;
        internal static Harmony? Harmony { get; set; }

        internal static ConfigEntry<int>? CameraPrice { get; private set; }

        public static List<Item> AllItems => Resources.FindObjectsOfTypeAll<Item>().Concat(FindObjectsByType<Item>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID)).ToList();

        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;

            CameraPrice = Config.Bind("General", "CameraPrice", 450, "The price of camera in shop");

            AddShopItem();
            Patch();

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
        }

        internal static void Patch()
        {
            Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

            Logger.LogDebug("Patching...");

            Harmony.PatchAll();

            Logger.LogDebug("Finished patching!");
        }

        private void AddShopItem()
        {
            Item? item = AllItems.FirstOrDefault(item => item.itemType.Equals(Item.ItemType.Camera) && item.displayName.ToLower().StartsWith("camera"));
            
            if (item != null)
            {
                item.purchasable = true;
                item.price = CameraPrice!.Value;
                item.quantity = 1;
                item.Category = ShopItemCategory.Gadgets;

                Logger?.LogWarning($"{item.displayName} added to shop for {item.price}$ !");
                return;
            }

            Logger?.LogWarning("No item catched for shop...");
        }
    }
}

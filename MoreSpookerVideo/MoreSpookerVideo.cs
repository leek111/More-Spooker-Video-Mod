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

        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;

            CameraPrice = Config.Bind("General", "CameraPrice", 50,
            "The price of camera in shop");

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

        internal static void Unpatch()
        {
            Logger.LogDebug("Unpatching...");

            Harmony?.UnpatchSelf();

            Logger.LogDebug("Finished unpatching!");
        }

        private void AddShopItem()
        {
            List<Item> AllItems = Resources.FindObjectsOfTypeAll<Item>().Concat(FindObjectsByType<Item>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID)).ToList();

            Item? cameraItem = AllItems.FirstOrDefault(item => item.itemType.Equals(Item.ItemType.Camera));

            if (cameraItem != null)
            {
                cameraItem.purchasable = true;
                cameraItem.price = CameraPrice.Value;
                cameraItem.quantity = 1;
                cameraItem.Category = ShopItemCategory.Gadgets;

                Logger?.LogWarning("Add Camera in shop !");
                return;
            }

            Logger?.LogWarning("No Camera catched for shop...");
        }
    }
}

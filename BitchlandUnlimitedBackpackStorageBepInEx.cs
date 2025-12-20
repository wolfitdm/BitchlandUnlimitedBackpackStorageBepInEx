using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;
using SemanticVersioning;
using System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine;

namespace BitchlandUnlimitedBackpackStorageBepInEx
{
    [BepInPlugin("com.wolfitdm.BitchlandUnlimitedBackpackStorageBepInEx", "BitchlandUnlimitedBackpackStorageBepInEx Plugin", "1.0.0.0")]
    public class BitchlandUnlimitedBackpackStorageBepInEx : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        private ConfigEntry<bool> configEnableMe;

        public BitchlandUnlimitedBackpackStorageBepInEx()
        {
        }

        public static Type MyGetType(string originalClassName)
        {
            return Type.GetType(originalClassName + ",Assembly-CSharp");
        }

        private static string pluginKey = "General.Toggles";

        public static bool enableThisMod = false;

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;

            configEnableMe = Config.Bind(pluginKey,
                                              "EnableThisMod",
                                              true,
                                             "Whether or not you want enable this mod (default true also yes, you want it, and false = no)");


            enableThisMod = configEnableMe.Value;

            Harmony.CreateAndPatchAll(typeof(BitchlandUnlimitedBackpackStorageBepInEx));

            Logger.LogInfo($"Plugin BitchlandUnlimitedBackpackStorageBepInEx BepInEx is loaded!");
        }

        [HarmonyPatch(typeof(int_PickupToBag), "CheckCanInteract")]
        [HarmonyPrefix] // call after the original method is called
        public static bool CheckCanInteract(Person person, object __instance)
        {
            if (!enableThisMod)
            {
                return true;
            }

            int_PickupToBag _this = (int_PickupToBag)__instance;

            if (Main.Instance.Player.CurrentBackpack != null)
            {
                try
                {
                    Main.Instance.Player.CurrentBackpack.ThisStorage.StorageMax = int.MaxValue;
                }
                catch (Exception ex)
                {
                }
            }

            if (person.CurrentBackpack != null)
            {
                try
                {
                    person.CurrentBackpack.ThisStorage.StorageMax = int.MaxValue;
                }
                catch (Exception ex)
                {
                }
            }

            return true;
        }

        [HarmonyPatch(typeof(int_PickupToBag), "Interact")]
        [HarmonyPrefix] // call after the original method is called
        public static bool Interact(Person person, object __instance)
        {
            if (!enableThisMod)
            {
                return true;
            }

            int_PickupToBag _this = (int_PickupToBag)__instance;

            if (Main.Instance.Player.CurrentBackpack != null)
            {
                try
                {
                    Main.Instance.Player.CurrentBackpack.ThisStorage.StorageMax = int.MaxValue;
                }
                catch (Exception ex)
                {
                }
            }

            if (person.CurrentBackpack != null)
            {
                try
                {
                    person.CurrentBackpack.ThisStorage.StorageMax = int.MaxValue;
                }
                catch (Exception ex)
                {
                }
            }

            return true;
        }

        [HarmonyPatch(typeof(misc_invItem), "Click_TakeButton")]
        [HarmonyPrefix] // call after the original method is called
        public static bool Click_TakeButton(object __instance)
        {
            if (!enableThisMod)
            {
                return true;
            }

            if (Main.Instance.Player.CurrentBackpack != null)
            {
                try
                {
                    Main.Instance.Player.CurrentBackpack.ThisStorage.StorageMax = int.MaxValue;
                }
                catch (Exception ex)
                {
                }
            }

            return true;
        }
    }
}

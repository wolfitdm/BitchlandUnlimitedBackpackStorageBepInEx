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

            PatchAllHarmonyMethods();

            Logger.LogInfo($"Plugin BitchlandUnlimitedBackpackStorageBepInEx BepInEx is loaded!");
        }

        public static void PatchAllHarmonyMethods()
        {
            if (!enableThisMod)
            {
                return;
            }

            try
            {
                PatchHarmonyMethodUnity(typeof(misc_invItem), "Click_BackpackEquip", "Click_BackpackEquip", true, false);

                PatchHarmonyMethodUnity(typeof(misc_invItem), "Click_Opem", "Click_Opem", true, false);

                PatchHarmonyMethodUnity(typeof(misc_invItem), "Click_PutButton", "Click_PutButton", true, false);

                PatchHarmonyMethodUnity(typeof(misc_invItem), "Click_TakeButton", "Click_TakeButton", true, false);

                PatchHarmonyMethodUnity(typeof(int_PickupToBag), "Interact", "Interact", true, false);

                PatchHarmonyMethodUnity(typeof(int_PickupToBag), "CheckCanInteract", "CheckCanInteract", true, false);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }
        }

        public static void PatchHarmonyMethodUnity(Type originalClass, string originalMethodName, string patchedMethodName, bool usePrefix, bool usePostfix, Type[] parameters = null)
        {
            string uniqueId = "com.wolfitdm.BitchlandUnlimitedBackpackStorageBepInEx";
            Type uniqueType = typeof(BitchlandUnlimitedBackpackStorageBepInEx);

            // Create a new Harmony instance with a unique ID
            var harmony = new Harmony(uniqueId);

            if (originalClass == null)
            {
                Logger.LogInfo($"GetType originalClass == null");
                return;
            }

            MethodInfo patched = null;

            try
            {
                patched = AccessTools.Method(uniqueType, patchedMethodName);
            }
            catch (Exception ex)
            {
                patched = null;
            }

            if (patched == null)
            {
                Logger.LogInfo($"AccessTool.Method patched {patchedMethodName} == null");
                return;

            }

            // Or apply patches manually
            MethodInfo original = null;

            try
            {
                if (parameters == null)
                {
                    original = AccessTools.Method(originalClass, originalMethodName);
                }
                else
                {
                    original = AccessTools.Method(originalClass, originalMethodName, parameters);
                }
            }
            catch (AmbiguousMatchException ex)
            {
                Type[] nullParameters = new Type[] { };
                try
                {
                    if (patched == null)
                    {
                        parameters = nullParameters;
                    }

                    ParameterInfo[] parameterInfos = patched.GetParameters();

                    if (parameterInfos == null || parameterInfos.Length == 0)
                    {
                        parameters = nullParameters;
                    }

                    List<Type> parametersN = new List<Type>();

                    for (int i = 0; i < parameterInfos.Length; i++)
                    {
                        ParameterInfo parameterInfo = parameterInfos[i];

                        if (parameterInfo == null)
                        {
                            continue;
                        }

                        if (parameterInfo.Name == null)
                        {
                            continue;
                        }

                        if (parameterInfo.Name.StartsWith("__"))
                        {
                            continue;
                        }

                        Type type = parameterInfos[i].ParameterType;

                        if (type == null)
                        {
                            continue;
                        }

                        parametersN.Add(type);
                    }

                    parameters = parametersN.ToArray();
                }
                catch (Exception ex2)
                {
                    parameters = nullParameters;
                }

                try
                {
                    original = AccessTools.Method(originalClass, originalMethodName, parameters);
                }
                catch (Exception ex2)
                {
                    original = null;
                }
            }
            catch (Exception ex)
            {
                original = null;
            }

            if (original == null)
            {
                Logger.LogInfo($"AccessTool.Method original {originalMethodName} == null");
                return;
            }

            HarmonyMethod patchedMethod = new HarmonyMethod(patched);
            var prefixMethod = usePrefix ? patchedMethod : null;
            var postfixMethod = usePostfix ? patchedMethod : null;

            harmony.Patch(original,
                prefix: prefixMethod,
                postfix: postfixMethod);
        }
        public static bool CheckCanInteract(Person person, object __instance)
        {
            if (!enableThisMod)
            {
                return true;
            }


            int_PickupToBag _this = (int_PickupToBag)__instance;

            bool haveBackpack = false;

            try
            {
                haveBackpack = Main.Instance.Player.CurrentBackpack != null;
            }
            catch (Exception ex)
            {
                haveBackpack = false;
            }

            if (haveBackpack)
            {
                try
                {
                    Main.Instance.Player.CurrentBackpack.ThisStorage.StorageMax = int.MaxValue;
                }
                catch (Exception ex)
                {
                }
            }

            try
            {
                haveBackpack = person.CurrentBackpack != null;
            }
            catch (Exception ex)
            {
                haveBackpack = false;
            }

            if (haveBackpack)
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
        public static bool Interact(Person person, object __instance)
        {
            if (!enableThisMod)
            {
                return true;
            }

            int_PickupToBag _this = (int_PickupToBag)__instance;

            bool haveBackpack = false;

            try
            {
                haveBackpack = Main.Instance.Player.CurrentBackpack != null;
            }
            catch (Exception ex)
            {
                haveBackpack = false;
            }

            if (haveBackpack)
            {
                try
                {
                    Main.Instance.Player.CurrentBackpack.ThisStorage.StorageMax = int.MaxValue;
                }
                catch (Exception ex)
                {
                }
            }

            try
            {
                haveBackpack = person.CurrentBackpack != null;
            } catch (Exception ex)
            {
                haveBackpack = false;
            }

            if (haveBackpack)
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
        public static bool Click_TakeButton(object __instance)
        {
            if (!enableThisMod)
            {
                return true;
            }

            bool haveBackpack = false;

            try
            {
                haveBackpack = Main.Instance.Player.CurrentBackpack != null;
            }
            catch (Exception ex)
            {
                haveBackpack = false;
            }

            if (haveBackpack)
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
        public static bool Click_PutButton(object __instance)
        {
            if (!enableThisMod)
            {
                return true;
            }

            bool haveBackpack = false;

            try
            {
                haveBackpack = Main.Instance.Player.CurrentBackpack != null;
            }
            catch (Exception ex)
            {
                haveBackpack = false;
            }

            if (haveBackpack)
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
        public static bool Click_Opem(object __instance)
        {
            if (!enableThisMod)
            {
                return true;
            }

            bool haveBackpack = false;

            try
            {
                haveBackpack = Main.Instance.Player.CurrentBackpack != null;
            } catch(Exception ex)
            {
                haveBackpack = false;
            }

            if (haveBackpack)
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

        [HarmonyPatch(typeof(misc_invItem), "Click_BackpackEquip")]
        [HarmonyPrefix] // call before the original method is called
        public static bool Click_BackpackEquip(object __instance)
        {
            if (!enableThisMod)
            {
                return true;
            }

            bool haveBackpack = false;

            try
            {
                haveBackpack = Main.Instance.Player.CurrentBackpack != null;
            }
            catch (Exception ex)
            {
                haveBackpack = false;
            }

            if (haveBackpack)
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

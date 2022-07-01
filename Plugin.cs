using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Strongman
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        static ManualLogSource logger;
        static ConfigEntry<int> maxShingles;
        static ConfigEntry<int> maxBricks;
        static ConfigEntry<int> maxPlasterboard;
        static ConfigEntry<float> weightLimit;

        private void Awake()
        {
            maxShingles = Config.Bind("General", "Max Shingles", 20, "Max shingles that can be carried (Vanilla:12)");
            maxBricks = Config.Bind("General", "Max Bricks", 20, "Max bricks that can be carried (Vanilla:11)");
            maxPlasterboard = Config.Bind("General", "Max Plasterboards", 20, "Max plasterboards that can be carried (Vanilla:8 - only in game version 0.08.02 and up)");
            weightLimit = Config.Bind("General", "Weight Limit", 100000f, "Weight that can be lifted and still sprint/jump (Vanilla:10)");

            // Plugin startup logic
            logger = Logger;
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded");
            Logger.LogInfo($"Patching...");
            Harmony.CreateAndPatchAll(typeof(Plugin));
            Logger.LogInfo($"Patched");
        }

        [HarmonyPatch(typeof(HandToFront), "Awake")]
        [HarmonyPostfix]
        static void Awake_Postfix(HandToFront __instance, ref HandToFront.GenericStuffs ____refLibrary)
        {
            FieldInfo maxPlasterBoardAmmo = AccessTools.Field(typeof(HandToFront), "maxPlasterBoardAmmo");

            if (maxPlasterBoardAmmo != null)
            {
                maxPlasterBoardAmmo.SetValue(__instance, maxPlasterboard.Value);
            }

            __instance.maxShingleAmmo = maxShingles.Value;
            __instance.maxBrickAmmo = maxBricks.Value;

            int brickSlotsNeeded = Math.Max(maxBricks.Value, maxPlasterboard.Value);

            Array.Resize<Transform>(ref ____refLibrary.shingleHolder.GetComponent<BrickCarrierLogic>().brickLocs, __instance.maxShingleAmmo);
            Array.Resize<Transform>(ref ____refLibrary.brickHolder.GetComponent<BrickCarrierLogic>().brickLocs, brickSlotsNeeded);
            for (int i = 0; i < ____refLibrary.shingleHolder.GetComponent<BrickCarrierLogic>().brickLocs.Length; i++)
            {
                if (____refLibrary.shingleHolder.GetComponent<BrickCarrierLogic>().brickLocs[i] == null)
                {
                    ____refLibrary.shingleHolder.GetComponent<BrickCarrierLogic>().brickLocs[i] = Object.Instantiate<Transform>(____refLibrary.shingleHolder.GetComponent<BrickCarrierLogic>().brickLocs[i - 1]).transform;
                    ____refLibrary.shingleHolder.GetComponent<BrickCarrierLogic>().brickLocs[i].position = ____refLibrary.shingleHolder.GetComponent<BrickCarrierLogic>().brickLocs[i - 1].transform.position;
                }
            }
            for (int j = 0; j < ____refLibrary.brickHolder.GetComponent<BrickCarrierLogic>().brickLocs.Length; j++)
            {
                if (____refLibrary.brickHolder.GetComponent<BrickCarrierLogic>().brickLocs[j] == null)
                {
                    ____refLibrary.brickHolder.GetComponent<BrickCarrierLogic>().brickLocs[j] = Object.Instantiate<Transform>(____refLibrary.brickHolder.GetComponent<BrickCarrierLogic>().brickLocs[j - 1]).transform;
                    ____refLibrary.brickHolder.GetComponent<BrickCarrierLogic>().brickLocs[j].position = ____refLibrary.brickHolder.GetComponent<BrickCarrierLogic>().brickLocs[j - 1].transform.position;
                }
            }
        }

        [HarmonyPatch(typeof(vp_FPInput), "HoldingHeavyObject")]
        [HarmonyPatch(typeof(HandToFront), "HaulingObject")]
        [HarmonyPatch(typeof(HandToFront), "StopHaulingObject")]
        [HarmonyPatch(typeof(HandToFront), "EquipPickUp", MethodType.Enumerator)]
        [HarmonyPatch(typeof(HandToFront), "UnlockCameraAndMovement")]
        [HarmonyPatch(typeof(Pause), "UnTabGame")]
        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> MassCheck_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatcher matcher = new CodeMatcher(instructions)
            .MatchForward(false, // false = move at the start of the match, true = move at the end of the match
                new CodeMatch(i =>
                {
                    return
                    (i.opcode == OpCodes.Callvirt && i.operand == AccessTools.Method(typeof(Rigidbody), "get_mass")) ||
                    (i.opcode == OpCodes.Ldfld && i.operand == AccessTools.Field(typeof(ItemDetails), "weight"));
                }),
                new CodeMatch(OpCodes.Ldc_R4, 10f),
                new CodeMatch(i => i.Branches(out Label? label))
            );

            matcher.Repeat(matcher =>
            {
                matcher.Advance(1).SetOperandAndAdvance(weightLimit.Value);
            });
            
            return matcher.InstructionEnumeration();
        }
    }
}

using System;
using BepInEx;
using UnityEngine;

namespace HandsSimulatorVRMod {
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin {

        bool vrMode = false;
        bool isVrMode() {
            string[] commandLineArgs = Environment.GetCommandLineArgs();
            for (int i = 0; i < commandLineArgs.Length; i++)
            {
                if (commandLineArgs[i] == "-vr")
                {
                    return true;
                }
            }
            return false;
        }

        void OnEnable() {
            if (!isVrMode()) {
                return;
            }
            InputSystem.InitControllers();
            HarmonyPatches.ApplyHarmonyPatches();


        }

        void OnDisable() {
            HarmonyPatches.RemoveHarmonyPatches();
        }
    }
}

using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using EasyBlasting.Patches;
using HarmonyLib;
using UnityEngine;

namespace EasyBlasting
{
    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class EasyBlastingPlugin : BaseUnityPlugin
    {
        private const string MyGUID = "com.equinox.EasyBlasting";
        private const string PluginName = "EasyBlasting";
        private const string VersionString = "1.0.0";

        private static readonly Harmony Harmony = new Harmony(MyGUID);
        public static ManualLogSource Log = new ManualLogSource(PluginName);

        // Config Entries

        public static ConfigEntry<bool> EasyBlastMiners;
        public static ConfigEntry<bool> EasyBlastSmelters;
        public static ConfigEntry<bool> PauseProcessing;


        // Unity Functions

        private void Awake() {
            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loading...");
            Harmony.PatchAll();

            ApplyPatches();
            CreateConfigEntries();

            Logger.LogInfo($"PluginName: {PluginName}, VersionString: {VersionString} is loaded.");
            Log = Logger;
        }

        private void Update() {
            // ToDo: Delete If Not Needed
        }

        // Private Functions

        private void ApplyPatches() {
            Harmony.CreateAndPatchAll(typeof(BlastSmelterInstancePatch));
            Harmony.CreateAndPatchAll(typeof(DrillInstancePatch));
        }

        private void CreateConfigEntries() {
            EasyBlastMiners = Config.Bind("General", "Easy Blast Miners", true, new ConfigDescription("When true, miners will not waste mining charges"));
            EasyBlastSmelters = Config.Bind("General", "Easy Blast Smelters", true, new ConfigDescription("When true, blast smelters will only consume 1 charge per cycle"));
            PauseProcessing = Config.Bind("General", "Pause Processing", true, new ConfigDescription("When true, blast machines will stop processing when their output is full"));
        }
    }
}

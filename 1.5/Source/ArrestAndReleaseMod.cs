using Verse;
using HarmonyLib;

namespace ArrestAndRelease
{
    public class ArrestAndReleaseMod : Mod
    {
        public const string PACKAGE_ID = "arrestandrelease.1trickPonyta";
        public const string PACKAGE_NAME = "Arrest and Release";

        public ArrestAndReleaseMod(ModContentPack content) : base(content)
        {
            var harmony = new Harmony(PACKAGE_ID);
            harmony.PatchAll();

            Log.Message($"[{PACKAGE_NAME}] Loaded.");
        }
    }
}

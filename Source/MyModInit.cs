using HarmonyLib;
using Verse;

namespace MyCustomBedMod
{
    [StaticConstructorOnStartup]
    public static class MyModInit
    {
        static MyModInit()
        {
            var harmony = new Harmony("com.yourname.mycustombedmod");
            harmony.PatchAll();
        }
    }
}
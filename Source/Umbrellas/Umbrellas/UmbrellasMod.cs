using Verse;
using HarmonyLib;

namespace Umbrellas {
    public class UmbrellasMod : Mod {
        public UmbrellasMod(ModContentPack content) : base(content) {
            UmbrellasPatcher.DoPatching();
        }
    }
}
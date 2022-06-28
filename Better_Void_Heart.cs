using Modding;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Satchel;

using IntCompare = HutongGames.PlayMaker.Actions.IntCompare;
using UObject = UnityEngine.Object;

namespace Better_Void_Heart
{
    public class Better_Void_Heart : Mod, IMenuMod, ITogglableMod, IGlobalSettings<Settings>
    {
        internal static Better_Void_Heart Instance;

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing");

            Instance = this;
            On.PlayMakerFSM.OnEnable += OnFSM;

            Log("Initialized");
        }

        public void Unload()
        {
            On.PlayMakerFSM.OnEnable -= OnFSM;
        }


        new public string GetName() => "Better Void Heart";
        public override string GetVersion() => "v1.0";

        public Settings settings = new Settings();
        public void OnLoadGlobal(Settings s) => settings = s;
        public Settings OnSaveGlobal() => settings;

        private void OnFSM(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);
            if (self.gameObject.name == "Charm Effects" && self.FsmName == "White Charm")
            {
                IntCompare intCompare = self.GetAction<IntCompare>("Check", 2);
                intCompare.greaterThan = FsmEvent.GetFsmEvent("ACTIVE");

                SendMessageV2 addSoulAction = self.GetAction<SendMessageV2>("Soul UP", 0);
                addSoulAction.functionCall.IntParameter = settings.soulRegenAmount;

            }
        }

        private List<int> soulRegenAmounts = new List<int> { 1, 2, 4, 7, 11 };

        public bool ToggleButtonInsideMenu => true;
        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry) =>
            new List<IMenuMod.MenuEntry> {
                toggleButtonEntry.Value,
                new IMenuMod.MenuEntry {
                    Name = "Soul Regen Amount",
                    Description = "Affects both Kingsoul and Void Heart. Default is 4.",
                    Values = new string[] {"1", "2", "4", "7", "11" }, // I can't seem to use Cast<string> here, even when using Linq. Weird.
                    Saver = option => settings.soulRegenAmount = soulRegenAmounts[option],
                    Loader = () => soulRegenAmounts.IndexOf(settings.soulRegenAmount) >= 0 ? soulRegenAmounts.IndexOf(settings.soulRegenAmount) : soulRegenAmounts.IndexOf(4)
                }
            };

    }
}
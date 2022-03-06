using BepInEx;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace TeamScaledInteractables
{
    [BepInPlugin("com.Moffein.TeamScaledInteractables", "Team Scaled Interactables", "1.0.2")]
    public class TeamScaledInteractables : BaseUnityPlugin
    {
        public static bool scaleCombat = true;
        public static bool scaleBoss = true;
        public static bool scaleVoid = true;
        public void Awake()
        {
            //shrineBoss = LegacyResourcesAPI.Load<SpawnCard>("spawncards/interactablespawncard/iscshrineboss");
            //shrineCombat = LegacyResourcesAPI.Load<SpawnCard>("spawncards/interactablespawncard/iscshrinecombat");
            //voidSeed = LegacyResourcesAPI.Load<SpawnCard>("spawncards/interactablespawncard/iscvoidcamp");    //Why isn't this detected?

            scaleCombat = Config.Bind("Settings", "Shrine of Combat", true, "Apply scaling to Shrines of Combat.").Value;
            scaleBoss= Config.Bind("Settings", "Shrine of the Mountain", true, "Apply scaling to Shrines of the Mountain.").Value;
            scaleVoid = Config.Bind("Settings", "Void Seed", true, "Apply scaling to Void Seeds.").Value;

            IL.RoR2.SceneDirector.PopulateScene += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                    x => x.MatchCallvirt<DirectorCard>("get_cost")
                    );
                c.Index++;
                c.Emit(OpCodes.Ldloc_2);    //DirectorCard
                c.EmitDelegate<Func<int, DirectorCard, int>>((cost, card) =>
                {
                    if ((scaleCombat && card.spawnCard.name == "iscShrineCombat") || (scaleBoss && card.spawnCard.name == "iscShrineBoss") || (scaleVoid && card.spawnCard.name == "iscVoidCamp"))
                    {
                        cost = (int)(cost * (1f + 0.5f * (Run.instance.participatingPlayerCount - 1)));
                    }
                    return cost;
                });
            };
        }
    }
}

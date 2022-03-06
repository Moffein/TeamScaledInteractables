using BepInEx;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace TeamScaledInteractables
{
    [BepInPlugin("com.Moffein.TeamScaledInteractables", "Team Scaled Interactables", "1.0.1")]
    public class TeamScaledInteractables : BaseUnityPlugin
    {
        public void Awake()
        {
            //shrineBoss = LegacyResourcesAPI.Load<SpawnCard>("spawncards/interactablespawncard/iscshrineboss");
            //shrineCombat = LegacyResourcesAPI.Load<SpawnCard>("spawncards/interactablespawncard/iscshrinecombat");
            //voidSeed = LegacyResourcesAPI.Load<SpawnCard>("spawncards/interactablespawncard/iscvoidcamp");    //Why isn't this detected?

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
                    if (card.spawnCard.name == "iscShrineCombat" || card.spawnCard.name == "iscShrineBoss" || card.spawnCard.name == "iscVoidCamp")
                    {
                        cost = (int)(cost * (1f + 0.5f * (Run.instance.participatingPlayerCount - 1)));
                    }
                    return cost;
                });
            };
        }
    }
}

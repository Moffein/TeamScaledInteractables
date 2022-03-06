using BepInEx;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace TeamScaledInteractables
{
    [BepInPlugin("com.Moffein.TeamScaledInteractables", "Team Scaled Interactables", "1.0.0")]
    public class TeamScaledInteractables : BaseUnityPlugin
    {
        SpawnCard shrineBoss;
        SpawnCard shrineCombat;
        SpawnCard voidSeed;

        public void Awake()
        {
            shrineBoss = LegacyResourcesAPI.Load<SpawnCard>("spawncards/interactablespawncard/iscshrineboss");
            shrineCombat = LegacyResourcesAPI.Load<SpawnCard>("spawncards/interactablespawncard/iscshrinecombat");
            voidSeed = LegacyResourcesAPI.Load<SpawnCard>("spawncards/interactablespawncard/iscvoidsuppressor");

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
                    if (card.spawnCard == shrineBoss || card.spawnCard == shrineCombat || card.spawnCard == voidSeed)
                    {
                        cost = (int)(cost * (1f + 0.5f * (Run.instance.participatingPlayerCount - 1)));
                    }
                    return cost;
                });
            };
        }
    }
}

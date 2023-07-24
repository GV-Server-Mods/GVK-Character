using System;
using System.Collections.Generic;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Weapons;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.Utils;
using VRageMath;


namespace Klime.StarterGrinder
{
    [MySessionComponentDescriptor(MyUpdateOrder.BeforeSimulation)]
    public class StarterGrinder : MySessionComponentBase
    {
        IMySlimBlock reuse_slim;
        IMyAngleGrinder reuse_grinder;
        IMyFaction reuse_faction;
        IMyFaction reuse_faction_grinder;
        string grinder_sub = "AngleGrinder";
        float multiplier = 1.2f;

        public override void Init(MyObjectBuilder_SessionComponent sessionComponent)
        {
            if (MyAPIGateway.Session.IsServer)
            {
                MyAPIGateway.Session.DamageSystem.RegisterBeforeDamageHandler(0, grinder_handler);
            }
        }

        public override void LoadData()
        {
            if (!MyAPIGateway.Utilities.GetVariable<float>("basic_grinder_multiplier", out multiplier))
            {
                multiplier = 1.2f;
                MyAPIGateway.Utilities.SetVariable<float>("basic_grinder_multiplier", multiplier);
            }
        }

        private void grinder_handler(object target, ref MyDamageInformation info)
        {
            try
            {
                reuse_slim = target as IMySlimBlock;
                if (reuse_slim != null)
                {
                    reuse_grinder = MyAPIGateway.Entities.GetEntityById(info.AttackerId) as IMyAngleGrinder;
                    if (reuse_grinder != null && reuse_grinder.DefinitionId.SubtypeName == grinder_sub)
                    {
                        var slim_owner_id = reuse_slim.OwnerId;
                        var slim_built_id = reuse_slim.BuiltBy;
                        var grinder_owner = reuse_grinder.OwnerIdentityId;

                        if (slim_owner_id == 0)
                        {
                            if (reuse_slim.FatBlock != null)
                            {
                                info.Amount *= multiplier;
                                return;
                            }

                            if (slim_built_id == 0)
                            {
                                info.Amount *= multiplier;
                                return;
                            }

                            if (grinder_owner == slim_built_id)
                            {
                                info.Amount *= multiplier;
                                return;
                            }

                            reuse_faction = null;
                            reuse_faction_grinder = null;

                            reuse_faction = MyAPIGateway.Session.Factions.TryGetPlayerFaction(slim_built_id);
                            reuse_faction_grinder = MyAPIGateway.Session.Factions.TryGetPlayerFaction(grinder_owner);

                            if (reuse_faction != null && reuse_faction_grinder != null && reuse_faction.FactionId == reuse_faction_grinder.FactionId)
                            {
                                info.Amount *= multiplier;
                                return;
                            }

                            info.Amount = 0f;
                        }
                        else
                        {
                            if (slim_owner_id == grinder_owner)
                            {
                                info.Amount *= multiplier;
                                return;
                            }

                            reuse_faction = null;
                            reuse_faction_grinder = null;

                            reuse_faction = MyAPIGateway.Session.Factions.TryGetPlayerFaction(slim_owner_id);
                            reuse_faction_grinder = MyAPIGateway.Session.Factions.TryGetPlayerFaction(grinder_owner);

                            if (reuse_faction != null && reuse_faction_grinder != null && reuse_faction.FactionId == reuse_faction_grinder.FactionId)
                            {
                                info.Amount *= multiplier;
                                return;
                            }
                            info.Amount = 0f;
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        protected override void UnloadData()
        {
        }
    }
}
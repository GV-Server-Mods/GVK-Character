using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using SpaceEngineers.Game.ModAPI;
using Sandbox.ModAPI;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using VRage.Game;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.Game.Components;
using VRage.Utils;
using VRageMath;

namespace AlwaysSpawnWithoutHydrogen 
{

    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class Main : MySessionComponentBase 
	{

        public override void Init(MyObjectBuilder_SessionComponent sessionComponent) 
		{
            base.Init(sessionComponent);
            MyVisualScriptLogicProvider.PlayerSpawned += PlayerSpawned;

        }

        protected override void UnloadData() 
		{
            base.UnloadData();
            MyVisualScriptLogicProvider.PlayerSpawned -= PlayerSpawned;
        }

        private void PlayerSpawned(long playerId) 
		{
            IMyIdentity playerIdentity = Player(playerId);
            if (playerIdentity != null) 
			{
                var playerList = new List<IMyPlayer>();
                MyAPIGateway.Players.GetPlayers(playerList, p => p != null && p.IdentityId == playerIdentity.IdentityId);
                var player = playerList.FirstOrDefault();
                if (player != null) 
                    MyVisualScriptLogicProvider.SetPlayersHydrogenLevel(playerIdentity.IdentityId, 0f);
                    //MyVisualScriptLogicProvider.SetPlayersOxygenLevel(playerIdentity.IdentityId, 0f);
            }
        }

        private IMyIdentity Player(long entityId) 
		{
            try 
			{
                List<IMyIdentity> listIdentities = new List<IMyIdentity>();
                MyAPIGateway.Players.GetAllIdentites(listIdentities,
                    p => p != null && p.DisplayName != "" && p.IdentityId == entityId);
                if (listIdentities.Count == 1) return listIdentities[0];
                return null;
            } 
			catch (Exception e) 
			{
                return null;
            }
        }
    }
}
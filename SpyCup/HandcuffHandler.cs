﻿using System;
using Smod2;
using Smod2.API;
using System.Collections.Generic;
using System.Threading;

namespace SpyCup
{
	class HandcuffHandler
	{
		public HandcuffHandler(SpyCup sc, EventHandler ev)
		{
			while (sc.roundStarted)
			{
				System.Threading.Thread.Sleep(1000);

				if (sc.RoleDict.Count > 0)
				{
					foreach (Player player in sc.pluginManager.Server.GetPlayers())
					{
						if (player.IsHandcuffed() && sc.RoleDict.ContainsKey(player.SteamId) && player.GetHealth() > 0 && player.TeamRole.Team.Equals(Team.NINETAILFOX))
						{
							Vector pos = player.GetPosition();
							player.ChangeRole(Role.CHAOS_INSUGENCY);
							foreach (Item item in player.GetInventory()) { item.Remove(); }
							player.Teleport(pos);
						}
					}
				}
			}
		}
	}
}
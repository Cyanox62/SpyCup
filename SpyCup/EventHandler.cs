using System;
using Smod2;
using Smod2.API;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2.EventSystem.Events;
using System.Collections.Generic;
using System.Threading;

namespace SpyCup
{
	class EventHandler : IEventHandlerRoundStart, IEventHandlerPlayerDropItem, IEventHandlerPlayerDie, IEventHandlerPlayerHurt, IEventHandlerTeamRespawn, IEventHandlerSetRole
	{
		private Plugin plugin;
		SpyCup sc;
		Random rand = new Random();

		public EventHandler(Plugin plugin, SpyCup scc)
		{
			this.plugin = plugin;
			sc = scc;
		}

		public void ChangeSpyRole(Smod2.API.Team team, Player player)
		{
			List<Smod2.API.Item> Inventory = player.GetInventory();
			Vector pos = player.GetPosition();
			UnityEngine.GameObject pObj = (UnityEngine.GameObject)player.GetGameObject();
			float rot = pObj.GetComponent<PlyMovementSync>().rotation;

			int health = player.GetHealth();
			int ammo5 = player.GetAmmo(AmmoType.DROPPED_5);
			int ammo7 = player.GetAmmo(AmmoType.DROPPED_7);
			int ammo9 = player.GetAmmo(AmmoType.DROPPED_9);

			if (team.Equals(Smod2.API.Team.NINETAILFOX))
				player.ChangeRole(Smod2.API.Role.CHAOS_INSUGENCY);
			else if (team.Equals(Smod2.API.Team.CHAOS_INSURGENCY))
				player.ChangeRole(sc.RoleDict[player.SteamId]);

			foreach (Smod2.API.Item item in player.GetInventory()) { item.Remove(); }
			foreach (Smod2.API.Item item in Inventory) { player.GiveItem(item.ItemType); }

			player.SetAmmo(AmmoType.DROPPED_5, ammo5);
			player.SetAmmo(AmmoType.DROPPED_7, ammo7);
			player.SetAmmo(AmmoType.DROPPED_9, ammo9);

			player.Teleport(pos, false);
			player.SetHealth(health);

			Thread ChangeSpyRole = new Thread(new ThreadStart(() => new ChangeSpyRole(sc, player, false)));
			ChangeSpyRole.Start();
			Thread SetRotation = new Thread(new ThreadStart(() => new SetRotation(sc, pObj, rot)));
			SetRotation.Start();
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			if (plugin.GetConfigBool("spycup_enabled"))
			{
				sc.RoleDict.Clear();

				int guardChance = rand.Next(1, 101);
				plugin.Info("Chance of guard spawn: " + guardChance.ToString());
				if (guardChance <= plugin.GetConfigInt("spycup_guard_chance"))
				{
					plugin.Info("Spawning guard spy...");
					List<Player> Guards = new List<Player>();
					List<Player> Players = new List<Player>(plugin.pluginManager.Server.GetPlayers());
					List<Role> Roles = new List<Role>();

					foreach (Player player in Players) { Roles.Add(player.TeamRole.Role); }

					if (Roles.Contains(Role.FACILITY_GUARD))
					{
						foreach (Player player in Players)
						{
							if (player.TeamRole.Role.Equals(Role.FACILITY_GUARD))
								Guards.Add(player);
						}

						Player guardSpy = Guards[rand.Next(Guards.Count)];

						sc.RoleDict.Add(guardSpy.SteamId, Role.FACILITY_GUARD);
						guardSpy.GiveItem(ItemType.CUP);
					}
				}
				//sc.roundStarted = true;

				//Thread HandcuffHandler = new Thread(new ThreadStart(() => new HandcuffHandler(sc, this)));
				//HandcuffHandler.Start();
			}
		}

		/*public void OnRoundEnd(RoundEndEvent ev)
		{
			if (plugin.GetConfigBool("spycup_enabled"))
				sc.roundStarted = false;
		}*/

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			if (plugin.GetConfigBool("spycup_enabled"))
			{
				if (!ev.SpawnChaos)
				{
					Player player = ev.PlayerList[rand.Next(ev.PlayerList.Count)];
					//plugin.Info("Got player " + player.Name);
					Role MTFRole = sc.NTFRoles[rand.Next(sc.NTFRoles.Count)];
					//plugin.Info("Set role to " + MTFRole.ToString());
					sc.RoleDict.Add(player.SteamId, MTFRole);
					//plugin.Info("Added player " + player.Name + " to dict with role " + MTFRole.ToString());

					Thread ChangeSpyRole = new Thread(new ThreadStart(() => new ChangeSpyRole(sc, player, true)));
					ChangeSpyRole.Start();

					plugin.Info(player.Name + " has become a spy.");
				}
			}
		}

		public void OnSetRole(PlayerSetRoleEvent ev)
		{
			if (plugin.GetConfigBool("spycup_enabled"))
			{
				if (ev.Player.TeamRole.Role.Equals(Role.UNASSIGNED)) return;
				if (sc.RoleDict.ContainsKey(ev.Player.SteamId))
				{
					if (!ev.Player.TeamRole.Team.Equals(Smod2.API.Team.NINETAILFOX) && !ev.Player.TeamRole.Team.Equals(Smod2.API.Team.CHAOS_INSURGENCY))
					{
						sc.RoleDict.Remove(ev.Player.SteamId);
						//plugin.Info("Removed player " + ev.Player.Name + " from dict (ONSETROLE)");
					}
				}
			}
		}

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (plugin.GetConfigBool("spycup_enabled"))
			{
				if (ev.Player.TeamRole.Role.Equals(Role.UNASSIGNED)) return;
				if (sc.RoleDict.ContainsKey(ev.Player.SteamId))
				{
					sc.RoleDict.Remove(ev.Player.SteamId);
					//plugin.Info("Removed player " + ev.Player.Name + " from dict (ONPLAYERDIE)");
				}
			}
		}

		public void OnPlayerHurt(PlayerHurtEvent ev)
		{
			if (plugin.GetConfigBool("spycup_enabled"))
			{
				if (ev.Attacker.TeamRole.Team.Equals(Smod2.API.Team.CHAOS_INSURGENCY) || ev.Attacker.TeamRole.Team.Equals(Smod2.API.Team.CLASSD))
				{
					if (sc.RoleDict.ContainsKey(ev.Player.SteamId))
					{
						if (!ev.DamageType.Equals(DamageType.POCKET))
							ev.Damage = 0.0f;
					}
				}

				if (sc.RoleDict.ContainsKey(ev.Attacker.SteamId))
				{
					if (ev.Player.TeamRole.Team.Equals(Smod2.API.Team.CLASSD) || ev.Player.TeamRole.Team.Equals(Smod2.API.Team.CHAOS_INSURGENCY))
					{
						if (!ev.DamageType.Equals(DamageType.POCKET))
							ev.Damage = 0.0f;
					}
				}

				if (ev.Player.TeamRole.Team.Equals(Smod2.API.Team.NINETAILFOX) && ev.Player.IsHandcuffed() && sc.RoleDict.ContainsKey(ev.Player.SteamId) && ev.Player.GetHealth() > 0)
				{
					Vector pos = ev.Player.GetPosition();
					int health = ev.Player.GetHealth();
					ev.Player.ChangeRole(Role.CHAOS_INSUGENCY);
					foreach (Smod2.API.Item item in ev.Player.GetInventory()) { item.Remove(); }
					ev.Player.Teleport(pos);
					ev.Player.SetHealth(health);
					sc.RoleDict.Remove(ev.Player.SteamId);
				}
			}
		}

		/*public void OnHandcuffed(PlayerHandcuffedEvent ev) // THIS EVENT IS CURRENTLY BROKEN
		{
			//plugin.Info("onhandcuffed");
			if (plugin.GetConfigBool("spycup_enabled"))
			{
				//plugin.Info("spycup enabled");
				//plugin.Info("if " + ev.Player.TeamRole.Team.ToString() + " is team ninetailed and " + ev.Player.SteamId);
				//foreach (KeyValuePair<string, Role> entry in sc.RoleDict)
				//{
				//    plugin.Info(entry.Key.ToString());
				//}
				if (ev.Player.TeamRole.Team.Equals(Team.NINETAILFOX) && sc.RoleDict.ContainsKey(ev.Player.SteamId))
				{
					ChangeSpyRole(Team.NINETAILFOX, ev.Player);
				}
			}
		}*/

		public void OnPlayerDropItem(PlayerDropItemEvent ev)
		{
			if (plugin.GetConfigBool("spycup_enabled"))
			{
				if (ev.Item.ItemType.Equals(ItemType.CUP))
				{
					if (ev.Player.TeamRole.Team.Equals(Smod2.API.Team.NINETAILFOX))
						ChangeSpyRole(Smod2.API.Team.NINETAILFOX, ev.Player);
					else if (ev.Player.TeamRole.Team.Equals(Smod2.API.Team.CHAOS_INSURGENCY))
						ChangeSpyRole(Smod2.API.Team.CHAOS_INSURGENCY, ev.Player);
				}
			}
		}
	}
}

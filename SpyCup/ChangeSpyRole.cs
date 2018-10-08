using System;
using Smod2;
using Smod2.API;
using System.Collections.Generic;
using System.Threading;

namespace SpyCup
{
    class ChangeSpyRole
    {
        public ChangeSpyRole(SpyCup sc, Player player, bool delaySpawn)
        {
            float time = sc.GetConfigFloat("spycup_cooldown") * 1000;

            if (delaySpawn)
            {
                System.Threading.Thread.Sleep(25);
                player.ChangeRole(sc.RoleDict[player.SteamId]);
				//plugin.Info("Changed player " + player.Name + "'s role to " + sc.RoleDict[player.SteamId]);
            }
            System.Threading.Thread.Sleep((int)time);

			List<Item> inv = player.GetInventory();
			sc.Info(inv.Count.ToString());
			if (inv.Count > 7 && !player.HasItem(ItemType.CUP))
			{
				Item lastItem = inv[7];
				sc.Info(inv[7].ToString());
				foreach (Item item in player.GetInventory())
				{
					if (item.ItemType == lastItem.ItemType)
					{
						item.Remove();
						break;
					}
				}
				sc.pluginManager.Server.Map.SpawnItem(lastItem.ItemType, player.GetPosition(), Vector.Zero);
			}

			player.GiveItem(ItemType.CUP);
            Thread.CurrentThread.Abort();
        }
    }
}

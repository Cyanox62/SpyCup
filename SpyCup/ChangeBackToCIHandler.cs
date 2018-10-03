using System;
using Smod2;
using Smod2.API;
using System.Collections.Generic;
using System.Threading;

namespace SpyCup
{
	// IGNORE ALL THIS CODE, THIS DOESN'T HAVE ANY APPLICATION TO THE PLUGIN ANYMORE

	class ChangeBackToCIHandler
    {
        public ChangeBackToCIHandler(Player player, SpyCup sc)
        {
            Vector pos = player.GetPosition();
            List<Item> Inventory = player.GetInventory();
            player.ChangeRole(Role.CHAOS_INSUGENCY);
            player.Teleport(pos, false);
            foreach (Item item in player.GetInventory()) { item.Remove(); }
            foreach (Item item in Inventory) { player.GiveItem(item.ItemType); }
            sc.RoleDict.Remove(player.SteamId);
            Thread.CurrentThread.Abort();
        }
    }
}

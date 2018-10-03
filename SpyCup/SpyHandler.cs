using System;
using Smod2;
using Smod2.API;
using System.Collections.Generic;
using System.Threading;

namespace SpyCup
{
	// IGNORE ALL THIS CODE, THIS DOESN'T HAVE ANY APPLICATION TO THE PLUGIN ANYMORE

    class SpyHandler
    {
        public bool TeamsAreLastAlive(Team team1, Team team2, Plugin plugin)
        {
            List<Team> TeamList = new List<Team>();

            foreach (Player player in plugin.pluginManager.Server.GetPlayers())
            {
                TeamList.Add(player.TeamRole.Team);
            }
            return true;
            //unfinished
        }
        /// <summary>
        /// Checks to see if any NTf are alive that are not spies.
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="sc"></param>
        /// <returns></returns>
        public bool RealNTFStillAlive(Plugin plugin, SpyCup sc)
        {
            List<Player> ntfs = new List<Player>();

            foreach (Player player in plugin.pluginManager.Server.GetPlayers())
            {
                if (player.TeamRole.Team == Team.NINETAILFOX)
                {
                    if (!sc.RoleDict.ContainsKey(player.SteamId))
                    {
                        ntfs.Add(player);
                    }
                }
            }

            return (ntfs.Count > 0) ? true : false;
        }

        public SpyHandler(SpyCup sc, EventHandler evc, Plugin plugin)
        {
            System.Threading.Thread.Sleep(250);
            
            EventHandler ev = evc;

            List<Team> TeamList = new List<Team>();

            foreach (Player player in plugin.pluginManager.Server.GetPlayers())
            {
                TeamList.Add(player.TeamRole.Team);
            }

            if (!TeamList.Contains(Team.NINETAILFOX) && 
                !TeamList.Contains(Team.SCIENTISTS) && 
                !TeamList.Contains(Team.SCP) && 
                !TeamList.Contains(Team.TUTORIAL) && 
                (TeamList.Contains(Team.CLASSD) || TeamList.Contains(Team.CHAOS_INSURGENCY)))
            {
                //Only CI and d class are alive
                plugin.Info("only d class and/or ci alive");
                foreach (KeyValuePair<string, Role> entry in sc.RoleDict)
                {
                    foreach (Player player in plugin.pluginManager.Server.GetPlayers())
                    {
                        if (player.SteamId == entry.Key)
                        {
                            plugin.Info("found player");
                            Thread ChangeBackToCIHandler = new Thread(new ThreadStart(() => new ChangeBackToCIHandler(player, sc)));
                            ChangeBackToCIHandler.Start(); // Change player to ci to end the round since only d class and ci are left
                        }
                    }
                }
            }
            else if (!TeamList.Contains(Team.CHAOS_INSURGENCY) && 
                !TeamList.Contains(Team.CLASSD) && 
                !TeamList.Contains(Team.SCP) && 
                !TeamList.Contains(Team.TUTORIAL) &&
                (RealNTFStillAlive(plugin, sc) || TeamList.Contains(Team.SCIENTISTS)))
            {
                //Only scientist and MTF are alive
                plugin.Info("only scientist and/or mtf alive");
                foreach (KeyValuePair<string, Role> entry in sc.RoleDict)
                {
                    foreach (Player player in plugin.pluginManager.Server.GetPlayers())
                    {
                        if (player.SteamId == entry.Key)
                        {
                            //ev.canChange = false; // Prevent the player from changing classes
                            plugin.Info("found player, disabling change");
                            Thread ChangeBackToCIHandler = new Thread(new ThreadStart(() => new ChangeBackToCIHandler(player, sc)));
                            ChangeBackToCIHandler.Start(); // Change them to a ci
                        }
                    }
                }
            }
        }
    }
}

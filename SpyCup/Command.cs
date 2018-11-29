using System;
using Smod2;
using Smod2.Commands;
using Smod2.API;

namespace SpyCup
{
    class Command : ICommandHandler
    {
        private Plugin plugin;
        SpyCup sc;

        Random rand = new Random();

        public Command(SpyCup scc)
        {
            plugin = scc;
            sc = scc;
        }

        public string GetCommandDescription()
        {
            return "Turns a player into a spy.";
        }

        public string GetUsage()
        {
            return "(SC / SPYCUP) (PLAYER)";
        }

        public string[] OnCall(ICommandSender sender, string[] args)
        {
            if (!plugin.GetConfigBool("spycup_enabled"))
                return new string[] { "Plugin is not enabled." };

            if (args.Length > 0)
            {
                Player player = SpyCup.GetPlayerFromString.GetPlayer(args[0], out player);
                if (player == null) return new string[] { "Couldn't find player: " + args[0] };
                if (sc.RoleDict.ContainsKey(player.SteamId)) return new string[] { player.Name + " is already a spy." };

				Role MTFRole = sc.NTFRoles[rand.Next(sc.NTFRoles.Count)];
				player.ChangeRole(MTFRole);
				sc.RoleDict.Add(player.SteamId, MTFRole);
				player.GiveItem(ItemType.CUP);

				//plugin.Info("Added player " + player.Name + " to dict with role " + MTFRole.ToString());

				plugin.Info(player.Name + " has become a spy.");
                return new string[] { player.Name + " has been made a spy." };
            }
            else
            {
                return new string[] { GetUsage() };
            }
        }
    }
}

using Smod2;
using Smod2.API;
using Smod2.Attributes;
using System;
using System.Collections.Generic;

namespace SpyCup
{
    [PluginDetails(
    author = "Cyanox",
    name = "SpyCup",
    description = "A plugin for SCP:SL",
    id = "cyan.spycup",
    version = "0.5",
    SmodMajor = 3,
    SmodMinor = 0,
    SmodRevision = 0
    )]

    public class SpyCup : Plugin
    {
        public List<Role> NTFRoles = new List<Role>(new Role[] { Role.NTF_LIEUTENANT, Role.NTF_CADET });

        public Dictionary<string, Role> RoleDict = new Dictionary<string, Role>();

		//public bool roundStarted = false;

        public override void OnDisable() { }

        public override void OnEnable() { }

        public override void Register()
        {
            this.AddEventHandlers(new EventHandler(this, this));
            this.AddCommands(new string[] { "sc", "spycup" }, new Command(this, this));
            this.AddConfig(new Smod2.Config.ConfigSetting("spycup_enabled", true, Smod2.Config.SettingType.BOOL, true, "Enables SpyCup."));
			this.AddConfig(new Smod2.Config.ConfigSetting("spycup_cooldown", 10f, Smod2.Config.SettingType.FLOAT, true, "Determines the cooldown from switching classes."));
			this.AddConfig(new Smod2.Config.ConfigSetting("spycup_guard_chance", 50, Smod2.Config.SettingType.NUMERIC, true, "The chance for a facility guard to spawn as a spy"));
		}

        public static class LevenshteinDistance
        {
            /// <summary>
            /// Compute the distance between two strings.
            /// </summary>
            public static int Compute(string s, string t)
            {
                int n = s.Length;
                int m = t.Length;
                int[,] d = new int[n + 1, m + 1];

                // Step 1
                if (n == 0)
                {
                    return m;
                }

                if (m == 0)
                {
                    return n;
                }

                // Step 2
                for (int i = 0; i <= n; d[i, 0] = i++)
                {
                }

                for (int j = 0; j <= m; d[0, j] = j++)
                {
                }

                // Step 3
                for (int i = 1; i <= n; i++)
                {
                    //Step 4
                    for (int j = 1; j <= m; j++)
                    {
                        // Step 5
                        int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                        // Step 6
                        d[i, j] = Math.Min(
                            Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                            d[i - 1, j - 1] + cost);
                    }
                }
                // Step 7
                return d[n, m];
            }
        }

        public class GetPlayerFromString
        {
            public static Player GetPlayer(string args, out Player playerOut)
            {
                //Takes a string and finds the closest player from the playerlist
                int maxNameLength = 31, LastnameDifference = 31/*, lastNameLength = 31*/;
                Player plyer = null;
                string str1 = args.ToLower();
                foreach (Player pl in PluginManager.Manager.Server.GetPlayers(str1))
                {
                    if (!pl.Name.ToLower().Contains(args.ToLower())) { goto NoPlayer; }
                    if (str1.Length < maxNameLength)
                    {
                        int x = maxNameLength - str1.Length;
                        int y = maxNameLength - pl.Name.Length;
                        string str2 = pl.Name;
                        for (int i = 0; i < x; i++)
                        {
                            str1 += "z";
                        }
                        for (int i = 0; i < y; i++)
                        {
                            str2 += "z";
                        }
                        int nameDifference = LevenshteinDistance.Compute(str1, str2);
                        if (nameDifference < LastnameDifference)
                        {
                            LastnameDifference = nameDifference;
                            plyer = pl;
                        }
                    }
                NoPlayer:;
                }
                playerOut = plyer;
                return playerOut;
            }
        }
    }
}

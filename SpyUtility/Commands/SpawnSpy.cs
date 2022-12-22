using System;
using System.Linq;
using CommandSystem;
using PlayerRoles;
using PluginAPI.Core;
using RemoteAdmin;

namespace SpyUtilityNW.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class SpawnSpy : ICommand
    {
        public string Command { get; } = "SpawnSpy";

        public string[] Aliases { get; } = new string[] { "spy" };

        public string Description { get; } = "Force spawn a Spy";
        
        public static string HelpString = "Please provide the correct syntax:\n" +
                                         "SPY add (Role) (Player)\n" +
                                         "SPY remove (Role) (Player)\n" +
                                         "SPY spawn (Role) (Player) (NewRole)";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission(PlayerPermissions.PlayerSensitiveDataAccess))
            {
                response = "You do not have permission to run this command";
                return false;
            }

            if (arguments.Count is <= 1 or > 4)
            {
                response = HelpString;
                return false;
            }

            String currentCommand = arguments.At(0).ToUpper();
            PlayerCommandSender commandSender = sender as PlayerCommandSender;
            Team.TryParse(arguments.At(1), out Team curTeam);
            Player player = arguments.Count is >= 3 ? Player.Get(int.Parse(arguments.At(2))) : Player.Get(commandSender.PlayerId);
            RoleTypeId newRoleToSpawn = RoleTypeId.None;
            if (arguments.Count >= 4)
            {
                Team.TryParse(arguments.At(3), out RoleTypeId outNewRole);
                newRoleToSpawn = outNewRole;
            }

            if (player == null || player == Player.Get(ReferenceHub.HostHub))
            {
                response = $"Failed to run command {currentCommand}, player was either null {player == null} or server host";
                return false;
            }
            
            switch (currentCommand)
            {
                case "ADD":
                    return AddSpy(out response, curTeam, player);
                case "REMOVE":
                    return RemoveSpy(out response, curTeam, player);
                case "SPAWN":
                    return CreateSpy(out response, curTeam, player, newRoleToSpawn);
            }
            
            response = $"Failed to run command {currentCommand}";
            return false;
        }

        private bool CreateSpy(out string response, Team curTeam, Player player, RoleTypeId newRole)
        {
            Log.Debug($" CreateSpy player {player}, curTeam {curTeam}", SpyUtilityNW.Instance.Config.Debug);
            if (SpyManager.ForceCreateSpy(player, team: curTeam, newRole))
            {
                response = "Success in creating new spy";
                return true;
            }

            response = "Failed to create new Spy";
            return false;
        }


        private bool RemoveSpy(out string response, Team curTeam, Player player)
        {
            Log.Debug($" RemoveSpy Player, {player}, curTeam {curTeam}", SpyUtilityNW.Instance.Config.Debug);
            SpyManager.ForceRemoveSpy(player, team: curTeam);
            response = "Attempted to remove spy.";
            return true;
        }

        private bool AddSpy(out string response, Team curTeam, Player player)
        {
            Log.Debug($" AddSpy player {player}, curTeam {curTeam}", SpyUtilityNW.Instance.Config.Debug);
            if (SpyManager.ForceAddSpy(player, team: curTeam))
            {
                response = "Success in creating new spy";
                return true;
            }

            response = "Failed to create new Spy";
            return false;
        }
    }
}
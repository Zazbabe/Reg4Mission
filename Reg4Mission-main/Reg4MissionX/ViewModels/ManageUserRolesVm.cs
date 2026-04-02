using System.ComponentModel.DataAnnotations;

namespace Reg4MissionX.ViewModels
{
    public class ManageUserRolesVm
    {
        public string UserId { get; set; } = "";
        public string UserEmail { get; set; } = "";

        [MinLength(1, ErrorMessage = "Minst en roll måste väljas")]
        public List<string> CurrentRoles { get; set; } = new();
        public List<string> AvailableRoles { get; set; } = new();
    }
}

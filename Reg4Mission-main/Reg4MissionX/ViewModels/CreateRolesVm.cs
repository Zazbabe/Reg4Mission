using System.ComponentModel.DataAnnotations;

namespace Reg4MissionX.ViewModels
{
    public class CreateRolesVm
    {
        [Required(ErrorMessage = "Minst en roll måste väljas")]
        public string Roles { get; set; } = "";
    }
}

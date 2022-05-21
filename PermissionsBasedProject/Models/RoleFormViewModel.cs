using System.ComponentModel.DataAnnotations;

namespace PermissionsBasedProject.Models
{
    public class RoleFormViewModel
    {
        //public string Id { get; set; }
        [Required, StringLength(256)]
        public string Name { get; set; }
    }
}

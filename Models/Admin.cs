using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMia.Models
{
    public class Admin
    {
        [Key]
        public int User_Id { get; set; }
        public string User_Name{ get; set; }
        public string Password{ get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }
        public int UserRolId { get; set; }

    }
}

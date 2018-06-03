using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMia.Models
{
    public class Rol
    {
        [Key]
        public int UserRolId { get; set; }
        public string RolDescription { get; set; }

    }
}

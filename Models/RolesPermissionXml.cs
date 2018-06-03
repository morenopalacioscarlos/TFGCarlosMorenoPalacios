using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace WebMia.Models
{
    public class RolesPermissionXml
    {
        [Key]
        public int IdXml { get; set; }
        public string xmlInfo { get; set; }
    }
}

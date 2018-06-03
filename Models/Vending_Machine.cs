using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebMia.Models
{
    public class Vending_Machine
    {
        [Key]
        public int Id_Machine { get; set; }
        public string Machine_Model { get; set; }
        public string City{ get; set; }
        public string Address { get; set; }
        public string Poblation { get; set; }
        public int UserAdministrator { get; set; }
        public string TokenId { get; set; }

        public Vending_Machine() { }
    }
}

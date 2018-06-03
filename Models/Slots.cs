using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebMia.Models
{
    public class Slots
    {
        [Key]
        public int Id_Slot { get; set; }       
        public int Id_Machine { get; set; }        
        public int Id_Product{ get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public int Slot_Number { get; set; }

        public Slots() { }
    }
}

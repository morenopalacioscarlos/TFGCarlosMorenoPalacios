using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebMia.Models
{
    public class PriceUpdated
    {
        [Key]
        public int IdUpdate { get; set; }
        public int IdProduct { get; set; }
        public int IdMachine { get; set; }
        public int IdSlot { get; set; }
        public decimal NewPrice { get; set; }
    }
}

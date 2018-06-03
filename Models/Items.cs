using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebMia.Models
{
    public class Items
    {
        [Key]
        public int Id { get; set; }
        public string ItemName { get; set; }
       

        public Items() { }
    }
}

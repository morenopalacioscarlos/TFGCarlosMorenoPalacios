using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebMia.Models
{
    public class Change
    {
        [Key]
        public int Id_Change { get; set; }
        public int Id_Machine{ get; set; }
        public int Coins_2_Eur { get; set; }
        public int Coins_1_Eur { get; set; }
        public int Coins_50_Cents { get; set; }
        public int Coins_20_Cents { get; set; }
        public int Coins_10_Cents { get; set; }
        public int Coins_5_Cents { get; set; }
        public int Coins_2_Cents { get; set; }
        public int Coins_1_Cents { get; set; }

        public Change() { }
     }

 


}

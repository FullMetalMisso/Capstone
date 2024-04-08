using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone.Models
{
    public class VinoCart
    {
        public Vini Vino {  get; set; }

        public int Quantita { get; set; }   

        public int UserId { get; set; }

        public Users User { get; set; }    
    }
}
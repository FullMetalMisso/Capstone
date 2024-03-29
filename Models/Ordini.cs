namespace Capstone.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Ordini")]
    public partial class Ordini
    {
        public int OrdiniId { get; set; }

        [Required]
        [StringLength(50)]
        public string Indirizzo { get; set; }

        [Required]
        [StringLength(50)]
        public string Stato { get; set; }

        public decimal Totale { get; set; }

        [Column(TypeName = "date")]
        public DateTime Consegna { get; set; }

        [StringLength(10)]
        public string ClienteId { get; set; }
    }
}

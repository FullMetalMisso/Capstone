namespace Capstone.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Vini")]
    public partial class Vini
    {
        [Key]
        public int VinoId { get; set; }

        [Required]
        [StringLength(50)]
        public string Nome { get; set; }

        [StringLength(255)]
        public string Img { get; set; }

        [Required]
        [StringLength(50)]
        public string Tipo { get; set; }

        public int? Anno { get; set; }

        public string Descrizione { get; set; }

        public decimal Prezzo { get; set; }

        public int? Magazzino { get; set; }

        [StringLength(50)]
        public string Produttore { get; set; }
    }
}

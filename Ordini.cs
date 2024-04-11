namespace Capstone
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Ordini")]
    public partial class Ordini
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Ordini()
        {
            Checkouts = new HashSet<Checkout>();
        }

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

        public int UserId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Checkout> Checkouts { get; set; }

        public virtual User User { get; set; }
    }
}

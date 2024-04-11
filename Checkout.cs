namespace Capstone
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Checkout")]
    public partial class Checkout
    {
        public int CheckoutId { get; set; }

        public int UserId { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DataCheckout { get; set; }

        public decimal Totale { get; set; }

        public int OrdiniId { get; set; }

        public virtual Ordini Ordini { get; set; }

        public virtual User User { get; set; }
    }
}

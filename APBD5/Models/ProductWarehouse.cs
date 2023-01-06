using System.ComponentModel.DataAnnotations;

namespace APBD5
{
    public class ProductWarehouse
    {
        [Required]
        public int IdProduct { get; set; }
        [Required]
        public int IdWarehouse { get; set; }
        [Required]
        [Range(1, int.MaxValue,ErrorMessage="Amount must be minimum 1")]
        public int Amount { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }

    }
}

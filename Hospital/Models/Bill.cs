using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Hospital.Models
{
    public class Bill
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public int? UserId { get; set; }
        public virtual User User { get; set; } = null!;

        [Required(ErrorMessage = "Please select Issue")]
        [ForeignKey(nameof(Issue))]
        public int? IssueId { get; set; }
        public virtual Issue Issue { get; set; } = null!;

        public double TotalFee { get; set; }

    }
}

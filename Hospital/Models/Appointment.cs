using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Models
{
    public class Appointment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [ForeignKey(nameof(User))]
        public int? UserId { get; set; }
        public virtual User User { get; set; } = null!;

        [ForeignKey(nameof(Doctor))]
        public int? DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; } = null!;

        [ForeignKey(nameof(Issue))]
        public int? IssueId { get; set; }
        public virtual Issue Issue { get; set; } = null!;

        [Required(ErrorMessage = "Please select Time")]
        public DateTime Time { get; set; }
        public string Description { get; set; }


    }
}

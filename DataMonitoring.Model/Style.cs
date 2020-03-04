using System.ComponentModel.DataAnnotations;

namespace DataMonitoring.Model
{
    public class Style
    {
        public long Id { get; set; }

        [Required]
        [StringLength(60)]
        public string Name { get; set; }

        [Required]
        [StringLength(500)]
        public string Code { get; set; }
    }
}

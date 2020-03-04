using System.ComponentModel.DataAnnotations;

namespace DataMonitoring.Model
{
    public class ColorHtml
    {
        public long Id { get; set; }

        [Required]
        [StringLength(60)]
        public string Name { get; set; }

        [StringLength(100)]
        public string TxtClassName { get; set; }

        [StringLength(100)]
        public string BgClassName { get; set; }

        [StringLength(7)]
        public string HexColorCode { get; set; }
    }
}

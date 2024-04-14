using System.ComponentModel.DataAnnotations;

namespace viki_01.Models.Dto
{
    public class ThemeDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public int AuthorId { get; set; }

        [Required]
        public string Css { get; set; }

        public bool IsPublic { get; set; }
    }
}

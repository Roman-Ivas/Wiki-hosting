using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace viki_01.Models.Dto
{
    [SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
    [SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")] 
    public class TemplateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public int AuthorId { get; set; }

        [Required]
        public string Html { get; set; }

        public string? Css { get; set; }
        public string? Js { get; set; }
        public string? Variables { get; set; }

        public bool IsPublic { get; set; }
    }
}
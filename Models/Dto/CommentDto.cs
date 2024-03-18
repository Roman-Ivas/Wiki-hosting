using System;
using System.ComponentModel.DataAnnotations;

namespace viki_01.Models.Dto
{
    public class CommentDto
    {
        public int Id { get; set; }

        public int PageId { get; set; }

        [Required]
        public int AuthorId { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string Text { get; set; }

        public DateTime PostedAt { get; set; }
        public DateTime EditedAt { get; set; }

        public int? ParentCommentId { get; set; }
    }
}

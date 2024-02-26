using System.ComponentModel.DataAnnotations;

namespace viki_01.Entities
{
    public class RefreshToken
    {
        [Key]
        public string Token { get; set; }
        public DateTime TimeCreated { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}

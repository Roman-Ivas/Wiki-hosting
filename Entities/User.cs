using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace viki_01.Entities
{
    [SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
    [SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
    [SuppressMessage("ReSharper", "EntityFramework.ModelValidation.CircularDependency")] // Can be solved by correct JSON serialization configuration
    public class User : IdentityUser<int>
    {
        public string Password { get; set; }



        [StringLength(512, MinimumLength = 1)]
        public string AvatarPath { get; set; } = null!;

        public UserSubscription Subscription { get; set; } = null!;
        public UserPreference Preference { get; set; } = null!;

        public IList<Topic> InterestedTopics { get; set; } = new List<Topic>();
        public IList<Contributor> Contributions { get; set; } = new List<Contributor>();
        public IList<Feedback> Feedbacks { get; set; } = new List<Feedback>();
        public IList<Comment> Comments { get; set; } = new List<Comment>();

        public IList<Rating> CreatedRatings { get; set; } = new List<Rating>();
        public IList<Report> CreatedReports { get; set; } = new List<Report>();

        public IList<Page> CreatedPages { get; set; } = new List<Page>();
        public IList<Theme> CreatedThemes { get; set; } = new List<Theme>();
        public IList<Template> CreatedTemplates { get; set; } = new List<Template>();

        [JsonIgnore]
        public virtual List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        public User()
        {
        }

        public User(RegistrationCredentials credentials)
        {
            UserName = credentials.Username;
            Email = credentials.Email;
            Password = credentials.Password;
        }
    }
}
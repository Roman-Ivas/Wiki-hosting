using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using viki_01.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;



namespace viki_01.Contexts
{
    [SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
    public class WikiHostingSqlServerContext
    : IdentityDbContext<User, IdentityRole<int>, int>
    {


        //
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        //

        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<Contributor> Contributors { get; set; } = null!;
        public DbSet<ContributorRole> ContributorRoles { get; set; } = null!;
        public DbSet<Feedback> Feedbacks { get; set; } = null!;
        public DbSet<MediaContent> MediaContents { get; set; } = null!;
        public DbSet<Page> Pages { get; set; } = null!;
        public DbSet<Rating> Ratings { get; set; } = null!;
        public DbSet<Report> Reports { get; set; } = null!;
        public DbSet<Subscription> Subscriptions { get; set; } = null!;
        public DbSet<Template> Templates { get; set; } = null!;
        public DbSet<Theme> Themes { get; set; } = null!;
        public DbSet<Topic> Topics { get; set; } = null!;
        public DbSet<UserPreference> UserPreferences { get; set; } = null!;
        public DbSet<UserSubscription> UserSubscriptions { get; set; } = null!;
        public DbSet<Wiki> Wikis { get; set; } = null!;
        public DbSet<Link> Links { get; set; } = null!;
        
        public WikiHostingSqlServerContext(DbContextOptions<WikiHostingSqlServerContext> options) :base(options)
        {
            //Database.Migrate();
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //
            builder.Entity<RefreshToken>()
    .HasOne(x => x.User)
    .WithMany(x => x.RefreshTokens)
    .OnDelete(DeleteBehavior.Cascade);


            //
            builder.Entity<Comment>(entity =>
            {
                entity.HasKey(comment => comment.Id);

                entity.HasIndex(comment => comment.PageId);
                entity.HasIndex(comment => comment.AuthorId);
                entity.HasIndex(comment => comment.ParentCommentId);

                entity
                    .HasOne(comment => comment.Page)
                    .WithMany(page => page.Comments)
                    .HasForeignKey(comment => comment.PageId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity
                    .HasOne(comment => comment.Author)
                    .WithMany(author => author.Comments)
                    .HasForeignKey(comment => comment.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity
                    .HasOne(comment => comment.ParentComment)
                    .WithMany()
                    .HasForeignKey(comment => comment.ParentCommentId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.Property(comment => comment.Text)
                    .HasMaxLength(500)
                    .HasColumnType("nvarchar(500)")
                    .IsUnicode()
                    .IsRequired();

                entity.Property(comment => comment.PostedAt)
                    .HasDefaultValueSql("GETDATE()")
                    .IsRequired();

                entity.Property(comment => comment.EditedAt)
                    .HasDefaultValueSql("GETDATE()")
                    .IsRequired();

                entity.ToTable(nameof(Comments),
                    tableBuilder =>
                    {
                        tableBuilder.HasCheckConstraint($"CK_{nameof(Comments)}_{nameof(Comment.Text)}",
                            $"[{nameof(Comment.Text)}] != ''");
                        tableBuilder.HasCheckConstraint(
                            $"CK_{nameof(Comments)}_{nameof(Comment.PostedAt)}",
                            $"[{nameof(Comment.PostedAt)}] <= [{nameof(Comment.EditedAt)}]");
                    });
            });

            builder.Entity<Contributor>(entity =>
            {
                entity.HasKey(contributor => contributor.Id);

                entity.HasIndex(contributor => contributor.UserId);
                entity.HasIndex(contributor => contributor.WikiId);
                entity.HasIndex(contributor => new { contributor.UserId, contributor.WikiId });

                entity.HasOne(contributor => contributor.User)
                    .WithMany(user => user.Contributions)
                    .HasForeignKey(contributor => contributor.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(contributor => contributor.Wiki)
                    .WithMany(wiki => wiki.Contributors)
                    .HasForeignKey(contributor => contributor.WikiId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(contributor => contributor.ContributorRole)
                    .WithMany(contributorRole => contributorRole.Contributors)
                    .HasForeignKey(contributor => contributor.ContributorRoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(nameof(Contributors));
            });

            builder.Entity<ContributorRole>(entity =>
            {
                entity.HasKey(contributorRole => contributorRole.Id);

                entity.HasIndex(contributorRole => contributorRole.Name);

                entity.HasMany(contributorRole => contributorRole.Contributors)
                    .WithOne(contributor => contributor.ContributorRole)
                    .HasForeignKey(contributor => contributor.ContributorRoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(contributorRole => contributorRole.Name)
                    .HasMaxLength(100)
                    .HasColumnType("nvarchar(100)")
                    .IsUnicode(false)
                    .IsRequired();

                entity.ToTable(nameof(ContributorRoles));
            });

            builder.Entity<Feedback>(entity =>
            {
                entity.HasKey(feedback => feedback.Id);

                entity.HasIndex(feedback => feedback.PostedAt);

                entity.HasOne(feedback => feedback.User)
                    .WithMany(user => user.Feedbacks)
                    .HasForeignKey(feedback => feedback.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity
                    .Property(feedback => feedback.Text)
                    .HasMaxLength(500)
                    .IsRequired();

                entity.ToTable(nameof(Feedbacks),
                    tableBuilder =>
                    {
                        tableBuilder.HasCheckConstraint(
                            $"CK_{nameof(Feedbacks)}_{nameof(Feedback.Text)}",
                            $"[{nameof(Feedback.Text)}] != ''");
                    });
            });

            builder.Entity<MediaContent>(entity =>
            {
                entity.HasKey(mediaContent => mediaContent.Id);
                
                entity.Property(mediaContent => mediaContent.Path)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .IsRequired();

                entity.ToTable(nameof(MediaContents),
                    tableBuilder =>
                    {
                        tableBuilder.HasCheckConstraint(
                            $"CK_{nameof(MediaContents)}_{nameof(MediaContent.Path)}",
                            $"[{nameof(MediaContent.Path)}] != ''");
                    });
            });

            builder.Entity<Page>(entity =>
            {
                entity.HasKey(page => page.Id);

                entity.HasIndex(page => page.WikiId);

                entity.HasOne(page => page.Wiki)
                    .WithMany(wiki => wiki.Pages)
                    .HasForeignKey(page => page.WikiId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(page => page.Author)
                    .WithMany(author => author.CreatedPages)
                    .HasForeignKey(page => page.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(page => page.UserRatings)
                    .WithOne(rating => rating.Page)
                    .HasForeignKey(rating => rating.PageId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(page => page.Comments)
                    .WithOne(comment => comment.Page)
                    .HasForeignKey(comment => comment.PageId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(page => page.CreatedAt)
                    .HasDefaultValueSql("GETDATE()")
                    .IsRequired();

                entity.Property(page => page.EditedAt)
                    .HasDefaultValueSql("GETDATE()")
                    .IsRequired();

                entity.Property(page => page.RawHtml)
                    .IsUnicode()
                    .HasColumnType("nvarchar(max)")
                    .IsRequired();

                entity.Property(page => page.ProcessedHtml)
                    .IsUnicode()
                    .HasColumnType("nvarchar(max)")
                    .IsRequired();

                entity.ToTable(nameof(Pages),
                    tableBuilder =>
                    {
                        tableBuilder.HasCheckConstraint(
                            $"CK_{nameof(Pages)}_{nameof(Page.RawHtml)}",
                            $"[{nameof(Page.RawHtml)}] != ''");

                        tableBuilder.HasCheckConstraint(
                            $"CK_{nameof(Pages)}_{nameof(Page.ProcessedHtml)}",
                            $"[{nameof(Page.ProcessedHtml)}] != ''");

                        tableBuilder.HasCheckConstraint(
                            $"CK_{nameof(Pages)}_{nameof(Page.CreatedAt)}",
                            $"[{nameof(Page.CreatedAt)}] <= [{nameof(Page.EditedAt)}]");
                    });
            });

            builder.Entity<Rating>(entity =>
            {
                entity.HasKey(rating => rating.Id);

                entity.HasIndex(rating => rating.UserId);
                entity.HasIndex(rating => rating.PageId);

                entity.HasOne(rating => rating.User)
                    .WithMany(user => user.CreatedRatings)
                    .HasForeignKey(rating => rating.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rating => rating.Page)
                    .WithMany(page => page.UserRatings)
                    .HasForeignKey(rating => rating.PageId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(rating => rating.PostedAt)
                    .HasDefaultValueSql("GETDATE()")
                    .IsRequired();

                entity.Property(rating => rating.NumberOfLikes)
                    .IsRequired();

                entity.Property(rating => rating.NumberOfDislikes)
                    .IsRequired();

                entity.ToTable(nameof(Ratings),
                    tableBuilder =>
                    {
                        tableBuilder.HasCheckConstraint($"CK_{nameof(Ratings)}_{nameof(Rating.NumberOfLikes)}", $"[{nameof(Rating.NumberOfLikes)}] >= 0");
                        tableBuilder.HasCheckConstraint($"CK_{nameof(Ratings)}_{nameof(Rating.NumberOfDislikes)}", $"[{nameof(Rating.NumberOfDislikes)}] >= 0");
                    });
            });

            builder.Entity<Report>(entity =>
            {
                entity.HasKey(report => report.Id);

                entity.HasIndex(report => report.PostedAt);

                entity.HasOne(report => report.User)
                    .WithMany(user => user.CreatedReports)
                    .HasForeignKey(report => report.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(report => report.ReportedContentUrl)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(report => report.PostedAt)
                    .HasDefaultValueSql("GETDATE()")
                    .IsRequired();

                entity.Property(report => report.Text)
                    .HasMaxLength(1000)
                    .IsUnicode()
                    .IsRequired();

                entity.Property(report => report.Result)
                    .HasMaxLength(500)
                    .IsUnicode()
                    .IsRequired(false);

                entity.ToTable(nameof(Reports),
                    tableBuilder =>
                    {
                        tableBuilder.HasCheckConstraint($"CK_{nameof(Reports)}_{nameof(Report.ReportedContentUrl)}", $"[{nameof(Report.ReportedContentUrl)}] != ''");
                        tableBuilder.HasCheckConstraint($"CK_{nameof(Reports)}_{nameof(Report.Text)}", $"[{nameof(Report.Text)}] != ''");
                    });
            });

            builder.Entity<Subscription>(entity =>
            {
                entity.HasKey(subscription => subscription.Id);

                entity.HasIndex(subscription => subscription.Name);

                entity.HasMany(subscription => subscription.Users)
                    .WithOne(user => user.Subscription)
                    .HasForeignKey(userSubscription => userSubscription.SubscriptionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(subscription => subscription.Name)
                    .HasMaxLength(100)
                    .IsUnicode()
                    .IsRequired();

                entity.Property(subscription => subscription.Description)
                    .HasMaxLength(500)
                    .IsUnicode()
                    .IsRequired();

                entity.Property(subscription => subscription.DurationDays)
                    .IsRequired();

                entity.Property(subscription => subscription.Price)
                    .HasColumnType("smallmoney")
                    .IsRequired();

                entity.ToTable(nameof(Subscriptions),
                    tableBuilder =>
                    {
                        tableBuilder.HasCheckConstraint($"CK_{nameof(Subscriptions)}_{nameof(Subscription.Name)}", $"[{nameof(Subscription.Name)}] != ''");
                        tableBuilder.HasCheckConstraint($"CK_{nameof(Subscriptions)}_{nameof(Subscription.Description)}", $"[{nameof(Subscription.Description)}] != ''");
                        tableBuilder.HasCheckConstraint($"CK_{nameof(Subscriptions)}_{nameof(Subscription.DurationDays)}", $"[{nameof(Subscription.DurationDays)}] >= 7");
                        tableBuilder.HasCheckConstraint($"CK_{nameof(Subscriptions)}_{nameof(Subscription.Price)}", $"[{nameof(Subscription.Price)}] >= 0");
                    });
            });

            builder.Entity<Template>(entity =>
            {
                entity.HasKey(template => template.Id);

                entity.HasIndex(template => template.AuthorId);
                entity.HasIndex(template => template.Name);

                entity.HasOne(template => template.Author)
                    .WithMany(author => author.CreatedTemplates)
                    .HasForeignKey(template => template.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(template => template.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(template => template.Html)
                    .HasColumnType("nvarchar(max)")
                    .IsUnicode()
                    .IsRequired();

                entity.Property(template => template.Css)
                    .HasColumnType("nvarchar(max)")
                    .IsUnicode()
                    .IsRequired(false);

                entity.Property(template => template.Js)
                    .HasColumnType("nvarchar(max)")
                    .IsUnicode()
                    .IsRequired(false);

                entity.Property(template => template.Variables)
                    .HasColumnType("nvarchar(max)")
                    .IsUnicode()
                    .IsRequired(false);

                entity.Property(template => template.IsPublic)
                    .HasDefaultValue(false)
                    .IsRequired();

                entity.Property(template => template.ImagePath)
                    .HasMaxLength(512)
                    .HasDefaultValue("https://dummyimage.com/300x300/ccc/000000")
                    .IsRequired();

                entity.ToTable(nameof(Templates),
                    tableBuilder =>
                    {
                        tableBuilder.HasCheckConstraint($"CK_{nameof(Templates)}_{nameof(Template.Name)}", $"[{nameof(Template.Name)}] != ''");
                        tableBuilder.HasCheckConstraint($"CK_{nameof(Templates)}_{nameof(Template.Html)}", $"[{nameof(Template.Html)}] != ''");
                    });
            });

            builder.Entity<Theme>(entity =>
            {
                entity.HasKey(theme => theme.Id);

                entity.HasIndex(theme => theme.AuthorId);
                entity.HasIndex(theme => theme.Name);

                entity.HasOne(theme => theme.Author)
                    .WithMany(author => author.CreatedThemes)
                    .HasForeignKey(theme => theme.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(theme => theme.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(theme => theme.Css)
                    .HasColumnType("nvarchar(max)")
                    .IsUnicode()
                    .IsRequired();

                entity.Property(theme => theme.IsPublic)
                    .HasDefaultValue(false)
                    .IsRequired();

                entity.ToTable(nameof(Themes),
                    tableBuilder =>
                    {
                        tableBuilder.HasCheckConstraint($"CK_{nameof(Themes)}_{nameof(Theme.Name)}",
                            $"[{nameof(Theme.Name)}] != ''");

                        tableBuilder.HasCheckConstraint($"CK_{nameof(Themes)}_{nameof(Theme.Css)}",
                            $"[{nameof(Theme.Css)}] != ''");
                    });
            });

            builder.Entity<Topic>(entity =>
            {
                entity.HasKey(topic => topic.Id);

                entity.HasIndex(topic => topic.Name);

                entity.HasMany(topic => topic.InterestedUsers)
                    .WithMany(user => user.InterestedTopics);

                entity.HasMany(topic => topic.RelevantWikis)
                    .WithMany(wiki => wiki.Topics);

                entity.Property(topic => topic.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .IsRequired();

                entity.ToTable(nameof(Topics),
                    tableBuilder =>
                    {
                        tableBuilder.HasCheckConstraint($"CK_{nameof(Topics)}_{nameof(Topic.Name)}",
                            $"[{nameof(Topic.Name)}] != ''");
                    });
            });

            builder.Entity<Wiki>(entity =>
            {
                entity.HasKey(wiki => wiki.Id);

                entity.HasIndex(wiki => wiki.Name);
                entity.HasIndex(wiki => wiki.IsArchived);

                entity.Property(wiki => wiki.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(wiki => wiki.IsArchived)
                    .HasDefaultValue(false)
                    .IsRequired();

                entity.Property(wiki => wiki.BackgroundImagePath)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(wiki => wiki.MainWikiImagePath)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .IsRequired();

                entity.HasMany(wiki => wiki.Topics)
                    .WithMany(topic => topic.RelevantWikis);

                entity.HasMany(wiki => wiki.Pages)
                    .WithOne(page => page.Wiki)
                    .HasForeignKey(page => page.WikiId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(wiki => wiki.Contributors)
                    .WithOne(contributor => contributor.Wiki)
                    .HasForeignKey(contributor => contributor.WikiId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasMany(wiki => wiki.MainLinks)
                    .WithOne(link => link.Wiki)
                    .HasForeignKey(link => link.WikiId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable(nameof(Wikis), tableBuilder =>
                {
                    tableBuilder.HasCheckConstraint($"CK_{nameof(Wikis)}_{nameof(Wiki.Name)}", $"[{nameof(Wiki.Name)}] != ''");
                    tableBuilder.HasCheckConstraint($"CK_{nameof(Wikis)}_{nameof(Wiki.BackgroundImagePath)}", $"[{nameof(Wiki.BackgroundImagePath)}] != ''");
                    tableBuilder.HasCheckConstraint($"CK_{nameof(Wikis)}_{nameof(Wiki.MainWikiImagePath)}", $"[{nameof(Wiki.MainWikiImagePath)}] != ''");
                });
            });

            builder.Entity<Link>(entity =>
            {
                entity.HasKey(link => link.Id);

                entity.HasIndex(link => link.Url);
                entity.HasIndex(link => link.Title);
                
                entity.Property(link => link.Url)
                    .HasMaxLength(512)
                    .IsUnicode(false)
                    .IsRequired();
                
                entity.Property(link => link.Title)
                    .HasMaxLength(128)
                    .IsUnicode()
                    .IsRequired();

                entity.HasOne(link => link.Wiki)
                    .WithMany(wiki => wiki.MainLinks)
                    .HasForeignKey(link => link.WikiId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable(nameof(Links));
            });

            builder.Entity<User>(entity =>
            {
                entity.HasKey(user => user.Id);

                entity.HasIndex(user => user.UserName);
                entity.HasIndex(user => user.Email);

                entity.HasOne(user => user.Subscription)
                    .WithOne(userSubscription => userSubscription.User)
                    .HasForeignKey<UserSubscription>(userSubscription => userSubscription.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(user => user.Preference)
                    .WithOne(userPreference => userPreference.User)
                    .HasForeignKey<UserPreference>(userPreference => userPreference.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(user => user.InterestedTopics)
                    .WithMany(topic => topic.InterestedUsers);

                entity.HasMany(topic => topic.Contributions)
                    .WithOne(contributor => contributor.User)
                    .HasForeignKey(contributor => contributor.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(contributor => contributor.Feedbacks)
                    .WithOne(feedback => feedback.User)
                    .HasForeignKey(feedback => feedback.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(contributor => contributor.Comments)
                    .WithOne(comment => comment.Author)
                    .HasForeignKey(comment => comment.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(user => user.CreatedRatings)
                    .WithOne(rating => rating.User)
                    .HasForeignKey(rating => rating.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(user => user.CreatedReports)
                    .WithOne(report => report.User)
                    .HasForeignKey(report => report.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(user => user.CreatedPages)
                    .WithOne(page => page.Author)
                    .HasForeignKey(page => page.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(user => user.CreatedThemes)
                    .WithOne(theme => theme.Author)
                    .HasForeignKey(theme => theme.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(user => user.CreatedTemplates)
                    .WithOne(template => template.Author)
                    .HasForeignKey(template => template.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(user => user.UserName)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(user => user.Email)
                    .HasMaxLength(256)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(user => user.AvatarPath)
                    .HasMaxLength(512)
                    .HasDefaultValue("/assets/avatar_placeholder.jpg")
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(user => user.About)
                    .HasMaxLength(1024)
                    .IsUnicode()
                    .IsRequired(false);

                entity.ToTable(nameof(Users),
                    tableBuilder =>
                    {
                        tableBuilder.HasCheckConstraint($"CK_{nameof(Users)}_{nameof(User.AvatarPath)}",
                            $"[{nameof(User.AvatarPath)}] != ''");
                    });
            });

            builder.Entity<UserPreference>(entity =>
            {
                entity.HasKey(userPreference => userPreference.UserId);

                entity.HasOne(userPreference => userPreference.User)
                    .WithOne(user => user.Preference)
                    .HasForeignKey<UserPreference>(userPreference => userPreference.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(userPreference => userPreference.SiteThemeId)
                    .HasDefaultValue(0)
                    .IsRequired();

                entity.Property(userPreference => userPreference.PreferToReceiveUpdateEmails)
                    .HasDefaultValue(true)
                    .IsRequired();

                entity.Property(userPreference => userPreference.PreferToReceivePromotionalEmails)
                    .HasDefaultValue(true)
                    .IsRequired();

                entity.Property(userPreference => userPreference.PreferToReceiveSurveyEmails)
                    .HasDefaultValue(true)
                    .IsRequired();

                entity.Property(userPreference => userPreference.PreferToReceiveEventNotifications)
                    .HasDefaultValue(true)
                    .IsRequired();

                entity.Property(userPreference => userPreference.PreferToReceiveMessageNotifications)
                    .HasDefaultValue(true)
                    .IsRequired();

                entity.ToTable(nameof(UserPreferences),
                    tableBuilder =>
                    {
                        tableBuilder.HasCheckConstraint($"CK_{nameof(UserPreferences)}_{nameof(UserPreference.SiteThemeId)}",
                            $"[{nameof(UserPreference.SiteThemeId)}] >= 0");
                    });
            });

            builder.Entity<UserSubscription>(entity =>
            {
                entity.HasKey(userSubscription => userSubscription.UserId);

                entity.HasOne(userSubscription => userSubscription.User)
                    .WithOne(user => user.Subscription)
                    .HasForeignKey<UserSubscription>(userSubscription => userSubscription.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(userSubscription => userSubscription.Subscription)
                    .WithMany(subscription => subscription.Users)
                    .HasForeignKey(userSubscription => userSubscription.SubscriptionId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(userSubscription => userSubscription.StartDate)
                    .IsRequired();

                entity.Property(userSubscription => userSubscription.EndDate)
                    .IsRequired();

                entity.ToTable(nameof(UserSubscriptions));
            });

            //builder.SeedUsers()
            //    .SeedRoles()
            //    .SeedUserRoles()
            //    .SeedContributorRoles()
            //    .SeedSubscriptions()
            //    .SeedTopics();

            base.OnModelCreating(builder);
        }
    }
}
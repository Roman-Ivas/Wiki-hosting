﻿using viki_01.Entities;

namespace viki_01.Models.Dto;

public class UserProfileDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public string AvatarPath { get; set; } = null!;
    public bool LockoutEnabled { get; set; }
    public Subscription Subscription { get; set; } = null!;
    public IEnumerable<Topic> InterestedTopics { get; set; } = new List<Topic>();
    public IEnumerable<Contributor> Contributions { get; set; } = new List<Contributor>();
    public IEnumerable<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    public IEnumerable<Comment> Comments { get; set; } = new List<Comment>();

    public IEnumerable<Rating> CreatedRatings { get; set; } = new List<Rating>();
    public IEnumerable<Report> CreatedReports { get; set; } = new List<Report>();

    public IEnumerable<Page> CreatedPages { get; set; } = new List<Page>();
    public IEnumerable<Theme> CreatedThemes { get; set; } = new List<Theme>();
    public IEnumerable<Template> CreatedTemplates { get; set; } = new List<Template>();
    public IEnumerable<string> Roles { get; set; } = new List<string>();
    public string? About { get; set; }
    public int NumberOfPages { get; set; }
}
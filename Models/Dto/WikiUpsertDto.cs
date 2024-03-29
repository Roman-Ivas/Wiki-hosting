﻿using System.ComponentModel.DataAnnotations;

namespace viki_01.Models.Dto;

public class WikiUpsertDto
{
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = null!;
}
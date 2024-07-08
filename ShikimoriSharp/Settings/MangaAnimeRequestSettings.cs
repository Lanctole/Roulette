﻿#nullable enable
using ShikimoriSharp.Enums;

namespace ShikimoriSharp.Settings
{
    public class MangaAnimeRequestSettingsBase : BasicSettings
    {
        public bool? censored;
        public int[]? exclude_ids;
        public int[]? franchise;
        public int? genre;
        public int[]? ids;
        public string? kind;
        public Rating? rating;
        public MyList? mylist;
        public Order? order;
        public int? score;
        public string? search;
        public string? season;
        public string? status;
    }
}
#nullable enable
using ShikimoriSharp.Enums;

namespace ShikimoriSharp.Settings;

public class MangaAnimeRanobeRequestSettingsBase : BasicSettings
{
    public bool? censored;
    public int[]? exclude_ids;
    public int[]? franchise;
    public string? genre;
    public int[]? ids;
    public string? kind;
    public Rating? rating;
    public Order? order;
    public int? score;
    public string? search;
    public string? season;
    public string? status;
}
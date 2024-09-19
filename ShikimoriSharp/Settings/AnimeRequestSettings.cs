#nullable enable
using ShikimoriSharp.Enums;

namespace ShikimoriSharp.Settings;

public class AnimeRequestSettings : MangaAnimeRanobeRequestSettingsBase
{
    public Duration? duration;
    public string? studio;
}
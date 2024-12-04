using System.ComponentModel;

namespace ShikimoriSharp.Enums;

public enum AnimeKind
{
    /// <summary>
    /// TV-сериал.
    /// </summary>
    [Description("TV-сериал")]
    tv,

    /// <summary>
    /// Фильм.
    /// </summary>
    [Description("Фильм")]
    movie,

    /// <summary>
    /// OVA — сокращение термина Original Video Animation, в Японии так обозначается особый формат аниме-сериала.
    /// </summary>
    [Description("OVA — сокращение термина Original Video Animation")]
    ova,

    /// <summary>
    /// ONA — веб-аниме.
    /// </summary>
    [Description("ONA — веб-аниме")]
    ona,

    /// <summary>
    /// Спецвыпуск.
    /// </summary>
    [Description("Спецвыпуск")]
    special,

    /// <summary>
    /// Телевизионный спецвыпуск.
    /// </summary>
    [Description("Телевизионный спецвыпуск")]
    tv_special,

    /// <summary>
    /// Музыкальный клип.
    /// </summary>
    [Description("Музыкальный клип")]
    music,

    /// <summary>
    /// Промо-материал.
    /// </summary>
    [Description("Промо-материал")]
    pv,

    /// <summary>
    /// Реклама.
    /// </summary>
    [Description("Реклама")]
    cm,

    /// <summary>
    /// ТВ-сериал с 12-13 сериями.
    /// </summary>
    [Description("ТВ-сериал с 12-13 сериями")]
    tv_13,

    /// <summary>
    /// ТВ-сериал с 24 сериями.
    /// </summary>
    [Description("ТВ-сериал с 24 сериями")]
    tv_24,

    /// <summary>
    /// ТВ-сериал с 40 и более сериями.
    /// </summary>
    [Description("ТВ-сериал с 40 и более сериями")]
    tv_48
}

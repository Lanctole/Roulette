using System.ComponentModel;

namespace ShikimoriSharp.Enums;

public enum Rating
{
    /// <summary>
    ///  G - All ages
    /// </summary>
    [Description("G - Нет возрастных ограничений")]
    g,
    /// <summary>
    ///  PG - Children
    /// </summary>
    [Description("PG - Детское")]
    pg,
    /// <summary>
    ///  PG-13 - Teens 13 or older
    /// </summary>
    [Description("PG - 13 Для детей старше 13 лет")]
    pg_13,
    /// <summary>
    /// R - 17 plus recommended (violence and profanity)
    /// </summary>
    [Description("R - 18+ Содержит жестокость и/или ненормативную лексику")]
    r,
    /// <summary>
    /// R_plus - Mild Nudity (may also contain violence and profanity)
    /// </summary>
    [Description("R+ -18+ Содержит сцены оголения")]
    r_plus,
    /// <summary>
    /// Rx - Hentai (extreme sexual content/nudity)
    /// </summary>
    [Description("RX - 18+ Хентай")]
    rx
}
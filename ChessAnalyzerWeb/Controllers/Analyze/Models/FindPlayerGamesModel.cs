using Domain.GameAggregate;
using System.ComponentModel.DataAnnotations;

namespace ChessAnalyzerApi.Controllers.Analyze.Models;

public class FindPlayerGamesModel
{
    /// <summary>
    /// Логин игрока на шахматной платформе, партии которого мы ищем
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Обязательное поле!")]
    [StringLength(maximumLength: 50, ErrorMessage = "Длина логина должна быть в диапазоне [1, 50] символов!", MinimumLength = 1)]
    public string userName { get; set; }

    /// <summary>
    /// Шахматная платформа на которой ищем игру
    /// </summary>
    [Required(ErrorMessage = "Обязательное поле!")]
    //  [JsonConverter(typeof(JsonStringEnumConverter))]
    public ChessPlatform platform { get; set; }

    /// <summary>
    /// Дата поиска игр "с"
    /// </summary>
    public DateTime? since { get; set; }

    /// <summary>
    /// Дата поиска игр "по"
    /// </summary>
    public DateTime? until { get; set; }
}
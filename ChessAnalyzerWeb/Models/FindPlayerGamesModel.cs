using Domain.GameAggregate;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ChessAnalyzerApi.Models;

public class FindPlayerGamesModel
{
    /// <summary>
    /// Логин игрока на шахматной платформе, партии которого мы ищем
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Обязательное поле!")]
    [StringLength(maximumLength: 50, ErrorMessage = "Длина логина должна в диапазоне [1, 50] символов!", MinimumLength = 1)]
    public string userName { get; set; }

    /// <summary>
    /// Шахматная платформа на которой ищем игру
    /// </summary>
   // [JsonConverter(typeof(JsonStringEnumConverter))]
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
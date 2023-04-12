using Domain.GameAggregate;
using System.ComponentModel.DataAnnotations;

namespace ChessAnalyzerApi.Models;

public class AnalyzeInfoModel
{
    /// <summary>
    /// Логин игрока, партии которого будут анализироваться
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "Обязательное поле!")]
    [StringLength(maximumLength: 50, ErrorMessage = "Длина логина должна в диапазоне [1, 50] символов!", MinimumLength = 1)]
    public string userName { get; set; }

    /// <summary>
    /// Точность перепада оценки для признания хода в позиции ошибочным 
    /// </summary>
    [Required(ErrorMessage = "Обязательное поле!")]
    [Range(minimum: 0.1d, maximum: 10.0d, ErrorMessage = "Точность перепада оценки ∈ [0.1, 10]")]
    public double precision { get; set; }

    /// <summary>
    /// Шахматная платформа на которой игрок играл свои партии
    /// </summary>
    public ChessPlatform platform { get; set; }
}
using System.Linq;
using AutoMapper;
using ChessAnalyzerApi.ExternalApi.Lichess.Models;
using Domain.GameAggregate;

namespace ChessAnalyzerApi.ExternalApi.Lichess.Mapping;

public class EvaluationProfile : Profile
{
    public EvaluationProfile()
    {
        CreateMap<LichessEvaluationModel, PositionEvaluation>()
            .ForMember(dest => dest.Depth, opt => opt.MapFrom(u => u.depth))
            .ForMember(dest => dest.Fen, opt => opt.MapFrom(u => u.fen))
            .ForMember(dest => dest.Cp, opt => opt.MapFrom(u => u.pvs != null
                                                               && u.pvs!.FirstOrDefault() != null
                                                               ? u.pvs!.FirstOrDefault()!.cp / 100.0d : 0))
            .ForMember(dest => dest.OneMove, opt => opt.MapFrom(u => u.pvs != null
                                                                    && u.pvs!.FirstOrDefault() != null
                                                                    && u.pvs!.FirstOrDefault()!.moves.Length > 4
                                                                    ? u.pvs!.FirstOrDefault()!.moves.Substring(0, 4) : string.Empty))
            .ReverseMap();
    }
}

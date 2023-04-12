using AutoMapper;
using Domain.GameAggregate;
using ChessAnalyzerApi.ExternalApi.Lichess.Models;

namespace ChessAnalyzerApi.Services.Lichess.Mapping;

public class LichessEvaluationProfile : Profile
{
    public LichessEvaluationProfile()
    {
        CreateMap<LichessEvaluationModel, PositionEvaluation>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
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
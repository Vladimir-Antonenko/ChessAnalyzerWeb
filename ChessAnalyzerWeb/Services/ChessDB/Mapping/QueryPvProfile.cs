using AutoMapper;
using ChessAnalyzerApi.Services.ChessDB.Models;
using Domain.GameAggregate;

namespace ChessAnalyzerApi.Services.ChessDB.Mapping
{
    public class QueryPvProfile : Profile
    {
        public QueryPvProfile()
        {
            CreateMap<QueryPvModel, PositionEvaluation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Depth, opt => opt.MapFrom(u => u.depth))
                .ForMember(dest => dest.Fen, opt => opt.Ignore())
                .ForMember(dest => dest.Cp, opt => opt.MapFrom(u => u.score / 100.0d))
                .ForMember(dest => dest.OneMove, opt => opt.MapFrom(u => u.pv != null
                                                                        && u.pv!.FirstOrDefault() != null
                                                                        && u.pv!.FirstOrDefault()!.Length >= 4
                                                                        ? u.pv!.FirstOrDefault()! : string.Empty))
                .ReverseMap();
        }
    }
}

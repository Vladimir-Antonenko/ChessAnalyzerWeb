using AutoMapper;
using ChessAnalyzerApi.ExternalApi.Lichess.Models;
using Domain.GameAggregate;

namespace ChessAnalyzerApi.Services.Lichess.Mapping;

public class PgnProfile : Profile
{
    public PgnProfile()
    {
        CreateMap<LichessPgnModel, Pgn>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(u => u.Content))
            .ReverseMap();
    }
}
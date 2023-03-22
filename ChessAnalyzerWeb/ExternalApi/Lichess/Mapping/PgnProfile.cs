using AutoMapper;
using ChessAnalyzerApi.ExternalApi.Lichess.Models;
using Domain.GameAggregate;
using System.Linq;

namespace ChessAnalyzerApi.ExternalApi.Lichess.Mapping;

public class PgnProfile : Profile
{
    public PgnProfile()
    {
        CreateMap<LichessPgnModel, Pgn>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(u => u.Content))
            .ReverseMap();
    }
}

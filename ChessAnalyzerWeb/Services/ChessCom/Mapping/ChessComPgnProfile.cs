using AutoMapper;
using Domain.GameAggregate;
using ChessAnalyzerApi.Services.ChessCom.Models;

namespace ChessAnalyzerApi.Services.ChessCom.Mapping;

public class ChessComPgnProfile : Profile
{
    public ChessComPgnProfile()
    {
        CreateMap<ChessComPgnModel, Pgn>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(u => u.Content))
            .ReverseMap();
    }
}
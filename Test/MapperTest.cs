using Xunit;
using AutoMapper;
using ChessAnalyzerApi.Services.Lichess.Mapping;
using ChessAnalyzerApi.Services.ChessDB.Mapping;
using ChessAnalyzerApi.Services.ChessCom.Mapping;

namespace Test;
public class MapperTest
{
    [Fact]
    public void LichessEvaluationProfile_VerifyMappings()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<LichessEvaluationProfile>());
        var mapper = new Mapper(config);

        (mapper as IMapper).ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public void LichessPgnProfile_VerifyMappings()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<LichessPgnProfile>());
        var mapper = new Mapper(config);

        (mapper as IMapper).ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public void QueryPvProfile_VerifyMappings()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<QueryPvProfile>());
        var mapper = new Mapper(config);

        (mapper as IMapper).ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public void ChessComPgnProfile_VerifyMappings()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<ChessComPgnProfile>());
        var mapper = new Mapper(config);

        (mapper as IMapper).ConfigurationProvider.AssertConfigurationIsValid();
    }
}
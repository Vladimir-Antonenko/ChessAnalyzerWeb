using Xunit;
using AutoMapper;
using ChessAnalyzerApi.Services.Lichess.Mapping;

namespace Test;
public class MapperTest
{
    [Fact]
    public void EvaluationProfile_VerifyMappings()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<EvaluationProfile>());
        var mapper = new Mapper(config);

        (mapper as IMapper).ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public void PgnProfile_VerifyMappings()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<PgnProfile>());
        var mapper = new Mapper(config);

        (mapper as IMapper).ConfigurationProvider.AssertConfigurationIsValid();
    }
}
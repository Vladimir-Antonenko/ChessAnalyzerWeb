using AutoMapper;
using ChessAnalyzerApi.ExternalApi.Lichess.Mapping;
using Xunit;

namespace Test
{
    public class MapperTest
    {
        [Fact]
        public void EvaluationProfile_VerifyMappings()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<EvaluationProfile>());
            var mapper = new Mapper(config);

            (mapper as IMapper).ConfigurationProvider.AssertConfigurationIsValid();
        }

        public void PgnProfile_VerifyMappings()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<PgnProfile>());
            var mapper = new Mapper(config);

            (mapper as IMapper).ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
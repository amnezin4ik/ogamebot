using AutoMapper;
using NUnit.Framework;
using OGame.Bot.Application;
using OGame.Bot.Domain.Services;
using OGame.Bot.Wpf;

namespace OGame.Bot.UnitTests
{
    [TestFixture]
    public class MappingProfilesTest
    {
        [Test]
        public void ShouldHaveValidMappingProfiles()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile(typeof(WpfMappingProfile));
                cfg.AddProfile(typeof(ApplicationMappingProfile));
                cfg.AddProfile(typeof(DomainMappingProfile));
            });

            Mapper.Configuration.AssertConfigurationIsValid();
        }
    }
}
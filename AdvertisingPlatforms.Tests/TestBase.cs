using AutoFixture;
using AutoFixture.AutoMoq;

namespace AdvertisingPlatformsTests;

public abstract class TestBase
{
    protected readonly IFixture Fixture;
    
    protected TestBase()
    {
        Fixture = new Fixture()
            .Customize(new AutoMoqCustomization());
    }
} 
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;

namespace SchoolMS.Application.Tests.Shared;

public static class TestCacheHelper
{
    public static HybridCache CreateHybridCache()
    {
        var services = new ServiceCollection();

        services.AddLogging();                
        services.AddDistributedMemoryCache(); 
        services.AddHybridCache();            

        var sp = services.BuildServiceProvider();
        return sp.GetRequiredService<HybridCache>();
    }
}
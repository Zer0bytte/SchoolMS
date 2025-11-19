using SchoolMS.Application.Common.Interfaces;

namespace SchoolMS.Infrastructure.Settings;

public class AppSettings : IAppSettings
{
    public int LocalCacheExpirationInMins { get; set; }
    public int DistributedCacheExpirationMins { get; set; }
    public string CorsPolicyName { get; set; } = default!;
    public string[] AllowedOrigins { get; set; } = default!;
}
namespace SchoolMS.Application.Common.Interfaces;

public interface IAppSettings
{
    string[] AllowedOrigins { get; set; }
    string CorsPolicyName { get; set; }
    int DistributedCacheExpirationMins { get; set; }
    int LocalCacheExpirationInMins { get; set; }
}

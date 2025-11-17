namespace SchoolMS.Application.Common.Interfaces;

public interface IAppSettings
{
    string[] AllowedOrigins { get; set; }
    string CorsPolicyName { get; set; }
    string MeetingDay { get; set; }
    int DefaultPageNumber { get; set; }
    int DefaultPageSize { get; set; }
    int DistributedCacheExpirationMins { get; set; }
    int LocalCacheExpirationInMins { get; set; }
}

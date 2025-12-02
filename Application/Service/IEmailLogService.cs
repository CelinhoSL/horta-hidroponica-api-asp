namespace Horta_Api.Application.Interfaces
{
    public interface IEmailLogService
    {
        
        Task<bool> SendUserLogEmailAsync(int userId, int logId);

        
        Task<bool> SendUserLogEmailAsync(int userId, int logId, string customEmail);

       
        Task<bool> SendUserLogsReportAsync(int userId);

        
        Task<bool> SendSecurityAlertEmailAsync(int userId, int logId, string registeredIp, string currentIp);
    }
}
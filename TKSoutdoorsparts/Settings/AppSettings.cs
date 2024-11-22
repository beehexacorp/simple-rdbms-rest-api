namespace SimpleRDBMSRestfulAPI.Settings;
public class AppSettings : IAppSettings
{
    public string ConnectionString => Environment.GetEnvironmentVariable("CONNECTION_STRING")!;
}
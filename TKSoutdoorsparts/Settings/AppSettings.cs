using hexasync.infrastructure.dotnetenv;

namespace TKSoutdoorsparts.Settings
{
    public class AppSettings
    {
        private readonly IEnvReader _envReader;
        public AppSettings(IEnvReader envReader)
        {
            _envReader = envReader;
            EnvLoader.LoadEnvs();
        }
        public string ODBCConnectionString => _envReader.Read("TKS_ODBC_CONNECTION_STRING");
    }
}

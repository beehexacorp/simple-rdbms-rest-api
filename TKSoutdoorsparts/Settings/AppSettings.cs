using hexasync.infrastructure.dotnetenv;
using IEnvReader = hexasync.infrastructure.dotnetenv.IEnvReader;

namespace TKSoutdoorsparts.Settings
{
    public class AppSettings : IAppSettings
    {
        private readonly IEnvReader _envReader;

        public AppSettings(IEnvReader envReader)
        {
            _envReader = envReader;
            EnvLoader.LoadEnvs();
        }

        public string ConnectionString => _envReader.Read("CONNECTION_STRING")!;
    }
}

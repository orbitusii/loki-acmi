global using loki_bms_common;
using loki_acmi;
using loki_bms_common.Plugins;

[assembly: LokiPlugin(
    RootType = typeof(LokiACMIPlugin),
    PluginName = "LOKI-ACMI",
    Description = "A plugin for LOKI that interfaces with Tacview Real Time Telementry",
    Author = "Robin Lodholm",
    DesiredLokiVersion = LokiVersion.Any,
    Version = "0.1.0",
    DataSourceTypes = new Type[] { typeof(ACMISource) },
    CustomMenuTypes = new Type[] { }
    )]

namespace loki_acmi;

public class LokiACMIPlugin : LokiPlugin
{
    public override void Init()
    {
        // Nothing to do here
    }
}

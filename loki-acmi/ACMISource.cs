using acmi_interpreter;
using loki_bms_common.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using tacview_net;

namespace loki_acmi;

[XmlInclude(typeof(ACMISource))]
public class ACMISource : LokiDataSource
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public TacviewNetworker Networker { get; set; } = new TacviewNetworker("hostname", 0);
    private ACMIMission Mission { get; set; } = new ACMIMission();
    private CancellationTokenSource cts = new CancellationTokenSource();

    public override void Activate()
    {
        Networker.Start(Username, Password);
        Task.Run(() => { UpdateMission(Networker, cts.Token)});
    }

    public override bool CheckAlive() => Networker.IsRunning;

    private void UpdateMission(TacviewNetworker networker, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            List<string> messages = new List<string>();
            while(networker.QueuedMessages.TryDequeue(out string? result))
                messages.Add(result);

            Mission.UpdateWithData(messages.ToArray());
            messages.Clear();
        }
    }

    public override void Deactivate()
    {
        Networker.Stop();
        cts.Cancel();
    }

    public override TrackDatum[] GetFreshData()
    {

    }
}

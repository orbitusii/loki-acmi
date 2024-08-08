using acmi_interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tacview_net;

namespace TestConsole;

internal class DummyPlugin
{
    public DummyPlugin() { }

    public ACMIMission Mission { get; init; } = new ACMIMission();

    internal async Task UpdateMissionAsync(TacviewNetworker source, CancellationToken cancellationToken)
    {
        await Task.Run(() => UpdateMission(source, cancellationToken));
    }

    /// <summary>
    /// Synchronous update function for this DummyPlugin
    /// </summary>
    /// <param name="source"></param>
    /// <param name="cancellationToken"></param>
    internal void UpdateMission(TacviewNetworker source, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (source.QueuedMessages.TryDequeue(out string? message))
            {
                Mission.UpdateWithData(new string[] { message });
                if (message.StartsWith('#'))
                {
                    var airObjects = Mission.GetAirObjects().ToArray();
                    var navaids = Mission.GetObjectsByTag("Navaid").ToArray();

                    Console.Clear();
                    Console.WriteLine($"T={Mission.CurrentTime_UTC} => {Mission.Objects.Count()} objects; {airObjects.Count()} air objects; {navaids.Count()} navaids");
                }
            }
        }
    }
}

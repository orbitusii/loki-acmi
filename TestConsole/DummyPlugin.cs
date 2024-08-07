using acmi_interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tacview_net;

namespace TestConsole
{
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
                List<string> messages = new List<string>();
                if (source.QueuedMessages.TryDequeue(out string message))
                    messages.Add(message);

                if (messages.Count <= 0)
                    continue;

                Mission.UpdateWithData(messages.ToArray());
                messages.Clear();
                Console.WriteLine($"T={Mission.CurrentFrame} => {Mission.Objects.Count()} objects");
            }
        }
    }
}

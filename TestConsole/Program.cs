// See https://aka.ms/new-console-template for more information
using tacview_net;
using TestConsole;

Console.WriteLine("Hello, World!");

string? hostname = null;
int? port = null;
string? username = null;
string? password = null;

while (hostname is null && hostname != "exit")
{
    Console.Write("Input a hostname: ");
    hostname = Console.ReadLine();
}
while(port is null)
{
    Console.Write("Input the port you want to use: ");
    if(int.TryParse(Console.ReadLine(), out int result))
        port = result;
}
while (username is null && username != "exit")
{
    Console.Write("Input a username: ");
    username = Console.ReadLine();
}
Console.Write("Input the password for this Tacview host: ");
password = Console.ReadLine();

CancellationTokenSource cts = new CancellationTokenSource();

TacviewNetworker networker = new TacviewNetworker(hostname, port ?? 42674);
networker.Start(username, password);

DummyPlugin plugin = new DummyPlugin();
await plugin.UpdateMissionAsync(networker, cts.Token);

while (!cts.Token.IsCancellationRequested)
{
    Console.Write("Enter command: ");
    var command = Console.ReadLine();

    if (command == "exit") cts.Cancel();
}


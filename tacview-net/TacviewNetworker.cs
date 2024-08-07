using System.Net;
using System.Net.Sockets;
using System.IO.Hashing;
using System.Security;
using System.Text;
using System;
using System.Text.Unicode;
using acmi_interpreter;
using System.Collections.Concurrent;
using System.Threading;

namespace tacview_net;

public class TacviewNetworker : IDisposable
{
    protected Socket TCPSocket;
    public IPAddress Address { get; set; }
    public int Port { get; set; }
    public bool PurgeQueueOnNewFrame { get; set; }
    private NetworkStream? NetworkStream { get; set; }

    public ConcurrentQueue<string> QueuedMessages = new ConcurrentQueue<string>();

    public TacviewNetworker(string hostname, int port, bool PurgeOnNewFrame = false)
    {
        TCPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        TCPSocket.ReceiveBufferSize = 8192;
        var host = Dns.GetHostEntry(hostname);
        Address = host.AddressList[0];
        Port = port;
        PurgeQueueOnNewFrame = PurgeOnNewFrame;
    }

    public async Task TryStreamDataAsync(string username, string? password, CancellationToken cancellationToken)
    {
        IPEndPoint ipe = new IPEndPoint(Address, Port);

        string hash = string.IsNullOrWhiteSpace(password) ? "0" : HashPassword(password);

        TCPSocket.Connect(ipe);

        if (!await Handshake(username, hash))
        {
            Console.WriteLine("Unable to connect - no handshake sent");
        }

        NetworkStream = new NetworkStream(TCPSocket, true);
        StreamReader Reader = new StreamReader(NetworkStream, Encoding.UTF8);
        StringBuilder lineBuilder = new StringBuilder(512);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                char c = (char)Reader.Read();
                if (c == '\n')
                {
                    QueuedMessages.Enqueue(lineBuilder.ToString());
                    lineBuilder.Clear();
                    continue;
                }
                else if (c == -1)
                {
                    continue;
                }
                else lineBuilder.Append(c);

            }
            catch (Exception ex) { }
        }

        NetworkStream.Dispose();
        Reader.Dispose();
    }

    private string HashPassword(string password)
    {
        byte[] bytes = Encoding.Unicode.GetBytes(password);
        Crc64 crc = new Crc64();
        crc.Append(bytes);

        byte[] hash = crc.GetHashAndReset();
        return Encoding.UTF8.GetString(hash);
    }

    private async Task<bool> Handshake(string username, string passHash)
    {
        byte[] buffer = new byte[8192];
        var received = await TCPSocket.ReceiveAsync(buffer, SocketFlags.None);

        string[] handshake = Encoding.UTF8.GetString(buffer, 0, received).Split("\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (handshake.Length > 0)
        {
            Console.WriteLine($"Low level: {handshake[0]}\nHigh level: {handshake[1]}\nHost user: {handshake[2]}");

            string reply = $"{handshake[0]}\n{handshake[1]}\n{username}\n{passHash}\0";
            Console.WriteLine($"Reply: {reply}\n-----=====-----\n");

            await TCPSocket.SendAsync(Encoding.UTF8.GetBytes(reply), SocketFlags.None);

            return true;
        }
        else return false;
    }

    public void Dispose()
    {
        TCPSocket.Disconnect(true);
        TCPSocket.Dispose();
    }
}
using System.Net;
using System.Net.Sockets;
using System.IO.Hashing;
using System.Security;
using System.Text;
using System;
using System.Text.Unicode;
using acmi_interpreter;
using System.Collections.Concurrent;

namespace tacview_net;

public class TacviewNetworker : IDisposable
{
    protected Socket TCPSocket;
    public IPAddress Address { get; set; }
    public int Port { get; set; }
    public bool PurgeQueueOnNewFrame { get; set; }

    public ConcurrentQueue<string> QueuedMessages = new ConcurrentQueue<string>();

    public TacviewNetworker(string hostname, int port, bool PurgeOnNewFrame = false)
    {
        TCPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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

        byte[] buffer = new byte[8192];

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                int received = await TCPSocket.ReceiveAsync(buffer, SocketFlags.None, cancellationToken);

                if (received == 0) continue;

                string rawMessage = Encoding.UTF8.GetString(buffer, 0, received);
                //Console.WriteLine(rawMessage);

                string[] lines = rawMessage.Split("\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                
                if(PurgeQueueOnNewFrame) QueuedMessages.Clear();
                foreach(string line in lines)
                    QueuedMessages.Enqueue(line);
            }
            catch (Exception ex)
            {

            }
        }
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
            Console.WriteLine($"Reply: {reply}");

            await TCPSocket.SendAsync(Encoding.UTF8.GetBytes(reply), SocketFlags.None);

            return true;
        }
        else return false;
    }

    public void Dispose()
    {
        TCPSocket.Dispose();
    }
}
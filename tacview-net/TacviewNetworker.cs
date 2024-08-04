using System.Net;
using System.Net.Sockets;
using System.IO.Hashing;
using System.Security;
using System.Text;
using System;

namespace tacview_net;

public class TacviewNetworker : IDisposable
{
    protected Socket TCPSocket;
    public IPAddress Address { get; set; }
    public int Port { get; set; }

    public TacviewNetworker(string hostname, int port)
    {
        TCPSocket = new Socket(AddressFamily.Unspecified, SocketType.Stream, ProtocolType.Tcp);
        var host = Dns.GetHostEntry(hostname);
        Address = host.AddressList[0];
        Port = port;
    }

    public async Task TryStreamData(string username, string? password, CancellationToken cancellationToken)
    {
        IPEndPoint ipe = new IPEndPoint(Address, Port);

        string hash = password is null ? "0" : HashPassword(password);

        TCPSocket.Bind(ipe);

        await Handshake(username, hash);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {

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

    private async Task<bool> Handshake (string username, string passHash)
    {
        byte[] buffer = new byte[8192];
        var received = await TCPSocket.ReceiveAsync(buffer, SocketFlags.None);

        string[] handshake = Encoding.UTF8.GetString(buffer, 0, received).Split("\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (handshake.Length > 0) { return true; }
        else return false;
    }

    public void Dispose()
    {
        TCPSocket.Dispose();
    }
}
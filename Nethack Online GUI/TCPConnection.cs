using System;

using System.IO;
using System.Net.Sockets;
//using System.Net;

class TCPConnection
{
    TcpClient tcpClient;
    NetworkStream networkStream;
    byte[] bufferReceived;
    int bytesReceived;

    public TCPConnection()
    {
        tcpClient = new TcpClient();
        bufferReceived = new byte[8192];
        bytesReceived = 0;
    }

    public TCPConnection(string host, int port)
    {
        tcpClient = new TcpClient();
        bufferReceived = new byte[8192];
        bytesReceived = 0;

        Connect(host, port);
    }

    public bool Connect(string host, int port)
    {
        tcpClient.Connect(host, port);

        networkStream = tcpClient.GetStream();
        
        return tcpClient.Connected;
    }

    public bool Disconnect()
    {
        if (tcpClient.Connected == true)
        {
            networkStream.Close();
            tcpClient.Close();
        }

        return tcpClient.Connected;
    }

    public bool SendData(byte[] data)
    {
        networkStream.Write(data, 0, data.Length);

        return true;
    }
    
    public int ReceiveData()
    {
        bytesReceived = networkStream.Read(bufferReceived, 0, 8192);

        return bytesReceived;
    }

    public byte[] GetData()
    {
        return bufferReceived;
    }

    public int GetBytesReceived()
    {
        return bytesReceived;
    }

    public bool Connected
    {
        get
        {
            return tcpClient.Connected;
        }
    }

    public bool DataAvailable
    {
        get
        {
            return networkStream.DataAvailable;
        }
    }
}

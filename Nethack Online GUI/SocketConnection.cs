using System;

using System.IO;
using System.Net.Sockets;
//using System.Net;

class SocketConnection
{
    Socket socket;
    byte[] bufferReceived;
    int bytesReceived;

    public SocketConnection()
    {
        //connected = false;
    }

    public SocketConnection(string host, int port)
    {
        Connect(host, port);
    }

    public bool Connect(string host, int port)
    {
        //connected = false;

        try
        {
            //connected = true;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            bufferReceived = new byte[socket.ReceiveBufferSize];

            socket.Connect(host, port);
        }

        catch (Exception e)
        {
            //connected = false;
            Console.WriteLine("Error: " + e.ToString());
        }

        //return connected;
        return socket.Connected;
    }

    public bool Disconnect()
    {
        if (socket.Connected == true)
        {
            socket.Close();

            //connected = false;
        }

        //return connected;

        return socket.Connected;
    }

    public bool SendData(byte[] data)
    {
        socket.Send(data);

        return true;
    }

    public bool SendData(byte[] data, bool partial)
    {
        if (partial == true)
            socket.Send(data, SocketFlags.Partial);
        else
            socket.Send(data);

        return true;
    }

    public int ReceiveData()
    {
        bytesReceived = socket.Receive(bufferReceived);
        
        return bytesReceived;
    }

    public byte[] GetData()
    {
        return bufferReceived;
    }

    public int getBytesReceived()
    {
        return bytesReceived;
    }

    public bool isConnected()
    {
        return socket.Connected;
    }
}

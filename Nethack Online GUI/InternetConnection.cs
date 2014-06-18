using System;

using System.IO;
using System.Net.Sockets;
//using System.Net;

class InternetConnection
{
    TcpClient client;
    NetworkStream nStream;
    StreamReader sr;
    StreamWriter sw;
    bool connected;

    public InternetConnection()
    {
        connected = false;
    }

    public InternetConnection(string server, int port)
    {
        Connect(server, port);
    }

    public bool Connect(string server, int port)
    {
        connected = false;

        try
        {
            connected = true;
            client = new TcpClient(server, port);

            nStream = client.GetStream();
            sr = new StreamReader(nStream);
            sw = new StreamWriter(nStream);
        }

        catch (Exception e)
        {
            connected = false;
            Console.WriteLine("Error: " + e.ToString());
        }

        return connected;
    }

    public bool Disconnect()
    {
        if (connected == true)
        {
            client.Close();

            connected = false;
        }

        return connected;
    }

    public bool SendData(string data)
    {
        sw.WriteLine(data);
        sw.Flush();

        return true;
    }

    public bool SendData(byte[] data)
    {
        client.GetStream().Write(data, 0, data.Length);

        sw.Flush();

        return true;
    }

    public string GetData()
    {
        char[] buffer = new char[client.ReceiveBufferSize];

        sr.ReadBlock(buffer, 0, client.ReceiveBufferSize);

        return buffer.ToString();
    }

    public bool isConnected()
    {
        return connected;
    }
}

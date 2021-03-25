using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace websocket
{
    class Program
    {
        private static byte[] buffer = new byte[1024];
        private static List<Socket> clientSockets = new();
        private static Socket serverSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static void Main(string[] args)
        {
            Console.Title = "Server";
            SetupServer();
            Console.ReadKey(); 
        }
        
        private static void SetupServer()
        {
            Console.WriteLine("Setting up server....");
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 100));
            serverSocket.Listen(5);

            serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }
        
        private static void AcceptCallback(IAsyncResult AR)
        {
            Socket socket = serverSocket.EndAccept(AR);
            clientSockets.Add(socket);
            Console.WriteLine($"{serverSocket.LocalEndPoint}: connected");
            socket.BeginReceive(buffer, 0 , buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private static void ReceiveCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            int received = socket.EndReceive(AR);
            byte[] dataBuf = new byte[received];
            Array.Copy(buffer, dataBuf, received);
             
            string text = Encoding.ASCII.GetString(dataBuf);
            Console.WriteLine($"Text recieved from Client: \t{text} \t {DateTime.Now.ToLongTimeString()}");

            string response = $"Hello from Server \t {DateTime.Now.ToLongTimeString()}";
            byte[] data = Encoding.ASCII.GetBytes(response);
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);

        }

        private static void SendCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }
    }
}

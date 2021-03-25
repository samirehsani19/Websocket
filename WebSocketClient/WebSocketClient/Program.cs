using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WebSocketClient
{
    class Program
    {
        private static Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static void Main(string[] args)
        {
            Console.Title = "Client";
            LoopConnect();
            SendLoop();
            Console.ReadKey();
        }

        private static void SendLoop()
        {
            while (true)
            {
                Console.WriteLine("Enter a request");
                var kP = Console.ReadKey();
                var message = kP.KeyChar;

                byte[] buffer = Encoding.ASCII.GetBytes(message.ToString());
                clientSocket.Send(buffer);

                byte[] recieveBuffer = new byte[1024];
                int rec = clientSocket.Receive(recieveBuffer);
                byte[] data = new byte[rec];
                Array.Copy(recieveBuffer, data, rec);
                Console.WriteLine($"\nText received from Server: {Encoding.ASCII.GetString(data)}");
            }
        }

        private static void LoopConnect()
        {
            int attempts = 0;
            while (!clientSocket.Connected)
            {
                try
                {
                    attempts++;
                    clientSocket.Connect(IPAddress.Loopback, 100);
                }
                catch(SocketException)
                {
                    Console.Clear();
                    Console.WriteLine($"Connections attemps: {attempts}");
                }
            }
            Console.Clear();
            Console.WriteLine($"{clientSocket.RemoteEndPoint}: Connected");
        }
    }
}

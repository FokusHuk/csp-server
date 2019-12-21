using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace myapp
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server(new IPEndPoint(IPAddress.Parse("0.0.0.0"), 1111));

            server.Start();

            Console.WriteLine("Listening started.");

            int count = 1;

            while (true)
            {
                server.Listen();
                
                string message = server.ReceiveMessage();
                Console.WriteLine("Message from client: " + message);

                server.SendMessage("Server message: " + count);
                count += 1;

                server.CloseConnection();
            }

            server.Close();
        }


    }

    class Server
    {
        private IPEndPoint _endPoint;
        private readonly Encoding _baseEncoding = Encoding.UTF8;
        private Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private Socket _clientSocket;

        public Server(IPEndPoint endPoint)
        {
            _endPoint = endPoint;
        }

        public void Start()
        {
            _serverSocket.Bind(_endPoint);
            _serverSocket.Listen(10);
        }

        public void Listen()
        {
            _clientSocket = _serverSocket.Accept();
        }

        public void Close()
        {
            CloseConnection();
            _serverSocket.Close();
        }

        public void CloseConnection()
        {
            if (_clientSocket != null)
            {
                _clientSocket.Close();
            }
        }

        public void SendMessage(string message)
        {
            SendMessage(message, _baseEncoding);
        }

        public void SendMessage(string message, Encoding encoding)
        {
            if (_clientSocket.Connected == false)
            {
                return;
            }

            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    BinaryWriter writer = new BinaryWriter(stream, encoding, false);
                    writer.Write(message);
                    _clientSocket.Send(stream.ToArray());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public string ReceiveMessage()
        {
            return ReceiveMessage(_baseEncoding);
        }

        public string ReceiveMessage(Encoding encoding)
        {
            if (_clientSocket.Connected == false)
            {
                return "";
            }

            string message = null;

            try
            {
                using (MemoryStream stream = new MemoryStream(new byte[256], 0, 256, true, true))
                {
                    BinaryReader reader = new BinaryReader(stream, encoding, false);
                    _clientSocket.Receive(stream.GetBuffer());
                    message = reader.ReadString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return message;
        }
    }
}

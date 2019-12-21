using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace TestClient
{
    class Client
    {
        private IPEndPoint _endPoint;
        private readonly Encoding _baseEncoding = Encoding.UTF8;
        private Socket _clientSocket;

        public Client(IPEndPoint endPoint)
        {
            _endPoint = endPoint;
        }

        public void Connect()
        {
            try
            {
                _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _clientSocket.Connect(_endPoint);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Close()
        {
            _clientSocket.Close();
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
                return "";
            }

            return message;
        }
    }
}

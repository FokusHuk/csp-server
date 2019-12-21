using System;
using System.Net;

namespace TestClient
{
    class MainClass
    {
        static void Main(string[] args)
        {
            Client client = new Client(new IPEndPoint(IPAddress.Parse("192.168.99.100"), 1111));

            while (true)
            {
                client.Connect();

                Console.WriteLine("Enter the message: ");
                string message = Console.ReadLine();

                client.SendMessage(message);

                string responce = client.ReceiveMessage();
                Console.WriteLine(responce);

                client.Close();

                ConsoleKey key = Console.ReadKey().Key;
                if (key == ConsoleKey.Escape)
                    break;
            }

            client.Close();

            Console.ReadLine();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace DiceWars
{
    class Client
    {
        public Client()
        {

        }

        public string Connect(string server, string message)
        {
            string responseData = null;
            try
            {
                int port = 300;
                TcpClient client = new TcpClient(server, port);

                byte[] data = Encoding.ASCII.GetBytes(message);
                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);
                Console.WriteLine("Sent: {0}", message);
                data = new byte[256];

                int bytes = stream.Read(data, 0, data.Length);
                responseData = Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);
                stream.Close();
                client.Close();

            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine("IOException: {0}", e);
            }
            return responseData;
        }
    }
}

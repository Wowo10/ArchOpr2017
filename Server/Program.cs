using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Program
    {
        struct Game
        {
            string p1, p2; //porty
            List<int> p1hex, p2hex, hexvalues; //zwykle listy
            int xmap, ymap, mapsize; //rozmiar
        }

        static void Main(string[] args)
        {
            TcpListener server = null;
            Console.WriteLine("Wpisz adres serwera (np. 127.0.0.1, 127.0.0.2 itp)");
            string ip = Console.ReadLine();
            try
            {
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse(ip);
                server = new TcpListener(localAddr, port);
                server.Start();
                Byte[] bytes = new Byte[256];
                String data = null;

                while (true)
                {
                    Console.Write("Server started! Waiting for packets...");
                    TcpClient client = server.AcceptTcpClient();
                    data = null;
                    NetworkStream stream = client.GetStream();

                    int i;

                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        data = Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received\t{1}{0}", data, client.Client.RemoteEndPoint.ToString().Split(':')[1]);

                        data = data.ToUpper();
                        byte[] msg = Encoding.ASCII.GetBytes(data);
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent:\t{0}", data);
                    }
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }


            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }
    }
}

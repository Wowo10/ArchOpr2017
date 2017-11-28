using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Threading;

namespace Server
{
    class Program
    {
        static Dictionary<string, DateTime> TimeOut = new Dictionary<string, DateTime>();
        static List<string> keys = new List<string>();

        static void Manage()
        {
            while (true)
            {
                for (int i = 0; i < keys.Count; i++)
                {
                    string tmpKey = keys[i];

                    if (tmpKey != null && TimeOut.ContainsKey(tmpKey))
                    {
                        TimeSpan time = DateTime.Now - TimeOut[tmpKey];
                        if (time.TotalSeconds > 5)
                        {
                            Console.WriteLine("Someone left");
                            TimeOut.Remove(tmpKey);
                            keys.Remove(tmpKey);
                        }
                    }
                }
            }
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        static void Main(string[] args)
        {
            Thread thread = new Thread(Manage);
            thread.Start();

            int port = 300;
            string data;
            Byte[] bytes = new Byte[256];

            TcpListener server = null;
            try
            {
                server = new TcpListener(IPAddress.Parse(GetLocalIPAddress()), port);
                server.Start();
                data = null;

                while (true)
                {
                    Console.Write("Server started on ip - {0}, port - {1}! Waiting for packets...", GetLocalIPAddress(), port);
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Someone has connected!");

                    string key = client.Client.RemoteEndPoint.ToString();
                    if (TimeOut.ContainsKey(key))
                    {
                        TimeOut[key] = DateTime.Now;
                    }
                    else
                    {
                        Console.WriteLine("Someone new joined from {0}", key);
                        keys.Add(key);
                        TimeOut.Add(key, DateTime.Now);
                    }

                    NetworkStream stream = client.GetStream();

                    int i;
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        data = Encoding.ASCII.GetString(bytes, 0, i);
                        byte[] msg = Encoding.ASCII.GetBytes(data.ToUpper());
                        stream.Write(msg, 0, msg.Length);

                        Console.WriteLine("Received\t{1}\t from: {0}", data, client.Client.RemoteEndPoint.ToString());
                        Console.WriteLine("Sent:\t{0}", data.ToUpper());
                    }
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                server.Stop();
                server.Start();
            }
            finally
            {
                server.Stop();
            }


            Console.WriteLine("\nHit enter to exit...");
            Console.Read();
        }
    }
}

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Server
{
    class Program
    {
        static Dictionary<string, DateTime> TimeOut = new Dictionary<string, DateTime>();
        static List<string> keys = new List<string>();

        static void Mange()
        {
            while (true)
            {
                int index = 0;
                while (index<keys.Count)
                {
                    string tmpKey = keys[index];

                    if (tmpKey!=null && TimeOut.ContainsKey(tmpKey))
                    {
                        TimeSpan time = DateTime.Now - TimeOut[tmpKey];
                        if (time.TotalSeconds > 5)
                        {
                            Console.WriteLine("Has left");
                            TimeOut.Remove(tmpKey);
                            keys.Remove(tmpKey);
                        }
                    }
                    index += 1;
                }                                             
            
                
            }

        }


        static void Main(string[] args)
        {
            Thread thread = new Thread(Mange);
            thread.Start();

            TcpListener server = null;
            try
            {
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse("192.168.1.2");
                server = new TcpListener(localAddr, port);
                server.Start();
                Byte[] bytes = new Byte[256];
                String data = null;
                while (true)
                {
                    Console.Write("Waiting for a connection... ");
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");
                    string key = client.Client.RemoteEndPoint.ToString().Split(':')[0];
                    if (TimeOut.ContainsKey(key))
                    {
                        TimeOut[key] = DateTime.Now;
                    }
                    else
                    {
                        Console.WriteLine("Has joined");
                        keys.Add(key);
                        TimeOut.Add(key, DateTime.Now);
                    }
                    
                    data = null;
                    NetworkStream stream = client.GetStream();

                    int i;

                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        data = Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);
                        data = data.ToUpper();

                        byte[] msg = Encoding.ASCII.GetBytes(data);

                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
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


            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }
    }
}

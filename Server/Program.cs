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
        static Dictionary<string, Queue<string>> orders = new Dictionary<string, Queue<string>>();
        static int whoseTurn = -1;
        static int[] fieldsState =new int[36];//ilość kostek na danym polu
        static int[] diecesState = new int[36];//przyporządkowanie gracza do pola

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
                            orders.Remove(tmpKey);
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

        public static string GenerateMap(int players)
        {
            int numberOfFiels = 36;

            fieldsState = new int[numberOfFiels];
            diecesState = new int[numberOfFiels];


            Random rand = new Random();
            int division = numberOfFiels / players;
            List<int> list = new List<int>();
            int[] index = new int[numberOfFiels];
            int player = 1;
            for (int i = 0; i < numberOfFiels; i++)
            {
                list.Add(i);
            }
            for (int i = 0; i < players; i++)
            {
                for (int j = 0; j < division; j++)
                {

                    int los = rand.Next(list.Count);
                    fieldsState[list[los]] = player;
                    diecesState[list[los]] = rand.Next(1,5);
                    list.RemoveAt(los);
                }
                player++;
            }
            for (int i = 0; i < numberOfFiels; i++)
            {
                if (fieldsState[i]==0)
                {
                    fieldsState[i] = player;
                    diecesState[i] = rand.Next(1, 5);
                }
            }
            StringBuilder map = new StringBuilder();

            for (int i = 0; i < numberOfFiels; i++)
            {
                map.Append(fieldsState[i]);
                map.Append(':');
                map.Append(diecesState[i]);
                map.Append(';');
            }

            return map.ToString();
            
        }

        public static string Respond(int myFieldIndex, int enemyFieldIndex)
        {
            Random rand = new Random();
            if (fieldsState[myFieldIndex]==fieldsState[enemyFieldIndex] || diecesState[myFieldIndex]==1)
            {
                return null;
            }
            int los1 = 0;
            int los2 = 0;

            for (int i = 0; i < diecesState[myFieldIndex]; i++)
            {
                los1 += rand.Next(1, 7);
            }
            for (int i = 0; i < diecesState[enemyFieldIndex]; i++)
            {
                los2 += rand.Next(1, 7);
            }

            if (los1 > los2)
            {
                diecesState[enemyFieldIndex] = diecesState[myFieldIndex];
                diecesState[myFieldIndex] = 1;
                fieldsState[myFieldIndex] = fieldsState[myFieldIndex];
            }
            else if (los1 < los2)
            {
                diecesState[myFieldIndex] = 1;
            }
            else
            {
                diecesState[enemyFieldIndex] = 1;
                diecesState[myFieldIndex] = 1;
            }
            return myFieldIndex.ToString() + ":" + fieldsState[myFieldIndex] + "," + diecesState[myFieldIndex] + ";" + enemyFieldIndex+":"+fieldsState[enemyFieldIndex]+","+diecesState[enemyFieldIndex];

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
                    Console.WriteLine("Someone connected!");

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
                        Queue<string> queue = new Queue<string>();//gracz dostaje nową kolejkę rozkazów
                        orders.Add(key, queue);
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

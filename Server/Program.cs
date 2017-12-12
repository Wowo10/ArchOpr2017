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
        private static int setMaxPlayers = 4;
        static Dictionary<string, DateTime> timeOut = new Dictionary<string, DateTime>();
        static List<string> keys = new List<string>();
        static List<string> ListOfReadyPlayers = new List<string>();
        static bool[] avaible = new bool[setMaxPlayers];
        static Dictionary<string, int> numberOfPlayer = new Dictionary<string, int>();
        private static int mapSize = 36;
        static int[] fieldsState =new int[mapSize];//ilość kostek na danym polu
        static int[] diecesState = new int[mapSize];//przyporządkowanie gracza do pola
        private static int forHowManyPlayers = 0;
        private static bool isCreation = false;
        private static int whoseTurn = 1;

        
        static void Manage()
        {
            while (true)
            {
                for (int i = 0; i < keys.Count; i++)
                {
                    string tmpKey = keys[i];

                    if (tmpKey != null && timeOut.ContainsKey(tmpKey))
                    {
                        TimeSpan time = DateTime.Now - timeOut[tmpKey];
                        if (time.TotalSeconds > 10)
                        {
                            Console.WriteLine(keys.Count);
                            Console.WriteLine("Someone left");
                            timeOut.Remove(tmpKey);
                            keys.Remove(tmpKey);
                            avaible[numberOfPlayer[tmpKey]-1] = false;
                            numberOfPlayer.Remove(tmpKey);
                            if (ListOfReadyPlayers.Contains(tmpKey))
                            {
                                ListOfReadyPlayers.Remove(tmpKey);
                            }
                            isCreation = false;                 
                            break;
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
            forHowManyPlayers = players;
            int numberOfFiels = 36;

            fieldsState = new int[numberOfFiels];
            diecesState = new int[numberOfFiels];


            Random rand = new Random();
            int division = numberOfFiels / players;//liczba pól dla każdego gracza
            List<int> list = new List<int>();// lista indeksów pól
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
                    diecesState[list[los]] = rand.Next(4,8);
                    list.RemoveAt(los);
                }
                player++;
            }
            for (int i = 0; i < numberOfFiels; i++)
            {
                if (fieldsState[i]==0)
                {
                    fieldsState[i] = player;
                    diecesState[i] = rand.Next(4, 8);
                }
            }
            isCreation = true;
            StringBuilder map = new StringBuilder();

            for (int i = 0; i < numberOfFiels; i++)
            {
                map.Append(fieldsState[i]);
                map.Append(':');
                map.Append(diecesState[i]);
                map.Append(';');
            }
            map.Append(keys.Count);
            map.Append(';');
            map.Append(players);
            return map.ToString();
            
        }

        public static string ReturnMap(int myNumber)
        {

            StringBuilder map = new StringBuilder();

            for (int i = 0; i < mapSize; i++)
            {
                map.Append(fieldsState[i]);
                map.Append(':');
                map.Append(diecesState[i]);
                map.Append(';');
            }
            map.Append(myNumber);
            map.Append(';');
            map.Append(forHowManyPlayers);
            return map.ToString();

        }

        private static void AddDices(int player)
        {
            List<int> states = new List<int>();

            for (int i = 0; i < fieldsState.Length; i++)
            {
                if (fieldsState[i]==player)
                {
                    states.Add(i);
                }
            }

            int dieceToAdd = 2 * states.Count;

            Random rand = new Random();


            while (dieceToAdd != 0)
            {
                int los = rand.Next(0, states.Count);
                diecesState[states[los]]++;
                dieceToAdd--;
            }


        }


        public static string Respond(int myFieldIndex, int enemyFieldIndex)
        {
            Random rand = new Random();
            if (diecesState[myFieldIndex]==1)
            {
                return "" + 0 + ';' + diecesState[myFieldIndex] + ';' + 1 + ';' + diecesState[enemyFieldIndex];
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
                diecesState[enemyFieldIndex] = diecesState[myFieldIndex]-1;
                diecesState[myFieldIndex] = 1;
                fieldsState[enemyFieldIndex] = fieldsState[myFieldIndex];
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
            return los1.ToString()+';'+diecesState[myFieldIndex] +';' + los2 + ';' + diecesState[enemyFieldIndex];
        }

        static void Main(string[] args)
        {
            Thread thread = new Thread(Manage);//tworzenie wątku do czy gracz nie opuścił gry
            thread.Start();

            int port = 300;
            string data;
            Byte[] bytes = new Byte[256];

            TcpListener server = null;
            try
            {
                server = new TcpListener(IPAddress.Parse(GetLocalIPAddress()), port);
                server.Start();

                while (true)
                {
                    Console.Write("Server started on ip - {0}, port - {1}! Waiting for packets...", GetLocalIPAddress(), port);
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Someone connected!");

                    string key = client.Client.RemoteEndPoint.ToString().Split(':')[0];
                    if (timeOut.ContainsKey(key))
                    {
                        timeOut[key] = DateTime.Now;
                    }
                    else
                    {
                        Console.WriteLine("Someone new joined from {0}", key);
                        keys.Add(key);
                        timeOut.Add(key, DateTime.Now);
                        for (int k = 0; k < avaible.Length; k++)
                        {
                            if (!avaible[k])
                            {
                                avaible[k] = true;
                                numberOfPlayer.Add(key, k+1);
                                break;
                            }
                        }
                        isCreation = false;                                                               
                    }

                    NetworkStream stream = client.GetStream();              

                    int i = stream.Read(bytes, 0, bytes.Length);
                    data = Encoding.ASCII.GetString(bytes, 0, i);
                    byte[] buffer = new byte[256];
                    string message = "";
                    if (data == "!")
                    {
                        if (ListOfReadyPlayers.Count == keys.Count && keys.Count>1 && keys.Count <5 && !isCreation)
                        {
                            message = GenerateMap(keys.Count);
                            isCreation = true;
                        }
                        else
                        {
                            if (!isCreation)
                            {
                                message = "no";
                            }
                            else
                                message = ReturnMap(numberOfPlayer[key]);
                            
                        }                        
                    }
                    else if (data == "#")
                    {
                        message = "OK";
                    }
                    else if (data[0] == '*')
                    {
                        string[] tmp = data.Split(';');
                        message = Respond(Convert.ToInt16(tmp[1]), Convert.ToInt32(tmp[2]));
                    }
                    else if (data == "?")
                    {
                        if (whoseTurn == numberOfPlayer[key])
                        {
                            message = "Y";
                        }
                        else
                        {
                            message = "N";
                        }
                    }
                    else if (data == "&" && whoseTurn == numberOfPlayer[key])
                    {
                        AddDices(numberOfPlayer[key]);

                        if (whoseTurn == keys.Count)
                        {
                            whoseTurn = 1;
                        }
                        else
                            whoseTurn++;
                        message = "OK";
                    }
                    else if (data == "$")
                    {
                        if (!ListOfReadyPlayers.Contains(key))
                        {
                            ListOfReadyPlayers.Add(key);
                        }
                        message = "ok";
                    }
                    buffer = Encoding.ASCII.GetBytes(message);



                    stream.Write(buffer, 0, buffer.Length);
                    stream.Close();
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

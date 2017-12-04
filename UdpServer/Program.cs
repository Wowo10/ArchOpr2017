using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace UdpServer
{
    class Program
    {
        static bool lobby;//1-lobby, 0-gameplay
        const int MAXPLAYERS = 2;
        static UdpClient udpServer;

        static IPEndPoint remoteEp;

        static List<IPEndPoint> eps;

        static Program()
        {
            lobby = true;
            udpServer = new UdpClient(11000);
            eps = new List<IPEndPoint>();

            multicast = true;
            callback = "";
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Listening...");

            bool exit = false;

            while (!exit)
            {
                remoteEp = new IPEndPoint(IPAddress.Any, 11000);

                byte[] data = Receive();

                string message = Decode(data);

                //Console.WriteLine("RECEIVED: "+message);

                Verify(message);

                Handle(message);

                CallBack();
            }

            Console.WriteLine("The End!");
            Console.ReadKey();
        }

        //probably should be async and put every command on stack?
        static byte[] Receive()
        {
            return udpServer.Receive(ref remoteEp);
        }

        //probably in future we should user the <T> patterns
        //And obviously think about some standard of data, not String.
        static string Decode(byte[] data)
        {
            return Encoding.ASCII.GetString(data);
        }

        //Game Logic - basic rules - is valid target
        static void Verify(string message)
        {
            if (lobby)
            {
                if (message == "join")
                {
                    Console.WriteLine("Received join from: " + remoteEp);
                    if (eps.Count < MAXPLAYERS)
                    {
                        
                        if (!eps.Contains(remoteEp))
                        {
                            eps.Add(remoteEp);

                            callback = "new player";
                            multicast = true;
                        }
                        else
                        {
                            callback = "already joined";
                            multicast = false;
                        }
                    }
                    else
                    {
                        callback = "room full";
                        multicast = false;
                    }
                }
                else if(message == "quit")
                {
                    Console.WriteLine("Received quit from: "+remoteEp);
                    if (eps.Contains(remoteEp))
                    {
                        eps.Remove(remoteEp);

                        callback = "player left";
                        multicast = true;
                    }
                }
                else
                {
                    callback = "fuckup";
                    multicast = false;
                }
            }
            else
            { //gameplay verifying

            }
        }

        //Game Logic - dice rolls etc
        static void Handle(string message)
        {
            if (lobby)
            {/*
                if (message == "join")
                {
                    if (!eps.Contains(remoteEp))
                    {
                        eps.Add(remoteEp);

                        callback = "new player";
                        multicast = true;
                    }
                    else
                    {
                        callback = "already joined";
                        multicast = false;
                    }
                }*/
            }
            else
            { //gameplay handling

            }
        }

        static bool multicast;
        static string callback;
        static void CallBack()
        {
            Console.WriteLine("-----------SENDING-----------");

            byte[] data = Encoding.ASCII.GetBytes(callback);

            if (multicast)
            {
                Console.WriteLine("Multicasting: " + callback + " to: " + eps.Count + " EndPoints.");

                foreach (var ep in eps)
                {
                    if (ep != remoteEp)
                        udpServer.Send(data, data.Length, ep);
                }

                data = data = Encoding.ASCII.GetBytes("ok");
                udpServer.Send(data, data.Length, remoteEp);
            }
            else
            {
                Console.WriteLine("Sending: " + callback + " to: " + remoteEp);

                udpServer.Send(data, data.Length, remoteEp);
            }
            Console.WriteLine("----------------------------");
        }
    }
}

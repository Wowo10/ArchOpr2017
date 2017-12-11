using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using NetworkModule;

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

                Packet packet = Decode(data);

                //Console.WriteLine("RECEIVED: "+message);

                Verify(packet);

                Handle();

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
        static Packet Decode(byte[] data)
        {
            return new Packet(data);
        }

        //Game Logic - basic rules - is valid target
        static void Verify(Packet packet)
        {
            //Console.WriteLine(NetworkModule.Temporary.kek);

            if (lobby)
            {
                if (packet.GetPacketType == PacketType.JOIN)
                {
                    Console.WriteLine("Received join from: " + remoteEp);
                    if (eps.Count < MAXPLAYERS)
                    {
                        
                        if (!eps.Contains(remoteEp))
                        {
                            eps.Add(remoteEp);

                            callback = new Packet(PacketType.NEWPLAYER);
                            multicast = true;
                        }
                        else
                        {
                            callback = new Packet(PacketType.FAIL);
                            multicast = false;
                        }
                    }
                    else
                    {
                        callback = new Packet(PacketType.FAIL);
                        multicast = false;
                    }
                }
                else if(packet.GetPacketType == PacketType.QUIT)
                {
                    Console.WriteLine("Received quit from: "+remoteEp);
                    if (eps.Contains(remoteEp))
                    {
                        eps.Remove(remoteEp);

                        byte[] data = new byte[1];
                        data[0] = (byte)eps.IndexOf(remoteEp);

                        callback = new Packet(PacketType.PLAYERLEFT, data);
                        multicast = true;
                    }
                }
                else
                {
                    callback = new Packet(PacketType.FAIL);
                    Console.WriteLine("Spotted unexpected packet code!");
                    multicast = false;
                }
            }
            else
            { //gameplay verifying

            }
        }

        //Game Logic - dice rolls etc
        static void Handle()
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
        static Packet callback;
        static void CallBack()
        {
            Console.WriteLine("-----------SENDING-----------");

            byte[] data = callback.ToByteArray();

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

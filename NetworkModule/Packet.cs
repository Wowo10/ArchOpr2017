using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkModule
{
    public enum PacketType
    {
        OK = 0,
        FAIL,
        CHECK,
        JOIN,
        QUIT,
        NEWPLAYER,
        PLAYERLEFT
    }

    public class Packet
    {
        PacketType type;
        public PacketType GetPacketType
        {
            get{ return type; }
        }
        byte[] args;
        public byte[] GetArgs
        {
            get { return args; }
        }

        public Packet()
        {
            type = PacketType.OK;
            args = new byte[] { };
        }

        public Packet(PacketType packettype)
        {
            type = packettype;
            args = new byte[] { };
        }

        public Packet(PacketType packettype, byte[] data)
        {
            type = packettype;
            args = data;
        }

        public Packet(byte[] receivedcode)
        {
            if (receivedcode.Length > 0)
                type = (PacketType)Enum.ToObject(typeof(Type), receivedcode[0]);
            else
            {
                type = PacketType.OK;
                args = new byte[] { };
                return;
            }

            if (receivedcode.Length > 1)
            {
                args = receivedcode.Skip(1).ToArray();                
            }
        }

        public byte[] ToByteArray()
        {
            List<byte> temp = new List<byte>();

            temp.Add((byte)type);

            foreach (var item in args)
            {
                temp.Add(item);
            }            

            return temp.ToArray();
        }
    }
}

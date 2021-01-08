using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;

namespace FusionLibrary
{
    public static class Network
    {
        private static readonly List<IPAddress> broadcastAddress;

        static Network()
        {
            broadcastAddress = new List<IPAddress>();

            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.NetworkInterfaceType != NetworkInterfaceType.Ethernet) { continue; }
                if (adapter.Supports(NetworkInterfaceComponent.IPv4) == false) { continue; }

                try
                {
                    IPInterfaceProperties adapterProperties = adapter.GetIPProperties();

                    foreach (var ua in adapterProperties.UnicastAddresses)
                    {
                        if (ua.Address.AddressFamily == AddressFamily.InterNetwork)
                            broadcastAddress.Add(GetBroadcastAddress(ua.Address, ua.IPv4Mask));
                    }
                }
                catch { }
            }
        }

        private static IPAddress GetBroadcastAddress(IPAddress address, IPAddress mask)
        {
            uint ipAddress = BitConverter.ToUInt32(address.GetAddressBytes(), 0);
            uint ipMaskV4 = BitConverter.ToUInt32(mask.GetAddressBytes(), 0);
            uint broadCastIpAddress = ipAddress | ~ipMaskV4;

            return new IPAddress(BitConverter.GetBytes(broadCastIpAddress));
        }

        public static void SendMsg(string msg, int port)
        {
            SendMsg(Encoding.UTF8.GetBytes(msg), port);
        }

        public static void SendBool(string name, bool value, int port)
        {
            SendMsg($"{name}={value}", port);
        }

        public static void SendInt(string name, int value, int port)
        {
            SendMsg($"{name}={value}", port);
        }

        public static void SendMsg(byte[] data, int port)
        {
            UdpClient udpClient = new UdpClient();

            udpClient.EnableBroadcast = true;

            foreach(var addr in broadcastAddress)
            {
                try
                {
                    udpClient.SendAsync(data, data.Length, new IPEndPoint(addr, port));
                }
                catch { }
            }                

            udpClient.Close();
            udpClient.Dispose();
        }
    }
}

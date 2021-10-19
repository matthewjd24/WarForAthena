using System;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Collections.Generic;

namespace WFA_Server
{
    public static class SslServer
    {
        public static int bufferLength = 4000;
        public static X509Certificate serverCertificate = null;
        public static Dictionary<int, ClientConnection> ClientSlots = new();
        public static bool isLocal = false;
        public static TcpListener listener;

        //public delegate void MessageHandler();
        //public static Dictionary<int, MessageHandler> messageHandlers = new Dictionary<int, MessageHandler>();

        public static void Main()
        {
            //messageHandlers.Add(1, ReceiveText);
            Console.WriteLine("WFA Server 2000 (C)");

            string filePath;
            if (Environment.MachineName == "WOLCOTTDESKTOP") {
                Console.WriteLine("Running locally.");
                filePath = @"C:\Users\Matthew\Documents\TempCert.cer";
                isLocal = true;
            }
            else {
                Console.WriteLine("Running remotely.");
                filePath = @"C:\Users\WFA Server\Desktop\TempCert.cer";
            }

            for (int i = 0; i < 20; i++) {
                ClientSlots.Add(i, null);
            }

            serverCertificate = new X509Certificate2(filePath, "");

            listener = new TcpListener(IPAddress.Any, 25040);
            listener.Start();

            WaitForClients();
        }

        static void WaitForClients()
        {
            while (true) {
                Console.WriteLine("Waiting for a client to connect...");


                TcpClient client = listener.AcceptTcpClient();

                //to do: check client's IP address. Check if matches any current IPs
                int? ID = GetID();
                int? GetID()
                {
                    foreach (var e in ClientSlots) {
                        if (e.Value == null) {
                            return e.Key;
                        }
                    }

                    return null;
                }

                if(ID == null) {
                    Console.WriteLine("Client slots are full. Rejecting new user");
                    continue;
                }
                
                ClientConnection newClient = new ClientConnection(client, (int)ID);
                ClientSlots[(int)ID] = newClient;
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WFA_Server
{
    public class ClientConnection
    {
        public class Stream : SslStream
        {
            public Stream(System.IO.Stream innerStream) : base(innerStream, false) { }

            public bool IsClosed { get; set; }
            protected override void Dispose(bool disposing)
            {
                IsClosed = true;
                base.Dispose(disposing);
            }
        }
        byte[] byteBuffer = new byte[SslServer.bufferLength];

        public Stream sslStream;
        TcpClient tcpClient;

        public int clientID;
        public int secsSinceLastPingResponse;

        Dictionary<int, NetMsg.Message> dict = new Dictionary<int, NetMsg.Message>();

        public ClientConnection(TcpClient client, int id)
        {
            SetUpDict();
            void SetUpDict()
            {
                NetMsg.Ping resp = new();
                NetMsg.Login req = new();
                NetMsg.Register reg = new();
                NetMsg.WriteTile til = new();
                NetMsg.RequestTileRange reqTilR = new();
                NetMsg.TileData tilDat = new();
                dict.Add(resp.ID, resp);
                dict.Add(req.ID, req);
                dict.Add(reg.ID, reg);
                dict.Add(til.ID, til);
                dict.Add(reqTilR.ID, reqTilR);
                dict.Add(tilDat.ID, tilDat);
            }

            Console.WriteLine("A client connected.");
            sslStream = new Stream(client.GetStream());
            tcpClient = client;
            clientID = id;

            try {
                sslStream.AuthenticateAsServer(SslServer.serverCertificate, clientCertificateRequired: false, checkCertificateRevocation: true);
            }
            catch (Exception e) {
                Console.WriteLine("Exception: {0}", e.Message);
                if (e.InnerException != null) {
                    Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
                }
                Console.WriteLine("Authentication failed - closing the connection.");
                sslStream.Close();
                client.Close();
                return;
            }

            sslStream.ReadTimeout = 5000;
            sslStream.WriteTimeout = 5000;

            Console.WriteLine($"I authenticated as the server and gave him ID {id}.");

            _ = PingClient();

            StartReadingStream();
        }


        void ReceiveMessage(IAsyncResult ar)
        {
            if (sslStream.IsClosed) {
                Console.WriteLine("Stream is closed");
                return;
            }

            int bytes;
            try {
                bytes = sslStream.EndRead(ar);
            }
            catch (Exception e) {
                Console.WriteLine("Exception");
                if(e.Message.Contains("forcibly closed")) {
                    Console.WriteLine("They closed the connection.");
                    CloseConnection();
                }
                return;
            }

            if (bytes == 0) {
                Console.WriteLine("Received message with 0 bytes");
                CloseConnection();
                return;
            }


            byte[] data = new byte[bytes];
            Array.Copy(byteBuffer, data, bytes);

            int msgID = BitConverter.ToInt32(data, 0);
            data = data.Skip(4).ToArray();

            dict.TryGetValue(msgID, out NetMsg.Message message);

            if (message != null) {
                message = Deserialize(message, data);
                static NetMsg.Message Deserialize(NetMsg.Message msg, byte[] data)
                {
                    Type myType = msg.GetType();
                    FieldInfo[] info = myType.GetFields();

                    using MemoryStream m = new(data);
                    using BinaryReader reader = new(m);

                    foreach (var e in info) {
                        if (e.Name == "ID") continue;
                        if(e.Name == "response") {
                            e.SetValue(msg, "");
                            continue;
                        }

                        if (e.FieldType == typeof(int)) {
                            e.SetValue(msg, reader.ReadInt32());
                        }
                        else if (e.FieldType == typeof(string)) {
                            e.SetValue(msg, reader.ReadString());
                        }
                        else if (e.FieldType == typeof(bool)) {
                            e.SetValue(msg, reader.ReadBoolean());
                        }
                    }

                    return msg;
                }

                _ = message.Receive(this);
            }
            else {
                Console.WriteLine("msg is not in the dict");
            }


            StartReadingStream();
        }

        void StartReadingStream()
        {
            if (sslStream == null) {
                Console.WriteLine("Stream is null");
                return;
            }
            if (sslStream.IsClosed) {
                Console.WriteLine("Stream is closed");
                return;
            }
            sslStream.BeginRead(byteBuffer, 0, SslServer.bufferLength, ReceiveMessage, null);
        }

        async Task PingClient()
        {
            secsSinceLastPingResponse = 0;

            while (true) {
                await Task.Delay(2000);
                secsSinceLastPingResponse++;

                _ = new NetMsg.Ping().Send(sslStream);

                if(secsSinceLastPingResponse > 8) {
                    Console.WriteLine($"We have a problem. {clientID} isn't responding.");
                    CloseConnection();
                    break;
                }

                if (sslStream.IsClosed) break;
            }
        }

        public void CloseConnection()
        {
            if (sslStream.IsClosed) return;

            Console.WriteLine("Closing the connection to player " + clientID);
            SslServer.ClientSlots[clientID] = null;
            sslStream.Close();
            tcpClient.Close();
            byteBuffer = null;
        }
    }
}

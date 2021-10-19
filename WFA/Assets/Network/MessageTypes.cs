using System;
using System.IO;
using System.Net.Security;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

public static class NetMsg
{
    public static bool isServer = false;

    public class Message
    {
        public int ID;
        public string response;
        public byte[] data;

        public void Send() {
            Type myType = GetType();
            FieldInfo[] info = myType.GetFields();

            using MemoryStream m = new MemoryStream();
            using BinaryWriter writer = new BinaryWriter(m);

            writeStuff();
            void writeStuff()
            {
                writer.Write(ID);

                if (isServer) {
                    writer.Write(response);
                    return;
                }


                foreach (var e in info) {
                    if (e.Name == "ID") continue;
                    if (e.Name == "response") continue;

                    object val = e.GetValue(this);

                    if (e.FieldType == typeof(string)) {
                        writer.Write((string)val);
                    }
                    else if (e.FieldType == typeof(int)) {
                        writer.Write((int)val);
                    }
                    else if (e.FieldType == typeof(bool)) {
                        writer.Write((bool)val);
                    }
                }
            }


            byte[] bytes = m.ToArray();

            if (SslClient.sslStream == null) {
                Debug.LogError("Stream is null");
                return;
            }
            if (SslClient.sslStream.IsClosed) {
                Debug.LogError("Stream is closed");
                return;
            }

            SslClient.sslStream.Write(bytes, 0, bytes.Length);
        }
        public virtual void Receive(byte[] data) { }
    }

    public class Ping : Message
    {
        public Ping()
        {
            ID = 1;
        }

        public override void Receive(byte[] data)
        {
            WaitForPingsFromServer.inst.ReceivedPing();
        }
    }
    public class Login : Message
    {
        public Login()
        {
            ID = 2;
        }
        public string username;
        public string password;

        public override void Receive(byte[] data)
        {
            using MemoryStream m = new MemoryStream(data);
            using BinaryReader reader = new BinaryReader(m);

            response = reader.ReadString();

            Debug.Log("Login response: " + response);

            UILogInScreen.inst.SetStatusText(response);
            if (response.Contains("Success")) {
                PlayerID.isLoggedIn = true;
            }
            StartingScreenManager.inst.GoToGame();
        }
    }
    public class Register : Message
    {
        public Register()
        {
            ID = 3;
        }
        public string username;
        public string password;
        public string email;

        public override void Receive(byte[] data)
        {
            using MemoryStream m = new MemoryStream(data);
            using BinaryReader reader = new BinaryReader(m);

            response = reader.ReadString();

            Debug.Log("Register response: " + response);

            UIRegisterScreen.inst.SetStatusText(response);
            if (response.Contains("Success")) {
                PlayerID.isLoggedIn = true;
            }
            StartingScreenManager.inst.GoToGame();
        }
    }
    public class WriteTile : Message
    {
        public WriteTile()
        {
            ID = 4;
        }

        public int world;
        public int x;
        public int y;
        public int tileType;
        public bool hasCity;
    }

    public class RequestTileRange : Message 
    { 
        public RequestTileRange()
        {
            ID = 5;
        }

        public int world;
        public int startX;
        public int endX;
        public int startY;
        public int endY;
    }

    public class TileData : Message
    {
        public TileData()
        {
            ID = 6;
        }

        public int world;
        public int x;
        public int y;
        public int tileType;
        public int city;

        public override void Receive(byte[] data)
        {

            using MemoryStream m = new MemoryStream(data);
            using BinaryReader reader = new BinaryReader(m);

            world = reader.ReadInt32();
            x = reader.ReadInt32();
            y = reader.ReadInt32();
            tileType = reader.ReadInt32();
            city = reader.ReadInt32();

            Debug.Log("Received tile data. x: " + x + ", y: " + y + ", type " + tileType);

            TileMapGenerator.inst.SetTile(x, y, (TileType)tileType);
        }
    }
}
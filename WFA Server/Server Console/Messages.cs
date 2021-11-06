using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Reflection;
using System.Threading.Tasks;

namespace WFA_Server
{
    public static class NetMsg
    {
        public static bool isServer = true;

        public static async Task SendMsg(string str, SslStream stream)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);

            await stream.WriteAsync(bytes, 0, bytes.Length);
        }

        public class Message
        {
            public int ID;
            public string response;
            public async Task Send(SslStream stream) {
                Type myType = GetType();
                FieldInfo[] info = myType.GetFields();

                using MemoryStream m = new MemoryStream();
                using BinaryWriter writer = new BinaryWriter(m);

                writeStuff();
                void writeStuff()
                {
                    writer.Write(ID);

                    if (isServer && response != null) {
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

                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
            public virtual async Task Receive(ClientConnection conn = null) { }

            public static Message Desserialize(Message msg, byte[] data)
            {
                Type myType = msg.GetType();
                FieldInfo[] info = myType.GetFields();
                //Console.WriteLine(myType + " " + info.Length);

                using (MemoryStream m = new(data)) {
                    using BinaryReader reader = new(m);

                    foreach (var e in info) {
                        if (e.Name != "response") continue;

                        if (e.Name == "response") {
                            e.SetValue(msg, "");
                        }


                        Console.WriteLine("Response: " + e.Name);
                    }
                }

                return msg;
            }
        }

        public class Ping : Message
        {
            public Ping()
            {
                ID = 1;
            }

            public override async Task Receive(ClientConnection conn)
            {
                conn.secsSinceLastPingResponse = 0;
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

            public override async Task Receive(ClientConnection conn = null)
            {
                Console.WriteLine($"Login request: {username}, {password}");

                response = "Success"; //await Login(username, password);
                async Task<string> Login(string username, string password)
                {
                    string queryString = @"
                        DECLARE	@responseMessage nvarchar(250)
                        EXEC dbo.Login
                        @username = @user,
		                @password = @pass,
		                @responseMessage = @responseMessage OUTPUT
                        SELECT @responseMessage as N'@responseMessage'";

                    string connectionString = GetConnectionString();
                    string GetConnectionString()
                    {
                        if (!SslServer.isLocal)
                            return @"Server=.\SQLEXPRESS;Database=WFA;User Id=" + SslServer.dbUser + ";" +
                                " Password = " + SslServer.dbPass;
                        else {
                            return @"Server=35.164.92.71\SQLEXPRESS,1433;Initial Catalog=WFA;" +
                                "User ID=" + SslServer.dbUser + ";" +
                                " Password = " + SslServer.dbPass;
                        }
                    }

                    string response = "start";

                    try {
                        using SqlConnection connection = new SqlConnection(connectionString);
                        SqlCommand command = new SqlCommand(queryString, connection);
                        command.Parameters.AddWithValue("@user", username);
                        command.Parameters.AddWithValue("@pass", password);

                        await connection.OpenAsync();
                        SqlDataReader reader = command.ExecuteReader();
                        try {
                            while (reader.Read()) {
                                response = string.Format("{0}", reader["@responseMessage"]);
                                Console.WriteLine(response);
                            }
                        }
                        catch (Exception e) {
                            Console.WriteLine(e.Message);
                            Console.ReadKey();
                            return "error";
                        }
                        finally {
                            // Always call Close when done reading.
                            reader.Close();
                        }
                    }
                    catch (Exception e) {
                        Console.WriteLine(e.Message);
                        Console.ReadKey();
                        return "error";
                    }

                    return response;
                }

                await Send(conn.sslStream);
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

            public override async Task Receive(ClientConnection conn = null)
            {
                Console.WriteLine($"Register request: {username}, {password}, {email}");

                response = await Register(username, password, email);
                async Task<string> Register(string username, string password, string email)
                {
                    string queryString = @"
                        DECLARE @responseMessage NVARCHAR(250)
                        EXEC dbo.NewAccount
                        @username = @user,
                        @password = @pass,
                        @email = @eml,
                        @responseMessage=@responseMessage OUTPUT
                        SELECT @responseMessage as N'@responseMessage'";

                    string connectionString = SslServer.GetConnectionString();

                    string resp = "start";
                    try {
                        using SqlConnection connection = new SqlConnection(connectionString);
                        SqlCommand command = new SqlCommand(queryString, connection);
                        command.Parameters.AddWithValue("@user", username);
                        command.Parameters.AddWithValue("@pass", password);
                        command.Parameters.AddWithValue("@eml", email);

                        await connection.OpenAsync();
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        try {
                            while (reader.Read()) {
                                resp = string.Format("{0}", reader["@responseMessage"]);
                                Console.WriteLine(resp);
                            }
                        }
                        catch (Exception e) {
                            Console.WriteLine(e.Message);
                            Console.ReadKey();
                        }
                        finally {
                            // Always call Close when done reading.
                            reader.Close();
                        }
                    }
                    catch (Exception e) {
                        Console.WriteLine(e.Message);
                        Console.ReadKey();
                    }

                    return resp;
                }

                await Send(conn.sslStream);
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


            public override async Task Receive(ClientConnection conn = null)
            {
                Console.WriteLine($"Received tile to write.");

                await WriteTheTile();
                async Task<string> WriteTheTile()
                {
                    string xstring = (x + 1000).ToString().Substring(1);
                    string ystring = (y + 1000).ToString().Substring(1);
                    int xandy = int.Parse(xstring + ystring);

                    string queryString = $@"set identity_insert MapTiles on;
                        DECLARE @responseMessage nvarchar(250)
                        EXEC WriteTile 
                        @ID = '{xandy}',
                        @world = '{world}',
                        @x = '{x}',
                        @y = '{y}',
                        @type = '{tileType}',
                        @city = null,
                        @playerowner = null,
                        @responseMessage = @responseMessage OUTPUT;
                        SELECT @responseMessage as N'@responseMessage'";

                    string connectionString = SslServer.GetConnectionString();
                    

                    string resp = "";
                    using SqlConnection connection = new SqlConnection(connectionString);
                    SqlCommand command = new SqlCommand(queryString, connection);

                    try {
                        await connection.OpenAsync();

                        SqlDataReader reader = await command.ExecuteReaderAsync();

                        await reader.CloseAsync();
                    }
                    catch (Exception e) {
                        Console.WriteLine(e.Message);
                    }

                    await Task.Delay(20);

                    return resp;
                }
            }
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

            public override async Task Receive(ClientConnection conn = null)
            {
                //Console.WriteLine($"Received tile range request.");

                DataTable table = new DataTable();

                await GetTiles();
                async Task GetTiles()
                {
                    string queryString = @"select * from MapTiles " +
                      "where x >= @startX " +
                      "and x <= @endX " +
                      "and y >= @startY " +
                      "and y <= @endY " +
                      "and world = @world";

                    string connectionString = GetConnectionString();
                    string GetConnectionString()
                    {
                        if (!SslServer.isLocal)
                            return @"Server=.\SQLEXPRESS;Database=WFA; User ID = " + SslServer.dbUser + ";" +
                                "Password = " + SslServer.dbPass;
                        else {
                            return @"Server=35.164.92.71\SQLEXPRESS,1433;Initial Catalog=WFA;" +
                                "User ID= " + SslServer.dbUser + ";" +
                                "Password = " + SslServer.dbPass;
                        }
                    }

                    try {
                        using SqlConnection connection = new SqlConnection(connectionString);
                        SqlCommand command = new SqlCommand(queryString, connection);
                        command.Parameters.AddWithValue("@startX", startX);
                        command.Parameters.AddWithValue("@endX", endX);
                        command.Parameters.AddWithValue("@startY", startY);
                        command.Parameters.AddWithValue("@endY", endY);
                        command.Parameters.AddWithValue("@world", world);

                        await connection.OpenAsync();
                        SqlDataAdapter da = new SqlDataAdapter(command);
                        da.Fill(table);
                        connection.Close();
                        da.Dispose();
                    }
                    catch (Exception e) {
                        Console.WriteLine(e.Message);
                        Console.ReadKey();
                    }
                }

                //Console.WriteLine("Got the tiles. The table has " + table.Rows.Count + " rows");
                foreach(DataRow row in table.Rows) {
                    int city = 0;
                    int world = 0;
                    int x = 0;
                    int y = 0;
                    int tileType = 0;

                    try {
                        tileType = (int)row.ItemArray[3];
                        world = (int)row.ItemArray[0];
                        x = (int)row.ItemArray[1];
                        y = (int)row.ItemArray[2];
                    }
                    catch (Exception e) {
                        Console.WriteLine(e.Message);
                    }

                    if (!row.IsNull("city")) {
                        city = (int)row.ItemArray[4];
                    }

                    //row.ItemArray[0].ToString();
                    foreach (var item in row.ItemArray) {
                        if (item == DBNull.Value) continue;
                        //Console.WriteLine(item);
                    }

                    //Console.WriteLine("############");

                    _ = new TileData() {
                        world = world,
                        x = x,
                        y = y,
                        tileType = tileType,
                        city = city,
                    }.Send(conn.sslStream);
                }
            }
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
        }
    }
}

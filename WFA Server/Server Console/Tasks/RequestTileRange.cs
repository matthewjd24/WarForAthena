using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFA_Server
{
    class RequestTileRange : MsgHandler
    {
        public override async Task Handle(string[] msg)
        {
            string connectionString = SslServer.GetConnectionString();

            Console.WriteLine("Handling msg");

            int world = int.Parse(msg[1]);
            int startX = int.Parse(msg[2]);
            int endX = int.Parse(msg[3]);
            int startY = int.Parse(msg[4]);
            int endY = int.Parse(msg[5]);

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

            Console.WriteLine("Got " + table.Rows.Count + " tiles.");

            foreach (DataRow row in table.Rows) {
                int city = 0;
                int tileworld = 0;
                int x = 0;
                int y = 0;
                int tileType = 0;

                try {
                    tileType = (int)row.ItemArray[3];
                    tileworld = (int)row.ItemArray[0];
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

                if (conn.sslStream.IsClosed) break;

                _ = NetMsg.SendMsg($"tiledata;{tileworld};{x};{y};{tileType};{city};", conn.sslStream);

            }
        }
    }
}

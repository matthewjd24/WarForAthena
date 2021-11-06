using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFA_Server
{
    class WriteTile : MsgHandler
    {
        public override async Task Handle(string[] msg)
        {
            int world = int.Parse(msg[1]);
            int x = int.Parse(msg[2]);
            int y = int.Parse(msg[3]);
            int tileType = int.Parse(msg[4]);
            bool hasCity = bool.Parse(msg[5]);

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
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFA_Server
{
    class Login : MsgHandler
    {
        public override async Task Handle(string[] msg)
        {
            string username = msg[1];
            string password = msg[2];
            Console.WriteLine($"Login request: {username}, {password}");


            await NetMsg.SendMsg($"login;Success;", conn.sslStream);
            return;

            string queryString = @"
                    DECLARE	@responseMessage nvarchar(250)
                    EXEC dbo.Login
                    @username = @user,
		            @password = @pass,
		            @responseMessage = @responseMessage OUTPUT
                    SELECT @responseMessage as N'@responseMessage'";

            string connectionString = SslServer.GetConnectionString();

            string response = "";

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
                    response = "error";
                }
                finally {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                Console.ReadKey();
                response = "error";
            }

            await NetMsg.SendMsg($"login;{response};", conn.sslStream);
        }
    }
}

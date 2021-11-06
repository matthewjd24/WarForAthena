using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFA_Server
{
    class Register : MsgHandler
    {
        public override async Task Handle(string[] msg)
        {
            string username = msg[1];
            string password = msg[2];
            string email = msg[3];
            Console.WriteLine($"Register request: {username}, {password}, {email}");

            string queryString = @"
                        DECLARE @responseMessage NVARCHAR(250)
                        EXEC dbo.NewAccount
                        @username = @user,
                        @password = @pass,
                        @email = @eml,
                        @responseMessage=@responseMessage OUTPUT
                        SELECT @responseMessage as N'@responseMessage'";

            string connectionString = SslServer.GetConnectionString();

            string resp = "error";
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
                }
                finally {
                    reader.Close();
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }

            await NetMsg.SendMsg($"register;{resp}", conn.sslStream);
        }
    }
}

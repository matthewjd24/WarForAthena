using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFA_Server
{
    abstract class MsgHandler
    {
        public ClientConnection conn;
        public virtual async Task Handle(string[] msg) { }
    }

    public class HandleMessages
    {
        public ClientConnection conn;

        Dictionary<string, MsgHandler> msgHandlerDict = new();

        public HandleMessages(ClientConnection c)
        {
            conn = c;

            msgHandlerDict.Add("ping", new Ping());
            msgHandlerDict.Add("login", new Login());
            msgHandlerDict.Add("register", new Register());
            msgHandlerDict.Add("writetile", new WriteTile());
            msgHandlerDict.Add("requesttilerange", new RequestTileRange());
        }

        public void MsgReceived(string[] msg)
        {
            Console.WriteLine($"Client {conn.clientID} sent {msg[0]}");

            MsgHandler handler = msgHandlerDict[msg[0]];
            handler.conn = conn;
            _ = handler.Handle(msg);
        }
    }
}

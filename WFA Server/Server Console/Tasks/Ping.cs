using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFA_Server
{
    class Ping : MsgHandler
    {
        public override async Task Handle(string[] msg)
        {
            conn.secsSinceLastPingResponse = 0;
        }
    }
}

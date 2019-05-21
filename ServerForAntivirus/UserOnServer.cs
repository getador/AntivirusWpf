using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerForAntivirus
{
    class UserOnServer
    {
        Socket userSocketUpdate;
        Socket userSocketGetVirus;
        int userThreadIndex;

        public UserOnServer(Socket userSocketUpdate, Socket userSocketGetVirus, int userThreadIndex)
        {
            this.userSocketUpdate = userSocketUpdate;
            this.userSocketGetVirus = userSocketGetVirus;
            this.userThreadIndex = userThreadIndex;
        }

        public Socket UserSocketUpdate { get => userSocketUpdate; set => userSocketUpdate = value; }
        public Socket UserSocketGetVirus { get => userSocketGetVirus; set => userSocketGetVirus = value; }
        public int UserThreadIndex { get => userThreadIndex; set => userThreadIndex = value; }
    }
}

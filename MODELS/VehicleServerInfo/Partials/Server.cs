using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.VehicleServerInfo
{
    public partial class Server
    {
        public override bool Equals(object obj)
        {
            return obj is Server server &&
                   ServerCredentialsID == server.ServerCredentialsID &&
                   ServerTypeID == server.ServerTypeID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ServerCredentialsID, ServerTypeID);
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.VehicleServerInfo
{
    public class Server
    {
        private int _id;
        private string _ip_URL;
        private string _name;
        private ServerType _serverType;
        private ServerCredentials _serverCredentials;

        public ServerCredentials SelectedServerCredentials
        {
            get { return _serverCredentials; }
            set { _serverCredentials = value; }
        }


        public ServerType SelectedServerType
        {
            get { return _serverType; }
            set { _serverType = value; }
        }


        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }


        public string IP_URL
        {
            get { return _ip_URL; }
            set { _ip_URL = value; }
        }


        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

    }
}

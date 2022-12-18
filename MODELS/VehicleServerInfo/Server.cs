using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.VehicleServerInfo
{
    public partial class Server
    {
        private int _id;
        private string _ip_URL;
        private string _name;
        private ServerType _serverType;
        private ServerCredentials _serverCredentials;
        private int _serverTypeID;
        private int _serverCredentialsID;

        [Required]
        public int ServerCredentialsID
        {
            get { return _serverCredentialsID; }
            set { _serverCredentialsID = value; }
        }

        [Required]
        public int ServerTypeID
        {
            get { return _serverTypeID; }
            set { _serverTypeID = value; }
        }

        [ForeignKey("ServerCredentialsID")]
        public ServerCredentials ServerCredentials
        {
            get { return _serverCredentials; }
            set { _serverCredentials = value; }
        }

        [ForeignKey("ServerTypeID")]
        public ServerType ServerType
        {
            get { return _serverType; }
            set { _serverType = value; }
        }

        [Required]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [Required]
        public string IP_URL
        {
            get { return _ip_URL; }
            set { _ip_URL = value; }
        }

        [Key]
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

    }
}

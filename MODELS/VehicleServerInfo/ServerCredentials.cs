namespace MODELS.VehicleServerInfo
{
    public class ServerCredentials
    {
        private int _id;
        private string _login;
        private string _password;
        private string _token;

        public string Token
        {
            get { return _token; }
            set { _token = value; }
        }


        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }


        public string Login
        {
            get { return _login; }
            set { _login = value; }
        }


        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

    }
}
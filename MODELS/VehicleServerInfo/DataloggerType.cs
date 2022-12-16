namespace MODELS.VehicleServerInfo
{
    public class DataloggerType
    {
        private int _id;
        private string _name;
        private string _type;
        private int? _memoryStorage;
        private int? _canChannels;

        public int? CanChannels
        {
            get { return _canChannels; }
            set { _canChannels = value; }
        }


        public int? MemoryStorage
        {
            get { return _memoryStorage; }
            set { _memoryStorage = value; }
        }


        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }


        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }


        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

    }
}
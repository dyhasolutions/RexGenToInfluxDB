namespace MODELS.VehicleServerInfo
{
    public class Datalogger
    {
        private int _id;
        private string _serialNumber;
        private DataloggerType _dataloggerType;

        public DataloggerType SelectedDataloggerType
        {
            get { return _dataloggerType; }
            set { _dataloggerType = value; }
        }


        public string SerialNumber
        {
            get { return _serialNumber; }
            set { _serialNumber = value; }
        }


        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

    }
}
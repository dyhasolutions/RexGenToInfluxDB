using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MODELS.VehicleServerInfo
{
    public class Datalogger
    {
        private int _id;
        private string _serialNumber;
        private DataloggerType _dataloggerType;
        private int? _dataloggerID;

        public int? DataloggerID
        {
            get { return _dataloggerID; }
            set { _dataloggerID = value; }
        }


        [ForeignKey("DataloggerTypeID")]
        public DataloggerType DataloggerType
        {
            get { return _dataloggerType; }
            set { _dataloggerType = value; }
        }

        [Required]
        public string SerialNumber
        {
            get { return _serialNumber; }
            set { _serialNumber = value; }
        }

        [Key]
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

    }
}
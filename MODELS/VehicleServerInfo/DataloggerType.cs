using System.ComponentModel.DataAnnotations;

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

        [Required]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [Required]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [Key]
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

    }
}
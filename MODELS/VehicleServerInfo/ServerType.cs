using System.ComponentModel.DataAnnotations;

namespace MODELS.VehicleServerInfo
{
    public class ServerType
    {
        private int _id;
        private string _name;
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
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.VehicleServerInfo
{
    public partial class Vehicle
    {
        private int _id;
        private string _manufacturer;
        private string _model;
        private string _type;
        private string _vin;
        private Datalogger _installedDatalogger;
        private int? _dataloggerID;

        public int? DataloggerID
        {
            get { return _dataloggerID; }
            set { _dataloggerID = value; }
        }

        [ForeignKey("DataloggerID")]
        public Datalogger Datalogger
        {
            get { return _installedDatalogger; }
            set { _installedDatalogger = value; }
        }

        [Required]
        public string VIN
        {
            get { return _vin; }
            set { _vin = value; }
        }

        [Required]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [Required]
        public string Model
        {
            get { return _model; }
            set { _model = value; }
        }

        [Required]
        public string Manufacturer
        {
            get { return _manufacturer; }
            set { _manufacturer = value; }
        }

        [Key]
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

    }
}

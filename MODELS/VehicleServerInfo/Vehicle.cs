using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.VehicleServerInfo
{
    public class Vehicle
    {
        private int _id;
        private string _manufacturer;
        private string _model;
        private string _type;
        private string _vin;
        private Datalogger _installedDatalogger;

        public Datalogger InstalledDatalogger
        {
            get { return _installedDatalogger; }
            set { _installedDatalogger = value; }
        }


        public string VIN
        {
            get { return _vin; }
            set { _vin = value; }
        }


        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }


        public string Model
        {
            get { return _model; }
            set { _model = value; }
        }


        public string Manufacturer
        {
            get { return _manufacturer; }
            set { _manufacturer = value; }
        }


        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

    }
}

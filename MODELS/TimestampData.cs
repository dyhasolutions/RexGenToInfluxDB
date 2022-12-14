using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS
{
    public class TimestampData
    {
        private DateTime _timestamp;
        private double _latitude;
        private double _longitude;
        private List<Signal> _signals;

        public List<Signal> Signals
        {
            get { return _signals; }
            set { _signals = value; }
        }

        public double Longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }

        public double Latitude
        {
            get { return _latitude; }
            set { _latitude = value; }
        }


        public DateTime Timestamp
        {
            get { return _timestamp; }
            set { _timestamp = value; }
        }

    }
}

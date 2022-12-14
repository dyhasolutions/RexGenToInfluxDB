using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS
{
    public class Signal
    {
        private double _sigValue;
        private string _signalName;
        private string _signalUnit;

        public string SignalUnit
        {
            get { return _signalUnit; }
            set { _signalUnit = value; }
        }



        public string SignalName
        {
            get { return _signalName; }
            set { _signalName = value; }
        }


        public double SigValue
        {
            get { return _sigValue; }
            set { _sigValue = value; }
        }

    }
}

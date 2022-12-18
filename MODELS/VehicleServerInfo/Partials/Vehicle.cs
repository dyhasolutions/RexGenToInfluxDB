using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODELS.VehicleServerInfo
{
    public partial class Vehicle
    {
        public override bool Equals(object obj)
        {
            return obj is Vehicle vehicle &&
                   VIN == vehicle.VIN &&
                   ID == vehicle.ID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(VIN, ID);
        }

        public override string ToString()
        {
            return this.Manufacturer + " " + this.Model + "-" + this.Type + " " + this.VIN;
        }
    }
}

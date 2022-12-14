using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using MODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.InfluxDBService
{
    public class InfluxDBHelper
    {
        InfluxDBService _influxDBService = new InfluxDBService();

        public void WriteToInfluxDB(List<TimestampData> timestampDataSamples, string dataloggerSerialNumber)
        {
            List<PointData> fields = new List<PointData>();

            foreach (TimestampData dataPoint in timestampDataSamples)
            {
                PointData point = PointData.Measurement("InfluxLoggerTest").Tag("logger", dataloggerSerialNumber)
                    .Timestamp(dataPoint.Timestamp.ToUniversalTime(), WritePrecision.Ns);

                foreach (Signal signal in dataPoint.Signals)
                {
                    point = point.Field(signal.SignalName, signal.SigValue);
                }

                fields.Add(point);
            }


            _influxDBService.Write(write =>
            {
                write.WritePoints(fields, "test2", "Influx Technology");
            });
        }
    }
}

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

        public void WriteToInfluxDB(List<TimestampData> timestampDataSamples, string manufacturer, string model_type, string VIN, string IP_URL, string token)
        {
            List<PointData> fields = new List<PointData>();

            foreach (TimestampData dataPoint in timestampDataSamples)
            {
                PointData point = PointData.Measurement(model_type)
                    .Tag("VIN", VIN)
                    .Timestamp(dataPoint.Timestamp.ToUniversalTime(), WritePrecision.Ns);

                foreach (Signal signal in dataPoint.Signals)
                {
                    point = point.Field(signal.SignalName, signal.SigValue);
                }

                fields.Add(point);

                if (fields.Count >= 5000)
                {
                    _influxDBService.Write(write =>
                    {
                        write.WritePoints(fields, manufacturer, "Influx Technology");
                    }, IP_URL, token);
                    fields.Clear();
                }
            }

            if (fields.Count > 0)
            {
                _influxDBService.Write(write =>
                {
                    write.WritePoints(fields, manufacturer, "Influx Technology");
                }, IP_URL, token);
                fields.Clear();
            }
        }
    }
}

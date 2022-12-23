using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using MODELS;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
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
        private string organization = "Influx Technology";

        public async Task WriteToInfluxDB(List<TimestampData> timestampDataSamples, string manufacturer, 
            string model_type, string VIN, string IP_URL, string token, string dataloggerSerialNumber)
        {
            List<Organization> organizations = await GetOrganizationsByName(organization, IP_URL, token);
            Organization selectedOrganization = organizations
                .Where(x => x.Name == organization)
                .FirstOrDefault();

            Bucket bucket = await CheckOrCreateBucket(manufacturer, IP_URL, token, selectedOrganization);

            List<PointData> fields = new List<PointData>();

            if (bucket is not null && selectedOrganization is not null)
            {
                foreach (TimestampData dataPoint in timestampDataSamples)
                {
                    PointData point = PointData.Measurement(model_type)
                        .Tag("VIN", VIN)
                        .Tag("DataloggerSerialNumber", dataloggerSerialNumber)
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
                            write.WritePoints(fields, manufacturer, selectedOrganization.Name);
                        }, IP_URL, token);
                        fields.Clear();
                    }
                }

                if (fields.Count > 0)
                {
                    _influxDBService.Write(write =>
                    {
                        write.WritePoints(fields, manufacturer, selectedOrganization.Name);
                    }, IP_URL, token);
                    fields.Clear();
                }
            }
        }

        private async Task<List<Organization>> GetOrganizationsByName(string organisation, string IP_URL, string token)
        {
            using var client = InfluxDBClientFactory.Create(IP_URL, token);
            var organizations = await client.GetOrganizationsApi().FindOrganizationsAsync(null, null, null, organization);

            return organizations;
        }

        private async Task<Bucket> CheckOrCreateBucket(string bucket, string IP_URL, string token, Organization organization)
        {
            using var client = InfluxDBClientFactory.Create(IP_URL, token);
            var bucketInfo = await client.GetBucketsApi().FindBucketByNameAsync(bucket);

            if (bucketInfo == null)
            {
                bucketInfo = await client.GetBucketsApi().CreateBucketAsync(bucket, organization) ;
            }

            return bucketInfo;
        }
    }
}

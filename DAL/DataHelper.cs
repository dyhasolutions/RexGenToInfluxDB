﻿using InfluxShared.FileObjects;
using MatlabFile.Base;
using MDF4xx.IO;
using MODELS;
using RXD.Base;
using RXD.DataRecords;
using RXD.Objects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public static class DataHelper
    {
        public class FileType
        {
            public string Filter;
            public string Extension;
            public override string ToString() => Filter.Split('|')[0];
            public bool supportMerge =>
                Filter == DoubleDataCollection.Filter ||
                Filter == Matlab.Filter ||
                Filter == ASC.Filter ||
                Filter == BLF.Filter ||
                Filter == MDF.Filter ||
                Filter == BinRXD.Filter;
        }

        public class FileTypeList : List<FileType>
        {
            public FileTypeList()
            {
                Add(new FileType()
                {
                    Filter = DoubleDataCollection.Filter,
                    Extension = DoubleDataCollection.Extension
                });
                Add(new FileType()
                {
                    Filter = Matlab.Filter,
                    Extension = Matlab.Extension
                });
                Add(new FileType()
                {
                    Filter = ASC.Filter,
                    Extension = ASC.Extension
                });
                Add(new FileType()
                {
                    Filter = BLF.Filter,
                    Extension = BLF.Extension
                });
                Add(new FileType()
                {
                    Filter = TRC.Filter,
                    Extension = TRC.Extension
                });
                Add(new FileType()
                {
                    Filter = MDF.Filter,
                    Extension = MDF.Extension
                });
                Add(new FileType()
                {
                    Filter = BinRXD.Filter,
                    Extension = BinRXD.Extension
                });
                Add(new FileType()
                {
                    Filter = XmlHandler.Filter,
                    Extension = XmlHandler.Extension
                });
            }

            public bool ValidExtension(string extension) => Exists(e => extension.Equals(e.Extension, StringComparison.OrdinalIgnoreCase));
        }

        public static FileTypeList FileTypeCollection = new();

        public static string LastConvertMessage = "";
        public static bool LastConvertStatus;

        private static void blfProcessing(BLF blf, TraceRow row)
        {
            switch (row.TraceType)
            {
                case RecordType.Unknown:
                    break;
                case RecordType.CanTrace:
                    if (row.flagEDL)
                        blf.WriteCanFDMessage(
                            row.flagIDE ? (row._CanID | 0x80000000) : row._CanID,
                            (UInt64)(row._Timestamp * 1000000),
                            (byte)(row._BusChannel + 1),
                            row.flagDIR, row.flagBRS,
                            (byte)row._DLC, row._Data
                        );
                    else
                        blf.WriteCanMessage(
                            row.flagIDE ? (row._CanID | 0x80000000) : row._CanID,
                            (UInt64)(row._Timestamp * 1000000),
                            (byte)(row._BusChannel + 1),
                            row.flagDIR,
                            (byte)row._DLC, row._Data
                        );
                    break;
                case RecordType.CanError:
                    blf.WriteCanError((UInt64)(row._Timestamp * 1000000), (byte)(row._BusChannel + 1), row.ErrorCode);
                    break;
                case RecordType.LinTrace:
                    if (!row.LinError)
                        blf.WriteLinMessage((byte)row._CanID, (UInt64)(row._Timestamp * 1000000), (byte)(row._BusChannel + 1), row.flagDIR, (byte)row._DLC, row._Data);
                    else
                    {
                        if (row.flagLCSE)
                            blf.WriteLinCrcError((byte)row._CanID, (UInt64)(row._Timestamp * 1000000), (byte)(row._BusChannel + 1), row.flagDIR, (byte)row._DLC, row._Data);
                        else if (row.flagLTE)
                            blf.WriteLinSendError((byte)row._CanID, (UInt64)(row._Timestamp * 1000000), (byte)(row._BusChannel + 1), (byte)row._DLC);
                    }
                    break;
                case RecordType.MessageData:
                    break;
                default:
                    break;
            }
        }

        public static bool ToBLF(this TraceCollection trace, string outputPath, Action<object> ProgressCallback)
        {
            ProgressCallback?.Invoke("Writing BLF file...");
            ProgressCallback?.Invoke(0);
            try
            {
                using (BLF blf = new BLF())
                {
                    if (blf.CreateFile(outputPath, trace.StartLogTime))
                        for (int i = 0; i < trace.Count; i++)
                        {
                            blfProcessing(blf, trace[i]);
                            ProgressCallback?.Invoke(i * 100 / trace.Count);
                        }
                    ProgressCallback?.Invoke(100);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool ToBLF(this TraceCollection trace, Stream blfStream, Action<object> ProgressCallback)
        {
            ProgressCallback?.Invoke("Writing BLF file...");
            ProgressCallback?.Invoke(0);
            try
            {
                using (BLF blf = new BLF())
                {
                    if (blf.CreateStream(blfStream, trace.StartLogTime))
                        for (int i = 0; i < trace.Count; i++)
                        {
                            blfProcessing(blf, trace[i]);
                            ProgressCallback?.Invoke(i * 100 / trace.Count);
                        }
                    ProgressCallback?.Invoke(100);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool ToBLF(this BinRXD rxd, string outputPath, Action<object> ProgressCallback)
        {
            try
            {
                using (BLF blf = new BLF())
                    if (blf.CreateFile(outputPath, rxd.DatalogStartTime))
                    {
                        rxd.ProcessTraceRecords(
                            (tc) =>
                            {
                                foreach (var row in tc)
                                    blfProcessing(blf, row);
                            },
                            ProgressCallback
                        );
                        return true;
                    }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool ToBLF(this BinRXD rxd, Stream blfStream, Action<object> ProgressCallback)
        {
            try
            {
                using (BLF blf = new BLF())
                    if (blf.CreateStream(blfStream, rxd.DatalogStartTime))
                    {
                        rxd.ProcessTraceRecords(
                            (tc) =>
                            {
                                foreach (var row in tc)
                                    blfProcessing(blf, row);
                            },
                            ProgressCallback
                        );
                        return true;
                    }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool ToASCII(this BinRXD rxd, string outputPath, Action<object> ProgressCallback)
        {
            try
            {
                using (ASC asc = new ASC())
                    if (asc.Start(outputPath, rxd.DatalogStartTime))
                    {
                        rxd.ProcessTraceRecords((tc) => asc.WriteLine(tc.asASCII), ProgressCallback);
                        return true;
                    }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool ToASCII(this BinRXD rxd, Stream outputStream, Action<object> ProgressCallback)
        {
            try
            {
                using (ASC asc = new ASC())
                    if (asc.Start(outputStream, rxd.DatalogStartTime))
                    {
                        rxd.ProcessTraceRecords((tc) => asc.WriteLine(tc.asASCII), ProgressCallback);
                        return true;
                    }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool ToTRC(this BinRXD rxd, string outputPath, Action<object> ProgressCallback)
        {
            try
            {
                using (TRC trc = new TRC())
                    if (trc.Start(outputPath, rxd.DatalogStartTime))
                    {
                        rxd.ProcessTraceRecords((tc) => trc.WriteLine(tc.asTRC), ProgressCallback);
                        return true;
                    }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool ToTRC(this BinRXD rxd, Stream outputStream, Action<object> ProgressCallback)
        {
            try
            {
                using (TRC trc = new TRC())
                    if (trc.Start(outputStream, rxd.DatalogStartTime))
                    {
                        rxd.ProcessTraceRecords((tc) => trc.WriteLine(tc.asTRC), ProgressCallback);
                        return true;
                    }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool ToMatlab(this DoubleDataCollection ddata, string MatlabFileName, Action<object> ProgressCallback = null) =>
            Matlab.CreateFromDoubleData(MatlabFileName, ddata, ProgressCallback);

        public static bool ToMatlab(this DoubleDataCollection ddata, Stream MatlabStream, Action<object> ProgressCallback = null) =>
            Matlab.CreateFromDoubleData(MatlabStream, ddata, ProgressCallback);

        public static async Task<bool> Convert(BinRXD rxd, BinRXD.ExportSettings settings, string outputPath, string outputFormat = "", Action<object> ProgressCallback = null) =>
            await Convert(rxd, null, null, settings, outputPath, outputFormat, ProgressCallback);

        public static async Task<bool> Convert(BinRXD rxd, TraceCollection trace, DoubleDataCollection channels, BinRXD.ExportSettings settings, string outputPath, string outputFormat = "", Action<object> ProgressCallback = null)
        {
            bool Exported = true;
            bool isError = false;
            settings ??= new();
            try
            {
                string ext = Path.GetExtension(outputPath);
                if (rxd != null)
                {
                    if (ext.Equals(BinRXD.Extension, StringComparison.OrdinalIgnoreCase))
                    {
                        if (!rxd.rxdUri.Equals(outputPath, StringComparison.OrdinalIgnoreCase))
                            File.Copy(rxd.rxdUri, outputPath);
                        return true;
                    }
                    else if (ext.Equals(XmlHandler.Extension, StringComparison.OrdinalIgnoreCase))
                        return rxd.ToXML(outputPath);
                    else if (ext.Equals(MDF.Extension, StringComparison.OrdinalIgnoreCase))
                        return rxd.ToMF4(outputPath, settings.SignalsDatabase, ProgressCallback);
                }

                if (trace != null || rxd != null)
                {
                    Func<string, Action<object>, bool> TraceConvert = null;
                    if (ext.Equals(ASC.Extension, StringComparison.OrdinalIgnoreCase))
                        TraceConvert = trace is null ? rxd.ToASCII : trace.ToASCII;
                    else if (ext.Equals(BLF.Extension, StringComparison.OrdinalIgnoreCase))
                        TraceConvert = trace is null ? rxd.ToBLF : trace.ToBLF;
                    else if (ext.Equals(TRC.Extension, StringComparison.OrdinalIgnoreCase))
                        TraceConvert = trace is null ? rxd.ToTRC : trace.ToTRC;

                    if (TraceConvert != null)
                        return TraceConvert(outputPath, ProgressCallback);
                }

                if (channels != null || rxd != null)
                {
                    Func<string, Action<object>, bool> GetChannelConverter()
                    {
                        DoubleDataCollection BuildChannels()
                        {
                            channels ??= rxd.ToDoubleData(settings);
                            if (channels is null || channels.Count == 0)
                                throw new Exception("There is no data channels to export!");
                            else
                                return channels;
                        }

                        if (ext.Equals(Matlab.Extension, StringComparison.OrdinalIgnoreCase))
                            return BuildChannels().ToMatlab;
                        else if (ext.Equals(DoubleDataCollection.Extension, StringComparison.OrdinalIgnoreCase))
                            if (outputFormat.Equals("InfluxDB", StringComparison.OrdinalIgnoreCase))
                                return BuildChannels().ToInfluxDBCSV;
                            else
                                return BuildChannels().ToCSV;
                        return null;
                    }

                    var ChannelConvert = GetChannelConverter();
                    if (ChannelConvert != null)
                        return ChannelConvert(outputPath, ProgressCallback);
                }

                Exported = false;
                return false;
            }
            catch (Exception e)
            {
                isError = true;
                LastConvertMessage = e.Message;
                LastConvertStatus = false;
                return false;
            }
            finally
            {
                if (Exported && !isError)
                {
                    LastConvertMessage = "File " + Path.GetFileName(outputPath) + " successfully exported!";
                    LastConvertStatus = true;
                }
            }
        }

        public static async Task<bool> Convert(BinRXD rxd, BinRXD.ExportSettings settings, Stream outputStream, string outputFormat = "", Action<object> ProgressCallback = null) =>
            await Convert(rxd, null, null, settings, outputStream, outputFormat, ProgressCallback);

        public static async Task<bool> Convert(BinRXD rxd, TraceCollection trace, DoubleDataCollection channels, BinRXD.ExportSettings settings, Stream outputStream, string outputFormat = "", Action<object> ProgressCallback = null)
        {
            bool Exported = true;
            bool isError = false;
            settings ??= new()
            {
                StorageCache = StorageCacheType.Memory
            };

            try
            {
                outputFormat = outputFormat.Trim();
                var tmp = outputFormat.Split(':');
                outputFormat = tmp.Length > 1 ? tmp[1] : "";
                string ext = "." + tmp[0];
                if (rxd != null)
                {
                    if (ext.Equals(BinRXD.Extension, StringComparison.OrdinalIgnoreCase))
                        return rxd.ToRXData(outputStream);
                    else if (ext.Equals(XmlHandler.Extension, StringComparison.OrdinalIgnoreCase))
                        throw new Exception("Not implemented!");
                    //return rxd.ToXML(outputPath);
                    else if (ext.Equals(MDF.Extension, StringComparison.OrdinalIgnoreCase))
                        throw new Exception("Not implemented!");
                    //return rxd.ToMF4(outputPath, settings.SignalsDatabase, ProgressCallback);
                }

                if (trace != null || rxd != null)
                {
                    Func<Stream, Action<object>, bool> TraceConvert = null;
                    if (ext.Equals(ASC.Extension, StringComparison.OrdinalIgnoreCase))
                        TraceConvert = trace is null ? rxd.ToASCII : trace.ToASCII;
                    else if (ext.Equals(BLF.Extension, StringComparison.OrdinalIgnoreCase))
                        TraceConvert = trace is null ? rxd.ToBLF : trace.ToBLF;
                    else if (ext.Equals(TRC.Extension, StringComparison.OrdinalIgnoreCase))
                        TraceConvert = trace is null ? rxd.ToTRC : trace.ToTRC;

                    if (TraceConvert != null)
                        TraceConvert(outputStream, ProgressCallback);
                }

                if (channels != null || rxd != null)
                    
                
            {
                    Func<Stream, Action<object>, bool> GetChannelConverter()
                    {
                        DoubleDataCollection BuildChannels()
                        {
                            channels ??= rxd.ToDoubleData(settings);
                            if (channels is null || channels.Count == 0)
                                throw new Exception("There is no data channels to export!");
                            else
                                return channels;
                        }

                        if (ext.Equals(Matlab.Extension, StringComparison.OrdinalIgnoreCase))
                            return BuildChannels().ToMatlab;
                        else if (ext.Equals(DoubleDataCollection.Extension, StringComparison.OrdinalIgnoreCase))
                            if (outputFormat.Equals("InfluxDB", StringComparison.OrdinalIgnoreCase))
                                return BuildChannels().ToInfluxDBCSV;
                            else
                                return BuildChannels().ToCSV;
                        return null;
                    }

                    var ChannelConvert = GetChannelConverter();
                    if (ChannelConvert != null)
                        return ChannelConvert(outputStream, ProgressCallback);
                }

                Exported = false;
                return false;
            }
            catch (Exception e)
            {
                isError = true;
                LastConvertMessage = e.Message;
                LastConvertStatus = false;
                return false;
            }
            finally
            {
                if (Exported && !isError)
                {
                    LastConvertMessage = "Data stream successfully exported!";
                    LastConvertStatus = true;
                }
            }
        }

        public static List<TimestampData> ExportToCustomObjects(this BinRXD rxd, BinRXD.ExportSettings settings, Action<object> ProgressCallback = null)
        {
            DoubleDataCollection BuildChannels()
            {
                var channels = rxd.ToDoubleData(settings);
                if (channels is null || channels.Count == 0)
                    throw new Exception("There is no data channels to export!");
                else
                    return channels;
            }

            return BuildChannels().ToCustomObjects(TimeFormatType.DateTime, ProgressCallback);
        }

        private static List<TimestampData> ToCustomObjects(this DoubleDataCollection ddc, TimeFormatType TimeFormat, Action<object> ProgressCallback = null)
        {
            Func<double, DateTime> TimestampToString = TimeFormat switch
            {
                TimeFormatType.DateTime => delegate (double ts) { return DateTime.FromOADate(ddc.RealTime.ToOADate() + ts / 86400); }
            };
            var ci = new CultureInfo("en-US", false);

            ProgressCallback?.Invoke(0);
            ProgressCallback?.Invoke("Exporting data ...");
            ddc.InitReading();

            double[] values = ddc.GetValues();
            List<TimestampData> timestampDataSamples = new List<TimestampData>();
            while (values != null)
            {
                List<Signal> dataSampleSignals = new List<Signal>();

                for (int i = 1; i < values.Length - 1; i++)
                {
                    if (i-1 <= 12)
                    {
                        if (!Double.IsNaN(values[i - 1]) && !Double.IsInfinity(values[i - 1]))
                        {
                            Signal signalValue = new Signal()
                            {
                                SignalName = ddc[i - 1].ChannelName,
                                SignalUnit = ddc[i - 1].ChannelUnits,
                                SigValue = values[i]
                            };
                            dataSampleSignals.Add(signalValue);
                        }
                    }
                    else
                    {
                        if (!Double.IsNaN(values[i - 1]) && !Double.IsInfinity(values[i - 1]))
                        {
                            Signal signalValue = new Signal()
                            {
                                SignalName = ddc[i - 1].ChannelName,
                                SignalUnit = ddc[i - 1].ChannelUnits,
                                SigValue = values[i-1]
                            };
                            dataSampleSignals.Add(signalValue);
                        }
                    }
                    
                    
                }
                if (dataSampleSignals.Count != 0)
                {
                    TimestampData dataSample = new TimestampData()
                    {
                        Timestamp = TimestampToString(values[0]),
                        Signals = dataSampleSignals,
                        DataloggerSerialNumber = ddc.DisplayName
                    };
                    timestampDataSamples.Add(dataSample);

                    //resetting dataSampleSignals
                    dataSampleSignals = null;

                    values = ddc.GetValues();
                }
                
            };

            ProgressCallback?.Invoke(100);
            return timestampDataSamples;
        }
    }
    
}

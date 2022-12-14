﻿using InfluxShared.FileObjects;
using MDF4xx.Blocks;
using MDF4xx.Frames;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MDF4xx.IO
{
    class BlockBuilder : IDisposable
    {
        internal Int64 lastlink;
        readonly BlockCollection collection;
        readonly byte TimestampSize;
        readonly UInt32 TimestampPrecision;

        internal BlockBuilder(BlockCollection bcollection, byte DefaultTimeSize, UInt32 DefaultTimePrecision)
        {
            collection = bcollection;
            TimestampSize = DefaultTimeSize;
            TimestampPrecision = DefaultTimePrecision;
            lastlink = 0;
        }

        public void Dispose() => GC.SuppressFinalize(this);

        internal void BuildID()
        {
            collection.id = new IDBlock();
            collection.id.Finalized = true;
            //collection.id.data.id_unfin_flags = 37;
            lastlink = (Int64)collection.id.Size;
        }

        internal void BuildHD(DateTime InitialTimestamp)
        {
            collection.hd = new HDBlock();
            collection.hd.data.hd_start_time_ns = (UInt64)(new DateTimeOffset(InitialTimestamp.ToLocalTime()).ToUnixTimeMilliseconds() * 1000000);
            collection.hd.data.hd_time_flags = 2;
            collection.hd.SetWriteFileLink(ref lastlink);
            collection.Add(collection.hd);
        }

        internal void BuildFH()
        {
            FHBlock fh = new FHBlock();
            fh.SetWriteFileLink(ref lastlink);
            fh.data.fh_time_ns = collection.hd.data.hd_start_time_ns;
            collection.Add(fh);
            collection.hd.links.SetObject(HDLinks.hd_fh_first, fh);

            MDBlock md = new MDBlock();
            md.md_data = XmlTemplate.FH;
            md.SetWriteFileLink(ref lastlink);
            collection.Add(md);
            fh.links.SetObject(FHLinks.fh_md_comment, md);
        }

        internal DGBlock BuildDG(byte GroupIDSize)
        {
            DGBlock dg = new DGBlock();
            dg.data.dg_rec_id_size = GroupIDSize;
            dg.SetWriteFileLink(ref lastlink);
            collection.Add(dg);
            collection.hd.links.SetObject(HDLinks.hd_dg_first, dg);

            return dg;
        }

        internal CGBlock BuildCG(DGBlock dg, string GroupName, UInt64 RecordID)
        {
            CGBlock cg = new CGBlock();
            cg.SetWriteFileLink(ref lastlink);
            cg.data.cg_record_id = RecordID;
            cg.data.cg_size.cg_data_bytes = 0;
            cg.data.cg_path_separator = '.';
            //cg.FlagBusEvent = true;
            //cg.FlagPlainBusEvent = true;
            collection.Add(cg);
            dg.AppendCG(cg);

            TXBlock tx = new TXBlock();
            tx.tx_data = GroupName;
            tx.SetWriteFileLink(ref lastlink);
            collection.Add(tx);
            cg.links.SetObject(CGLinks.cg_tx_acq_name, tx);

            return cg;
        }

        internal CGBlock BuildCG(DGBlock dg, string GroupName, UInt64 RecordID, BaseBlock source)
        {
            CGBlock cg = BuildCG(dg, GroupName, RecordID);
            if (source != null)
                cg.links.SetObject(CGLinks.cg_si_acq_source, source);

            return cg;
        }

        internal SIBlock BuildSI(SIType SourceType, SIBusType BusType, string SourceName = "", string SourcePath = "", string mdComment = "")
        {
            SIBlock si = new SIBlock();
            si.data.si_type = SourceType;
            si.data.si_bus_type = BusType;
            si.SetWriteFileLink(ref lastlink);
            collection.Add(si);

            if (SourceName != "")
            {
                TXBlock tx = new TXBlock();
                tx.tx_data = SourceName;
                tx.SetWriteFileLink(ref lastlink);
                collection.Add(tx);
                si.links.SetObject(SILinks.si_tx_name, tx);
            }
            if (SourcePath!= "")
            {
                TXBlock tx = new TXBlock();
                tx.tx_data = SourcePath;
                tx.SetWriteFileLink(ref lastlink);
                collection.Add(tx);
                si.links.SetObject(SILinks.si_tx_path, tx);
            }
            if (mdComment != "")
            {
                MDBlock md = new MDBlock();
                md.md_data = mdComment;
                md.SetWriteFileLink(ref lastlink);
                collection.Add(md);
                si.links.SetObject(SILinks.si_md_comment, md);
            }

            return si;
        }

        internal CNBlock BuildCN(string ChannelName, CNDataType DataType, UInt32 ByteOffset, byte BitOffset, UInt32 BitCount, CNFlags flags)
        {
            CNBlock cn = new CNBlock();
            cn.data.cn_type = CNType.FixedLength;
            cn.data.cn_sync_type = CNSyncType.None;
            cn.data.cn_data_type = DataType;
            cn.data.cn_byte_offset = ByteOffset;
            cn.data.cn_bit_offset = BitOffset;
            cn.data.cn_bit_count = BitCount;
            cn.data.cn_flags = flags;
            cn.SetWriteFileLink(ref lastlink);
            collection.Add(cn);

            TXBlock tx = new TXBlock();
            tx.tx_data = ChannelName;
            tx.SetWriteFileLink(ref lastlink);
            collection.Add(tx);
            cn.links.SetObject(CNLinks.cn_tx_name, tx);

            return cn;
        }

        internal CNBlock BuildCN(CNBlock cnFrame, string ChannelName, CNDataType DataType, UInt32 BitCount)
        {
            return BuildCN(ChannelName, DataType, cnFrame.LastByteOffset, cnFrame.LastBitOffset, BitCount, cnFrame.data.cn_flags);
        }

        internal CNBlock BuildTimeChannel(CGBlock cg, UInt32 RecordSize, double Precision)
        {
            CNBlock cnTime = new CNBlock();
            cnTime.data.cn_type = CNType.MasterChannel;
            cnTime.data.cn_sync_type = CNSyncType.Time;
            cnTime.data.cn_data_type = CNDataType.IntelUnsigned;
            cnTime.data.cn_byte_offset = 0;
            cnTime.data.cn_bit_offset = 0;
            cnTime.data.cn_bit_count = RecordSize*8;
            cnTime.data.cn_flags = 0;
            cnTime.SetWriteFileLink(ref lastlink);
            collection.Add(cnTime);
            cg.AppendCN(cnTime);

            TXBlock tx = new TXBlock();
            tx.tx_data = "Timestamp";
            tx.SetWriteFileLink(ref lastlink);
            collection.Add(tx);
            cnTime.links.SetObject(CNLinks.cn_tx_name, tx);

            CCBlock cc = new CCBlock();
            cc.data.cc_type = Blocks.ConversionType.Linear;
            cc.cc_val_length = 2;
            cc.cc_val[1].AsDouble = Precision;
            cc.SetWriteFileLink(ref lastlink);
            collection.Add(cc);
            cnTime.links.SetObject(CNLinks.cn_cc_conversion, cc);

            // Update channel group record size
            cg.data.cg_size.cg_data_bytes += RecordSize;

            return cnTime;
        }

        internal CNBlock BuildTimeChannel(CGBlock cg)
        {
            return BuildTimeChannel(cg, TimestampSize, TimestampPrecision * 0.000001);
        }

        internal CNBlock BuildFrameChannel(CGBlock cg, UInt32 offset, string FrameName)
        {
            CNBlock cn = new CNBlock();
            cn.data.cn_type = CNType.FixedLength;
            cn.data.cn_sync_type = CNSyncType.None;
            cn.data.cn_data_type = CNDataType.ByteArray;
            cn.data.cn_byte_offset = offset;
            cn.data.cn_bit_offset = 0;
            cn.data.cn_bit_count = 0;
            cn.FlagBusEvent = true;
            cn.SetWriteFileLink(ref lastlink);
            collection.Add(cn);
            cg.AppendCN(cn);

            TXBlock tx = new TXBlock();
            tx.tx_data = FrameName;
            tx.SetWriteFileLink(ref lastlink);
            collection.Add(tx);
            cn.links.SetObject(CNLinks.cn_tx_name, tx);

            return cn;
        }

        internal void BuildCanDataFrameGroup(DGBlock dg)
        {
            //cg.data.cg_size.cg_data_bytes = 4/*timestamp*/ + 8/*fixed hdr*/ + 64/*can data*/;
            string framename = "CAN_DataFrame";

            CGBlock cg = BuildCG(dg, framename, Convert.ToUInt32(FrameType.CAN_DataFrame), BuildSI(SIType.BUS, SIBusType.CAN, "CAN"));
            cg.FlagBusEvent = true;
            cg.FlagPlainBusEvent = true;
            CNBlock cnTime = BuildTimeChannel(cg);
            CNBlock cnFrame = BuildFrameChannel(cg, cnTime.LastByteOffset, framename);
            framename += (char)cg.data.cg_path_separator;
            cnFrame.AppendArrayChildCN(BuildCN(cnFrame, framename + "BusChannel", CNDataType.IntelUnsigned, 1 * 8));
            cnFrame.AppendArrayChildCN(BuildCN(cnFrame, framename + "DLC", CNDataType.IntelUnsigned, 1 * 8));
            cnFrame.AppendArrayChildCN(BuildCN(cnFrame, framename + "DataLength", CNDataType.IntelUnsigned, 1 * 8));
            cnFrame.AppendArrayChildCN(BuildCN(cnFrame, framename + "ID", CNDataType.IntelUnsigned, 4 * 8));
            cnFrame.AppendArrayChildCN(BuildCN(cnFrame, framename + "IDE", CNDataType.IntelUnsigned, 1));
            cnFrame.AppendArrayChildCN(BuildCN(cnFrame, framename + "SRR", CNDataType.IntelUnsigned, 1));
            cnFrame.AppendArrayChildCN(BuildCN(cnFrame, framename + "EDL", CNDataType.IntelUnsigned, 1));
            cnFrame.AppendArrayChildCN(BuildCN(cnFrame, framename + "BRS", CNDataType.IntelUnsigned, 1));
            cnFrame.AppendArrayChildCN(BuildCN(cnFrame, framename + "Dir", CNDataType.IntelUnsigned, 1));
            cnFrame.AppendArrayChildCN(BuildCN(framename + "DataBytes", CNDataType.ByteArray, cnFrame.LastByteOffset + 1, 0, 64 * 8, cnFrame.data.cn_flags));

            // Update channel group record size
            cg.data.cg_size.cg_data_bytes += cnFrame.LastByteOffset - cnFrame.data.cn_byte_offset;
        }

        internal void BuildCanErrorFrameGroup(DGBlock dg)
        {
            //cg.data.cg_size.cg_data_bytes = 4/*timestamp*/ + 3/*fixed hdr*/;
            string framename = "CAN_ErrorFrame";

            CGBlock cg = BuildCG(dg, framename, Convert.ToUInt32(FrameType.CAN_ErrorFrame), BuildSI(SIType.BUS, SIBusType.CAN, "CAN"));
            cg.FlagBusEvent = true;
            cg.FlagPlainBusEvent = true;
            CNBlock cnTime = BuildTimeChannel(cg);
            CNBlock cnFrame = BuildFrameChannel(cg, cnTime.LastByteOffset, framename);

            framename += (char)cg.data.cg_path_separator;
            cnFrame.AppendArrayChildCN(BuildCN(cnFrame, framename + "BusChannel", CNDataType.IntelUnsigned, 1 * 8));
            cnFrame.AppendArrayChildCN(BuildCN(cnFrame, framename + "Dir", CNDataType.IntelUnsigned, 1));
            cnFrame.AppendArrayChildCN(BuildCN(cnFrame, framename + "EDL", CNDataType.IntelUnsigned, 1));
            cnFrame.AppendArrayChildCN(BuildCN(framename + "ErrorType", CNDataType.IntelUnsigned, cnFrame.LastByteOffset + 1, 0, 1 * 8, cnFrame.data.cn_flags));
            cnFrame.AppendArrayChildCN(BuildCN(cnFrame, framename + "ErrorCount", CNDataType.IntelUnsigned, 1 * 8));

            // Update channel group record size
            cg.data.cg_size.cg_data_bytes += cnFrame.LastByteOffset - cnFrame.data.cn_byte_offset;
        }

        internal void BuildLinDataFrameGroup(DGBlock dg)
        {
            //cg.data.cg_size.cg_data_bytes = 4/*timestamp*/ + 8/*fixed hdr*/ + 64/*can data*/;
            string framename = "LIN_Frame";

            CGBlock cg = BuildCG(dg, framename, Convert.ToUInt32(FrameType.LIN_DataFrame), BuildSI(SIType.BUS, SIBusType.LIN, "LIN"));

            cg.FlagBusEvent = true;
            cg.FlagPlainBusEvent = true;
            CNBlock cnTime = BuildTimeChannel(cg);
            CNBlock cnFrame = BuildFrameChannel(cg, cnTime.LastByteOffset, framename);
            
            framename += (char)cg.data.cg_path_separator;
            cnFrame.AppendArrayChildCN(BuildCN(cnFrame, framename + "BusChannel", CNDataType.IntelUnsigned, 1 * 8));
            cnFrame.AppendArrayChildCN(BuildCN(cnFrame, framename + "ID", CNDataType.IntelUnsigned, 1 * 8));
            cnFrame.AppendArrayChildCN(
                BuildCN(cnFrame, framename + "DataLength", CNDataType.IntelUnsigned, 1 * 8),
                BuildCN(cnFrame, framename + "ReceivedDataByteCount", CNDataType.IntelUnsigned, 1 * 8)
            );
            cnFrame.AppendArrayChildCN(BuildCN(cnFrame, framename + "Dir", CNDataType.IntelUnsigned, 1));
            cnFrame.AppendArrayChildCN(BuildCN(framename + "DataBytes", CNDataType.ByteArray, cnFrame.LastByteOffset + 1, 0, 8 * 8, cnFrame.data.cn_flags));

            // Update channel group record size
            cg.data.cg_size.cg_data_bytes += cnFrame.LastByteOffset - cnFrame.data.cn_byte_offset;
        }

        internal void BuildLinChecksumErrorFrameGroup(DGBlock dg)
        {
            //cg.data.cg_size.cg_data_bytes = 4/*timestamp*/ + 8/*fixed hdr*/ + 64/*can data*/;
            string framename = "LIN_ChecksumError";

            CGBlock cg = BuildCG(dg, framename, Convert.ToUInt32(FrameType.LIN_ChecksumErrorFrame), BuildSI(SIType.BUS, SIBusType.LIN, "LIN"/*, framename*/));

            cg.FlagBusEvent = true;
            cg.FlagPlainBusEvent = true;
            CNBlock cnTime = BuildTimeChannel(cg);
            CNBlock cnFrame = BuildFrameChannel(cg, cnTime.LastByteOffset, framename);

            framename += (char)cg.data.cg_path_separator;
            cnFrame.AppendArrayChildCN(BuildCN(cnFrame, framename + "BusChannel", CNDataType.IntelUnsigned, 1 * 8));
            cnFrame.AppendArrayChildCN(BuildCN(cnFrame, framename + "ID", CNDataType.IntelUnsigned, 1 * 8));
            cnFrame.AppendArrayChildCN(
                BuildCN(cnFrame, framename + "ReceivedDataByteCount", CNDataType.IntelUnsigned, 1 * 8),
                BuildCN(cnFrame, framename + "DataLength", CNDataType.IntelUnsigned, 1 * 8)
            );
            cnFrame.AppendArrayChildCN(BuildCN(cnFrame, framename + "Dir", CNDataType.IntelUnsigned, 1));
            cnFrame.AppendArrayChildCN(BuildCN(framename + "DataBytes", CNDataType.ByteArray, cnFrame.LastByteOffset + 1, 0, 8 * 8, cnFrame.data.cn_flags));

            // Align bit count for unfinished byte
            //cnFrame.data.cn_bit_count = (uint)((cnFrame.data.cn_bit_count + 7) & ~7);
            // Update channel group record size
            cg.data.cg_size.cg_data_bytes += cnFrame.LastByteOffset - cnFrame.data.cn_byte_offset;
        }

        internal void BuildLinTransmissionErrorFrameGroup(DGBlock dg)
        {
            //cg.data.cg_size.cg_data_bytes = 4/*timestamp*/ + 8/*fixed hdr*/ + 64/*can data*/;
            string framename = "LIN_TransmissionError";

            CGBlock cg = BuildCG(dg, framename, Convert.ToUInt32(FrameType.LIN_TransmissionErrorFrame), BuildSI(SIType.BUS, SIBusType.LIN, "LIN"));

            cg.FlagBusEvent = true;
            cg.FlagPlainBusEvent = true;
            CNBlock cnTime = BuildTimeChannel(cg);
            CNBlock cnFrame = BuildFrameChannel(cg, cnTime.LastByteOffset, framename);

            framename += (char)cg.data.cg_path_separator;
            cnFrame.AppendArrayChildCN(BuildCN(cnFrame, framename + "BusChannel", CNDataType.IntelUnsigned, 1 * 8));
            cnFrame.AppendArrayChildCN(BuildCN(cnFrame, framename + "ID", CNDataType.IntelUnsigned, 1 * 8));

            // Update channel group record size
            cg.data.cg_size.cg_data_bytes += cnFrame.LastByteOffset - cnFrame.data.cn_byte_offset;
        }

        internal CCBlock BuildCCRational(CNBlock cn, double[] parameters)
        {
            CCBlock cc = new CCBlock();
            cc.data.cc_type = Blocks.ConversionType.Rational;
            cc.cc_val_length = 6;
            for (int i = 0; i < Math.Min(cc.cc_val_length, parameters.Length); i++)
                cc.cc_val[i].AsDouble = parameters[i];
            cc.SetWriteFileLink(ref lastlink);
            collection.Add(cc);
            cn.links.SetObject(CNLinks.cn_cc_conversion, cc);

            return cc;
        }

        internal CCBlock BuildCCLinear(CNBlock cn, double[] parameters)
        {
            CCBlock cc = new CCBlock();
            cc.data.cc_type = Blocks.ConversionType.Linear;
            cc.cc_val_length = 2;
            for (int i = 0; i < Math.Min(cc.cc_val_length, parameters.Length); i++)
                cc.cc_val[i].AsDouble = parameters[i];
            cc.SetWriteFileLink(ref lastlink);
            collection.Add(cc);
            cn.links.SetObject(CNLinks.cn_cc_conversion, cc);

            return cc;
        }

        internal CNDataType ConvertType(DbcItem signal)
        {
            switch (signal.ValueType)
            {
                case DBCValueType.Unsigned: return (signal.ByteOrder == DBCByteOrder.Intel) ? CNDataType.IntelUnsigned : CNDataType.MotorolaUnsigned;
                case DBCValueType.Signed: return (signal.ByteOrder == DBCByteOrder.Intel) ? CNDataType.IntelSigned : CNDataType.MotorolaSigned;
                case DBCValueType.IEEEFloat: return (signal.ByteOrder == DBCByteOrder.Intel) ? CNDataType.IntelFloat : CNDataType.MotorolaFloat;
                default: return CNDataType.IntelSigned;
            }
        }

        internal CNDataType ConvertType(LdfItem signal)
        {
            switch (signal.ValueType)
            {
                case DBCValueType.Unsigned: return (signal.ByteOrder == DBCByteOrder.Intel) ? CNDataType.IntelUnsigned : CNDataType.MotorolaUnsigned;
                case DBCValueType.Signed: return (signal.ByteOrder == DBCByteOrder.Intel) ? CNDataType.IntelSigned : CNDataType.MotorolaSigned;
                case DBCValueType.IEEEFloat: return (signal.ByteOrder == DBCByteOrder.Intel) ? CNDataType.IntelFloat : CNDataType.MotorolaFloat;
                default: return CNDataType.IntelSigned;
            }
        }

        internal CCBlock BuildConvertion(CNBlock cn, ItemConversion conversion)
        {
            switch (conversion.Type)
            {
                case InfluxShared.FileObjects.ConversionType.Formula:
                    return BuildCCRational(
                        cn,
                        new double[] {
                                conversion.Formula.CoeffA,
                                conversion.Formula.CoeffB,
                                conversion.Formula.CoeffC,
                                conversion.Formula.CoeffD,
                                conversion.Formula.CoeffE,
                                conversion.Formula.CoeffF
                        }
                    );
                case InfluxShared.FileObjects.ConversionType.TableNumeric: return null;
                case InfluxShared.FileObjects.ConversionType.TableVerbal: return null;
                case InfluxShared.FileObjects.ConversionType.FormulaAndTableNumeric: return null;
                case InfluxShared.FileObjects.ConversionType.FormulaAndTableVerbal: return null;
                default: return null;
            }
        }

        internal void BuildFrameSignalGroups(DGBlock dg, ExportCollections exSignals)
        {
            UInt32 groupid = 0;
            foreach (var msg in exSignals.dbcCollection)
            {
                while (collection.Any(g => g.Value is CGBlock && (g.Value as CGBlock).data.cg_record_id == groupid))
                    groupid++;
                msg.uniqueid = groupid;

                CGBlock cg = BuildCG(dg, "CAN" + msg.BusChannel.ToString() + "." + msg.Message.Name, groupid);
                //cg.FlagBusEvent = true;
                CNBlock cnTime = BuildTimeChannel(cg);
                CNBlock cn;
                foreach (var sig in msg.Signals)
                {
                    cg.AppendCN(cn = BuildCN(
                        sig.Name,
                        ConvertType(sig),
                        cnTime.LastByteOffset + (UInt32)(sig.StartBit / 8),
                        (byte)((sig.ByteOrder == DBCByteOrder.Intel) ? sig.StartBit % 8 : (65 - sig.BitCount + sig.StartBit) % 8),
                        sig.BitCount,
                        0
                    ));
                    if (sig.Conversion != null)
                        BuildConvertion(cn, sig.Conversion);
                }
                cg.data.cg_size.cg_data_bytes += msg.Message.DLC;
            }

            foreach (var msg in exSignals.ldfCollection)
            {
                while (collection.Any(g => g.Value is CGBlock && (g.Value as CGBlock).data.cg_record_id == groupid))
                    groupid++;
                msg.uniqueid = groupid;

                CGBlock cg = BuildCG(dg, "LIN" + msg.BusChannel.ToString() + "." + msg.Message.Name, groupid);
                //cg.FlagBusEvent = true;
                CNBlock cnTime = BuildTimeChannel(cg);
                CNBlock cn;
                foreach (var sig in msg.Signals)
                {
                    cg.AppendCN(cn = BuildCN(
                        sig.Name,
                        CNDataType.IntelUnsigned,
                        cnTime.LastByteOffset + (UInt32)(sig.StartBit / 8),
                        (byte)((sig.ByteOrder == DBCByteOrder.Intel) ? sig.StartBit % 8 : (65 - sig.BitCount + sig.StartBit) % 8),
                        sig.BitCount,
                        0
                    ));
                    if (sig.Conversion != null)
                        BuildConvertion(cn, sig.Conversion);
                }
                cg.data.cg_size.cg_data_bytes += msg.Message.DLC;
            }
        }

        CNDataType ConvertType(ChannelDescriptor sig)
        {
            if (sig.HexType == typeof(UInt64) || sig.HexType == typeof(UInt32) || sig.HexType == typeof(UInt16) || sig.HexType == typeof(byte))
                return sig.isIntel ? CNDataType.IntelUnsigned : CNDataType.MotorolaUnsigned;
            else if (sig.HexType == typeof(Int64) || sig.HexType == typeof(Int32) || sig.HexType == typeof(Int16) || sig.HexType == typeof(sbyte))
                return sig.isIntel ? CNDataType.IntelSigned : CNDataType.MotorolaSigned;
            else if (sig.HexType == typeof(double) || sig.HexType == typeof(Single))
                return sig.isIntel ? CNDataType.IntelFloat : CNDataType.MotorolaFloat;
            else
                return CNDataType.ByteArray;
        }

        internal void BuildSignals(DGBlock dg, Dictionary<UInt16, ChannelDescriptor> Signals)
        {
            CNBlock cn;
            foreach (var sig in Signals)
                if (sig.Value is not null)
                {
                    CGBlock cg = BuildCG(dg, sig.Value.Name, sig.Key);
                    CNBlock cnTime = BuildTimeChannel(cg);
                    cg.AppendCN(cn = BuildCN(
                        sig.Value.Name,
                        ConvertType(sig.Value),
                        cnTime.LastByteOffset,
                        0,
                        sig.Value.BitCount,
                        0
                    ));

                    BuildCCLinear(cn, new double[] { sig.Value.Offset, sig.Value.Factor });

                    cg.data.cg_size.cg_data_bytes += cn.LastByteOffset - cn.data.cn_byte_offset;
                }
        }

        internal void BuildDT(DGBlock dg, Int64 DataBytes)
        {
            DTBlock dt = new DTBlock();
            dt.DataLength = DataBytes;
            dt.SetWriteFileLink(ref lastlink);
            collection.Add(dt);
            dg.links.SetObject(DGLinks.dg_data, dt);
        }

        internal void BuildDZ(DGBlock dg, Int64 DataBytes)
        {
            DZBlock dz = new DZBlock();
            dz.DataLength = DataBytes;
            dz.data.dz_org_block = BlockType.DT.ToString().ToCharArray();
            dz.data.dz_zip_type = 0; // Deflate
            dz.data.dz_zip_parameter = 0;
            dz.SetWriteFileLink(ref lastlink);
            collection.Add(dz);
            dg.links.SetObject(DGLinks.dg_data, dz);
        }
    }
}

﻿
using DbcParserLib;
using InfluxShared.FileObjects;
using System;
using System.Globalization;

namespace DbcParserLib.Influx
{
    public static class DbcToInfluxObj
    {
        public static DBC FromDBC(Dbc dbc)
        {
            DBC influxDBC = new DBC();
            foreach (var msg in dbc.Messages)
            {
                DbcMessage msgI = new DbcMessage();
                msgI.CANID = msg.ID;
                msgI.DLC = msg.DLC;
                msgI.Comment = msg.Comment;
                msgI.Name = msg.Name;
                msgI.MsgType = msg.IsExtID == true ? DBCMessageType.J1939PG : DBCMessageType.Standard;
                msgI.Transmitter = msg.Transmitter;
                
                influxDBC.Messages.Add(msgI);
                foreach (var sig in msg.Signals)
                {
                    DbcItem sigI = new DbcItem();
                    sigI.Name = sig.Name;
                    sigI.Comment = sig.Comment;
                    sigI.ByteOrder = sig.ByteOrder == 0 ? DBCByteOrder.Motorola : DBCByteOrder.Intel;
                    sigI.StartBit = sig.StartBit;
                    sigI.BitCount = sig.Length;
                    sigI.Units = sig.Unit;
                    sigI.MinValue = sig.Minimum;
                    sigI.MaxValue = sig.Maximum;
                    sigI.Conversion.Type = ConversionType.Formula;
                    sigI.Conversion.Formula.CoeffB = sig.Factor;
                    sigI.Conversion.Formula.CoeffC = sig.Offset;
                    sigI.Conversion.Formula.CoeffF = 1;
                    sigI.Type = DBCSignalType.Standard;
                    sigI.ValueType = sig.IsSigned == 0 ? DBCValueType.Unsigned : DBCValueType.Signed;
                    sigI.ItemType = 0;                    
                    //sigI.Mode = sig.Multiplexing

                    msgI.Items.Add(sigI);
                }
                
            }
            return influxDBC;
        }

        public static ExportDbcCollection LoadExportSignalsFromDBC(DBC dbc)
        {
            ExportDbcCollection signalsCollection = new ExportDbcCollection();
            foreach (var msg in dbc.Messages)
            {
                for (int i = 0; i < 5; i++)
                {
                    var expmsg = signalsCollection.AddMessage(byte.Parse(i.ToString(), NumberStyles.AllowHexSpecifier), msg);
                    foreach (var sig in msg.Items)
                        expmsg.AddSignal(sig);
                }
            }
            return signalsCollection;
        }
    }
}

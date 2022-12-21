﻿using InfluxShared.Helpers;
using MDF4xx.Frames;
using RXD.Objects;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RXD.DataRecords
{
    public enum CanErrorFlags : byte
    {
        /// <summary>
        /// Extended Data Length bit (CANFD)						
        /// </summary>
        EDL = 1 << 0,
        /// <summary>
        /// Direction (Rx, Tx)						
        /// </summary>
        DIR = 1 << 1,
    }

    class RecCanTraceError : RecBase
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        internal class DataRecord
        {
            public UInt32 Timestamp;
            public CanErrorFlags Flags;
            public byte Error1;
            public byte Error2;
            public byte ErrorCount1;
            public byte ErrorCount2;
        }

        internal new DataRecord data { get => (DataRecord)base.data; set => base.data = value; }

        public RecCanTraceError()
        {
            data = new DataRecord();
        }

        public override List<BaseDataFrame> ToMdfFrame()
        {
            var frames = base.ToMdfFrame();

            if (data.Error1 != 0)
            {
                CAN_ErrorFrame frame = new CAN_ErrorFrame();

                // Copy fixed length data
                frame.data.Timestamp = data.Timestamp;
                frame.data.BusChannel = (byte)(BusChannel + 1);
                frame.data.Flags = (byte)data.Flags;
                frame.data.ErrorType = data.Error1;
                frame.data.ErrorCount = data.ErrorCount1;

                // Copy variable data
                frame.VariableData = null;

                frames.Add(frame);
            }

            if (data.Error2 != 0)
            {
                CAN_ErrorFrame frame = new CAN_ErrorFrame();

                // Copy fixed length data
                frame.data.Timestamp = data.Timestamp;
                frame.data.BusChannel = BusChannel;
                frame.data.Flags = (byte)(((byte)data.Flags) | (1 << 1));
                frame.data.ErrorType = data.Error2;
                frame.data.ErrorCount = data.ErrorCount2;

                // Copy variable data
                frame.VariableData = null;

                frames.Add(frame);
            }

            return frames;
        }

        public override TraceCollection ToTraceRow(UInt32 TimestampPrecision)
        {
            var frames = base.ToTraceRow(TimestampPrecision);

            if (data.Error1 != 0)
                frames.Add(new TraceRow()
                {
                    TraceType = RecordType.CanError,
                    _Timestamp = (double)data.Timestamp * TimestampPrecision * 0.000001,
                    _BusChannel = BusChannel,
                    NotExportable = NotExportable,
                    flagIDE = false,
                    flagSRR = false,
                    flagEDL = false,
                    flagBRS = false,
                    flagDIR = data.Flags.HasFlag(CanErrorFlags.DIR),
                    ErrorCode = data.Error1,
                    ErrorCount = data.ErrorCount1,
                });

            if (data.Error2 != 0)
                frames.Add(new TraceRow()
                {
                    TraceType = RecordType.CanError,
                    _Timestamp = (double)data.Timestamp * TimestampPrecision * 0.000001,
                    _BusChannel = BusChannel,
                    NotExportable = NotExportable,
                    flagIDE = false,
                    flagSRR = false,
                    flagEDL = true,
                    flagBRS = false,
                    flagDIR = data.Flags.HasFlag(CanErrorFlags.DIR),
                    ErrorCode = data.Error2,
                    ErrorCount = data.ErrorCount2,
                });

            return frames;
        }


    }
}

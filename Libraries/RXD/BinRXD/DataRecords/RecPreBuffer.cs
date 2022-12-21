﻿using RXD.Objects;
using System;
using System.Runtime.InteropServices;

namespace RXD.DataRecords
{
    class RecPreBuffer : RecBase
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
        internal class DataRecord
        {
            public UInt32 Timestamp;
            public UInt32 PreStartSector;
            public UInt32 PreCurrentSector;
            public UInt32 PreEndSector;
            public UInt32 PostStartSector;
            public UInt32 PostEndSector;
            public UInt32 NextPreBufferSector;
            public UInt32 InitialTimestamp;

            public UInt32 PreSize => PreEndSector - PreStartSector;
            public bool ContainPreBufferInfo => PreStartSector > 0 && PreCurrentSector > 0 && PreEndSector > 0;
            public bool ContainPostBufferInfo => PostStartSector > 0 && PostEndSector > 0;
            public bool ContainPostData => ContainPostBufferInfo || Timestamp > 0;
            public bool isLast => NextPreBufferSector == 0;
        }

        internal new DataRecord data { get => (DataRecord)base.data; set => base.data = value; }

        private bool RelativeSectors = false;

        public RecPreBuffer()
        {
            data = new DataRecord();
        }

        public void FixOffsetBy(UInt32 SectorOffset)
        {
            if (RelativeSectors)
                return;

            RelativeSectors = true;

            if (data.ContainPreBufferInfo)
            {
                data.PreStartSector -= SectorOffset;
                data.PreCurrentSector -= SectorOffset;
                data.PreEndSector -= SectorOffset;
                //if (data.PreCurrentSector < data.PreEndSector)
                    //data.PreCurrentSector += 1;
            }

            if (data.ContainPostBufferInfo)
            {
                data.PostStartSector -= SectorOffset;
                data.PostEndSector -= SectorOffset;
            }

            if (!data.isLast)
                data.NextPreBufferSector -= SectorOffset;
        }

        public override TraceCollection ToTraceRow(UInt32 TimestampPrecision)
        {
            var frames = base.ToTraceRow(TimestampPrecision);

            if (data.Timestamp > 0)
            {
                TraceRow trace = new TraceRow()
                {
                    TraceType = RecordType.PreBuffer,
                    _Timestamp = (double)data.Timestamp * TimestampPrecision * 0.000001,
                    NotExportable = NotExportable,
                    _DLC = header.DLC,
                    _Data = new byte[header.DLC]
                };

                // Copy variable data
                Buffer.BlockCopy(VariableData, 0, trace._Data, 0, header.DLC);

                frames.Add(trace);
            }

            return frames;
        }


    }
}

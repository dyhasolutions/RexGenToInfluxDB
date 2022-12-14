﻿using InfluxShared.Objects;
using System;

namespace InfluxShared.FileObjects
{
    public class ChannelDescriptor
    {
        public string Name { get; set; }
        public string Units { get; set; }
        public UInt16 StartBit { get; set; }
        public UInt16 BitCount { get; set; }
        public bool isIntel { get; set; }
        public Type HexType { get; set; }
        public int TypeIndex 
        { 
            get
            {
                if (HexType == typeof(uint) || HexType == typeof(UInt16) || HexType == typeof(UInt32) || HexType == typeof(UInt64))
                    return Array.IndexOf(BinaryData.BinaryTypes, typeof(UInt64));
                else if (HexType == typeof(int) || HexType == typeof(Int16) || HexType == typeof(Int32) || HexType == typeof(Int64))
                    return Array.IndexOf(BinaryData.BinaryTypes, typeof(Int64));
                else
                    return Array.IndexOf(BinaryData.BinaryTypes, HexType);
            }
        }
        public double Factor { get; set; }
        public double Offset { get; set; }

        public BinaryData CreateBinaryData() => new BinaryData(StartBit, BitCount, isIntel, TypeIndex, Factor, Offset);
    }
}

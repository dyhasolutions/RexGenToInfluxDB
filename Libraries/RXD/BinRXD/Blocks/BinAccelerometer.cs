﻿using InfluxShared.FileObjects;
using System;

namespace RXD.Blocks
{
    public class BinAccelerometer : BinBase
    {
        #region Enumerations for Property type definitions
        #endregion

        internal enum BinProp
        {
            PhysicalNumber,
            Axis,
            SamplingRate,
            RangeHi,
            RangeLow,
        }

        #region Do not touch these
        public BinAccelerometer(BinHeader hs = null) : base(hs) { }

        internal dynamic this[BinProp index]
        {
            get => data.GetProperty(index.ToString());
            set => data.SetProperty(index.ToString(), value);
        }
        #endregion

        internal override string GetName => $"Accelerometer {this[BinProp.Axis]}";
        internal override string GetUnits => "g";
        internal override ChannelDescriptor GetDataDescriptor => new ChannelDescriptor()
        { StartBit = 0, BitCount = 32, isIntel = true, HexType = typeof(Single), Factor = 1, Offset = 0, Name = GetName, Units = GetUnits };

        internal override void SetupVersions()
        {
            Versions[1] = new Action(() =>
            {
                data.AddProperty(BinProp.PhysicalNumber, typeof(byte));
                data.AddProperty(BinProp.Axis, typeof(AxisType));
                data.AddProperty(BinProp.SamplingRate, typeof(UInt16));
                data.AddProperty(BinProp.RangeHi, typeof(Single));
                data.AddProperty(BinProp.RangeLow, typeof(Single));
            });
        }

    }
}

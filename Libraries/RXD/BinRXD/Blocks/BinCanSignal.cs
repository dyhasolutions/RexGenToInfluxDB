﻿using InfluxShared.FileObjects;
using InfluxShared.Objects;
using System;

namespace RXD.Blocks
{
    #region Enumerations for Property type definitions
    enum SignalByteOrder : byte
    {
        MOTOROLA,
        INTEL
    }

    enum SignalDataType : byte
    {
        UNSIGNED,
        SIGNED,
        FLOAT32,
        FLOAT64
    }

    enum SignalInputType : byte
    {
        COMMON,
        MESSAGE
    }
    #endregion

    class BinCanSignal : BinBase
    {
        internal enum BinProp
        {
            InputType,
            InputUID,
            MessageUID,
            StartBit,
            BitCount,
            Endian,
            DefaultValue,
            ParA,
            ParB,
            SignalType,
            NameSize,
            Name,
        }

        #region Do not touch these
        public BinCanSignal(BinHeader hs = null) : base(hs) { }

        internal dynamic this[BinProp index]
        {
            get => data.GetProperty(index.ToString());
            set => data.SetProperty(index.ToString(), value);
        }
        #endregion

        internal override ChannelDescriptor GetDataDescriptor => new ChannelDescriptor()
        { 
            StartBit = this[BinProp.StartBit], BitCount = this[BinProp.BitCount], 
            isIntel = this[BinProp.Endian] == SignalByteOrder.INTEL, HexType = BinaryData.BinaryTypes[(int)this[BinProp.SignalType]], 
            Factor = this[BinProp.ParA], Offset = this[BinProp.ParB], 
            Name = GetName, Units = GetUnits 
        };

        internal override void SetupVersions()
        {
            Versions[1] = new Action(() =>
            {
                data.AddProperty(BinProp.InputType, typeof(SignalInputType));
                data.AddProperty(BinProp.InputUID, typeof(UInt16));
                data.AddProperty(BinProp.MessageUID, typeof(UInt16));
                data.AddProperty(BinProp.StartBit, typeof(UInt16));
                data.AddProperty(BinProp.BitCount, typeof(byte));
                data.AddProperty(BinProp.Endian, typeof(SignalByteOrder));
                data.AddProperty(BinProp.DefaultValue, typeof(Single));
                data.AddProperty(BinProp.ParA, typeof(Single));
                data.AddProperty(BinProp.ParB, typeof(Single));
                data.AddProperty(BinProp.SignalType, typeof(SignalDataType));
            });
            Versions[2] = new Action(() =>
            {
                Versions[1].DynamicInvoke();
                data.AddProperty(BinProp.NameSize, typeof(byte));
                data.AddProperty(BinProp.Name, typeof(string), BinProp.NameSize);
            });
        }

    }
}

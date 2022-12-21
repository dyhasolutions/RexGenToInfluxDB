﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RXD.Blocks
{
    internal class BinDAQItem: BinBase
    {
        internal enum BinProp
        {
            DAQID,
            NameSize,
            Name,
            InputUID,
        }

        #region Do not touch these
        public BinDAQItem(BinHeader hs = null) : base(hs) { }

        internal dynamic this[BinProp index]
        {
            get => data.GetProperty(index.ToString());
            set => data.SetProperty(index.ToString(), value);
        }
        #endregion

        internal override void SetupVersions()
        {
            Versions[1] = new Action(() =>
            {
                data.AddProperty(BinProp.DAQID, typeof(UInt16));
                data.AddProperty(BinProp.NameSize, typeof(byte));
                data.AddProperty(BinProp.Name, typeof(string), BinProp.NameSize);
                data.AddProperty(BinProp.InputUID, typeof(UInt16));
            });
        }

    }
}

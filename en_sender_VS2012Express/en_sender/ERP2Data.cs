using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace en_sender
{
    public abstract class ERP2Data
    {
        //
        public virtual ushort data_length_
        {
            get;
            private set;
        }
        public virtual byte[] build()
        {
            return null;
        }
    }
}

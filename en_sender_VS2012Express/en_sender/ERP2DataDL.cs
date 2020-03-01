using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace en_sender
{
    enum R_ORG
    {
        R_ORG_RPS = 0x00,
        R_ORG_1BS = 0x01,
        R_ORG_4BS = 0x02,
        R_ORG_VLD = 0x04,
    }

    abstract class ERP2DataDL : ERP2Data
    {
        //
        protected abstract R_ORG getRORG();
        protected abstract byte[] getData();
        public override ushort data_length_
        {
            get { return (ushort)(getData().Length + 6); }
        }

        public override byte[] build()
        {
            byte[] data = new byte[this.data_length_];
            // Bit 5…7 Address Control, 001: Originator-ID 32 bit; no Destination-ID
            // Bit 4 Extended header available, 0: No extended header
            // Bit 0…3 Telegram type (R-ORG)
            //      0000: RPS telegram (0xF6) 
            //      0001: 1BS telegram (0xD5) 
            //      0010: 4BS telegram (0xA5)
            //      0100: Variable length data telegram (0xD2)
            data[0] = (byte)(0x20 | (int)getRORG());
            // Originator-ID
            data[1] = 0x11; // will be overwritten with the ID of the sending device
            data[2] = 0x22;
            data[3] = 0x33;
            data[4] = 0x44;
            // Data_DL
            Array.Copy(getData(), 0, data, 5, getData().Length);
            // CRC
            data[5 + getData().Length] = 0xFF; // ignored ?

            return data;
        }
    }

    // 
    // EEP
    // 
    class EEPF60204 : ERP2DataDL
    {
        byte[] data_ = new byte[1];
        public EEPF60204(int data)
        {
            this.data_[0] = (byte)data;
        }
        protected override R_ORG getRORG() { return R_ORG.R_ORG_RPS; }
        protected override byte[] getData() { return data_; }
    }
}

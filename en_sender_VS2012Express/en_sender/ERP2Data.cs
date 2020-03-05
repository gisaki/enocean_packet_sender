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

        // 
        // make repeated radio telegram (from not repeated data)
        // 
        public byte[] convToRepeated(byte[] not_repeated_data)
        {
            // rough error check, cant convert (too short)
            if (not_repeated_data.Length < 2)
            {
                return not_repeated_data;
            }

            // Header
            if ((not_repeated_data[0] & 0x10u) != 0) // Bit 4 Extended header available = 1: Extended header available
            {
                // Extended Header
                if ((not_repeated_data[1] & 0xF0u) != 0) // Bit 4…7 Repeater count = repeated
                {
                    // already repeated telegram
                    return not_repeated_data;
                }
                else
                {
                    byte[] repeated_telegram = new byte[not_repeated_data.Length];
                    Array.Copy(not_repeated_data, repeated_telegram, not_repeated_data.Length);

                    // Extended Header
                    // overwrite
                    System.Random r = new System.Random();
                    byte count = (byte)r.Next(1, 16); // 1..15
                    repeated_telegram[1] |= (byte)(count << 4); // Bit 4…7 Repeater count

                    return repeated_telegram;
                }
            }
            else
            {
                // Bit 4 Extended header available = 0: No extended header

                // insert Extended header
                byte[] repeated_telegram = new byte[not_repeated_data.Length + 1];
                Array.Copy(not_repeated_data, 0, repeated_telegram, 0, 1);
                repeated_telegram[1] = 0;
                Array.Copy(not_repeated_data, 1, repeated_telegram, 2, not_repeated_data.Length - 1);

                // Header
                // overwrite
                repeated_telegram[0] |= ((byte)0x10u); // Bit 4 Extended header available = 1: Extended header available

                // Extended Header
                // overwrite
                System.Random r = new System.Random();
                byte count = (byte)r.Next(1, 16); // 1..15
                repeated_telegram[1] |= (byte)(count << 4); // Bit 4…7 Repeater count

                return repeated_telegram;
            }
        }
    }
}

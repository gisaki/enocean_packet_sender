using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace en_sender
{
    public class ERP2DataBuilder : ERP2Data
    {
        private System.Windows.Forms.DataGridView data_grid_view_;

        // 
        // TabControl
        // 
        public void InitializeTabControl(ref System.Windows.Forms.TabControl tab_control, string text)
        {
            // TabPage
            System.Windows.Forms.TabPage tab_page = new System.Windows.Forms.TabPage();
            tab_page.UseVisualStyleBackColor = true;
            tab_page.Name = text;
            tab_page.Text = text;
            tab_control.Controls.Add(tab_page);

            // Panel
            System.Windows.Forms.Panel panel = new System.Windows.Forms.Panel();
            panel.Dock = System.Windows.Forms.DockStyle.Fill;
            panel.AutoScroll = true;
            tab_page.Controls.Add(panel);

            // DataGridView
            this.data_grid_view_ = new System.Windows.Forms.DataGridView();
            this.data_grid_view_.Dock = System.Windows.Forms.DockStyle.Fill;
            this.data_grid_view_.AllowUserToAddRows = false;
            this.data_grid_view_.AllowUserToResizeRows = false;
            this.data_grid_view_.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.data_grid_view_.MultiSelect = false;
            panel.Controls.Add(this.data_grid_view_);

            // Column
            {
                System.Windows.Forms.DataGridViewTextBoxColumn textbox = new System.Windows.Forms.DataGridViewTextBoxColumn();
                textbox.ReadOnly = true;
                this.data_grid_view_.Columns.Add(textbox);
                textbox.Name = "Name";
            }
            {
                System.Windows.Forms.DataGridViewTextBoxColumn textbox = new System.Windows.Forms.DataGridViewTextBoxColumn();
                textbox.ReadOnly = true;
                this.data_grid_view_.Columns.Add(textbox);
                textbox.Name = "Bit";
            }
            {
                System.Windows.Forms.DataGridViewComboBoxColumn combobox = new System.Windows.Forms.DataGridViewComboBoxColumn();
                combobox.DataPropertyName = "ValueSelect";
                //this.data_grid_view_.Columns.Insert(1, combobox);
                this.data_grid_view_.Columns.Add(combobox);
                combobox.Name = "ValueSelect";
            }
            {
                System.Windows.Forms.DataGridViewTextBoxColumn textbox = new System.Windows.Forms.DataGridViewTextBoxColumn();
                this.data_grid_view_.Columns.Add(textbox);
                textbox.Name = "Value";
            }
            foreach (System.Windows.Forms.DataGridViewColumn col in this.data_grid_view_.Columns)
            {
                col.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            }
        }

        // 
        // Parse
        // 
        public void Parse(string[] lines)
        {
            foreach (string line in lines)
            {
                this.Parse(line);
            }
        }
        private void Parse(string line)
        {
            int LINE_VALUES_NUM = this.data_grid_view_.ColumnCount;
            int idx = this.data_grid_view_.Rows.Add();

            string[] item = line.Split(',');
            if (item.Length < LINE_VALUES_NUM)
            {
                Array.Resize(ref item, LINE_VALUES_NUM);
            }
            int pos = -1;

            // Name
            pos++;
            {
                this.data_grid_view_.Rows[idx].Cells[pos].Value = item[pos];
            }

            // Bit
            pos++;
            {
                this.data_grid_view_.Rows[idx].Cells[pos].Value = item[pos];
            }

            // ValueSelect
            pos++;
            {
                if ((item[pos] == null) || (item[pos] == ""))
                {
                    this.data_grid_view_.Rows[idx].Cells[pos].ReadOnly = true;
                }
                else
                {
                    System.Windows.Forms.DataGridViewComboBoxCell combobox = new System.Windows.Forms.DataGridViewComboBoxCell();
                    string[] values = item[pos].Split('/');
                    foreach (string value in values)
                    {
                        combobox.Items.Add(value);
                    }
                    combobox.Value = values[0]; // set default (first item selected)
                    combobox.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
                    this.data_grid_view_.Rows[idx].Cells[pos] = combobox;
                }
            }

            // Value
            pos++;
            {
                this.data_grid_view_.Rows[idx].Cells[pos].Value = item[pos];
            }
        }

        // 
        // Build
        // 
        public override ushort data_length_
        {
            get {
                int bit_sum = 0;
                foreach (var row in this.data_grid_view_.Rows.Cast<System.Windows.Forms.DataGridViewRow>())
                {
                    // Name
                    // Bit
                    // ValueSelect
                    // Value

                    string bit_str = row.Cells[1].Value.ToString();
                    int bit = 0;
                    if (int.TryParse(bit_str, out bit))
                    {
                        bit_sum += bit;
                    }
                }
                return (ushort)Math.Ceiling(bit_sum / 8.0);
            }
        }
        public override byte[] build()
        {
            byte[] ret = new byte[this.data_length_];

            int pos_tgt_bit = 0;

            foreach (var row in this.data_grid_view_.Rows.Cast<System.Windows.Forms.DataGridViewRow>())
            {
                // Name
                // Bit
                // ValueSelect
                // Value

                string bit_str = row.Cells[1].Value.ToString();
                int bit = 0;
                if (int.TryParse(bit_str, out bit))
                {
                    if (bit > 0)
                    {
                        UInt64 num = 0;
                        if (conv(row.Cells[3], out num))
                        {
                            // ok
                        }
                        else if (conv(row.Cells[2], out num))
                        {
                            // ok
                        }
                        else
                        {
                            // ng
                        }
                        pos_tgt_bit = push(ref ret, pos_tgt_bit, num, bit);
                    }
                }
            }
            return ret;
        }
        private bool conv(System.Windows.Forms.DataGridViewCell cell, out UInt64 num)
        {
            num = 0;
            if ( (cell == null) || (cell.Value == null) || (cell.Value.ToString() == "") )
            {
                return false;
            }
            string str = cell.Value.ToString();
            int fromBase = 10;
            if ((str.IndexOf("0x") == 0) || (str.IndexOf("0X") == 0))
            {
                fromBase = 16;
            }
            try
            {
                num = Convert.ToUInt64(str, fromBase /* 16 or 10 */);
            }
            catch (Exception e)
            {
            }
            return true;
        }
        private int push(ref byte[] tgt, int pos_tgt_bit, UInt64 src, int src_bit_num)
        {
            for (int i=0; i<src_bit_num; i++)
            {
                UInt64 bit = (src >> (src_bit_num - 1 - i)) & 0x01u;
                int idx = (pos_tgt_bit + i) / 8;
                int pos_in_byte = (pos_tgt_bit + i) % 8;
                UInt64 xor_byte = (bit << (8 - 1 - pos_in_byte));
                tgt[idx] |= (byte)(xor_byte);
            }
            return (pos_tgt_bit + src_bit_num);
        }
    }
}

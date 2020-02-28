using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace en_sender
{
    public class ERP2DataBuilder
    {
        public string title_;

        public ERP2DataBuilder()
        {

        }

        public void build(ref System.Windows.Forms.TabControl tab_control)
        {
            System.Windows.Forms.TabPage tab_page = new System.Windows.Forms.TabPage();
            tab_page.UseVisualStyleBackColor = true;
            tab_page.Name = "aaa";
            tab_page.Text= "bbb";

            tab_control.Controls.Add(tab_page);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace en_sender
{
    public partial class MainForm : Form
    {
        SerialPortProcessor serial_ = new SerialPortProcessor();
        Boolean serial_connect_ = false;
        List<ERP2DataBuilder> builders_ = null;

        public MainForm()
        {
            InitializeComponent();

            // Initialize Tab
            {
                string[] files = System.IO.Directory.GetFiles(
                    System.IO.Path.GetDirectoryName(
                        System.Reflection.Assembly.GetEntryAssembly().Location
                    ) + "/def", 
                    "*", 
                    System.IO.SearchOption.TopDirectoryOnly
                );
                this.builders_ = new List<ERP2DataBuilder>();
                foreach (string file in files)
                {
                    string[] lines = System.IO.File.ReadAllLines(file, Encoding.Default);
                    ERP2DataBuilder builder = new ERP2DataBuilder();
                    builder.InitializeTabControl(ref tabControl1, System.IO.Path.GetFileName(file));
                    builder.Parse(lines);
                    this.builders_.Add(builder);
                }
            }

            // COM port list first update
            UpdateSerialPortList();

            // Update GUI
            updateUi();

            // Serial port data receive handler
            serial_.DataReceived += serial_DataReceived;
        }

        // --------------------------------------------------
        // Update button, text, ... etc.
        // --------------------------------------------------
        private void updateUi()
        {
            // Button
            //            buttonSendSample.Enabled = serial_connect_;
        }

        // --------------------------------------------------
        // Update COM port list just before opening comboBox
        // --------------------------------------------------
        private void comboBoxSerialPort_DropDown(object sender, EventArgs e)
        {
            // Update COM port list (hope not taking so long time ...)
            UpdateSerialPortList();
        }

        // --------------------------------------------------
        // Update COM port list
        // --------------------------------------------------
        private void UpdateSerialPortList()
        {
            string[] ports = serial_.GetPorts();
            comboBoxSerialPort.Items.Clear();
            foreach (string port in ports)
            {
                comboBoxSerialPort.Items.Add(port);
            }
        }

        // --------------------------------------------------
        // Connect COM port
        // --------------------------------------------------
        private void buttonConnect_Click(object sender, EventArgs e)
        {
            serial_.PortName = comboBoxSerialPort.Text;
            try
            {
                if (!serial_connect_)
                {
                    serial_.Start();

                    // The following procedure will only be executed if serial.Start () succeeds.
                    serial_connect_ = true;
                    buttonConnect.Text = "Disconnect";
                    // Update GUI
                    updateUi();
                }
                else
                {
                    // Regardless of the result of serial.Close (), the following procedure is always executed.
                    buttonConnect.Text = "Connect";
                    serial_connect_ = false;
                    // Update GUI
                    updateUi();

                    serial_.Close();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception");
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            int sel_tab_idx = tabControl1.SelectedIndex;
            ERP2DataBuilder builder = this.builders_[sel_tab_idx];

            // build data and send
            buildAndSend(builder);

            if (false) // for test
            {
                ERP2DataDL erp2data_dl = null;

                // EEP
                erp2data_dl = new EEPF60204(0);

                // build data and send
                buildAndSend(erp2data_dl);
            }
        }

        // --------------------------------------------------
        // Build and send packets
        // --------------------------------------------------
        private void buildAndSend(ERP2Data erp2data)
        {
            // build data
            ESP3Packet esp3pkt = new ESP3PacketType10(erp2data);

            // Serial port data send
            if (serial_connect_)
            {
                serial_SendData(esp3pkt.build());
            }
        }
        // --------------------------------------------------
        // Serial port data send
        // --------------------------------------------------
        private async void serial_SendData(byte[] buf)
        {
            // Start a thread and send data asynchronously
            await Task.Run(() =>
            {
                try
                {
//                    MessageBox.Show(BitConverter.ToString(buf));
                    Invoke(new Action(() =>
                    {
                        String str = BitConverter.ToString(buf).Replace("-", " ");
                        textBox_SendPacket.Text = "[USB] " + str;
                    }));
                    serial_.WriteData(buf);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Exception");
                }
            });
        }

        // --------------------------------------------------
        // Serial port data receive handler
        // --------------------------------------------------
        private void serial_DataReceived(byte[] data)
        {
            // nop
        }

        // --------------------------------------------------
        // Close form
        // --------------------------------------------------
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            serial_.Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.MemoryMappedFiles;
using System.Xml;
using System.Xml.Linq;
using System.Threading;
using System.IO;

namespace Aida64DataViewer
{
    public partial class Form1 : Form
    {
        BackgroundWorker aidaUpdateWorker = new BackgroundWorker();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            aidaUpdateWorker.DoWork += new DoWorkEventHandler(aidaUpdateWorker_DoWork);
            aidaUpdateWorker.WorkerSupportsCancellation = true;
            this.WindowState = FormWindowState.Minimized;
            aidaUpdateWorker.RunWorkerAsync(aidaUpdateWorker);
        }

        void aidaUpdateWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker localbg = (BackgroundWorker)e.Argument;
            while (!localbg.CancellationPending)
            {
                DateTime startTime = DateTime.Now;
                updateAidaInformation();
                DateTime stopTime = DateTime.Now;
                TimeSpan elapsedTime = stopTime - startTime;
                Thread.Sleep(5000 - (int)elapsedTime.TotalMilliseconds);
            }
        }

        private void updateAidaInformation()
        {

            string tempString = string.Empty;

            try
            {
                MemoryMappedFile mmf = MemoryMappedFile.OpenExisting("AIDA64_SensorValues");
                MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor();
                tempString = tempString + "<AIDA64>";
                for (int i = 0; i < accessor.Capacity; i++)
                {
                    tempString = tempString + (char)accessor.ReadByte(i);
                }
                tempString = tempString.Replace("\0", "");
                tempString = tempString + "</AIDA64>";
                accessor.Dispose();
                mmf.Dispose();
            }
            catch (FileNotFoundException)
            {
                return;
            }
            


            XDocument doc = XDocument.Parse(tempString);

            var aidaSysValues = doc.Descendants("AIDA64").Descendants("sys").Select(staff => new
            {
                Id = staff.Element("id").Value,
                Label = staff.Element("label").Value,
                Value = staff.Element("value").Value,
            }).ToList();

            var aidaTempValues = doc.Descendants("AIDA64").Descendants("temp").Select(staff => new
            {
                Id = staff.Element("id").Value,
                Label = staff.Element("label").Value,
                Value = staff.Element("value").Value,
            }).ToList();

            var aidaFanValues = doc.Descendants("AIDA64").Descendants("fan").Select(staff => new
            {
                Id = staff.Element("id").Value,
                Label = staff.Element("label").Value,
                Value = staff.Element("value").Value,
            }).ToList();

            var aidaDutyValues = doc.Descendants("AIDA64").Descendants("duty").Select(staff => new
            {
                Id = staff.Element("id").Value,
                Label = staff.Element("label").Value,
                Value = staff.Element("value").Value,
            }).ToList();

            if (!serialPort1.IsOpen)
                serialPort1.Open();

            int waitTime = 100;

            if (serialPort1.IsOpen)
            {
                serialPort1.WriteLine("BEGIN");

                foreach (var staff in aidaSysValues)
                {
                    String sensorString = staff.Label + ": " + staff.Value;
                    serialPort1.WriteLine(sensorString);
                    Console.WriteLine(sensorString);
                    Thread.Sleep(waitTime);
                }

                foreach (var staff in aidaTempValues)
                {
                    String sensorString = "Temp " + staff.Label + ": " + staff.Value;
                    serialPort1.WriteLine(sensorString);
                    Console.WriteLine(sensorString);
                    Thread.Sleep(waitTime);
                }

                foreach (var staff in aidaFanValues)
                {
                    String sensorString = "Fan: " + staff.Label + ": " + staff.Value;
                    serialPort1.WriteLine(sensorString);
                    Console.WriteLine(sensorString);
                    Thread.Sleep(waitTime);
                }

                foreach (var staff in aidaDutyValues)
                {
                    String sensorString = "Fan: " + staff.Label + ": " + staff.Value;
                    serialPort1.WriteLine(sensorString);
                    Console.WriteLine(sensorString);
                    Thread.Sleep(waitTime);
                }
            }

            if (!serialPort1.IsOpen)
            {
                serialPort1.Close();
            }

            //listView1.Invoke(new MethodInvoker(delegate()
            //{
            //    listView1.Items.Clear();
            //    foreach (var staff in aidaSensorValues)
            //    {
            //        ListViewItem lvi = new ListViewItem(staff.Id);
            //        lvi.SubItems.Add(staff.Label);
            //        lvi.SubItems.Add(staff.Value);
            //        listView1.Items.Add(lvi);
            //    }
            //}));



        }

        private void button1_Click(object sender, EventArgs e)
        {
            aidaUpdateWorker.RunWorkerAsync(aidaUpdateWorker);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            closeApplication();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            closeApplication();
        }

        private void bStopLogging_Click(object sender, EventArgs e)
        {
            aidaUpdateWorker.CancelAsync();
        }

        private void maximizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }

        private void stopLoggingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aidaUpdateWorker.CancelAsync();
        }

        private void startLoggingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!aidaUpdateWorker.IsBusy)
            {
                aidaUpdateWorker.RunWorkerAsync(aidaUpdateWorker);
            }
        }

        private void closeApplication()
        {
            aidaUpdateWorker.CancelAsync();
            notifyIcon1.Dispose();
            this.Close();
        }
    }
}

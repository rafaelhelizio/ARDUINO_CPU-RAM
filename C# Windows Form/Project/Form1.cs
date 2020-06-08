using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Management;

namespace ArduinoOLED
{


    public partial class Form1 : Form
    {

        PerformanceCounter ramCounter;
        PerformanceCounter cpuCounter;

        public Form1()
        {

            InitializeComponent();

            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        }

        private void atualizaListaCOMs()
        {
            int i;
            bool quantDiferente;    //flag para sinalizar que a quantidade de portas mudou

            i = 0;
            quantDiferente = false;

            //se a quantidade de portas mudou
            if (comboBox1.Items.Count == SerialPort.GetPortNames().Length)
            {
                foreach (string s in SerialPort.GetPortNames())
                {
                    if (comboBox1.Items[i++].Equals(s) == false)
                    {
                        quantDiferente = true;
                    }
                }
            }
            else
            {
                quantDiferente = true;
            }

            //Se não foi detectado diferença
            if (quantDiferente == false)
            {
                return;                     //retorna
            }

            //limpa comboBox
            comboBox1.Items.Clear();

            //adiciona todas as COM diponíveis na lista
            foreach (string s in SerialPort.GetPortNames())
            {
                comboBox1.Items.Add(s);
            }
            //seleciona a primeira posição da lista
            comboBox1.SelectedIndex = 0;
        }
    

    private void button1_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen == false)
            {
                try
                {
                    serialPort1.PortName = comboBox1.Items[comboBox1.SelectedIndex].ToString();
                    serialPort1.Open();

                }
                catch
                {
                    return;

                }
                if (serialPort1.IsOpen)
                {
                    btConectar.Text = "Desconectar";
                    comboBox1.Enabled = false;

                }
            }
            else
            {

                try
                {
                    serialPort1.Close();
                    comboBox1.Enabled = true;
                    btConectar.Text = "Conectar";
                }
                catch
                {
                    return;
                }

            }
        }
    


        private void timer1_Tick(object sender, EventArgs e)
        {
            atualizaListaCOMs();

        }

        public string getCurrentCpuUsage()
        {
            return "{\"C\":" + Math.Round(cpuCounter.NextValue(), MidpointRounding.ToEven) + "";
        }

        public string getAvailableRAM()
        {
            double percent = ((getMemoryTotal()-ramCounter.NextValue()) / getMemoryTotal()) * 100;
            int precentRam = (int)Math.Round(percent, MidpointRounding.ToEven);

            return ",\"R\":" + precentRam.ToString() + "}";
        }

        public double getMemoryTotal() {

            ObjectQuery wql = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(wql);
            ManagementObjectCollection results = searcher.Get();

            double res;
            double fres;

            res = 0;

            foreach (ManagementObject result in results)
            {
                res = Convert.ToDouble(result["TotalVisibleMemorySize"]) / 1024;
                fres = Math.Round((res / (1024 * 1024)), 2);

                //Console.WriteLine("Total usable memory size: " + fres + "GB");
                //Console.WriteLine("Total usable memory size: " + res/1024 + "KB");
            }

            return res;

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
           
                String textSend = getCurrentCpuUsage();
                textSend += getAvailableRAM();

            if (serialPort1.IsOpen == true)
            {
                serialPort1.Write(textSend);
            }
        }
    }
}

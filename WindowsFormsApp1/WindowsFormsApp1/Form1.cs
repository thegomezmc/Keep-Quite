using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

using NAudio.Wave; // installed with nuget

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        SoundPlayer sound = new SoundPlayer("Metal Gear Alert! Sound Effect.wav");
        private static double audioValueMax = 0;
        private static double audioValueLast = 0;
        private static int RATE = 44100;
        private static int BUFFER_SAMPLES = 1024;
        private static double lvl1=0;
        private static double lvl2=0;
        private static double lvl3=0;
        public Form1()
        {
            InitializeComponent();

            var waveIn = new WaveInEvent();
            waveIn.DeviceNumber = 0;
            waveIn.WaveFormat = new NAudio.Wave.WaveFormat(RATE, 1);
            waveIn.DataAvailable += OnDataAvailable;
            waveIn.BufferMilliseconds = (int)((double)BUFFER_SAMPLES / (double)RATE * 1000.0);
            waveIn.StartRecording();
        }

        private void OnDataAvailable(object sender, WaveInEventArgs args)
        {
            
            float max = 0;

            // interpret as 16 bit audio
            for (int index = 0; index < args.BytesRecorded; index += 2)
            {
                short sample = (short)((args.Buffer[index + 1] << 8) |
                                        args.Buffer[index + 0]);
                var sample32 = sample / 32768f; // to floating point
                if (sample32 < 0) sample32 = -sample32; // absolute value 
                if (sample32 > max) max = sample32; // is this the max value?
            }

            // calculate what fraction this peak is of previous peaks
            if (max > audioValueMax)
            {
                audioValueMax = (double) max;
            }
            audioValueLast = max;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
                 
            
            if(audioValueLast<=lvl1)
            {
                button1.BackColor =Color.Green;
                button2.BackColor = Color.White;
                button3.BackColor = Color.White;
            }
            if(audioValueLast>=lvl1 && audioValueLast<=lvl2)
            {
                button1.BackColor = Color.DarkGreen;
                button2.BackColor = Color.DarkGreen;
                button3.BackColor = Color.White;   
            }
            if(audioValueLast>=lvl2)
            {
                button1.BackColor = Color.Red;
                button2.BackColor = Color.Red;
                button3.BackColor = Color.Red;
            }
            if (audioValueLast >= lvl3)
            { 
                sound.PlaySync(); 
            }
         
            lvl1 = Convert.ToDouble(numericUpDown1.Value) /10 ;
            lvl2 = Convert.ToDouble(numericUpDown2.Value) /10 ;
            lvl3 = Convert.ToDouble(numericUpDown3.Value) /10;

            if (numericUpDown1.Value==10 || numericUpDown2.Value<=numericUpDown1.Value)
            {
                numericUpDown1.Value = numericUpDown2.Value-1;
            }
            if (numericUpDown2.Value == 10 || numericUpDown3.Value <= numericUpDown2.Value)
            {
                numericUpDown2.Value = numericUpDown3.Value-1;
            }
            if (numericUpDown3.Value == 10)
            {
                numericUpDown3.Value = 9;
            }

        }        
    }
}

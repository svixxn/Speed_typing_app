using System;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using System.Collections.Generic;


namespace Speed_Typing_App
{

    public partial class Form1 : Form
    {
        //основні класси:
        public class Result
        {
            public readonly double wpmRes;

            public readonly DateTime date;

            public Result(double wpm, DateTime date)
            {
                wpmRes = wpm;
                this.date = date;
            }
        }
        public class TextToPrint
        {
            public string TextTPrint { get; set; }
            public int SymbCount = 0;
        }
        public class Input
        {
            public string Text { get; set; }
            public double acc { get; set; }
            public double wordcount = 0;
            public double time { get; set; }
            public double Time { get; set; }
          
        }
        public class Music
        {
            public static WMPLib.WindowsMediaPlayer WMP = new WMPLib.WindowsMediaPlayer();
            Timer tmr = new Timer();
            public void play_music_1()
            {
                tmr.Interval = 10;
                tmr.Stop();
                WMP.URL = @"music.mp3";
                WMP.settings.volume = 10;
                WMP.controls.play();
                tmr.Tick += new EventHandler(tmr_Tick);
                WMP.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(wplayer_PlayStateChange);
            }

            void tmr_Tick(object sender, EventArgs e)
            {
                WMP.controls.stop();
                play_music_1();
            }
            void wplayer_PlayStateChange(int NewState)
            {
                if (NewState == (int)WMPLib.WMPPlayState.wmppsMediaEnded)
                {
                    tmr.Start();

                }
            }
            public void StopMusic()
            {
                WMP.controls.stop();
            }
        }
        //загальні змінні:
        string[] lines = File.ReadAllLines("records.txt");
        string[] linesToWrite = new string[100];
        int ticks = 6;
        int ticks1 = 0, vid = 0;
        bool flag = true;
        int misc = 0,lng1=0;
        bool lang = false, musfl = false, colfl = true,fl1=true,colfl1=true;
        string text1 = "";
        int length = 0;
        TextToPrint textToPrnt = new TextToPrint();
        Input name = new Input();
        Music music = new Music();
        public Form1()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture
               = CultureInfo.GetCultureInfo(Properties.Settings.Default.Language);
            System.Threading.Thread.CurrentThread.CurrentCulture
                = CultureInfo.GetCultureInfo(Properties.Settings.Default.Language);
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TextPanel.ReadOnly = true;
            textBox1.ReadOnly = true;
            label2.Visible = false;
            label3.Visible = false;
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            button2.Visible = false;
        }

        //метод для тіка першого таймеру(таймер відліку)
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true && musfl == false)
            {
                music.play_music_1();
                musfl = true;
            }
            else if (checkBox1.Checked == false)
            {
                music.StopMusic();
                musfl = false;
            }
            Input input = new Input();
            ticks--;
            timer1.Interval = 1000;
            if (ticks < 0)
            {
                timer1.Interval = 1;
                label1.Visible = false;
                textBox1.ReadOnly = false;
                label2.Visible = true;
                label3.Visible = true;
                label4.Visible = false;
                timer2.Start();
                button2.Visible = false;


            }
            label1.Text = ticks.ToString();
            ChangeClr1(TextPanel, input);
            CheckOnEnd(input);
        }
        //метод для тіка другого таймера(таймер під час гри)
        private void timer2_Tick(object sender, EventArgs e)
        {
            ticks1++;
            timer2.Interval = 1000;
            label2.Text = ticks1.ToString();
        }
        //початок гри
        private void button1_Click(object sender, EventArgs e)
        {
            button1.Visible = false;
            button2.Visible = true;
            nameBox.Visible = false;
            nameLabel.Visible = false;
            label4.Visible = true;
            timer1.Start();
            GenerateSent();
            TextPanel.Text = textToPrnt.TextTPrint;
            if (nameBox.Text == "") name.Text = "Unknown Player";
            else name.Text = nameBox.Text;
        }
        //метод для перевірки правильності вводу
        void ChangeClr1(RichTextBox TextPanel, Input input)
        {
            input.Text = textBox1.Text;
            int lng = input.Text.Length;
            string sub = "";
            try
            {
                fl1 = true;
                sub = textToPrnt.TextTPrint.Substring(0, lng);
            }
            catch
            {
                timer1.Enabled = false;
                timer2.Enabled = false;
                MessageBox.Show("Too much symbols!");
                TextPanel.Select(0, textToPrnt.TextTPrint.Length);
                TextPanel.SelectionColor = Color.Black;
                textBox1.Text = null;
                ticks1 = 0;
            }
            if (fl1)
            {
                timer1.Enabled = true;
                timer2.Enabled = true;
            }
            string text2 = input.Text;
            if (sub == input.Text)
            {              
                foreach (char c in sub)
                {
                    input.wordcount++;

                }
                if (lng1 < lng)
                {
                    colfl = true;
                }
                else if (lng < lng1)
                {
                    colfl1 = true;
                }
                if (input.Text.Length >= 0 && colfl1 == true)
                {
                    vid--;
                    lng1 = lng;
                    colfl1 = false;
                    TextPanel.Select(lng1, 1);
                    TextPanel.SelectionColor = Color.Black;
                }
                if (input.Text.Length > 0 && colfl == true)
                {
                        lng1 = lng;
                        colfl = false;
                        TextPanel.Select(vid + 1, 1);
                        TextPanel.SelectionColor = Color.WhiteSmoke;
                        vid++; 
                }
                TextPanel.BackColor = Color.LightGreen;
            }

            else if (sub != input.Text)
            {
                TextPanel.BackColor = Color.LightCoral;

                if (text1 != text2 && length > lng)
                {
                    misc++;
                }
            }
            text1 = input.Text;
            length = lng;
        }
        //метод для перевірки кінця гри
        void CheckOnEnd(Input input)
        {
            input.Text = textBox1.Text;
            if (input.Text.Length == textToPrnt.TextTPrint.Length
                && input.Text == textToPrnt.TextTPrint
                && flag == true)
            {
                flag = false;
                timer1.Stop();
                timer2.Stop();
                input.Time = ticks1;
                Print(input);
            }
        }
        //метод для для обчислення значень wpm та accuracy та виводу інформації
        void Print(Input input)
        {
            if (misc > 0)
                misc++;
            double correlem = textToPrnt.TextTPrint.Length - misc;
            input.acc = (correlem / textToPrnt.TextTPrint.Length) * 100.0;
            double wpm = (((input.wordcount / 5) / input.Time) * 60);
            CheckOnRecord(wpm);
            MessageBox.Show($"Швидкість,слів в хвилину(WPM):{wpm:f0}\nТочність(accuracy)={input.acc:f1}%");
            Form1 form = new Form1();
            this.Hide();
            form.Show();
        }
        //метод для обробки результату та запису у файл
        public void CheckOnRecord(double wpm)
        {
            List<string> list = new List<string>();
            bool swap = false;
            int number;
            Result result = new Result((double)wpm, DateTime.Now);
            lines.CopyTo(linesToWrite, 0);
            for (int i = 0; i < linesToWrite.Length; i++)
            {
                number = 0;
                if (!string.IsNullOrEmpty(linesToWrite[i]))
                {
                    string[] words = linesToWrite[i].Split(' ');
                    number = int.Parse(words[0]);
                }
                if (swap)
                {
                    list.Add(linesToWrite[i - 1]);
                }
                if (result.wpmRes > number && !swap)
                {
                    swap = true;
                    list.Add($"{(int)result.wpmRes} wpm, Player:{name.Text} {result.date.ToShortDateString()}");
                }
                else if (!swap)
                {
                    list.Add(linesToWrite[i]);
                }
            }
            File.WriteAllLines("records.txt", list);
        }
        //зміна тексту для вводу
        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            timer1.Start();
            GenerateSent();
            TextPanel.Text = textToPrnt.TextTPrint;
        }
        //метод для генерації тексту для вводу
        void GenerateSent()
        {
            string[] readText = File.ReadAllLines("text.txt");
            Random random = new Random();
            int i = random.Next(readText.Length);
            textToPrnt.TextTPrint = readText[i];
        }
        //в меню
        private void ReturnToMenu_Click(object sender, EventArgs e)
        {
            music.StopMusic();
            Form3 form3 = new Form3();
            this.Hide();
            form3.Show();
        }
    }
}


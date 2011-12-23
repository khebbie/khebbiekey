using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace OnTopWinApp
{
    public partial class Form1 : Form
    {
        private bool _atLaunch;
        private HotKeyManager _hkm;
        private List<string> _keyList;
        private Dictionary<string, string> keys;
        private string _previousTitle = string.Empty;

        public Form1()
        {
            InitializeComponent();
            SetupForm();
            SetupHotKey();
            var formState = new FormState(this, "KhebbieKeys");

            SetupKeys();

            SetupTimer();
        }

        [DllImport("user32.dll")]
        private static extern int GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowText(int hWnd, StringBuilder text, int count);

        /// <summary>
        /// At program launch sets the program to not visible
        ///
        /// Otherwise it toggles visibility
        /// </summary>
        /// <param name="id">not used</param>
        public void Handler(int id)
        {
            if (_atLaunch)
            {
                Opacity = 100;
                Visible = true;
                _atLaunch = false;
            }
            Visible = !Visible;
            if (Visible)
            {
                //this.Activate();
            }
        }

        private void SetupForm()
        {
            AutoScaleBaseSize = new Size(6, 17);
            BackColor = Color.AntiqueWhite;

            ControlBox = false;
            Font = new Font("Comic Sans MS", 9F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            ForeColor = Color.Coral;
            FormBorderStyle = FormBorderStyle.Sizable;

            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;

            //this.TransparencyKey = System.Drawing.Color.Coral;
            ResumeLayout(false);
        }

        private void SetupHotKey()
        {
            _hkm = new HotKeyManager();
            _hkm.Register(Keys.K, HotKeyModifier.Windows, 100, Handler);
            _atLaunch = true;
        }

        private void SetupTimer()
        {
            timer1 = new Timer {Interval = 1000};
            timer1.Start();
            timer1.Tick += timer1_Tick;
        }

        private void SetupKeys()
        {
            keys = new Dictionary<string, string>();
            _keyList = new List<string>();
            ReadXML();
        }

        private void AddKey(string key, string site)
        {
            _keyList.Add(key.ToLower());
            keys.Add(key.ToLower(), site);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string title = GetActiveWindowTitle();

            //If this is the same window as last tick do nothing
            if (title == _previousTitle)
            {
                return;
            }
            label1.Text = title;

            if (title.ToLower().Contains("khebbie key"))
            {
                return;
            }

            foreach (string key in _keyList)
            {
                if (title.ToLower().Contains(key))
                {
                    webBrowser1.Navigate(keys[key]);
                    return;
                }
            }
            _previousTitle = title;
        }

        private string GetActiveWindowTitle()
        {
            string title = string.Empty;
            const int nChars = 256;
            int handle = 0;
            var Buff = new StringBuilder(nChars);

            handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                title = Buff.ToString();
            }
            return title;
        }

        private void ReadXML()
        {
            XmlTextReader reader = null;
            try
            {
                reader = new XmlTextReader(@"C:\KhebbieKey\khebbiekeys.xml");

                reader.WhitespaceHandling = WhitespaceHandling.None;
                var xmlDoc = new XmlDocument();
                //Load the file into the XmlDocument
                xmlDoc.Load(reader);
                XmlNode xnod = xmlDoc.DocumentElement;
                foreach (XmlNode node in xnod.ChildNodes)
                {
                    try
                    {
                        string key = node.ChildNodes[0].InnerText;
                        string value = node.ChildNodes[1].InnerText;
                        AddKey(key, value);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        continue;
                    }
                }
            }
            finally
            {
                reader.Close();
            }
        }
    }
}
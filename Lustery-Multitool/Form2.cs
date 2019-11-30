using System;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using MaterialSkin;
using MaterialSkin.Controls;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.ComponentModel;

namespace Lustery_Multitool
{
    public partial class Form2 : MaterialForm
    {
        string gvideo = null;
        public Form2(string video)
        {
            InitializeComponent();
            gvideo = video;
            #region Set theme on load
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Pink400, Primary.Pink500, Primary.Pink600, Accent.Pink200, TextShade.WHITE);
            #endregion
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.uiMode = "full";
            axWindowsMediaPlayer1.enableContextMenu = false;
            axWindowsMediaPlayer1.URL = gvideo;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Mime;
using System.Diagnostics;
using System.Threading;
using System.Web;
using System.IO.Compression;
using System.Globalization;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

using NUnrar;
using NUnrar.Archive;
using NUnrar.Common;
using Stock_Car_Extreme_Launcher_3;

namespace Stock_Car_Extreme_Launcher_3
{
    public partial class Form1 : Form
    {
        static string modInstallInfoFile = "installLog.xml";
        static string modInstallInfoFileFullPath = Application.StartupPath + "\\Packages\\" + modInstallInfoFile;
        static string remoteDBFile = "https://www.dropbox.com/s/swlhx1vtlh5s6x8/RDFiles.xml?dl=1";
        static string localDBFile = Application.StartupPath + "\\Packages\\" + "DBFile.xml";
        public static string userData = Application.StartupPath + @"\UserData\";
        public static string gameInstallDir = Application.StartupPath;
        public static string configFile = "SCEL.xml";

        List<Mod> modTrackList = new List<Mod>();
        List<Mod> modCarList = new List<Mod>();
        List<Mod> modSkinList = new List<Mod>();
        List<Mod> modMiscList = new List<Mod>();
        List<Mod> extractList = new List<Mod>();
        List<News> newsList = new List<News>();
        List<QMod> downloadQueue = new List<QMod>();
        List<Player> players = new List<Player>();

        WebClient dwc = new WebClient();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!File.Exists(modInstallInfoFileFullPath))
                CreateInstallLogFile();
            newsPage.BringToFront();
            Task.Run(() => GetModList());
            Task.Run(() => GetNews());
            Task.Run(() => readPLRprofiles());
        }

        private void RDButton_Click(object sender, EventArgs e)
        {
            newsPage.BringToFront();
        }

        private void ModButton_Click(object sender, EventArgs e)
        {
            tabControl1.BringToFront();
        }

        private void OptionsButton_Click(object sender, EventArgs e)
        {
            optionsPage.BringToFront();
        }

        private void SPButton_Click(object sender, EventArgs e)
        {
            try
            {
                savePLRFile(playerComboBox.GetItemText(playerComboBox.SelectedItem));
                ProcessStartInfo GSC = new ProcessStartInfo();
                GSC.FileName = "GSC.exe";
                GSC.Arguments = parametersTextBox.Text.Replace("\"", "\\\"");
                Process.Start(GSC);
            }
            catch
            {
                MessageBox.Show("Can not find 'GSC.exe'. Make sure you run this launcher from the root of your Stock Car Extreme folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SyncButton_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("GSC Sync.exe");
            }
            catch
            {
                MessageBox.Show("Can not find 'GSC Sync.exe'. Make sure you run this launcher from the root of your Stock Car Extreme folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ServerButton_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("GSC Dedicated.exe");
            }
            catch
            {
                MessageBox.Show("Can not find 'GSC Dedicated.exe'. Make sure you run this launcher from the root of your Stock Car Extreme folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("https://www.paypal.com/au/cgi-bin/webscr?cmd=_xclick&business=martin.vindis.80@gmail.com&item_name=rFactor%20Yet%20Another%20Mod%20Launcher&currenty_code=&amount=0");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }



        private void videoSettingsButton_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("GSC Config.exe");
            }
            catch
            {
                MessageBox.Show("Can not find 'GSC Config.exe'. Make sure you run this launcher from the root of your Stock Car Extreme folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void savePLRButton_Click(object sender, EventArgs e)
        {
            savePLRFile(playerComboBox.GetItemText(playerComboBox.SelectedItem));
        }

        private void maxFramerateTrackBar_Scroll(object sender, EventArgs e)
        {
            maxFramerateLabel.Text = maxFramerateTrackBar.Value.ToString();
        }

        private void maxHeadlightsTrackBar_Scroll(object sender, EventArgs e)
        {
            maxHeadlightsLabel.Text = maxHeadlightsTrackBar.Value.ToString();
        }

        private void maxVisibleCarsTrackBar_Scroll(object sender, EventArgs e)
        {
            maxVisibleCarsLabel.Text = maxVisibleCarsTrackBar.Value.ToString();
        }

        private void rearviewFOVTrackBar_Scroll(object sender, EventArgs e)
        {
            rearviewFOVLabel.Text = rearviewFOVTrackBar.Value.ToString();
        }

        private void playerComboBox_MouseDown(object sender, MouseEventArgs e)
        {
            savePlayer((Player)playerComboBox.SelectedItem);
        }

        private void playerComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            playerComboBox.Invoke((MethodInvoker)delegate { setOptions((Player)playerComboBox.SelectedItem); });
        }

        public void savePlayer (Player player)
        {
            //AutoLift  
            player.AutoLift = autoLiftCheckBox.Checked ? "1" : "0";

            //AutoBlip  
            player.AutoBlip = autoBlipCheckBox.Checked ? "1" : "0";

            //VirtualRearviewInCockpit  
            player.VirtualRearviewInCockpit = virtualMirrorsCheckBox.Checked ? "1" : "0";

            //MeasurementUnits  
            player.MeasurementUnits = measurementUnitsCheckBox.Checked ? "0" : "1";

            //SpeedUnits  
            player.SpeedUnits = speedUnitsCheckBox.Checked ? "1" : "0";

            //DamperUnits  
            player.DamperUnits = damperUnitsCheckBox.Checked ? "1" : "0";

            //RelativeFuelStrategy  
            player.RelativeFuelStrategy = relativeFuelCheckBox.Checked ? "1" : "0";

            //PitStopDescription  
            player.PitStopDescription = pitstopDescriptionCheckBox.Checked ? "1" : "0";

            //SmartPitcrew  
            player.SmartPitcrew = smartPitcrewCheckBox.Checked ? "1" : "0";

            //AutoCalibrateAIMode  
            player.AutoCalibrateAIMode = autocalibrateAICheckBox.Checked ? "1" : "0";

            //HeadlightsOnCars  
            player.HeadlightsOnCars = headlightsOnCarsCheckBox.Checked ? "1" : "0";

            //RearviewCull  
            player.RearviewCull = rearviewCullCheckBox.Checked ? "1" : "0";

            //MaxFramerate  
            player.MaxFramerate = maxFramerateLabel.Text;

            //MaxHeadlights  
            player.MaxHeadlights = maxHeadlightsLabel.Text;

            //MaxVisibleCars  
            player.MaxVisibleCars = maxVisibleCarsLabel.Text;

            //RearviewFOV  
            player.RearviewFOV = rearviewFOVLabel.Text;

            //Control  
            player.Control = controllerComboBox.GetItemText(controllerComboBox.SelectedItem);

            //SelfInRearview  
            int x = 0;
            if (selfInRearviewCheckBox1.Checked) x += 1;
            if (selfInRearviewCheckBox1.Checked) x += 2;
            if (selfInRearviewCheckBox1.Checked) x += 4;
            if (selfInRearviewCheckBox1.Checked) x += 8;
            player.SelfInRearview = x.ToString();

            //KeepReceivedSetups  
            x = 0;
            if (keepReceivedSetupsRadioButton0.Checked) x = 0;
            else if (keepReceivedSetupsRadioButton1.Checked) x = 1;
            else if (keepReceivedSetupsRadioButton2.Checked) x = 2;
            else if (keepReceivedSetupsRadioButton3.Checked) x = 3;
            player.KeepReceivedSetups = x.ToString();

            //MovingRearview  
            x = 0;
            if (movingRearviewCheckBox1.Checked) x += 1;
            if (movingRearviewCheckBox2.Checked) x += 2;
            if (movingRearviewCheckBox3.Checked) x += 4;
            player.MovingRearview = x.ToString();
        }

        public void setOptions(Player player)
        {
            //Auto Lift
            if (player.AutoLift == "1")
                autoLiftCheckBox.Checked = true;
            else
                autoLiftCheckBox.Checked = false;

            //Auto Blip
            if (player.AutoBlip == "1")
                autoBlipCheckBox.Checked = true;
            else
                autoBlipCheckBox.Checked = false;

            //Virtual Rearview
            if (player.VirtualRearviewInCockpit == "1")
                virtualMirrorsCheckBox.Checked = true;
            else
                virtualMirrorsCheckBox.Checked = false;

            //Measurement Units
            if (player.MeasurementUnits == "0")
                measurementUnitsCheckBox.Checked = true;
            else
                measurementUnitsCheckBox.Checked = false;

            //Speed Units
            if (player.SpeedUnits == "1")
                speedUnitsCheckBox.Checked = true;
            else
                speedUnitsCheckBox.Checked = false;

            //Damper Units
            if (player.DamperUnits == "1")
                damperUnitsCheckBox.Checked = true;
            else
                damperUnitsCheckBox.Checked = false;

            //Relative Fuel
            if (player.RelativeFuelStrategy == "1")
                relativeFuelCheckBox.Checked = true;
            else
                relativeFuelCheckBox.Checked = false;

            //Pitstop Description
            if (player.PitStopDescription == "1")
                pitstopDescriptionCheckBox.Checked = true;
            else
                pitstopDescriptionCheckBox.Checked = false;

            //Smart Pitcrew
            if (player.SmartPitcrew == "1")
                smartPitcrewCheckBox.Checked = true;
            else
                smartPitcrewCheckBox.Checked = false;

            //Autocalibrate AI
            if (player.AutoCalibrateAIMode == "1")
                autocalibrateAICheckBox.Checked = true;
            else
                autocalibrateAICheckBox.Checked = false;

            //Headlights On Cars
            if (player.HeadlightsOnCars == "1")
                headlightsOnCarsCheckBox.Checked = true;
            else
                headlightsOnCarsCheckBox.Checked = false;

            //Rearview Cull
            if (player.RearviewCull == "1")
                rearviewCullCheckBox.Checked = true;
            else
                rearviewCullCheckBox.Checked = false;


            //Max Framerate
            maxFramerateTrackBar.Value = Convert.ToInt32(player.MaxFramerate);
            maxFramerateLabel.Text = player.MaxFramerate;

            //Max Headlights
            maxHeadlightsTrackBar.Value = Convert.ToInt32(player.MaxHeadlights);
            maxHeadlightsLabel.Text = player.MaxHeadlights;

            //Max Visible Cars
            maxVisibleCarsTrackBar.Value = Convert.ToInt32(player.MaxVisibleCars);
            maxVisibleCarsLabel.Text = player.MaxVisibleCars;

            //Rearview FOV
            if (player.RearviewFOV.Contains("."))
                player.RearviewFOV = player.RearviewFOV.Substring(0, player.RearviewFOV.IndexOf("."));
            rearviewFOVTrackBar.Value = Convert.ToInt32(player.RearviewFOV);
            rearviewFOVLabel.Text = player.RearviewFOV;


            //Self In Rearview
            switch(player.SelfInRearview)
            {
                case "1":
                    selfInRearviewCheckBox1.Checked = true;
                    selfInRearviewCheckBox2.Checked = false;
                    selfInRearviewCheckBox3.Checked = false;
                    selfInRearviewCheckBox4.Checked = false;
                    break;
                case "2":
                    selfInRearviewCheckBox1.Checked = false;
                    selfInRearviewCheckBox2.Checked = true;
                    selfInRearviewCheckBox3.Checked = false;
                    selfInRearviewCheckBox4.Checked = false;
                    break;
                case "3":
                    selfInRearviewCheckBox1.Checked = true;
                    selfInRearviewCheckBox2.Checked = true;
                    selfInRearviewCheckBox3.Checked = false;
                    selfInRearviewCheckBox4.Checked = false;
                    break;
                case "4":
                    selfInRearviewCheckBox1.Checked = false;
                    selfInRearviewCheckBox2.Checked = false;
                    selfInRearviewCheckBox3.Checked = true;
                    selfInRearviewCheckBox4.Checked = false;
                    break;
                case "5":
                    selfInRearviewCheckBox1.Checked = true;
                    selfInRearviewCheckBox2.Checked = false;
                    selfInRearviewCheckBox3.Checked = true;
                    selfInRearviewCheckBox4.Checked = false;
                    break;
                case "6":
                    selfInRearviewCheckBox1.Checked = false;
                    selfInRearviewCheckBox2.Checked = true;
                    selfInRearviewCheckBox3.Checked = true;
                    selfInRearviewCheckBox4.Checked = false;
                    break;
                case "7":
                    selfInRearviewCheckBox1.Checked = true;
                    selfInRearviewCheckBox2.Checked = true;
                    selfInRearviewCheckBox3.Checked = true;
                    selfInRearviewCheckBox4.Checked = false;
                    break;
                case "8":
                    selfInRearviewCheckBox1.Checked = false;
                    selfInRearviewCheckBox2.Checked = false;
                    selfInRearviewCheckBox3.Checked = false;
                    selfInRearviewCheckBox4.Checked = true;
                    break;
                case "9":
                    selfInRearviewCheckBox1.Checked = true;
                    selfInRearviewCheckBox2.Checked = false;
                    selfInRearviewCheckBox3.Checked = false;
                    selfInRearviewCheckBox4.Checked = true;
                    break;
                case "10":
                    selfInRearviewCheckBox1.Checked = false;
                    selfInRearviewCheckBox2.Checked = true;
                    selfInRearviewCheckBox3.Checked = false;
                    selfInRearviewCheckBox4.Checked = true;
                    break;
                case "11":
                    selfInRearviewCheckBox1.Checked = true;
                    selfInRearviewCheckBox2.Checked = true;
                    selfInRearviewCheckBox3.Checked = false;
                    selfInRearviewCheckBox4.Checked = true;
                    break;
                case "12":
                    selfInRearviewCheckBox1.Checked = false;
                    selfInRearviewCheckBox2.Checked = false;
                    selfInRearviewCheckBox3.Checked = true;
                    selfInRearviewCheckBox4.Checked = true;
                    break;
                case "13":
                    selfInRearviewCheckBox1.Checked = true;
                    selfInRearviewCheckBox2.Checked = false;
                    selfInRearviewCheckBox3.Checked = true;
                    selfInRearviewCheckBox4.Checked = true;
                    break;
                case "14":
                    selfInRearviewCheckBox1.Checked = false;
                    selfInRearviewCheckBox2.Checked = true;
                    selfInRearviewCheckBox3.Checked = true;
                    selfInRearviewCheckBox4.Checked = true;
                    break;
                case "15":
                    selfInRearviewCheckBox1.Checked = true;
                    selfInRearviewCheckBox2.Checked = true;
                    selfInRearviewCheckBox3.Checked = true;
                    selfInRearviewCheckBox4.Checked = true;
                    break;
                default:
                    selfInRearviewCheckBox1.Checked = false;
                    selfInRearviewCheckBox2.Checked = false;
                    selfInRearviewCheckBox3.Checked = false;
                    selfInRearviewCheckBox4.Checked = false;
                    break;
            }

            //Keep Received Setups
            switch(player.KeepReceivedSetups)
            {
                case "1":
                    keepReceivedSetupsRadioButton1.Checked = true;
                    break;
                case "2":
                    keepReceivedSetupsRadioButton2.Checked = true;
                    break;
                case "3":
                    keepReceivedSetupsRadioButton3.Checked = true;
                    break;
                default:
                    keepReceivedSetupsRadioButton0.Checked = true;
                    break;
            }

            //Moving Rearview
            switch(player.MovingRearview)
            {
                case "1":
                    movingRearviewCheckBox1.Checked = true;
                    movingRearviewCheckBox2.Checked = false;
                    movingRearviewCheckBox3.Checked = false;
                    break;
                case "2":
                    movingRearviewCheckBox1.Checked = false;
                    movingRearviewCheckBox2.Checked = true;
                    movingRearviewCheckBox3.Checked = false;
                    break;
                case "3":
                    movingRearviewCheckBox1.Checked = true;
                    movingRearviewCheckBox2.Checked = true;
                    movingRearviewCheckBox3.Checked = false;
                    break;
                case "4":
                    movingRearviewCheckBox1.Checked = false;
                    movingRearviewCheckBox2.Checked = false;
                    movingRearviewCheckBox3.Checked = true;
                    break;
                case "5":
                    movingRearviewCheckBox1.Checked = true;
                    movingRearviewCheckBox2.Checked = false;
                    movingRearviewCheckBox3.Checked = true;
                    break;
                case "6":
                    movingRearviewCheckBox1.Checked = false;
                    movingRearviewCheckBox2.Checked = true;
                    movingRearviewCheckBox3.Checked = true;
                    break;
                case "7":
                    movingRearviewCheckBox1.Checked = true;
                    movingRearviewCheckBox2.Checked = true;
                    movingRearviewCheckBox3.Checked = true;
                    break;
                default:
                    movingRearviewCheckBox1.Checked = false;
                    movingRearviewCheckBox2.Checked = false;
                    movingRearviewCheckBox3.Checked = false;
                    break;
            }

            //Controller
            controllerComboBox.SelectedIndex = controllerComboBox.FindStringExact(player.Control);

            //Auto Close Launcher 
            //Parameters
        }

        public void savePLRFile(string name)
        {
            string plrpath = userData + name + @"\" + name + ".PLR";
            string text = File.ReadAllText(plrpath);
            int x;
            float y;

            NumberFormatInfo nf = new CultureInfo("en-US", false).NumberFormat;
            nf.NumberDecimalSeparator = ".";

            //Rearview FOV
            text = Regex.Replace(text, @"^(Rearview Height="")(.+?)("")", "Rearview Height=\"" + rearviewFOVLabel.Text + "\"", RegexOptions.Multiline);

            //Max Headlights
            text = Regex.Replace(text, @"^(Max Headlights="")(\d+)("")", "Max Headlights=\"" + maxHeadlightsLabel.Text + "\"", RegexOptions.Multiline);

            //Max Visible Cars
            text = Regex.Replace(text, @"^(Max Visible Vehicles="")(\d+)("")", "Max Visible Vehicles=\"" + maxVisibleCarsLabel.Text + "\"", RegexOptions.Multiline);

            //Max Framerate
            text = Regex.Replace(text, @"^(Max Framerate="")-?(\d+)("")", "Max Framerate=\"" + maxFramerateTrackBar.Value * -1 + "\"", RegexOptions.Multiline);

            //Auto Lift
            if (autoLiftCheckBox.Checked)
                x = 1;
            else
                x = 0;
            text = Regex.Replace(text, @"^(Auto Lift="")(\d+)("")", "Auto Lift=\"" + x + "\"", RegexOptions.Multiline);

            //Auto Blip
            if (autoBlipCheckBox.Checked)
                x = 1;
            else
                x = 0;
            text = Regex.Replace(text, @"^(Auto Blip="")(\d+)("")", "Auto Blip=\"" + x + "\"", RegexOptions.Multiline);

            //Mirrors
            if (virtualMirrorsCheckBox.Checked)
                x = 1;
            else
                x = 0;
            text = Regex.Replace(text, @"^(Virtual Rearview In Cockpit="")(\d+)("")", "Virtual Rearview In Cockpit=\"" + x + "\"", RegexOptions.Multiline);

            //Relative Fuel
            if (relativeFuelCheckBox.Checked)
                x = 1;
            else
                x = 0;
            text = Regex.Replace(text, @"^(Relative Fuel Strategy="")(\d+)("")", "Relative Fuel Strategy=\"" + x + "\"", RegexOptions.Multiline);

            //Measurement Units
            if (measurementUnitsCheckBox.Checked)
                x = 0;
            else
                x = 1;
            text = Regex.Replace(text, @"^(Measurement Units="")(\d+)("")", "Measurement Units=\"" + x + "\"", RegexOptions.Multiline);

            //Speed Units
            if (speedUnitsCheckBox.Checked)
                x = 1;
            else
                x = 0;
            text = Regex.Replace(text, @"^(Speed Units="")(\d+)("")", "Speed Units=\"" + x + "\"", RegexOptions.Multiline);

            //Damper Units
            if (damperUnitsCheckBox.Checked)
                x = 1;
            else
                x = 0;
            text = Regex.Replace(text, @"^(Damper Units="")(\d+)("")", "Damper Units=\"" + x + "\"", RegexOptions.Multiline);

            //Pitstop Description
            if (pitstopDescriptionCheckBox.Checked)
                x = 1;
            else
                x = 0;
            text = Regex.Replace(text, @"^(Pitstop Description="")(\d+)("")", "Pitstop Description=\"" + x + "\"", RegexOptions.Multiline);

            //Smart Pitcrew
            if (smartPitcrewCheckBox.Checked)
                x = 1;
            else
                x = 0;
            text = Regex.Replace(text, @"^(Smart Pitcrew="")(\d+)("")", "Smart Pitcrew=\"" + x + "\"", RegexOptions.Multiline);

            //Autocalibrate AI
            if (autocalibrateAICheckBox.Checked)
                x = 1;
            else
                x = 0;
            text = Regex.Replace(text, @"^(Autocalibrate AI Mode="")(\d+)("")", "Autocalibrate AI Mode=\"" + x + "\"", RegexOptions.Multiline);

            //Headlights On Cars
            if (headlightsOnCarsCheckBox.Checked)
                x = 1;
            else
                x = 0;
            text = Regex.Replace(text, @"^(Headlights On Cars="")(\d+)("")", "Headlights On Cars=\"" + x + "\"", RegexOptions.Multiline);

            //Rearview Cull
            if (rearviewCullCheckBox.Checked)
                x = 1;
            else
                x = 0;
            text = Regex.Replace(text, @"^(Rearview Cull="")(\d+)("")", "Rearview Cull=\"" + x + "\"", RegexOptions.Multiline);

            //Self In Rearview
            x = 0;
            if (selfInRearviewCheckBox1.Checked)
                x += 1;
            if (selfInRearviewCheckBox2.Checked)
                x += 2;
            if (selfInRearviewCheckBox3.Checked)
                x += 4;
            if (selfInRearviewCheckBox4.Checked)
                x += 8;
            text = Regex.Replace(text, @"^(Self In Cockpit Rearview="")(\d+)("")", "Self In Cockpit Rearview=\"" + x + "\"", RegexOptions.Multiline);

            //Keep Received Setups
            if (keepReceivedSetupsRadioButton0.Checked)
                x = 0;
            else if (keepReceivedSetupsRadioButton1.Checked)
                x = 1;
            else if (keepReceivedSetupsRadioButton2.Checked)
                x = 2;
            else
                x = 3;
            text = Regex.Replace(text, @"^(Keep Received Setups="")(\d+)("")", "Keep Received Setups=\"" + x + "\"", RegexOptions.Multiline);

            //Moving Rearview
            x = 0;
            if (movingRearviewCheckBox1.Checked)
                x += 1;
            if (movingRearviewCheckBox2.Checked)
                x += 2;
            if (movingRearviewCheckBox3.Checked)
                x += 4;
            text = Regex.Replace(text, @"^(Moving Rearview="")(\d+)("")", "Moving Rearview=\"" + x + "\"", RegexOptions.Multiline);

            //Controller
            text = Regex.Replace(text, @"^(Current Control File="")(\d+)("")", "Current Control File=\"" + controllerComboBox.GetItemText(controllerComboBox.SelectedItem) + "\"", RegexOptions.Multiline);

            File.WriteAllText(plrpath, text);
        }

        public void readPLRprofiles()
        {
            try
            {
                playerComboBox.Invoke((MethodInvoker)delegate { playerComboBox.BeginUpdate(); });
                playerComboBox.Invoke((MethodInvoker)delegate { playerComboBox.Items.Clear(); });
                foreach (string subDir in Directory.GetDirectories(userData))
                {
                    foreach (string file in Directory.GetFiles(subDir, "*.PLR"))
                    {
                        string name = Path.GetFileNameWithoutExtension(file);
                        players.Add(new Player(name) { });
                    }
                }
                foreach (Player player in players)
                    playerComboBox.Invoke((MethodInvoker)delegate { playerComboBox.Items.Add(player); });
                playerComboBox.Invoke((MethodInvoker)delegate { playerComboBox.SelectedIndex = 0; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            playerComboBox.Invoke((MethodInvoker)delegate { playerComboBox.EndUpdate(); });
            readControllerFiles();
        }

        public void readControllerFiles()
        {
            try
            {
                controllerComboBox.Invoke((MethodInvoker)delegate { controllerComboBox.BeginUpdate(); });
                controllerComboBox.Invoke((MethodInvoker)delegate { controllerComboBox.Items.Clear(); });
                controllerComboBox.Invoke((MethodInvoker)delegate { controllerComboBox.Items.Add("Controller"); });
                foreach (string file in Directory.GetFiles(userData + @"Controller\", "*.ini"))
                {
                    string name = Path.GetFileNameWithoutExtension(file);
                    controllerComboBox.Invoke((MethodInvoker)delegate { controllerComboBox.Items.Add(name); });
                }
                Player player;
                int index = 0;
                playerComboBox.Invoke((MethodInvoker)delegate { player = (Player)playerComboBox.SelectedItem; index = controllerComboBox.FindStringExact(player.Control); });
                controllerComboBox.Invoke((MethodInvoker)delegate { controllerComboBox.SelectedIndex = index; });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            controllerComboBox.Invoke((MethodInvoker)delegate { controllerComboBox.EndUpdate(); });
        }

        public void GetNews()
        {
            string url = "http://www.racedepartment.com/forums/stock-car-extreme.77/";

            WebClient wc = new WebClient();
            string html = wc.DownloadString(url);
            MatchCollection mName = Regex.Matches(html, "<h3 class=\"title\">\\s*(.+?)<div class=\"listBlock stats pairsJustified\"", RegexOptions.Singleline);

            foreach (Match m in mName)
            {
                News news = new News();
                string substring = m.Groups[1].Value;
                string pat;
                Regex r;
                Match submatch;

                pat = ">(.+?)</a>";
                r = new Regex(pat, RegexOptions.Singleline);
                submatch = r.Match(substring);
                news.title = submatch.Groups[1].Value;
                news.title = HttpUtility.HtmlDecode(news.title);
                if (news.title.Length > 70)
                {
                    news.title = news.title.Substring(0, 70);
                    news.title += "...";
                }

                pat = "\"Thread starter\">(.+?)</a>";
                r = new Regex(pat, RegexOptions.Singleline);
                submatch = r.Match(substring);
                news.author = submatch.Groups[1].Value;

                pat = "([a-zA-Z]{3}\\s[0-9]{1,2},\\s[0-9]{4})";
                r = new Regex(pat, RegexOptions.Singleline);
                submatch = r.Match(substring);
                news.date = Convert.ToDateTime(submatch.Groups[1].Value);

                pat = "<a href=\"(.+?)\"";
                r = new Regex(pat, RegexOptions.Singleline);
                submatch = r.Match(substring);
                news.link = "http://www.racedepartment.com/" + submatch.Groups[1].Value;

                newsList.Add(news);
            }
            PopulateNews();
        }

        private void GetModList()
        {
            WebClient wc = new WebClient();
            wc.DownloadFile(remoteDBFile, localDBFile);
            XDocument xDoc = XDocument.Load(localDBFile);

            foreach (XElement xel in xDoc.Root.Element("cars").Elements())
            {
                Mod mod = new Mod();
                mod.name = xel.Element("name").Value;
                mod.version = xel.Element("version").Value;
                mod.author = xel.Element("author").Value;
                mod.downloadLink = xel.Element("downloadLink").Value;
                mod.fileName = xel.Element("fileName").Value;
                mod.size = xel.Element("size").Value;
                mod.date = xel.Element("date").Value;

                if (mod.size == "")
                    mod.size = "<1.0";
                modCarList.Add(mod);
            }

            foreach(XElement xel in xDoc.Root.Element("tracks").Elements())
            {
                Mod mod = new Mod();
                mod.name = xel.Element("name").Value;
                mod.version = xel.Element("version").Value;
                mod.author = xel.Element("author").Value;
                mod.downloadLink = xel.Element("downloadLink").Value;
                mod.fileName = xel.Element("fileName").Value;
                mod.size = xel.Element("size").Value;
                mod.date = xel.Element("date").Value;

                if (mod.size == "")
                    mod.size = "<1.0";
                modTrackList.Add(mod);
            }

            foreach (XElement xel in xDoc.Root.Element("skins").Elements())
            {
                Mod mod = new Mod();
                mod.name = xel.Element("name").Value;
                mod.version = xel.Element("version").Value;
                mod.author = xel.Element("author").Value;
                mod.downloadLink = xel.Element("downloadLink").Value;
                mod.fileName = xel.Element("fileName").Value;
                mod.size = xel.Element("size").Value;
                mod.date = xel.Element("date").Value;

                if (mod.size == "")
                    mod.size = "<1.0";
                modSkinList.Add(mod);
            }

            foreach (XElement xel in xDoc.Root.Element("misc").Elements())
            {
                Mod mod = new Mod();
                mod.name = xel.Element("name").Value;
                mod.version = xel.Element("version").Value;
                mod.author = xel.Element("author").Value;
                mod.downloadLink = xel.Element("downloadLink").Value;
                mod.fileName = xel.Element("fileName").Value;
                mod.size = xel.Element("size").Value;
                mod.date = xel.Element("date").Value;

                if (mod.size == "")
                    mod.size = "<1.0";
                modMiscList.Add(mod);
            }
            PopulateMods();
        }

        // Populate news panel 
        public void PopulateNews()
        {
            newsPage.Invoke((MethodInvoker)delegate { newsPage.Controls.Remove(loadingNewsLabel); });

            int news = 0;
            foreach (News n in newsList)
            {
                Panel newsPanel = new Panel();
                newsPanel.Size = new Size(865, 61);
                newsPanel.Location = new Point(-1, -1 + (newsPanel.Height - 1) * news);
                newsPanel.BorderStyle = BorderStyle.FixedSingle;
                newsPage.Invoke((MethodInvoker)delegate { newsPage.Controls.Add(newsPanel); });
                newsPanel.MouseHover += delegate { newsPage.Focus(); };

                Label newsTitleLabel = new Label();
                newsTitleLabel.Text = n.title.ToUpper();
                newsTitleLabel.ForeColor = Color.Black;
                newsTitleLabel.Location = new Point(0, 0);
                newsTitleLabel.Padding = new Padding(4, 10, 0, 0);
                newsTitleLabel.AutoSize = true;
                newsTitleLabel.Dock = DockStyle.Left;
                newsTitleLabel.Font = new Font("Arimo", 12, FontStyle.Bold);
                newsPanel.Invoke((MethodInvoker)delegate { newsPanel.Controls.Add(newsTitleLabel); });

                Label newsAuthorLabel = new Label();
                newsAuthorLabel.Text = n.author;
                newsAuthorLabel.ForeColor = Color.FromArgb(64, 64, 64);
                newsAuthorLabel.Location = new Point(5, newsPanel.Height - 20);
                newsAuthorLabel.AutoSize = true;
                newsAuthorLabel.Font = new Font("DejaVu Sans Condensed", 8, FontStyle.Regular);
                newsPanel.Invoke((MethodInvoker)delegate { newsPanel.Controls.Add(newsAuthorLabel); });

                Label newsDateLabel = new Label();
                newsDateLabel.Text = n.date.ToShortDateString();
                newsDateLabel.ForeColor = Color.FromArgb(64, 64, 64);
                newsDateLabel.Location = new Point(20 + newsAuthorLabel.Width, newsPanel.Height - 20);
                newsDateLabel.AutoSize = true;
                newsDateLabel.Font = new Font("DejaVu Sans Condensed", 8, FontStyle.Regular);
                newsPanel.Invoke((MethodInvoker)delegate { newsPanel.Controls.Add(newsDateLabel); });

                Button newsLinkButton = new Button();
                newsLinkButton.Size = new Size(100, 50);
                newsLinkButton.ForeColor = Color.Black;
                newsLinkButton.BackColor = Color.WhiteSmoke;
                newsLinkButton.UseVisualStyleBackColor = false;
                newsLinkButton.Dock = DockStyle.Right;
                newsLinkButton.Text = "Read more...";
                newsLinkButton.Font = new Font("DejaVu Sans Condensed", 10, FontStyle.Regular);
                newsPanel.Invoke((MethodInvoker)delegate { newsPanel.Controls.Add(newsLinkButton); });

                newsLinkButton.Click += delegate { Process.Start(n.link); };



                news++;
            }
        }

        // Populate mods panels
        public void PopulateMods()
        {
            List<string> modTypes = new List<string>() { "cars", "tracks", "skins", "misc" };

            foreach (string type in modTypes)
            {
                List<Mod> modList = new List<Mod>();
                int mods = 0;
                if (type == "cars")
                {
                    modList = modCarList;
                    carsTabPage.Invoke((MethodInvoker)delegate { carsTabPage.Controls.Remove(loadingCarsLabel); });
                    carsTabPage.Invoke((MethodInvoker)delegate { carsTabPage.Controls.Remove(loadingCarsProgressBar); });
                }
                else if (type == "tracks")
                {
                    modList = modTrackList;
                    tracksTabPage.Invoke((MethodInvoker)delegate { tracksTabPage.Controls.Remove(loadingTracksLabel); });
                    tracksTabPage.Invoke((MethodInvoker)delegate { tracksTabPage.Controls.Remove(loadingTracksProgressBar); });
                }
                else if (type == "skins")
                {
                    modList = modSkinList;
                    skinsTabPage.Invoke((MethodInvoker)delegate { skinsTabPage.Controls.Remove(loadingSkinsLabel); });
                    skinsTabPage.Invoke((MethodInvoker)delegate { skinsTabPage.Controls.Remove(loadingSkinsProgressBar); });
                }
                else if (type == "misc")
                {
                    modList = modMiscList;
                    miscTabPage.Invoke((MethodInvoker)delegate { miscTabPage.Controls.Remove(loadingMiscLabel); });
                    miscTabPage.Invoke((MethodInvoker)delegate { miscTabPage.Controls.Remove(loadingMiscProgressBar); });
                }

                foreach (Mod mod in modList)
                {
                    Panel modPanel = new Panel();
                    modPanel.Size = new Size(860, 61);
                    modPanel.Location = new Point(0, 1 + (modPanel.Height - 1) * mods);
                    modPanel.BorderStyle = BorderStyle.FixedSingle;
                    if (type == "cars")
                    {
                        carsTabPage.Invoke((MethodInvoker)delegate { carsTabPage.Controls.Add(modPanel); });
                        modPanel.MouseHover += delegate { carsTabPage.Focus(); };
                    }
                    else if (type == "tracks")
                    {
                        tracksTabPage.Invoke((MethodInvoker)delegate { tracksTabPage.Controls.Add(modPanel); });
                        modPanel.MouseHover += delegate { tracksTabPage.Focus(); };
                    }
                    else if (type == "skins")
                    {
                        skinsTabPage.Invoke((MethodInvoker)delegate { skinsTabPage.Controls.Add(modPanel); });
                        modPanel.MouseHover += delegate { skinsTabPage.Focus(); };
                    }
                    else if (type == "misc")
                    {
                        skinsTabPage.Invoke((MethodInvoker)delegate { miscTabPage.Controls.Add(modPanel); });
                        modPanel.MouseHover += delegate { miscTabPage.Focus(); };
                    }

                    Label modNameLabel = new Label();
                    modNameLabel.Text = mod.name.ToUpper();
                    modNameLabel.ForeColor = Color.Black;
                    modNameLabel.Location = new Point(0, 0);
                    modNameLabel.Padding = new Padding(4, 10, 0, 0);
                    modNameLabel.AutoSize = true;
                    modNameLabel.Dock = DockStyle.Left;
                    modNameLabel.Font = new Font("Arimo", 12, FontStyle.Bold);
                    modPanel.Invoke((MethodInvoker)delegate { modPanel.Controls.Add(modNameLabel); });

                    Label modVersionLabel = new Label();
                    modVersionLabel.Text = mod.version;
                    modVersionLabel.ForeColor = Color.FromArgb(64, 64, 64);
                    modVersionLabel.Location = new Point(modNameLabel.Width, 0);
                    modVersionLabel.Padding = new Padding(4, 10, 0, 0);
                    modVersionLabel.AutoSize = true;
                    modVersionLabel.Font = new Font("Arimo", 12, FontStyle.Regular);
                    modPanel.Invoke((MethodInvoker)delegate { modPanel.Controls.Add(modVersionLabel); });

                    Label modAuthorLabel = new Label();
                    modAuthorLabel.Text = mod.author;
                    modAuthorLabel.ForeColor = Color.FromArgb(64, 64, 64);
                    modAuthorLabel.Location = new Point(5, modPanel.Height - 20);
                    modAuthorLabel.AutoSize = true;
                    modAuthorLabel.Font = new Font("DejaVu Sans Condensed", 8, FontStyle.Regular);
                    modPanel.Invoke((MethodInvoker)delegate { modPanel.Controls.Add(modAuthorLabel); });

                    Label modDateLabel = new Label();
                    modDateLabel.Text = mod.date;
                    modDateLabel.ForeColor = Color.FromArgb(64, 64, 64);
                    modDateLabel.Location = new Point(20 + modAuthorLabel.Width, modPanel.Height - 20);
                    modDateLabel.AutoSize = true;
                    modDateLabel.Font = new Font("DejaVu Sans Condensed", 8, FontStyle.Regular);
                    modPanel.Invoke((MethodInvoker)delegate { modPanel.Controls.Add(modDateLabel); });

                    Button modDownloadButton = new Button();
                    modDownloadButton.Size = new Size(100, 52);
                    modDownloadButton.ForeColor = Color.Black;
                    modDownloadButton.BackColor = Color.WhiteSmoke;
                    modDownloadButton.UseVisualStyleBackColor = true;
                    modDownloadButton.Dock = DockStyle.Right;
                    modDownloadButton.Text = mod.size + " MB";
                    modDownloadButton.Font = new Font("DejaVu Sans Condensed", 10, FontStyle.Regular);
                    modPanel.Invoke((MethodInvoker)delegate { modPanel.Controls.Add(modDownloadButton); });

                    ProgressBar modProgressBar = new ProgressBar();
                    modProgressBar.Size = new Size(modDownloadButton.Width - 12, 8);
                    modProgressBar.Location = new Point(6, modDownloadButton.Height - modProgressBar.Height - 6);
                    modProgressBar.Visible = false;
                    modDownloadButton.Invoke((MethodInvoker)delegate { modDownloadButton.Controls.Add(modProgressBar); });
                    if (!File.Exists(Application.StartupPath + "\\Packages\\" + mod.fileName))
                    {
                        modDownloadButton.Click += delegate { Task.Run(() => DownloadFile(true, modDownloadButton, modProgressBar, mod, type)); };
                    }
                    else if (type == "cars" || type == "tracks")
                    {
                        if (ModIsInstalled(mod))
                        {
                            modDownloadButton.Invoke((MethodInvoker)delegate { modDownloadButton.Text = "Uninstall"; });
                            modDownloadButton.Invoke((MethodInvoker)delegate { modDownloadButton.UseVisualStyleBackColor = false; });
                            modDownloadButton.Click += delegate { Task.Run(() => UninstallMod(modDownloadButton, modProgressBar, mod, type)); };
                        }
                        else
                        {
                            modDownloadButton.Invoke((MethodInvoker)delegate { modDownloadButton.Text = "Install"; });
                            modDownloadButton.Invoke((MethodInvoker)delegate { modDownloadButton.UseVisualStyleBackColor = false; });
                            modDownloadButton.Click += delegate {
                                modDownloadButton.Invoke((MethodInvoker)delegate { modDownloadButton.Text = "Queued"; });
                                modProgressBar.Invoke((MethodInvoker)delegate { modProgressBar.Visible = true; });
                                Task.Run(() => Extract(modDownloadButton, modProgressBar, mod, type));
                            };
                        }
                    }
                    else
                    {
                        modDownloadButton.Invoke((MethodInvoker)delegate { modDownloadButton.Text = "Open"; });
                        modDownloadButton.Invoke((MethodInvoker)delegate { modDownloadButton.UseVisualStyleBackColor = false; });
                        modDownloadButton.Click += delegate { Process.Start(Application.StartupPath + "\\Packages\\" + mod.fileName); };
                    }
                    mods++;
                }
            }
        }

        private bool ModIsInstalled(Mod mod)
        {
            bool isInstalled = false;

            XDocument doc = XDocument.Load(modInstallInfoFileFullPath);
            IEnumerable<XElement> childList =
                from el in doc.Root.Element("modsLog").Elements() select el;

            foreach (XElement xel in childList)
            {
                if (xel.Value == mod.fileName)
                    isInstalled = true;
            }
            return isInstalled;
        }

        private bool DirectoryIsEmpty(string dir)
        {
            dir = dir.Substring(0, dir.LastIndexOf("\\"));
            if (Directory.Exists(dir))
                return !Directory.EnumerateFileSystemEntries(dir).Any();
            return false;
        }

        private string GetFileName(string downloadLink)
        {
            string fileName = "";
            using (WebClient wc = new WebClient())
            {
                Uri url = new Uri(downloadLink);
                Stream myStream = wc.OpenRead(downloadLink);
                string header_contentDisposition = wc.ResponseHeaders["content-disposition"];
                fileName = new ContentDisposition(header_contentDisposition).FileName;
                myStream.Close();
            }
            return fileName;
        }

        private void UpdateLoadingBar(string type)
        {
            if (type == "cars" && loadingCarsProgressBar.Value < loadingCarsProgressBar.Maximum)
                carsTabPage.Invoke((MethodInvoker)delegate { loadingCarsProgressBar.Value++; });
            else if (type == "tracks" && loadingTracksProgressBar.Value < loadingTracksProgressBar.Maximum)
                carsTabPage.Invoke((MethodInvoker)delegate { loadingTracksProgressBar.Value++; });
            else if (type == "skins" && loadingSkinsProgressBar.Value < loadingSkinsProgressBar.Maximum)
                carsTabPage.Invoke((MethodInvoker)delegate { loadingSkinsProgressBar.Value++; });
            else if (type == "misc" && loadingMiscProgressBar.Value < loadingMiscProgressBar.Maximum)
                carsTabPage.Invoke((MethodInvoker)delegate { loadingMiscProgressBar.Value++; });
        }

        private void CreateInstallLogFile()
        {
            Directory.CreateDirectory(modInstallInfoFileFullPath.Substring(0, modInstallInfoFileFullPath.LastIndexOf("\\")));

            XmlWriterSettings ws = new XmlWriterSettings();
            ws.Indent = true;
            ws.NewLineOnAttributes = true;

            using (XmlWriter xmlWriter = XmlWriter.Create(modInstallInfoFileFullPath, ws))
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("installLog");
                xmlWriter.WriteStartElement("modsLog");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteStartElement("filesLog");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
                xmlWriter.Flush();
                xmlWriter.Close();
            }
        }

        private void NewXmlEntry(string modKey, string archiveName = "", string filePath = "")
        {
            XDocument doc = XDocument.Load(modInstallInfoFileFullPath);
            XElement mods = doc.Element("installLog").Element("modsLog");
            XElement files = doc.Element("installLog").Element("filesLog");
            if (filePath == "")
                mods.Add(new XElement("mod", archiveName, new XAttribute("modKey", modKey)));
            else
                files.Add(new XElement("file", filePath, new XAttribute("modKey", modKey)));
            doc.Save(modInstallInfoFileFullPath);
        }

        private void UninstallMod(Button button, ProgressBar progressBar, Mod mod, string type)
        {
            lock(this)
            {
                progressBar.Invoke((MethodInvoker)delegate { progressBar.Visible = true; });
                button.Invoke((MethodInvoker)delegate { button.Text = "Uninstalling..."; });
                button.Invoke((MethodInvoker)delegate { button.Enabled = false; });
                XDocument doc = XDocument.Load(modInstallInfoFileFullPath);
                XElement mods = doc.Element("installLog").Element("modsLog");
                XElement files = doc.Element("installLog").Element("filesLog");
                string modKey = "";
                var nodes = doc.Root.Element("modsLog").Elements().ToList();

                foreach (XElement xel in nodes)
                {
                    if (xel.Value == mod.fileName)
                    {
                        modKey = xel.Attribute("modKey").Value;
                        xel.Remove();
                        break;
                    }
                }

                nodes = doc.Root.Element("filesLog").Elements().ToList();

                int fileCount = 0;

                foreach (XElement xel in nodes)
                {
                    if (xel.Attribute("modKey").Value == modKey)
                    {
                        fileCount++;
                    }
                }

                int fileCurrent = 0;

                foreach (XElement xel in nodes)
                {
                    if (xel.Attribute("modKey").Value == modKey)
                    {
                        if (File.Exists(xel.Value))
                            File.Delete(xel.Value);
                        if (DirectoryIsEmpty(xel.Value))
                            Directory.Delete(xel.Value.Substring(0, xel.Value.LastIndexOf("\\")));
                        xel.Remove();
                        fileCurrent++;
                        progressBar.Invoke((MethodInvoker)delegate { progressBar.Value = 100 * fileCurrent / fileCount; });
                    }
                }
                doc.Save(modInstallInfoFileFullPath);
                progressBar.Invoke((MethodInvoker)delegate { progressBar.Visible = false; });
                progressBar.Invoke((MethodInvoker)delegate { progressBar.Value = 0; });
                button.Invoke((MethodInvoker)delegate { button.Text = "Install"; });
                button.Invoke((MethodInvoker)delegate { button.Enabled = true; });
                RemoveClickEvent(button);
                button.Click += delegate {
                    button.Invoke((MethodInvoker)delegate { button.Text = "Queued"; });
                    progressBar.Invoke((MethodInvoker)delegate { progressBar.Visible = true; });
                    Task.Run(() => Extract(button, progressBar, mod, type));
                };
            }
        }

        private void RemoveClickEvent(Button b)
        {
            FieldInfo f1 = typeof(Control).GetField("EventClick",
                BindingFlags.Static | BindingFlags.NonPublic);
            object obj = f1.GetValue(b);
            PropertyInfo pi = b.GetType().GetProperty("Events",
                BindingFlags.NonPublic | BindingFlags.Instance);
            EventHandlerList list = (EventHandlerList)pi.GetValue(b, null);
            list.RemoveHandler(obj, list[obj]);
        }

        private void ModPanel_MouseHover(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void DownloadFile(bool newFile, Button button, ProgressBar progressBar, Mod mod, string type)
        {

            QMod qmod = new QMod();
            qmod.button = button;
            qmod.progressBar = progressBar;
            qmod.mod = mod;
            qmod.type = type;
            button.Invoke((MethodInvoker)delegate { button.Text = "Queued"; });
            progressBar.Invoke((MethodInvoker)delegate { progressBar.Visible = true; });

            if (newFile)
                downloadQueue.Add(qmod);

            if (downloadQueue.Count > 0 && !dwc.IsBusy)
            {


                lock (this)
                {
                    downloadQueue[0].button.Invoke((MethodInvoker)delegate { downloadQueue[0].button.Enabled = false; });
                    float sizef = float.Parse(downloadQueue[0].mod.size.Replace("<", ""), CultureInfo.InvariantCulture.NumberFormat);
                    downloadQueue[0].progressBar.Invoke((MethodInvoker)delegate { downloadQueue[0].progressBar.Visible = true; });

                    try
                    {
                        using (dwc = new WebClient())
                        {
                            Uri url = new Uri(downloadQueue[0].mod.downloadLink);
                            dwc.DownloadProgressChanged += (s, e) =>
                            {
                                downloadQueue[0].progressBar.Invoke((MethodInvoker)delegate { downloadQueue[0].progressBar.Value = e.ProgressPercentage; });
                                downloadQueue[0].button.Invoke((MethodInvoker)delegate { downloadQueue[0].button.Text = string.Format("{0:0.0}", sizef - sizef * e.ProgressPercentage / 100, 1).ToString().Replace(',', '.') + " MB"; });
                            };

                            Stream myStream = dwc.OpenRead(downloadQueue[0].mod.downloadLink);

                            string header_contentDisposition = dwc.ResponseHeaders["content-disposition"];
                            string fileName = new ContentDisposition(header_contentDisposition).FileName;

                            dwc.DownloadFileCompleted += (s, e) =>
                            {
                                downloadQueue[0].progressBar.Invoke((MethodInvoker)delegate { downloadQueue[0].progressBar.Visible = false; });
                                downloadQueue[0].progressBar.Invoke((MethodInvoker)delegate { downloadQueue[0].progressBar.Value = 0; });

                                if (e.Cancelled == true)
                                {
                                    MessageBox.Show("Download has been cancelled.");
                                    downloadQueue[0].button.Invoke((MethodInvoker)delegate { downloadQueue[0].button.Enabled = true; });
                                }
                                else if (type == "cars" || type == "tracks")
                                {
                                    Task.Run(() => Extract(downloadQueue[0].button, downloadQueue[0].progressBar, downloadQueue[0].mod, downloadQueue[0].type));
                                }
                                else
                                {
                                    downloadQueue[0].button.Invoke((MethodInvoker)delegate { downloadQueue[0].button.Text = "Open"; });
                                    downloadQueue[0].button.Invoke((MethodInvoker)delegate { downloadQueue[0].button.UseVisualStyleBackColor = false; });
                                    downloadQueue[0].button.Invoke((MethodInvoker)delegate { downloadQueue[0].button.Enabled = true; });
                                    Process.Start(Application.StartupPath + "\\Packages\\" + fileName);
                                    RemoveClickEvent(button);
                                    downloadQueue[0].button.Click += delegate { Process.Start(Application.StartupPath + "\\Packages\\" + fileName); };
                                }
                                Thread.Sleep(100);
                                downloadQueue.RemoveAt(0);
                                if (downloadQueue.Count > 0)
                                    DownloadFile(false, downloadQueue[0].button, downloadQueue[0].progressBar, downloadQueue[0].mod, downloadQueue[0].type);
                            };

                            Directory.CreateDirectory(Application.StartupPath + "\\Packages");
                            dwc.DownloadFileAsync(url, Application.StartupPath + "\\Packages\\" + fileName);
                            myStream.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
        }

        void Extract(object sender, ProgressBar progressBar, Mod mod, string type)
        {
            lock(this)
            {
                Button button = (Button)sender;
                button.Invoke((MethodInvoker)delegate { button.Enabled = false; });
                button.Invoke((MethodInvoker)delegate { button.UseVisualStyleBackColor = false; });
                button.Invoke((MethodInvoker)delegate { button.Text = "Installing..."; });
                string file = Application.StartupPath + "\\Packages\\" + mod.fileName;
                string targetLocation = "";
                int fileCountTotal;
                int fileCount = 0;
                progressBar.Invoke((MethodInvoker)delegate { progressBar.Visible = true; });

                ProcessStartInfo p = new ProcessStartInfo();
                p.FileName = "7z.exe";
                p.WindowStyle = ProcessWindowStyle.Hidden;

                string modKey = Path.GetRandomFileName().Replace(".", "");
                NewXmlEntry(modKey, mod.fileName);

                #region ZipFile
                if (mod.fileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    using (ZipArchive zip = ZipFile.Open(file, ZipArchiveMode.Read))
                    {
                        fileCountTotal = zip.Entries.Count;

                        foreach (ZipArchiveEntry entry in zip.Entries)
                        {
                            fileCount++;

                            progressBar.Invoke((MethodInvoker)delegate { progressBar.Value = (100 * fileCount) / fileCountTotal; });
                            if (progressBar.Value > 0)
                                progressBar.Invoke((MethodInvoker)delegate { progressBar.Value -= 1; });

                            if (entry.Name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) ||
                                entry.Name.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) ||
                                entry.Name.EndsWith(".psd", StringComparison.OrdinalIgnoreCase) ||
                                entry.Name.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                entry.Name.EndsWith(".rar", StringComparison.OrdinalIgnoreCase) ||
                                entry.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                                continue;

                            if (entry.Name != "")
                            {
                                try
                                {
                                    if (entry.FullName.StartsWith("GameData", StringComparison.OrdinalIgnoreCase) ||
                                        entry.FullName.StartsWith("rFm", StringComparison.OrdinalIgnoreCase))
                                    {
                                        targetLocation = Path.GetDirectoryName(Application.StartupPath + "\\" + entry.FullName);
                                        p.Arguments = "e \"" + file + "\" -o\"" + targetLocation + "\" \"" + entry.FullName + "\" -y";
                                        Process x = Process.Start(p);
                                        x.WaitForExit();
                                    }
                                    else if (entry.FullName.Contains("GameData"))
                                    {
                                        string trimmedEntryFullName = entry.FullName.Substring(entry.FullName.IndexOf("GameData"));
                                        targetLocation = Path.GetDirectoryName(Application.StartupPath + "\\" + trimmedEntryFullName);
                                        p.Arguments = "e \"" + file + "\" -o\"" + targetLocation + "\" \"" + entry.FullName + "\" -y";
                                        Process x = Process.Start(p);
                                        x.WaitForExit();
                                    }
                                    else if (entry.FullName.Contains("rFm"))
                                    {
                                        string trimmedEntryFullName = entry.FullName.Substring(entry.FullName.IndexOf("rFm"));
                                        targetLocation = Path.GetDirectoryName(Application.StartupPath + "\\" + trimmedEntryFullName);
                                        p.Arguments = "e \"" + file + "\" -o\"" + targetLocation + "\" \"" + entry.FullName + "\" -y";
                                        Process x = Process.Start(p);
                                        x.WaitForExit();
                                    }
                                    else if (entry.FullName.Contains("Locations"))
                                    {
                                        string trimmedEntryFullName = entry.FullName.Substring(entry.FullName.IndexOf("Locations"));
                                        targetLocation = Path.GetDirectoryName(Application.StartupPath + "\\GameData\\" + trimmedEntryFullName);
                                        p.Arguments = "e \"" + file + "\" -o\"" + targetLocation + "\" \"" + entry.FullName + "\" -y";
                                        Process x = Process.Start(p);
                                        x.WaitForExit();
                                    }
                                    else if (entry.FullName.Contains("Shared"))
                                    {
                                        string trimmedEntryFullName = entry.FullName.Substring(entry.FullName.IndexOf("Shared"));
                                        targetLocation = Path.GetDirectoryName(Application.StartupPath + "\\GameData\\" + trimmedEntryFullName);
                                        p.Arguments = "e \"" + file + "\" -o\"" + targetLocation + "\" \"" + entry.FullName + "\" -y";
                                        Process x = Process.Start(p);
                                        x.WaitForExit();
                                    }
                                    else if (entry.FullName.Contains("Sounds"))
                                    {
                                        string trimmedEntryFullName = entry.FullName.Substring(entry.FullName.IndexOf("Sounds"));
                                        targetLocation = Path.GetDirectoryName(Application.StartupPath + "\\GameData\\" + trimmedEntryFullName);
                                        p.Arguments = "e \"" + file + "\" -o\"" + targetLocation + "\" \"" + entry.FullName + "\" -y";
                                        Process x = Process.Start(p);
                                        x.WaitForExit();
                                    }
                                    else if (entry.FullName.Contains("Talent"))
                                    {
                                        string trimmedEntryFullName = entry.FullName.Substring(entry.FullName.IndexOf("Talent"));
                                        targetLocation = Path.GetDirectoryName(Application.StartupPath + "\\GameData\\" + trimmedEntryFullName);
                                        p.Arguments = "e \"" + file + "\" -o\"" + targetLocation + "\" \"" + entry.FullName + "\" -y";
                                        Process x = Process.Start(p);
                                        x.WaitForExit();
                                    }
                                    else if (entry.FullName.Contains("Vehicles"))
                                    {
                                        string trimmedEntryFullName = entry.FullName.Substring(entry.FullName.IndexOf("Vehicles"));
                                        targetLocation = Path.GetDirectoryName(Application.StartupPath + "\\GameData\\" + trimmedEntryFullName);
                                        p.Arguments = "e \"" + file + "\" -o\"" + targetLocation + "\" \"" + entry.FullName + "\" -y";
                                        Process x = Process.Start(p);
                                        x.WaitForExit();
                                    }
                                    else
                                    {
                                        if (type == "tracks")
                                        {
                                            targetLocation = Path.GetDirectoryName(Application.StartupPath + "\\GameData\\Locations\\" + entry.FullName);
                                            p.Arguments = "e \"" + file + "\" -o\"" + targetLocation + "\" \"" + entry.FullName + "\" -y";
                                            Process x = Process.Start(p);
                                            x.WaitForExit();
                                        }
                                        else if (type == "cars")
                                        {
                                            targetLocation = Path.GetDirectoryName(Application.StartupPath + "\\GameData\\Vehicles\\" + entry.FullName);
                                            p.Arguments = "e \"" + file + "\" -o\"" + targetLocation + "\" \"" + entry.FullName + "\" -y";
                                            Process x = Process.Start(p);
                                            x.WaitForExit();
                                        }
                                    }
                                    NewXmlEntry(modKey, mod.fileName, targetLocation + "\\" + entry.Name);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message.ToString());
                                    break;
                                }
                            }
                        }
                    }
                }
                #endregion

                #region RarFile
                else if (mod.fileName.EndsWith(".rar", StringComparison.OrdinalIgnoreCase))
                {
                    RarArchive rar = RarArchive.Open(file);
                    fileCountTotal = rar.Entries.Count;

                    foreach (RarArchiveEntry entry in rar.Entries)
                    {
                        string entryName = Path.GetFileName(entry.FilePath);

                        fileCount++;

                        progressBar.Invoke((MethodInvoker)delegate { progressBar.Value = (100 * fileCount) / fileCountTotal; });
                        if (progressBar.Value > 0)
                            progressBar.Invoke((MethodInvoker)delegate { progressBar.Value -= 1; });

                        if (entryName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) ||
                            entryName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase) ||
                            entryName.EndsWith(".psd", StringComparison.OrdinalIgnoreCase) ||
                            entryName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                            entryName.EndsWith(".rar", StringComparison.OrdinalIgnoreCase) ||
                            entryName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                            continue;

                        string entryFullName = entry.FilePath;
                        if (entryName != "")
                        {
                            try
                            {
                                if (entryFullName.StartsWith("GameData", StringComparison.OrdinalIgnoreCase) ||
                                    entryFullName.StartsWith("rFm", StringComparison.OrdinalIgnoreCase))
                                {
                                    targetLocation = Path.GetDirectoryName(Application.StartupPath + "\\" + entryFullName);
                                    p.Arguments = "e \"" + file + "\" -o\"" + targetLocation + "\" \"" + entryFullName + "\" -y";
                                    Process x = Process.Start(p);
                                    x.WaitForExit();
                                }
                                else if (entryFullName.Contains("GameData"))
                                {
                                    string trimmedEntryFullName = entryFullName.Substring(entryFullName.IndexOf("GameData"));
                                    targetLocation = Path.GetDirectoryName(Application.StartupPath + "\\" + trimmedEntryFullName);
                                    p.Arguments = "e \"" + file + "\" -o\"" + targetLocation + "\" \"" + entryFullName + "\" -y";
                                    Process x = Process.Start(p);
                                    x.WaitForExit();
                                }
                                else if (entryFullName.Contains("rFm"))
                                {
                                    string trimmedEntryFullName = entryFullName.Substring(entryFullName.IndexOf("rFm"));
                                    targetLocation = Path.GetDirectoryName(Application.StartupPath + "\\" + trimmedEntryFullName);
                                    p.Arguments = "e \"" + file + "\" -o\"" + targetLocation + "\" \"" + entryFullName + "\" -y";
                                    Process x = Process.Start(p);
                                    x.WaitForExit();
                                }
                                else if (entryFullName.Contains("Locations"))
                                {
                                    string trimmedEntryFullName = entryFullName.Substring(entryFullName.IndexOf("Locations"));
                                    targetLocation = Path.GetDirectoryName(Application.StartupPath + "\\GameData\\" + trimmedEntryFullName);
                                    p.Arguments = "e \"" + file + "\" -o\"" + targetLocation + "\" \"" + entryFullName + "\" -y";
                                    Process x = Process.Start(p);
                                    x.WaitForExit();
                                }
                                else if (entryFullName.Contains("Shared"))
                                {
                                    string trimmedEntryFullName = entryFullName.Substring(entryFullName.IndexOf("Shared"));
                                    targetLocation = Path.GetDirectoryName(Application.StartupPath + "\\GameData\\" + trimmedEntryFullName);
                                    p.Arguments = "e \"" + file + "\" -o\"" + targetLocation + "\" \"" + entryFullName + "\" -y";
                                    Process x = Process.Start(p);
                                    x.WaitForExit();
                                }
                                else if (entryFullName.Contains("Sounds"))
                                {
                                    string trimmedEntryFullName = entryFullName.Substring(entryFullName.IndexOf("Sounds"));
                                    targetLocation = Path.GetDirectoryName(Application.StartupPath + "\\GameData\\" + trimmedEntryFullName);
                                    p.Arguments = "e \"" + file + "\" -o\"" + targetLocation + "\" \"" + entryFullName + "\" -y";
                                    Process x = Process.Start(p);
                                    x.WaitForExit();
                                }
                                else if (entryFullName.Contains("Talent"))
                                {
                                    string trimmedEntryFullName = entryFullName.Substring(entryFullName.IndexOf("Talent"));
                                    targetLocation = Path.GetDirectoryName(Application.StartupPath + "\\GameData\\" + trimmedEntryFullName);
                                    p.Arguments = "e \"" + file + "\" -o\"" + targetLocation + "\" \"" + entryFullName + "\" -y";
                                    Process x = Process.Start(p);
                                    x.WaitForExit();
                                }
                                else if (entryFullName.Contains("Vehicles"))
                                {
                                    string trimmedEntryFullName = entryFullName.Substring(entryFullName.IndexOf("Vehicles"));
                                    targetLocation = Path.GetDirectoryName(Application.StartupPath + "\\GameData\\" + trimmedEntryFullName);
                                    p.Arguments = "e \"" + file + "\" -o\"" + targetLocation + "\" \"" + entryFullName + "\" -y";
                                    Process x = Process.Start(p);
                                    x.WaitForExit();
                                }
                                else
                                {
                                    if (type == "tracks")
                                    {
                                        targetLocation = Path.GetDirectoryName(Application.StartupPath + "\\GameData\\Locations\\" + entryFullName);
                                        p.Arguments = "e \"" + file + "\" -o\"" + targetLocation + "\" \"" + entryFullName + "\" -y";
                                        Process x = Process.Start(p);
                                        x.WaitForExit();
                                    }
                                    else if (type == "cars")
                                    {
                                        targetLocation = Path.GetDirectoryName(Application.StartupPath + "\\GameData\\Vehicles\\" + entryFullName);
                                        p.Arguments = "e \"" + file + "\" -o\"" + targetLocation + "\" \"" + entryFullName + "\" -y";
                                        Process x = Process.Start(p);
                                        x.WaitForExit();
                                    }
                                }
                                NewXmlEntry(modKey, mod.fileName, targetLocation + "\\" + entryName);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message.ToString());
                                break;
                            }
                        }
                    }
                }
                #endregion
                progressBar.Invoke((MethodInvoker)delegate { progressBar.Visible = false; });
                progressBar.Invoke((MethodInvoker)delegate { progressBar.Value = 0; });
                button.Invoke((MethodInvoker)delegate { button.Text = "Uninstall"; });
                button.Invoke((MethodInvoker)delegate { button.Enabled = true; });
                RemoveClickEvent(button);
                button.Click += delegate { Task.Run(() => UninstallMod(button, progressBar, mod, type)); };
            }
        }

        
    }






    public class Mod
    {
        public string   name,
                        version,
                        author,
                        downloadLink,
                        fileName,
                        size,
                        date;
    }

    public class QMod
    {
        public Button button;
        public ProgressBar progressBar;
        public Mod mod;
        public string type;
    }

    public class News
    {
        public string   title,
                        author,
                        link;

        public DateTime date;
    }
}

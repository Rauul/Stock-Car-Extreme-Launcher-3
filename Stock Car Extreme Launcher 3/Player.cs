using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Stock_Car_Extreme_Launcher_3
{
    public class Player
    {
        public string Name { get; set; }
        public string AutoLift { get; set; }
        public string AutoBlip { get; set; }
        public string VirtualRearviewInCockpit { get; set; }
        public string MeasurementUnits { get; set; }
        public string SpeedUnits { get; set; }
        public string DamperUnits { get; set; }
        public string RelativeFuelStrategy { get; set; }
        public string PitStopDescription { get; set; }
        public string SmartPitcrew { get; set; }
        public string AutoCalibrateAIMode { get; set; }
        public string HeadlightsOnCars { get; set; }
        public string RearviewCull { get; set; }
        public string MaxFramerate { get; set; }
        public string MaxHeadlights { get; set; }
        public string MaxVisibleCars { get; set; }
        public string RearviewFOV { get; set; }
        public string Control { get; set; }
        public string SelfInRearview { get; set; }
        public string KeepReceivedSetups { get; set; }
        public string MovingRearview { get; set; }


        public Player(string name)
        {
            string plrFilePath = Form1.userData + name + @"\" + name + ".plr";
            StreamReader sr;
            string strContent;
            string strValue;
            sr = new StreamReader(plrFilePath);
            strContent = sr.ReadToEnd();
            sr.Close();
            Regex r;
            Match m;

            //Name
            Name = name;

            //Rearview FOV
            r = new Regex(@"^(Rearview Height="")(.+?)("")", RegexOptions.Multiline);
            m = r.Match(strContent);

            if (m.Success)
            {
                strValue = m.Groups[2].Value;
                RearviewFOV = strValue;
            }

            //Max Headlights
            r = new Regex(@"^(Max Headlights="")(\d+)("")", RegexOptions.Multiline);
            m = r.Match(strContent);

            if (m.Success)
            {
                strValue = m.Groups[2].Value;
                if (Convert.ToInt16(strValue) > 50)
                    strValue = "50";
                MaxHeadlights = strValue;
            }

            //Max Visible Vehicles
            r = new Regex(@"^(Max Visible Vehicles="")(\d+)("")", RegexOptions.Multiline);
            m = r.Match(strContent);

            if (m.Success)
            {
                strValue = m.Groups[2].Value;
                if (Convert.ToInt16(strValue) > 50)
                    strValue = "50";
                MaxVisibleCars = strValue;
            }

            //Max Framerate
            r = new Regex(@"^(Max Framerate="")(-?)(\d+)("")", RegexOptions.Multiline);
            m = r.Match(strContent);

            if (m.Success)
            {
                strValue = m.Groups[3].Value;
                if (Convert.ToUInt16(strValue) > 200)
                    strValue = "200";
                MaxFramerate = strValue;
            }

            //Auto Lift
            r = new Regex(@"^(Auto Lift="")(\d+)("")", RegexOptions.Multiline);
            m = r.Match(strContent);

            if (m.Success)
            {
                strValue = m.Groups[2].Value;
                AutoLift = strValue;
            }

            //Auto Blip
            r = new Regex(@"^(Auto Blip="")(\d+)("")", RegexOptions.Multiline);
            m = r.Match(strContent);

            if (m.Success)
            {
                strValue = m.Groups[2].Value;
                AutoBlip = strValue;
            }

            //Virtual Rearview In Cockpit
            r = new Regex(@"^(Virtual Rearview In Cockpit="")(\d+)("")", RegexOptions.Multiline);
            m = r.Match(strContent);

            if (m.Success)
            {
                strValue = m.Groups[2].Value;
                VirtualRearviewInCockpit = strValue;
            }

            //Relative Fuel
            r = new Regex(@"^(Relative Fuel Strategy="")(\d+)("")", RegexOptions.Multiline);
            m = r.Match(strContent);

            if (m.Success)
            {
                strValue = m.Groups[2].Value;
                RelativeFuelStrategy = strValue;
            }

            //Measurement Units
            r = new Regex(@"^(Measurement Units="")(\d+)("")", RegexOptions.Multiline);
            m = r.Match(strContent);

            if (m.Success)
            {
                strValue = m.Groups[2].Value;
                MeasurementUnits = strValue;
            }

            //Speed Units
            r = new Regex(@"^(Speed Units="")(\d+)("")", RegexOptions.Multiline);
            m = r.Match(strContent);

            if (m.Success)
            {
                strValue = m.Groups[2].Value;
                SpeedUnits = strValue;
            }

            //Damper Units
            r = new Regex(@"^(Damper Units="")(\d+)("")", RegexOptions.Multiline);
            m = r.Match(strContent);

            if (m.Success)
            {
                strValue = m.Groups[2].Value;
                DamperUnits = strValue;
            }

            //Pitstop Description
            r = new Regex(@"^(Pitstop Description="")(\d+)("")", RegexOptions.Multiline);
            m = r.Match(strContent);

            if (m.Success)
            {
                strValue = m.Groups[2].Value;
                PitStopDescription = strValue;
            }

            //Smart Pitcrew
            r = new Regex(@"^(Smart Pitcrew="")(\d+)("")", RegexOptions.Multiline);
            m = r.Match(strContent);

            if (m.Success)
            {
                strValue = m.Groups[2].Value;
                SmartPitcrew = strValue;
            }

            //Autocalibrate AI
            r = new Regex(@"^(Autocalibrate AI Mode="")(\d+)("")", RegexOptions.Multiline);
            m = r.Match(strContent);

            if (m.Success)
            {
                strValue = m.Groups[2].Value;
                AutoCalibrateAIMode = strValue;
            }

            //Headlights On Cars
            r = new Regex(@"^(Headlights On Cars="")(\d+)("")", RegexOptions.Multiline);
            m = r.Match(strContent);

            if (m.Success)
            {
                strValue = m.Groups[2].Value;
                HeadlightsOnCars = strValue;
            }

            //Rearview Cull
            r = new Regex(@"^(Rearview Cull="")(\d+)("")", RegexOptions.Multiline);
            m = r.Match(strContent);

            if (m.Success)
            {
                strValue = m.Groups[2].Value;
                RearviewCull = strValue;
            }

            //Self In Rearview
            r = new Regex(@"^(Self In Cockpit Rearview="")(\d+)("")", RegexOptions.Multiline);
            m = r.Match(strContent);

            if (m.Success)
            {
                strValue = m.Groups[2].Value;
                SelfInRearview = strValue;
            }

            //Moving Rearview
            r = new Regex(@"^(Moving Rearview="")(\d+)("")", RegexOptions.Multiline);
            m = r.Match(strContent);

            if (m.Success)
            {
                strValue = m.Groups[2].Value;
                MovingRearview = strValue;
            }

            //Keep Received Setups
            r = new Regex(@"^(Keep Received Setups="")(\d+)("")", RegexOptions.Multiline);
            m = r.Match(strContent);

            if (m.Success)
            {
                strValue = m.Groups[2].Value;
                KeepReceivedSetups = strValue;
            }

            //Control
            r = new Regex(@"^(Current Control File="")(.+?)("")", RegexOptions.Multiline);
            m = r.Match(strContent);

            if (m.Success)
            {
                strValue = m.Groups[2].Value;
                Control = strValue;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace patientEditor
{
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;

    using HM4DesignTool.Properties;

    using Path = System.IO.Path;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        class Patient
        {
            public string patientType = "";
            public int delay = 0;
            public List<string> stepNames = new List<string>();
        }

        class TreatmentStep
        {
            public string name = "";
            public Bitmap icon;
            public string type = "normal";
        }

        class PatientType
        {
            public Bitmap image;
            public int chance;
        }

        List<TreatmentStep> m_Treatments = new List<TreatmentStep>();
        List<PatientType> m_PatientTypes = new List<PatientType>();
        List<Patient> m_Patients = new List<Patient>();

        public string m_RoomName = "";
        public bool m_hasTriggers = false;
        public string m_DataFolderPath = "I:\\hm4\\data\\";

        private int findClosingBracket(string text, int start)
        {
            int open = 0;
            for (int i = start; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '{') open++;
                if (c == '}') open--;

                if (open < 0)
                {
                    return i;
                }
            }
            return -1;
        }

        private int findNextChar(string text, int start, char c)
        {
            for (int i = start; i < text.Length; i++)
            {
                if (text[i] == c)
                {
                    return i;
                }
            }
            return -1;
        }

        private string getProductName(string text)
        {
            int tries = 10;
            while(tries > 0)
            {
                string tmp = getTableText(text, "");
                text = text.Replace("{" + tmp + "}", "");

                if (tmp == "")
                {
                    break;
                }
                tries--;
            }

            string[] lines = Regex.Split(text, "#");

            string name = "product";
            for (int i = 1; i < lines.Length; i++)
            {
                name += "#" + lines[i];
            }
            return name;
        }

        private string getTableText(string rawText, string tableName)
        {
            tableName += tableName != "" ? "={" : "{";

            int start = rawText.IndexOf(tableName) + tableName.Length;
            int end = findClosingBracket(rawText, start) - start;

            if (end < 0)
            {
                return "";
            }

            return rawText.Substring(start, end);
        }

        private string getVariable(string rawText, string varName)
        {
            if (!rawText.Contains(varName))
            {
                return "";
            }

            if (varName != "")
            {
                varName += "=";
            }

            int start = rawText.IndexOf(varName) + varName.Length;
            int end = findNextChar(rawText, start, ',') - start;

            if (end < 0)
            {
                return "";
            }

            return rawText.Substring(start, end).Replace("\"", "");
        }

        private string[] getStringsFromLine(string rawText)
        {
            string tmp = rawText.Replace("\"", "");
            return Regex.Split(tmp, ",");
        }

        private void readPatientChancesTable(string rawText)
        {
            string[] lines = Regex.Split(getTableText(rawText, "patientChances"), ",");
            for (int i = 0; i < lines.Length - 1; i++)
            {
                PatientType type = new PatientType();

                string[] parameters = Regex.Split(lines[i], "=");
                string name = parameters[0];
                type.chance = 0; // Int32.Parse(parameters[1]);

                string imagePath = m_DataFolderPath + @"images\patients\" + @name + @"\sel.png";
                if (File.Exists(imagePath))
                {
                    Bitmap img = new Bitmap(imagePath);
                    img = img.Clone(new Rectangle(0,0, img.Width, img.Width), img.PixelFormat);

                    type.image = new Bitmap(img);
                }
                m_PatientTypes.Add(type);
            }
        }

        private void readTriggerTable(string rawText)
        {
            string[] lines = Regex.Split(getTableText(rawText, "triggers"), "todo");
            m_hasTriggers = lines.Length > 1;

            for (int l = 0; l < lines.Length - 1; l++)
            {
                Patient p = new Patient();
                String var = getVariable(lines[l], "delay");
                if (var != "")
                {
                    p.delay = Int32.Parse(var);
                }

                string todoTableText = getTableText("todo"+lines[l], "todo");
                if (todoTableText == "")
                {
                    continue;
                }

                string[] treatments = getStringsFromLine(todoTableText);
                for (int t = 0; t < treatments.Length; t++)
                {
                    string step = treatments[t];
                    if (step != "")
                    {
                        p.stepNames.Add(treatments[t]);
                    }
                }

                m_Patients.Add(p);
            }
            getTreatmentsFromRoomFile(m_RoomName);
        }

        private string getRoomIdFromLevel(string levelName)
        {
            if (levelName.StartsWith("level"))
            {
                int roomId = (int)Math.Ceiling(Int32.Parse(levelName.Replace("level", "")) / (double)10);
                string path = @m_DataFolderPath + @"script\rooms\";
                if (!Directory.Exists(path))
                {
                    return "";
                }

                DirectoryInfo d = new DirectoryInfo(path);
                FileInfo[] Files = d.GetFiles(@"*.lua");

                foreach (FileInfo file in Files)
                {
                    string name = file.Name;
                    if (name.Contains("room" + roomId))
                    {
                        return name.Replace(".lua", "");
                    }
                }
                return "";
            }
            else
            {
                string @absPath = @m_DataFolderPath + @"script\GameLevels.lua";
                if (!File.Exists(@absPath))
                {
                    return "";
                }

                string rawText = File.ReadAllText(@absPath);
                rawText = rawText.Replace("\r\n", "");
                rawText = rawText.Replace("\r", "");
                rawText = rawText.Replace("\n", "");
                rawText = rawText.Replace("\t", "");
                rawText = rawText.Replace(" ", "");

                string challengeTable = getTableText(rawText, levelName);
                if (challengeTable != "")
                {
                    return getVariable(challengeTable, "room");
                }

                return "";
            }
        }

        private void readLevelFile(string levelName)
        {
            string absPath = @m_DataFolderPath + @"script\levels\" + levelName + @".lua";
            string fileName = Path.GetFileNameWithoutExtension(absPath);

            if (!File.Exists(absPath))
            {
                return;
            }

            m_RoomName = getRoomIdFromLevel(fileName);

            string rawText = File.ReadAllText(@absPath);
            rawText = rawText.Replace("\r\n", "");
            rawText = rawText.Replace("\r", "");
            rawText = rawText.Replace("\n", "");
            rawText = rawText.Replace("\t", "");
            rawText = rawText.Replace(" ", "");

            readPatientChancesTable(rawText);
            readTriggerTable(rawText);
        }
        
        private bool doesTreatmentExists(string name)
        {
            for(int t = 0; t < m_Treatments.Count; t++)
            {
                if (m_Treatments[t].name == "name")
                {
                    return true;
                }
            }
            return false;
        }

        private void getTreatmentsFromRoomFile(string roomName)
        {
            string absPath = m_DataFolderPath + @"script\rooms\defaults\" + @roomName + @"_game.lua";
            if (!File.Exists(absPath))
            {
                return;
            }

            string rawText = File.ReadAllText(@absPath);
            rawText = rawText.Replace("\r\n", "");
            rawText = rawText.Replace("\r", "");
            rawText = rawText.Replace("\n", "");
            rawText = rawText.Replace("\t", "");
            rawText = rawText.Replace(" ", "");

            string treatmentsTableText = getTableText(rawText, "treatments");

            for (int p = 0; p < m_Patients.Count; p++)
            {
                Patient patient = m_Patients[p];
                for(int t = 0; t < patient.stepNames.Count; t++)
                {
                    TreatmentStep step = new TreatmentStep();

                    string treatmentName = patient.stepNames[t];
                    if (treatmentName == "")
                    {
                        continue;
                    }

                    if (!doesTreatmentExists(treatmentName))
                    {
                        string tableText = getTableText(treatmentsTableText, treatmentName);

                        step.name = treatmentName;

                        string workIconName = getVariable(tableText, "workIcon");
                        string productName = getVariable(tableText, "product");

                        string fileName = workIconName != "" ? workIconName : getProductName(productName);

                        if (getVariable(tableText, "choices") != "")
                        {
                            fileName += "_1";
                        }

                        string iconPath = m_DataFolderPath + @"images\rooms\" + @roomName + @"\icons\" + fileName + ".png";
                        if (File.Exists(iconPath))
                        {
                            step.icon = new Bitmap(iconPath);
                        }
                        else
                        {
                            fileName = getProductName(productName);
                            iconPath = m_DataFolderPath + @"images\rooms\" + @roomName + @"\icons\" + fileName + ".png";
                            if (File.Exists(iconPath))
                            {
                                step.icon = new Bitmap(iconPath);
                            }
                            else
                            {
                                step.icon = Resources.noicon;
                            }
                        }

                        step.type = productName != "" ? "product" :
                                    getVariable(tableText, "minigame") != "" ?
                                    "minigame" :
                                    getVariable(tableText, "gesture") != "" ?
                                    "gesture" : "normal";

                        m_Treatments.Add(step);
                    }
                }
            }
        }

        private TreatmentStep getTreatmentStep(string name)
        {
            for (int i = 0; i < m_Treatments.Count; i++)
            {
                if (m_Treatments[i].name == name)
                {
                    return m_Treatments[i];
                }
            }
            return null;
        }

        private void drawPatients()
        {
            if (!m_hasTriggers)
            {
                return;
            }

            Bitmap timeBlock = Resources.blockTime;
            Bitmap balloon = Resources.balloon_normal;

            float scale = 0.8f;
            int balloonWidth = (int)(balloon.Width * scale);
            int balloonHeight = (int)(balloon.Height * scale);

            Rectangle balloonSpace = new Rectangle((int)(27 * scale), (int)(4 * scale), (int)(116 * scale), (int)(116 * scale));

            int iconWidth = (int)(96 * scale);
            int iconHeight = (int)(96 * scale);

            int iconOffsetX = balloonSpace.X + (balloonSpace.Width / 2) - (iconWidth / 2);
            int iconOffsetY = balloonSpace.Y + (balloonSpace.Height / 2) - (iconHeight / 2);

            int x = 0;
            int y = 0;

            Bitmap dest = new Bitmap(12 * balloonWidth, m_Patients.Count * balloonHeight, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(dest);

            Random rnd = new Random();

            for (int p = 0; p < m_Patients.Count; p++)
            {
                Patient patient = m_Patients[p];

                // delay block
                g.DrawImage(timeBlock, x, y, balloonHeight, balloonHeight);

                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                g.DrawString(patient.delay.ToString(), 
                            new Font(FontFamily.GenericSansSerif, 18 * scale, FontStyle.Bold), 
                            Brushes.White, 
                            new Rectangle(x, y, balloonHeight, balloonHeight), 
                            sf);

                x += balloonHeight;

                // patient avatar
                int id = rnd.Next(0, m_PatientTypes.Count);
                Bitmap img = m_PatientTypes[id].image;
                g.DrawImage(img, x, y, balloonHeight, balloonHeight);

                x += balloonHeight;

                for (int s = 0; s < patient.stepNames.Count; s++)
                {
                    string name = patient.stepNames[s];
                    TreatmentStep step = getTreatmentStep(patient.stepNames[s]);

                    balloon = (step.type == "product") ? Resources.balloon_product :
                              (step.type == "minigame") ? Resources.balloon_minigame :
                              (step.type == "gesture") ? Resources.balloon_minigame :
                              Resources.balloon_normal;

                    g.DrawImage(balloon, x, y, balloonWidth, balloonHeight);

                    g.DrawImage(step.icon, x + iconOffsetX, y + iconOffsetY, iconWidth, iconHeight);
                    x += balloonWidth;
                }
                x = 0;
                y += balloonHeight;
            }

            pictureBox1.Width = dest.Width;
            pictureBox1.Height = dest.Height;
            pictureBox1.Image = dest;
        }

        private void resetData()
        {
            m_Patients.Clear();
            m_PatientTypes.Clear();
            m_Treatments.Clear();
            m_RoomName = "";
            m_hasTriggers = false;
            pictureBox1.Image = null;
        }

        private void generateLevelList()
        {
            string path = m_DataFolderPath + @"script\levels\";
            DirectoryInfo d = new DirectoryInfo(path);
            FileInfo[] Files = d.GetFiles(@"*.lua");

            if (!Directory.Exists(path))
            {
                return;
            }

            foreach (FileInfo file in Files)
            {
                comboBox1.Items.Add(file.Name.Replace(".lua", ""));
            }
        }

        private void readSettingsFile()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string name = "settings.txt";
            string filename = @path + @name;

            if (File.Exists(filename))
            {
                string[] lines = System.IO.File.ReadAllLines(@filename);
                char symbol = '=';
                foreach (string line in lines)
                {
                    string[] tokens = line.Split(@symbol);

                    // skip everthing that starts with a comment token
                    if (tokens[0].Trim().StartsWith("//") || tokens[0].Trim().StartsWith("--"))
                    {
                        continue;
                    }

                    switch (tokens[0])
                    {
                        case "DATA":
                            if (Directory.Exists(@tokens[1]))
                            {
                                m_DataFolderPath = @tokens[1];
                            }
                            break;
                    }
                }
            }
        }

        private void loadAndDrawLevel(string level)
        {
            resetData();
            readLevelFile(level);
            drawPatients();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            readSettingsFile();
            generateLevelList();

            int index = comboBox1.FindString("level1");
            comboBox1.SelectedIndex = index;
            loadAndDrawLevel(comboBox1.SelectedItem.ToString());
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadAndDrawLevel(comboBox1.SelectedItem.ToString());
        }

    }
}

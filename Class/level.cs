using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using DataNameSpace;
using NaturalSort.Extension;
using NLua;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace LevelData
{

    struct DesignToolData
    {
        public float DifficultyLevel;
        public LevelType levelType;

        private const String startDesignToolDataText = "--[[HM4DesignToolData:";
        private const String DifficultyLevelText = "DifficultyLevel:";
        private const String LevelTypeText = "LevelType:";
        private const String endDesignToolDataText = "--]]";


        public DesignToolData(float difficultyLevel = 0, LevelType levelType = LevelType.Unknown)
        {
            this.DifficultyLevel = difficultyLevel;
            this.levelType = levelType;
        }

        public void ParseDesignData(String designToolData)
        {
            designToolData = designToolData.Replace("\t", "").Replace("\r", "").Replace(" ", "");
            designToolData.Replace(startDesignToolDataText, "").Replace(endDesignToolDataText, "");
            String[] delimiter = { "\n" };
            List<String> designToolList = designToolData.Split(delimiter, StringSplitOptions.None).ToList<String>();

            foreach (String entry in designToolList)
            {
                String textItem = entry;
                if (textItem.Contains(DifficultyLevelText))
                {
                    textItem = textItem.Replace(DifficultyLevelText, "");
                    DifficultyLevel = (float)Convert.ToDouble(textItem);
                }
                if (textItem.Contains(LevelTypeText))
                {
                    textItem = textItem.Replace(LevelTypeText, "");
                    switch (textItem)
                    {
                        case "Bonus":
                            levelType = LevelType.Bonus;
                            break;
                        case "Story":
                            levelType = LevelType.Story;
                            break;
                        case "MiniGame":
                            levelType = LevelType.MiniGame;
                            break;
                        case "TimeTrial":
                            levelType = LevelType.TimeTrial;
                            break;
                        case "Oliver":
                            levelType = LevelType.OliverOne;
                            break;
                        case "OliverOne":
                            levelType = LevelType.OliverOne;
                            break;
                        case "OliverAll":
                            levelType = LevelType.OliverAll;
                            break;
                        default:
                            levelType = LevelType.Unknown;
                            break;
                    }
                }

            }



        }

        public override string ToString()
        {
            String output = startDesignToolDataText + Environment.NewLine;

            if (DifficultyLevel > 0)
            {
                output += DifficultyLevelText + " \t" + DifficultyLevel.ToString() + Environment.NewLine;
            }

            if (levelType > 0)
            {
                output += LevelTypeText + " \t" + levelType.ToString() + Environment.NewLine;
            }

            output += endDesignToolDataText + Environment.NewLine;
            return output;
        }
    }

    public class LevelOverview : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Dictionary<string, Level> levelObjectData = new Dictionary<string, Level>();
        private Level _currentLevelLoaded = null;
        public Level GetLevelLoaded {
        get
            {
                return _currentLevelLoaded;
            }
        set
            {
                if (value != _currentLevelLoaded)
                {
                    _currentLevelLoaded = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool LevelOverviewActive
        {
            get
            {
                if (GetLevelLoaded != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        private List<String> levelList = new List<String> { };

        public LevelOverview()
        {

        }

        public static String ReadLevelTextFromFile(String levelName)
        {
            levelName = CleanLevelName(levelName);
            if (!levelName.EndsWith(".lua"))
            {
                levelName += ".lua";
            }

            String levelPath = Globals.GetSettings.projectPathLevel + levelName;

            if (File.Exists(levelPath))
            {
                string readContents;
                using (StreamReader streamReader = new StreamReader(levelPath, Encoding.UTF8))
                {
                    readContents = streamReader.ReadToEnd();
                    return readContents;
                }

            }
            else
            {
                Console.WriteLine("ERROR: Data.ReadLevelText, Could not find " + levelPath + "!");
                return null;
            }

        }

        public List<String> GetFilteredLevels(int roomIndex = 0, bool storyLevels = false, bool bonusLevels = false, bool unknownLevels = false)
        {
            List<String> rawLevelList = GetLevelsFromDisk(false, true);
            List<String> outputLevelList = new List<String> { };


            foreach (String level in rawLevelList)
            {

                if (storyLevels && level.StartsWith("level"))
                {
                    int levelIndex = Convert.ToInt16(level.Replace("level", ""));
                    int minIndex = (roomIndex - 1) * 10 + 1;
                    int maxIndex = (roomIndex * 10) + 1;

                    if (roomIndex == 0)
                    {
                        outputLevelList.Add(level);
                    }
                    else if (Enumerable.Range(minIndex, 10).Contains(levelIndex))
                    {
                        outputLevelList.Add(level);
                    }
                }
                // Ensure that the second character in the level name is a number as well to be in line with naming conventions.
                else if (bonusLevels && level.StartsWith("r") && int.TryParse(level[1].ToString(), out int n) && level[2].ToString() == "_")
                {
                    if (roomIndex == 0)
                    {
                        outputLevelList.Add(level);
                    }
                    else if (level.StartsWith("r" + roomIndex.ToString() + "_"))
                    {
                        outputLevelList.Add(level);
                    }
                }
                else if (unknownLevels && !level.StartsWith("level") && !(level.StartsWith("r") && int.TryParse(level[1].ToString(), out int j) && level[2].ToString() == "_"))
                {
                    outputLevelList.Add(level);
                }


            }
            //Naturaly sort the levelList 
            //TODO Find more efficient method of natural sorting without first converting to sequence and back
            IEnumerable<String> LevelSequence = outputLevelList.OrderBy(x => x, StringComparer.OrdinalIgnoreCase.WithNaturalSort());
            return LevelSequence.ToList();

        }

        public Dictionary<String, List<String>> GetCategorizedFilteredLevels(int roomIndex = 0, bool storyLevels = false, bool bonusLevels = false, bool unknownLevels = false)
        {
            Dictionary<String, List<String>> CategorizedFilterdLevels = new Dictionary<String, List<String>> { };
            List<String> FilteredLevelList = GetFilteredLevels(roomIndex, storyLevels, bonusLevels, unknownLevels);

            if (roomIndex == 0)
            {

                if (storyLevels)
                {
                    CategorizedFilterdLevels.Add("Story", GetFilteredLevels(0, true, false, false));
                }

                if (bonusLevels)
                {
                    for (int i = 1; i <= Globals.roomCategories.Count; i++)
                    {
                        CategorizedFilterdLevels.Add(Globals.roomCategories[i - 1], GetFilteredLevels(i, false, true, false));
                    }

                }

            }
            else
            {
                if (storyLevels)
                {
                    CategorizedFilterdLevels.Add("Story", GetFilteredLevels(roomIndex, true, false, false));
                }

                if (bonusLevels)
                {

                    CategorizedFilterdLevels.Add(Globals.roomCategories[roomIndex - 1], GetFilteredLevels(roomIndex, false, true, false));

                }

            }
            //Add all the uncategorized rooms
            if (unknownLevels)
            {
                CategorizedFilterdLevels.Add("Unknown", GetFilteredLevels(roomIndex, false, false, true));
            }

            return CategorizedFilterdLevels;
        }


        public void LoadLevel(String levelName)
        {
            levelName = CleanLevelName(levelName);

            if (LevelExist(levelName))
            {
                GetLevelLoaded = levelObjectData[levelName];
            }
            else
            {
                GetLevelLoaded = AddLevelByName(levelName);
            }
        }

        public Level GetLevel(String levelName)
        {
            levelName = CleanLevelName(levelName);

            if (!LevelExist(levelName))
            {
                AddLevelByName(levelName);
            }
            return levelObjectData[levelName];
        }

        private Level CreateLevel(String levelName)
        {
            levelName = CleanLevelName(levelName);

            Level newLevel = new Level(levelName);

            return newLevel;
        }

        private Level AddLevelByName(String levelName)
        {
            levelName = CleanLevelName(levelName);
            if (LevelExist(levelName))
             {
                levelObjectData.Remove(levelName);
             }
             
            levelObjectData.Add(levelName, CreateLevel(levelName));
            return levelObjectData[levelName];
        }

        private bool LevelExist(String levelName)
        {
            levelName = CleanLevelName(levelName);

            return levelObjectData.ContainsKey(levelName);

        }

        private static String CleanLevelName(String levelName)
        {
            levelName = levelName.Replace(" ", "");
            if (levelName.EndsWith(".lua"))
            {
                levelName = levelName.Replace(".lua", "");
            }
            if (levelName.Contains("(e)"))
            {
                levelName = levelName.Replace("(e)", "");
            }
            if (levelName.Contains("*"))
            {
                levelName = levelName.Replace("*", "");
            }

            return levelName;
        }
        #region Getters
        public List<String> GetLevelsFromDisk(bool reload = false, bool filterExtension = false)
        {
            if (levelList == null || levelList.Count() == 0 || reload)
            {
                String projectPath = Globals.GetSettings.projectPathLevel;
                if (Directory.Exists(projectPath))
                {

                    List<String> rawLevelList = new List<String>(System.IO.Directory.GetFiles(projectPath));
                    List<String> tmpLevelList = new List<String> { };

                    foreach (String level in rawLevelList)
                    {
                        String levelName = Path.GetFileName(level);
                        if (filterExtension)
                        {
                            tmpLevelList.Add(levelName.Replace(".lua", ""));
                        }
                        else
                        {
                            tmpLevelList.Add(levelName);
                        }


                    }

                    levelList = tmpLevelList;
                }
            }

            return levelList;
        }
        #endregion

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class Level
    {
        String LevelName = String.Empty;

        private Dictionary<int, Patient> patientDatabase = new Dictionary<int, Patient>();
        private Dictionary<String, float> patientChancesDict = new Dictionary<String, float>();
        private DesignToolData designToolData = new DesignToolData();
        private String currentLevelScript = null;
        public ObservableCollection<Patient> PatientCollection
        {
            get
            {
                ObservableCollection<Patient> patientCollection = new ObservableCollection<Patient>();
                if (patientDatabase.Count > 0)
                {
                    foreach (KeyValuePair<int, Patient> patient in patientDatabase)
                    {
                        patientCollection.Add(patient.Value);
                    }

                }
                return patientCollection;
            }
        }

        public Level(string levelName)
        {
            LevelName = levelName;
            ParseRawText(levelName);
        }

        #region Getters
        public String GetCurrentLevelScript
        {
            get
            {
                if (currentLevelScript == null || currentLevelScript == "")
                {
                    currentLevelScript = LevelOverview.ReadLevelTextFromFile(LevelName);
                }
                return currentLevelScript;
            }

        }
        public String GetNewLevelScript
        {
            get
            {
                string output = "";

                output += designToolData.ToString();
                output += Environment.NewLine;
                //Output the patient chances
                if (patientChancesDict.Count > 0)
                {
                    output += "levelDesc.patientChances = " + Environment.NewLine;
                    output += "{" + Environment.NewLine;
                    int i = 0;
                    foreach (KeyValuePair<string, float> patientChance in patientChancesDict)
                    {
                        output += "\t" + patientChance.Key + " \t= \t" + patientChance.Value;
                        if (i < patientChancesDict.Count - 1)
                        {
                            output += ",";
                        }
                        i++;
                        output += Environment.NewLine;

                    }
                    output += "}" + Environment.NewLine;
                }
                output += Environment.NewLine;
                //Output the patient treatments
                if (patientDatabase.Count() > 0)
                {
                    output += "levelDesc.triggers = " + Environment.NewLine;
                    output += "{" + Environment.NewLine;
                    foreach (Patient patient in patientDatabase.Values)
                    {
                        output += patient.ToString();
                    }
                    output += "}" + Environment.NewLine;

                }
                return output;

            }
        }
        #endregion


        private void AddPatient(String patientData, int index = -1)
        {
            if (index == -1)
            {
                index = patientDatabase.Count();
            }
            if (patientDatabase.ContainsKey(index))
            {
                patientDatabase.Remove(index);
            }
            String patientName = "Patient_" + index.ToString();
            Patient patientObject = new Patient(patientData, patientName);

            patientDatabase.Add(index, patientObject);
        }

        private void ParseRawText(String levelName)
        {
            String rawLevelText = GetCurrentLevelScript;
            // Clean up the text by removing tabs, enters and spaces and special characters.
            rawLevelText = rawLevelText.Replace("\t", "").Replace("\r", "").Replace("{\n", "{").Replace(" ", "").Replace("\n\n", "");



            // Parse DesignToolData embeded in file.
            String startDesignToolData = "--[[HM4DesignToolData:";
            String endDesignToolData = "--]]";
            String designToolDataText;
            if (rawLevelText.Contains(startDesignToolData) && rawLevelText.Contains(endDesignToolData))
            {
                int startDesignToolDataIndex = rawLevelText.IndexOf(startDesignToolData);
                int endDesignToolDataIndex = rawLevelText.IndexOf(endDesignToolData) + endDesignToolData.Length;

                if (startDesignToolDataIndex > -1 && endDesignToolDataIndex - endDesignToolData.Length > -1 && startDesignToolDataIndex < endDesignToolDataIndex)
                {
                    designToolDataText = rawLevelText.Substring(startDesignToolDataIndex, endDesignToolDataIndex - startDesignToolDataIndex);
                    designToolData.ParseDesignData(designToolDataText);
                    rawLevelText = rawLevelText.Replace(designToolDataText, "");

                }

            }

            // Parse the PatientChances section.
            String startPatientChancesText = "levelDesc.patientChances=";
            String endPatientChancesText = "}";
            String patientsChancesRawText;
            if (rawLevelText.Contains(startPatientChancesText) && rawLevelText.Contains(endPatientChancesText))
            {
                int startPatientChancesIndex = rawLevelText.IndexOf(startPatientChancesText);
                int endPatientChancesIndex = rawLevelText.IndexOf(endPatientChancesText) + endPatientChancesText.Length;

                if (startPatientChancesIndex > -1 && endPatientChancesIndex - endPatientChancesText.Length > -1 && startPatientChancesIndex < endPatientChancesIndex)
                {
                    // Do some extra formatting and cleaning up.
                    patientsChancesRawText = rawLevelText.Substring(startPatientChancesIndex, endPatientChancesIndex - startPatientChancesIndex);
                    rawLevelText = rawLevelText.Remove(startPatientChancesIndex, endPatientChancesIndex);

                    patientsChancesRawText = patientsChancesRawText.Replace("levelDesc.patientChances=", "").Replace(",}", "}");
                    patientsChancesRawText = patientsChancesRawText.Trim(' ').Replace("=", ":");

                    patientChancesDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<String, float>>(patientsChancesRawText);
                }


            }

            // Parse the PatientList with treatments
            String startPatientTriggerText = "levelDesc.triggers=";
            String endPatientTriggerText = "},\n}";
            String patientsTriggersRawText;
            if (rawLevelText.Contains(startPatientTriggerText) && rawLevelText.Contains(endPatientTriggerText))
            {
                int startPatientTriggerIndex = rawLevelText.IndexOf(startPatientTriggerText);
                int endPatientTriggerIndex = rawLevelText.IndexOf(endPatientTriggerText) + endPatientTriggerText.Length;

                if (startPatientTriggerIndex > -1 && endPatientTriggerIndex - endPatientChancesText.Length > -1 && startPatientTriggerIndex < endPatientTriggerIndex)
                {

                    patientsTriggersRawText = rawLevelText.Substring(startPatientTriggerIndex, endPatientTriggerIndex - startPatientTriggerIndex);
                    rawLevelText = rawLevelText.Remove(startPatientTriggerIndex, endPatientTriggerIndex);

                    patientsTriggersRawText = patientsTriggersRawText.Replace(startPatientTriggerText + "\n{", "").TrimEnd('}');

                    String[] delimiter = { "},\n" };
                    List<String> patientTriggers = patientsTriggersRawText.Split(delimiter, StringSplitOptions.None).ToList<String>();

                    foreach (String patientTrigger in patientTriggers)
                    {
                        AddPatient(patientTrigger);
                    }

                }
            }

            // Isolate any previous comments at the start of the text
            String startComments;
            if (rawLevelText.Contains(startDesignToolData))
            {
                int startDesignToolDataIndex = rawLevelText.IndexOf(startPatientChancesText);
                if (startDesignToolDataIndex > 0)
                {
                    startComments = rawLevelText.Substring(0, startDesignToolDataIndex);
                }
            }
            else if (rawLevelText.Contains(startDesignToolData))
            {
                int startPatientChancesIndex = rawLevelText.IndexOf(startPatientChancesText);
                if (startPatientChancesIndex > 0)
                {
                    startComments = rawLevelText.Substring(0, startPatientChancesIndex);
                }
            }
            else if (rawLevelText.Contains(startPatientTriggerText))
            {
                int startPatientTriggerIndex = rawLevelText.IndexOf(startPatientTriggerText);
                if (startPatientTriggerIndex > 0)
                {
                    startComments = rawLevelText.Substring(0, startPatientTriggerIndex);

                }

            }


        }
    }

    public class Patient
    {
        private String patientName = String.Empty;
        private int delay = -1;
        private int weight = -1;
        private List<String> treatmentList;
        private Dictionary<String, String> patientTraits;
        private bool weightEnabled = false;

        public String PatientName
        {
            get
            {
                return patientName;
            }
            set
            {
                patientName = value;
            }

        }
        public int Delay
        {
            get
            {
                return delay;
            }
            set
            {
                delay = value;
            }
        }
        public int Weight
        {
            get
            {
                return weight;
            }
            set
            {
                weight = value;
            }
        }

        public Patient()
        {


        }

        public Patient(String patientData, String patientName = null)
        {
            ParsePatientData(patientData);
            if (patientName != null)
            {
                this.patientName = patientName;
            }
        }

        public override string ToString()
        {
            String output = "";
            output += "\t{";

            if (delay > -1)
            {
                output += " delay = " + delay.ToString() + ",";
            }
            if (weightEnabled && weight > -1)
            {
                output += " weight = " + weight.ToString() + ",";
            }

            if (treatmentList != null && treatmentList.Count() > 0)
            {
                output += " todo = {";
                foreach (String treatment in treatmentList)
                {
                    output += "\"" + treatment + "\",";
                }
                output += "}," + Environment.NewLine;
            }

            if (patientTraits != null && patientTraits.Count() > 0)
            {
                foreach (KeyValuePair<string, string> trait in patientTraits)
                {
                    output += trait.Key + " = ";
                    if (trait.Value == "true")
                    {
                        output += trait.Value;
                    }
                    else
                    {
                        output += "\"" + trait.Value + "\"";
                    }
                }

            }

            return output;
        }

        private void ParsePatientData(String patientData)
        {
            // Clean up the patientData
            patientData = System.Text.RegularExpressions.Regex.Replace(patientData, @"\s+", "");

            patientData = RemoveFirstComma(patientData);
            //Parse the delay
            String delayText = "delay=";
            if (patientData.Contains(delayText))
            {
                int startIndex = patientData.IndexOf(delayText) + delayText.Length;
                int endIndex = patientData.IndexOf(",");
                if (startIndex > -1 && endIndex > -1)
                {
                    String delayString = patientData.Substring(startIndex, endIndex - startIndex);
                    delayString = Regex.Replace(delayString, @"[^\d]", "");
                    if (delayString != "None")
                    {
                        delay = Convert.ToInt32(delayString);
                    }
                    else
                    {
                        Console.WriteLine("ERROR: Level.ParsePatientData, delayString was none!");
                    }
                    patientData = patientData.Remove(startIndex - delayText.Length, endIndex);

                }
            }
            //Parse the weight
            patientData = RemoveFirstComma(patientData);
            String weightText = "weight=";
            if (patientData.Contains(weightText))
            {
                int startIndex = patientData.IndexOf(weightText) + weightText.Length;
                int endIndex = patientData.IndexOf(",");
                if (startIndex > -1 && endIndex > 0)
                {
                    String weightString = patientData.Substring(startIndex, endIndex - startIndex);
                    weightString = Regex.Replace(weightString, @"[^\d]", "");
                    weight = Convert.ToInt32(weightString);
                    patientData = patientData.Remove(startIndex - weightText.Length, endIndex);
                    weightEnabled = true;
                }
            }
            //Parse the treatment list
            patientData = RemoveFirstComma(patientData);
            String treatmentText = "todo={";
            if (patientData.Contains(treatmentText))
            {
                int startIndex = patientData.IndexOf(treatmentText) + treatmentText.Length;
                int endIndex = patientData.IndexOf('}');
                if (startIndex > -1 && endIndex > -1)
                {
                    String rawTreatments = patientData.Substring(startIndex, endIndex - startIndex);
                    rawTreatments = rawTreatments.Replace("\"", "");
                    treatmentList = rawTreatments.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<String>();
                    patientData = patientData.Remove(startIndex - treatmentText.Length, endIndex);

                }
            }
            // If there is remaining data then it is probably traits that have been added. 
            patientData = RemoveFirstComma(patientData);
            if (patientData.Length > 5)
            {
                 patientTraits = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<String, String>>(patientData);
            }
        }

        private static String RemoveFirstComma(String patientString)
        {
            if (patientString.StartsWith(","))
            {
                return patientString.Substring(1, patientString.Count()-1);
            }
            else
            {
                return patientString;
            }
        }
    }
}

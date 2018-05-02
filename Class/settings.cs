using nucs.JsonSettings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Media;
using DataNameSpace;
using System.IO;

namespace SettingsNamespace
{   //Storing settings in JSON: https://github.com/Nucs/JsonSettings
    public class Settings
    {

        private PersonalSettings pSettings;
        private GlobalSettings gSettings;
        private Dictionary<String, List<String>> patientTypeCategoriesDict
        {
            get
            {
                if (gSettings.patientTypeCategories != null)
                {
                    return gSettings.patientTypeCategories;
                }
                else
                {
                    return new Dictionary<String, List<String>> { };
                }
            }
            set
            {
                gSettings.patientTypeCategories = value;
            }
        }  // Room[N] -> List with only checked patientTypes
        private Dictionary<String, List<Treatment>> treatmentCategoriesDict
        {
            get
            {
                if (gSettings.treatmentCategories != null)
                {
                    //Convert Dictionary<String, Dictionary<String, List<String>>> -> Dictionary<String, List<Treatment>>
                    Dictionary<String, List<Treatment>> convertedDict = new Dictionary<String, List<Treatment>> { };
                    //Loop over each room/category
                    foreach (KeyValuePair<String, Dictionary<String, String>> roomCategory in gSettings.treatmentCategories)
                    {
                        List<Treatment> treatmentList = new List<Treatment> { };
                        //Loop over each treatment
                        foreach (KeyValuePair<String, String> treatmentRow in roomCategory.Value)
                        {
                            Treatment treatmentStruct = new Treatment(treatmentRow.Key, treatmentRow.Value);

                            treatmentList.Add(treatmentStruct);
                        }
                        convertedDict.Add(roomCategory.Key, treatmentList);
                    }

                    return convertedDict;
                }
                else
                {
                    return new Dictionary<String, List<Treatment>> { };
                }
            }
            set
            {
                //Convert Dictionary<String, List<Treatment>> -> Dictionary<String, Dictionary<String, List<String>>>
                Dictionary<String, Dictionary<String, String>> convertedDict = new Dictionary<String, Dictionary<String, String>> { };
                //Loop over each room/category
                foreach (KeyValuePair<String, List<Treatment>> roomCategory in value)
                {
                    Dictionary<String, String> treatmentList = new Dictionary<String, String> { };
                    //Loop over each treatment
                    foreach (Treatment treatmentRow in roomCategory.Value)
                    {
                        if (treatmentRow != null && treatmentRow.TreatmentName != null)
                        {
                            treatmentList.Add(treatmentRow.TreatmentName, treatmentRow.ToString());
                        }
                    }
                    convertedDict.Add(roomCategory.Key, treatmentList);
                }
                gSettings.treatmentCategories = convertedDict;
            }
        } //Room[N] -> List with Treatment Structs
        private Dictionary<String, List<String>> balancingCategoriesDict
        {
            get
            {
                if (gSettings.balancingCategories != null)
                {
                    return gSettings.balancingCategories;
                }
                else
                {
                    return new Dictionary<String, List<String>> { };
                }
            }
            set
            {
                gSettings.balancingCategories = value;
            }
        }  // Room[N] -> List with String difficulty Modifiers

        #region Settings

        public String projectPathData
        {
            get
            {
                return pSettings.ProjectDirectoryPath;
            }
            set
            {
                pSettings.ProjectDirectoryPath = value;
            }
        }

        public String projectPathScript
        {
            get
            {
                return projectPathData + "\\script\\";
            }
        }

        public String projectPathLevel
        {
            get
            {
                return projectPathData + "\\script\\levels\\";
            }
        }
        public String projectPathImages
        {
            get
            {
                return projectPathData + "\\images\\";
            }
        }

        #region Getters


        public Dictionary<String, List<String>> GetPatientTypes(String categoryKey = null)
        {
            if (categoryKey != null)
            {
                Dictionary<String, List<String>> filterdPatientTypeDict = new Dictionary<String, List<String>> { };  // Room[N] -> List with only checked patientTypes

                if (patientTypeCategoriesDict.ContainsKey(categoryKey))
                {
                    filterdPatientTypeDict.Add(categoryKey, patientTypeCategoriesDict[categoryKey]);
                }
                else
                {
                    Console.WriteLine("ERROR: Settings.GetPatientTypes, patientTypeCategoriesDict does not contain key: " + categoryKey);
                }
                return filterdPatientTypeDict;

            }
            else
            {
                return patientTypeCategoriesDict;
            }
        }
        public Dictionary<String, List<Treatment>> GetTreatmentDictionary(String categoryKey = null)
        {
            if (categoryKey != null)
            {
                Dictionary<String, List<Treatment>> filterdTreatmentDict = new Dictionary<String, List<Treatment>> { };  // Room[N] -> List with Treatment Structs

                if (treatmentCategoriesDict.ContainsKey(categoryKey))
                {
                    filterdTreatmentDict.Add(categoryKey, treatmentCategoriesDict[categoryKey]);
                }
                else
                {
                    Console.WriteLine("ERROR: Settings.GetTreatmentDictionary, treatmentCategoriesDict does not contain key: " + categoryKey);
                }
                return filterdTreatmentDict;

            }
            else
            {
                return treatmentCategoriesDict;
            }
        }
        public Dictionary<String, List<String>> GetBalancingCategories(String categoryKey = null)
        {
            if (categoryKey != null)
            {
                Dictionary<String, List<String>> filterdBalancingCategory = new Dictionary<String, List<String>> { };  // Room[N] -> List with Difficulty modifiers in String

                if (balancingCategoriesDict.ContainsKey(categoryKey))
                {
                    filterdBalancingCategory.Add(categoryKey, balancingCategoriesDict[categoryKey]);
                }
                else
                {
                    Console.WriteLine("ERROR: Settings.GetBalancingCategories, balancingCategoriesDict does not contain key: " + categoryKey);
                }
                return filterdBalancingCategory;

            }
            else
            {
                return balancingCategoriesDict;
            }
        }

        public List<String> GetDifficultyModifierList(String categoryKey)
        {
            Dictionary<String, List<String>> balancingCategoriesDict = GetBalancingCategories(categoryKey);
            if (balancingCategoriesDict.ContainsKey(categoryKey))
            {
                return balancingCategoriesDict[categoryKey];
            }
            else
            {
                return new List<String> { };
            }

        }
        public List<Treatment> GetTreatmentList(String categoryKey)
        {
            if (categoryKey != null)
            {
                Dictionary<String, List<Treatment>> treatmentDictionary = GetTreatmentDictionary(categoryKey);
                if (treatmentDictionary.ContainsKey(categoryKey))
                {
                    return treatmentDictionary[categoryKey];
                }
            }
            return new List<Treatment> { };

        }

        #endregion

        #region Setters

        public void SetPatientTypes(Dictionary<String, List<String>> patientTypeCategoriesDict)
        {
            this.patientTypeCategoriesDict = patientTypeCategoriesDict;
        }

        public void SetTreatmentCategories(Dictionary<String, List<Treatment>> treatmentDataDict)
        {
            this.treatmentCategoriesDict = treatmentDataDict;
            Dictionary<String, List<Treatment>> treatmentStoreDict = new Dictionary<String, List<Treatment>> { };

        }

        public void SetBalancingCategories(Dictionary<String, List<String>> balancingCategoriesDict)
        {
            this.balancingCategoriesDict = balancingCategoriesDict;
        }

        #endregion

        #endregion
        public Settings()
        {
            pSettings = new PersonalSettings();
            gSettings = new GlobalSettings();
            LoadSettings();
        }

        public void LoadSettings()
        {
            pSettings.Load();
            gSettings.Load();
        }

        public void SaveSettings()
        {
            pSettings.Save();
            gSettings.Save();
        }

        public int RoomFilterDropdownIndex
        {
            get
            {
                return pSettings.RoomFilterDropdownIndex;
            }
            set
            {
                pSettings.RoomFilterDropdownIndex = value;
            }
        }
        public bool LevelListFirstCategoryOpen
        {
            get
            {
                return pSettings.LevelListFirstCategoryOpen;
            }
            set
            {
                pSettings.LevelListFirstCategoryOpen = value;
            }
        }

        public bool RoomFilterStoryCheck
        {
            get
            {
                return pSettings.RoomFilterStoryCheck;
            }
            set
            {
                pSettings.RoomFilterStoryCheck = value;
            }
        }
        public bool RoomFilterBonusCheck
        {
            get
            {
                return pSettings.RoomFilterBonusCheck;
            }
            set
            {
                pSettings.RoomFilterBonusCheck = value;
            }
        }
        public bool RoomFilterUnknownCheck
        {
            get
            {
                return pSettings.RoomFilterUnknownCheck;
            }
            set
            {
                pSettings.RoomFilterUnknownCheck = value;
            }
        }

    }

    public class PersonalSettings : JsonSettings
    {
        public override string FileName { get; set; } = "personal.json"; //for loading and saving.

        public string ProjectDirectoryPath { get; set; } = "";

        public int RoomFilterDropdownIndex { get; set; } = 0;
        public bool LevelListFirstCategoryOpen { get; set; } = true;
        public bool RoomFilterStoryCheck { get; set; } = true;
        public bool RoomFilterBonusCheck { get; set; } = true;
        public bool RoomFilterUnknownCheck { get; set; } = true;




        public PersonalSettings()
        {
            //Check if save file exist and otherwise create it
            String defaultPath = AppDomain.CurrentDomain.BaseDirectory + FileName;
            if (!File.Exists(defaultPath))
            {
                File.Create(defaultPath).Dispose();
            }
        }
        public PersonalSettings(string fileName) : base(fileName) { }



    }

    public class GlobalSettings : JsonSettings
    {
        public override string FileName { get; set; } = "global.json"; //for loading and saving.

        public Dictionary<String, List<String>> patientTypeCategories { get; set; }
        public Dictionary<String, Dictionary<String, String>> treatmentCategories { get; set; }
        public Dictionary<String, List<String>> balancingCategories { get; set; }

        public GlobalSettings()
        {
            //Check if save file exist and otherwise create it
            String defaultPath = AppDomain.CurrentDomain.BaseDirectory + FileName;
            if (!File.Exists(defaultPath))
            {
                File.Create(defaultPath).Dispose();
            }
        }
        public GlobalSettings(string fileName) : base(fileName) { }



    }


}


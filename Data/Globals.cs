// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Globals.cs" company="Blue Giraffe">
//   Created by Jason Landbrug as part of an Design internship from 12-02-2018 / 18-06-2018 at Blue Giraffe
// </copyright>
// <author> Jason Landbrug </author>
// <summary>  Global references </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace HM4DesignTool.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Media;

    using HM4DesignTool.Forms;
    using HM4DesignTool.Level;

    using SettingsNamespace;

    public static class Globals
    {
        private static Settings SettingsObject;
        private static LevelOverview LevelOverviewObject;
        private static GameValues GameValueObject = new GameValues();

        private static Random random;

        public static Settings GetSettings => SettingsObject ?? (SettingsObject = new Settings());

        public static LevelOverview GetLevelOverview
        {
            get
            {
                if (LevelOverviewObject == null)
                {
                    // This is seperated as the setup of all children in the LevelOverview need a reference of this as object as well. 
                    LevelOverviewObject = new LevelOverview();
                    LevelOverviewObject.SetupLevelOverview();
                }

                return LevelOverviewObject;
            }
        }
        public static GameValues GetGameValues => GameValueObject ?? (GameValueObject = new GameValues());

        public static MainWindow GetMainWindow { get; set; }

        public static Random GetRandom => random ?? (random = new Random());

        /// <summary>
        /// The room categories.
        /// </summary>
        public static List<string> RoomCategories { get; } = new List<string> { "Room 1", "Room 2", "Room 3", "Room 4", "Room 5", "Room 6" };

        public static List<string> GetLevelTypes
        {
            get
            {
                IEnumerable<LevelTypeEnum> output = Enum.GetValues(typeof(LevelTypeEnum)).Cast<LevelTypeEnum>();
                List<string> LevelTypeString = new List<string> { };
                foreach (LevelTypeEnum leveltype in output)
                {
                    LevelTypeString.Add(leveltype.ToString());
                }
                return LevelTypeString;
            }
        }


        public static Double StringToDouble(String difficultyModifier = null)
        {
            if (difficultyModifier != null)
            {
                return double.Parse(difficultyModifier, System.Globalization.CultureInfo.InvariantCulture);
            }
            else
            {
                return 0;
            }
        }

        public static String FilterToNumerical(String str)
        {
            return Regex.Replace(str, @"[^\d]", "");
        }

        /// <summary>
        /// Remove first comma.
        /// </summary>
        /// <param name="stringObject">
        /// The patient string.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string RemoveFirstComma(string stringObject)
        {
            if (stringObject.StartsWith(","))
            {
                return stringObject.Substring(1, stringObject.Count() - 1);
            }
            else if (stringObject.StartsWith("{,"))
            {

                return stringObject.Remove(1, 1);
            }
            else
            {
                return stringObject;
            }
        }

        public static String GetCategoryKey(int RoomIndex)
        {
            //Convert RoomNumber to RoomIndex, [1,2,3,4..] => [0,1,2,3..] 
            RoomIndex--;
            if (-1 < RoomIndex && RoomIndex < RoomCategories.Count)
            {
                return RoomCategories[RoomIndex];
            }
            else
            {
                return String.Empty;
            }
        }

        public static String ColorToHex(Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        public static Color HexToColor(String hexCode)
        {
            if (!hexCode.StartsWith("#"))
            {
                hexCode = $"#{hexCode}";
            }

            if (hexCode.Count() == 7)
            {
                return (Color)ColorConverter.ConvertFromString(hexCode);
            }
            else
            {
                return Colors.White;
            }
        }
    }

}

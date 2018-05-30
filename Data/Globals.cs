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
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Media;

    using HM4DesignTool.Forms;
    using HM4DesignTool.Level;

    using SettingsNamespace;

    /// <summary>
    /// Globals class which contains global references to important instances and functions. 
    /// </summary>
    public static class Globals
    {
        #region Fields
        /// <summary>
        /// The object which is bound to the main ui, used for level management.
        /// </summary>
        private static LevelOverview levelOverviewObject;

        /// <summary>
        /// The settings object.
        /// </summary>
        private static Settings settingsObject;

        /// <summary>
        /// All Random functions should be based on this as prevents the same output.
        /// </summary>
        private static Random random;

        #endregion

        #region Properties

        #region Public

        /// <summary>
        /// Gets the reference to the Settings
        /// </summary>
        public static Settings GetSettings => settingsObject ?? (settingsObject = new Settings());

        /// <summary>
        /// Gets the reference to the level overview.
        /// </summary>
        public static LevelOverview GetLevelOverview
        {
            get
            {
                if (levelOverviewObject == null)
                {
                    // This is seperated as the setup of all children in the LevelOverview need a reference of this as object as well. 
                    levelOverviewObject = new LevelOverview();
                    levelOverviewObject.SetupLevelOverview();
                }

                return levelOverviewObject;
            }
        }

        /// <summary>
        /// Gets or sets the XAML Main Window
        /// </summary>
        public static MainWindow GetMainWindow { get; set; }

        /// <summary>
        /// Gets the reference to the Random object.
        /// </summary>
        public static Random GetRandom => random ?? (random = new Random());

        /// <summary>
        /// Gets the level types in a string list.
        /// </summary>
        public static List<string> GetLevelTypes
        {
            get
            {
                IEnumerable<LevelTypeEnum> output = Enum.GetValues(typeof(LevelTypeEnum)).Cast<LevelTypeEnum>();
                List<string> levelTypeString = new List<string>();
                levelTypeString.AddRange(output.Select(leveltype => leveltype.ToString()));
                return levelTypeString;
            }
        }

        /// <summary>
        /// Gets a string list of the room categories.
        /// Adding a room here is enough to make the tool functional with additional rooms.
        /// </summary>
        public static List<string> RoomCategories { get; } = new List<string> { "Room 1", "Room 2", "Room 3", "Room 4", "Room 5", "Room 6" };

        #endregion

        #endregion

        #region Methods

        #region Public

        #region Converters
        /// <summary>
        /// Translated the room index to the Category Key used to index data in dictionary.
        /// </summary>
        /// <param name="roomIndex">
        /// The room index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetCategoryKey(int roomIndex)
        {
            // Convert RoomNumber to RoomIndex, [1,2,3,4..] => [0,1,2,3..] 
            roomIndex--;
            if (-1 < roomIndex && roomIndex < RoomCategories.Count)
            {
                return RoomCategories[roomIndex];
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Converts a Hexidecimal code to a Color struct
        /// </summary>
        /// <param name="hexCode">
        /// The hex code.
        /// </param>
        /// <returns>
        /// The <see cref="Color"/>.
        /// </returns>
        public static Color HexToColor(string hexCode)
        {
            if (!hexCode.StartsWith("#"))
            {
                hexCode = $"#{hexCode}";
            }

            if (hexCode.Length == 7 && IsHexString(hexCode))
            {
                return (Color)ColorConverter.ConvertFromString(hexCode);
            }
            else
            {
                return Colors.White;
            }
        }

        /// <summary>
        /// Converts a Color struct to its Hexidecimal code.
        /// </summary>
        /// <param name="c">
        /// The c.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ColorToHex(Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }

        /// <summary>
        /// Converts a string to a double
        /// </summary>
        /// <param name="difficultyModifier">
        /// The difficulty modifier.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public static double StringToDouble(string difficultyModifier = null)
        {
            if (difficultyModifier != null)
            {
                return double.Parse(difficultyModifier, CultureInfo.InvariantCulture);
            }
            else
            {
                return 0;
            }
        }

        #endregion

        #region Filters

        /// <summary>
        /// Filter are non-numerical characters.
        /// </summary>
        /// <param name="str">
        /// The str.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string FilterToNumerical(string str)
        {
            return Regex.Replace(str, @"[^\d]", string.Empty);
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
                return stringObject.Substring(1, stringObject.Length - 1);
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

        #endregion

        #region Checks

        /// <summary>
        /// Test if the string is a valid hexcode.
        /// </summary>
        /// <param name="hexcode">
        /// The hexcode.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsHexString(string hexcode)
        {
            // https://stackoverflow.com/a/223857
            // For C-style hex notation (0xFF) you can use @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z"
            return Regex.IsMatch(hexcode, @"\A\b[0-9a-fA-F]+\b\Z");
        }

        #endregion
        #endregion

        #endregion
    }
}

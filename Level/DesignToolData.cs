// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DesignToolData.cs" company="Blue Giraffe">
//   Created by Jason Landbrug as part of an Design internship from 12-02-2018 / 18-06-2018 at Blue Giraffe
// </copyright>
// <summary>
//   Defines the DesignToolData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HM4DesignTool.Level
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using HM4DesignTool.Data;

    /// <inheritdoc />
    /// <summary>
    /// The Design Tool data Class.
    /// </summary>
    public class DesignToolData : INotifyPropertyChanged
    {
        #region Fields
        /// <summary>
        /// The difficulty level text.
        /// </summary>
        private const string DifficultyLevelText = "DifficultyLevel:";

        /// <summary>
        /// The end design tool data text.
        /// </summary>
        private const string EndDesignToolDataText = "--]]";

        /// <summary>
        /// The level type text.
        /// </summary>
        private const string LevelTypeText = "LevelType:";

        /// <summary>
        /// The room index text.
        /// </summary>
        private const string RoomIndexText = "RoomIndex:";

        /// <summary>
        /// The start design tool data text.
        /// </summary>
        private const string StartDesignToolDataText = "--[[HM4DesignToolData:";

        /// <summary>
        /// The difficulty level.
        /// </summary>
        private double difficultyLevel;

        /// <summary>
        /// The Level type.
        /// </summary>
        private LevelTypeEnum levelType = LevelTypeEnum.Unknown;

        /// <summary>
        /// The room index.
        /// </summary>
        private int roomIndex;

        #endregion

        #region Events  
        /// <inheritdoc />
        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets the Difficulty level.
        /// </summary>
        public double DifficultyLevel
        {
            get => this.difficultyLevel;
            set
            {
                this.difficultyLevel = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the level type.
        /// </summary>
        public LevelTypeEnum LevelType
        {
            get => this.levelType;
            set
            {
                this.levelType = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the level type string.
        /// </summary>
        public string LevelTypeString
        {
            get => Enum.GetName(typeof(LevelTypeEnum), this.LevelType);
            set => this.LevelType = (LevelTypeEnum)Enum.Parse(typeof(LevelTypeEnum), value);
        }

        /// <summary>
        /// Gets or sets the roomindex.
        /// </summary>
        public int Roomindex
        {
            get => this.roomIndex;
            set
            {
                this.roomIndex = value;
                this.OnPropertyChanged();
            }
        }
        #endregion

        /// <summary>
        /// Parse the raw design tool data string into workable values
        /// </summary>
        /// <param name="designToolData">
        /// The raw design tool data.
        /// </param>
        public void ParseDesignData(string designToolData)
        {
            designToolData = designToolData.Replace("\t", string.Empty).Replace("\r", string.Empty).Replace(" ", string.Empty);
            designToolData = designToolData.Replace(StartDesignToolDataText, string.Empty).Replace(EndDesignToolDataText, string.Empty);
            string[] delimiter = { "\n" };
            List<string> designToolList = designToolData.Split(delimiter, StringSplitOptions.None).ToList();

            foreach (string entry in designToolList)
            {
                string textItem = entry;

                if (textItem.Contains(RoomIndexText))
                {
                    textItem = textItem.Replace(RoomIndexText, string.Empty);
                    textItem = Globals.FilterToNumerical(textItem);
                    this.Roomindex = Convert.ToInt32(textItem);
                }

                if (textItem.Contains(DifficultyLevelText))
                {
                    textItem = textItem.Replace(DifficultyLevelText, string.Empty);
                    this.DifficultyLevel = (float)Convert.ToDouble(textItem);
                }

                if (textItem.Contains(LevelTypeText))
                {
                    textItem = textItem.Replace(LevelTypeText, string.Empty);
                    switch (textItem)
                    {
                        case "Bonus":
                            this.LevelType = LevelTypeEnum.Bonus;
                            break;

                        case "Story":
                            this.LevelType = LevelTypeEnum.Story;
                            break;

                        case "MiniGame":
                            this.LevelType = LevelTypeEnum.MiniGame;
                            break;

                        case "TimeTrial":
                            this.LevelType = LevelTypeEnum.TimeTrial;
                            break;

                        case "Oliver":
                            this.LevelType = LevelTypeEnum.OliverOne;
                            break;

                        case "OliverOne":
                            this.LevelType = LevelTypeEnum.OliverOne;
                            break;

                        case "OliverAll":
                            this.LevelType = LevelTypeEnum.OliverAll;
                            break;

                        default:
                            this.LevelType = LevelTypeEnum.Unknown;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Returns this object as an String
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            string output = StartDesignToolDataText + Environment.NewLine;

            if (this.Roomindex > 0)
            {
                output += RoomIndexText + " \t" + this.Roomindex.ToString() + Environment.NewLine;
            }

            if (this.DifficultyLevel > 0)
            {
                output += DifficultyLevelText + " \t" + this.DifficultyLevel + Environment.NewLine;
            }

            if (this.LevelType > 0)
            {
                output += LevelTypeText + " \t" + this.LevelType.ToString() + Environment.NewLine;
            }

            output += EndDesignToolDataText + Environment.NewLine;
            return output;
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged Members
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using HM4DesignTool.Level;

namespace HM4DesignTool.Data
{
    using System.IO;

    using HM4DesignTool.Utilities;

    class Data
    {

        #region Fields

        private const string textLevelDesc = "levelDesc";

        private const string textTriggers = "triggers";

        private const string textPatientChances = "patientChances";


        public static string LevelDescPatientChances => $"{textLevelDesc}.{textPatientChances}";

        public static string LevelDescTriggers => $"{textLevelDesc}.{textTriggers}";

        #endregion



        #region Constructors



        #endregion


        #region Events



        #endregion

        #region Properties

        #region Public

        #region Commands

        #endregion

        #endregion

        #region Private



        #endregion

        #endregion



        #region Methods

        #region Public



        #endregion

        #region Private

        #region Signals

        #endregion

        #endregion

        #endregion

        #region Operators


        #endregion


        public Data()
        {

        }


        public static string GetDesignData(string rawText)
        {
            string startDesignToolData = DesignToolData.StartDesignToolDataText;
            string endDesignToolData = DesignToolData.EndDesignToolDataText;

            if (rawText.Contains(startDesignToolData) && rawText.Contains(endDesignToolData))
            {
                // Clean the raw text
                rawText = FilterRawText(rawText);

                int startDesignToolDataIndex = rawText.IndexOf(startDesignToolData, StringComparison.Ordinal);
                int endDesignToolDataIndex = rawText.IndexOf(endDesignToolData, StringComparison.Ordinal) + endDesignToolData.Length;

                if (startDesignToolDataIndex > -1 && endDesignToolDataIndex - endDesignToolData.Length > -1 && startDesignToolDataIndex < endDesignToolDataIndex)
                {
                    return rawText.Substring(startDesignToolDataIndex, endDesignToolDataIndex - startDesignToolDataIndex);
                }
            }
            return string.Empty;
        }

        public static List<string> GetPatientChancesTable(string rawText)
        {
            // Create List of all patient Chances lines and remove empty entries.
            return Regex.Split(GetTableText(rawText, textPatientChances), ",").Where(s => s != string.Empty).ToList();
        }


        public static List<string> GetPatientTriggersTable(string rawText)
        {

            string tableText = GetTableText(rawText, textTriggers);
            List<string> triggerList = new List<string>();

            if (tableText != string.Empty && tableText.Length > 0)
            {
                // Start search after index 0 = "{"
                int previousIndex = 1;
                int loop = 0;
                // Search trigger and remove when found
                while (previousIndex < tableText.Length && loop < 100)
                {
                    int endIndex = FindClosingBracket(tableText, previousIndex) - previousIndex;

                    if (endIndex > 0)
                    {
                        string foundTrigger = tableText.Substring(previousIndex, endIndex);

                        if (foundTrigger != string.Empty)
                        {
                            triggerList.Add(foundTrigger);
                            tableText = tableText.Replace(foundTrigger, string.Empty);
                        }

                    }

                    // Remove the left over trigger shell.
                    if (tableText.StartsWith("{},"))
                    {
                        tableText = tableText.Remove(0, 3);
                    }
                    loop++;
                }
            }
            return triggerList;
        }


        public static string GetTableText(string rawText, string tableName)
        {

            // Clean the raw text
            //rawText = rawText.Replace("\r\n", string.Empty);

            rawText = FilterRawText(rawText);


            int startIndexName = rawText.IndexOf(tableName);
            int endIndexName = -1;

            if (startIndexName > -1)
            {
                for (int i = startIndexName; i < rawText.Length; i++)
                {
                    if (rawText[i] == '{')
                    {
                        endIndexName = i + 1;
                        break;
                    }
                }
            }

            if (endIndexName < startIndexName)
            {
                return string.Empty;
            }

            tableName = rawText.SelectString(startIndexName, endIndexName);

            int startIndex = rawText.IndexOf(tableName) + tableName.Length;
            int endIndex = FindClosingBracket(rawText, startIndex) - startIndex;

            if (endIndex < 0)
            {
                return string.Empty;
            }

            return rawText.Substring(startIndex, endIndex);
        }

        public static int FindClosingBracket(string text, int start)
        {
            int open = 0;
            for (int i = start; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '{')
                {
                    open++;
                }
                if (c == '}')
                {
                    open--;
                }

                if (open < 0)
                {
                    return i;
                }
            }
            return -1;
        }

        public static string FilterRawText(string rawText)
        {
            rawText = rawText.Replace("\r\n", string.Empty);
            rawText = rawText.Replace("\r", string.Empty);
            rawText = rawText.Replace("\n", string.Empty);
            rawText = rawText.Replace("\t", string.Empty);
            rawText = rawText.Replace(" ", string.Empty);

            return rawText;
        }


        public static string GetVariable(string rawText, string varName)
        {
            int start, end;

            if (!rawText.Contains(varName))
            {
                return "";
            }

            if (varName != "")
            {
                varName += "=";
            }

            start = rawText.IndexOf(varName) + varName.Length;

            // Is this variable a list/array
            if (rawText[start] == '{')
            {
                // Don't include the brackets
                end = FindNextChar(rawText, start, '}') - start - 1;
                start++;
            }
            else
            {
                end = FindNextChar(rawText, start, ',') - start;
            }

            return end < 0 ? string.Empty : rawText.Substring(start, end).Replace("\"", string.Empty);
        }

        private static int FindNextChar(string text, int start, char c)
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

    }



}

using System;
using System.Collections.Generic;
using System.Linq;
using SettingsNamespace;

using System.Windows.Media;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using HM4DesignTool;

namespace DataNameSpace
{
    using HM4DesignTool.Data;
    using HM4DesignTool.Level;



    public class Data
    {


        private List<String> patientTypeList = new List<String> { };
        public String StatusbarText = "";

        public Data()
        {
        }

        public List<String> GetPatientTypesFromDisk(bool reload = false)
        {
            if (patientTypeList == null || patientTypeList.Count() == 0 || reload)
            {
                String projectPath = Globals.GetSettings.projectPathImages + "patients\\";

                if (System.IO.Directory.Exists(projectPath))
                {
                    List<String> rawPatientTypeList = new List<String>(System.IO.Directory.GetDirectories(projectPath));
                    List<String> rawPatientList = new List<String> { };

                    foreach (String patientType in rawPatientTypeList)
                    {
                        rawPatientList.Add(patientType.Replace(projectPath, ""));
                    }

                    patientTypeList = rawPatientList;
                }

            }

            return patientTypeList;

        }



    }



}


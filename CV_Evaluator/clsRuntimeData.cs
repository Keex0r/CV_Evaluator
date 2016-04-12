using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CV_Evaluator.Import_Settings;

namespace CV_Evaluator
{
    public class clsRuntimeData
    {
        public clsRuntimeData()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.ImportSettings))
            {
                ImportSettings = new Import_Settings.ImportSettings();
            }
            else
            {
                ImportSettings = ImportSettings.FromXML(Properties.Settings.Default.ImportSettings);
                if (ImportSettings == null) ImportSettings = new ImportSettings();
            }
        }
        public Import_Settings.ImportSettings ImportSettings;
        
    }
}

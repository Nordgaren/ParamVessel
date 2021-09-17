using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowsBetterParamEditor
{
    class SavedParamRow
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public string RawData { get; set; }

        public SavedParamRow(string id, string name, string rawData)
        {
            ID = id;
            Name = name;
            RawData = rawData;
        }
    }

    class SavedParam
    {
        public string ParamName { get; set; }

        public List<SavedParamRow> SavedParams { get; set; }

        public SavedParam(string paramName, List<SavedParamRow> savedParams)
        {
            ParamName = paramName;

            SavedParams = savedParams;
        }
    }
}

using MeowDSIO.DataTypes.PARAM;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MeowsBetterParamEditor
{
    /// <summary>
    /// Interaction logic for BatchLoad.xaml
    /// </summary>
    public partial class BatchLoad : Window
    {
        ParamDataContext PARAMDATA;
        public BatchLoad(ParamDataContext paramdata)
        {
            InitializeComponent();
            PARAMDATA = paramdata;
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var index = Categories.SelectedIndex;
            //var selectedParamRow = PARAMDATA.Params[index].Value.Entries;
            //var selectedParam = PARAMDATA.Params[index];

            var browseDialog = new OpenFileDialog()
            {
                AddExtension = true,
                FileName = "",
                DefaultExt = ".json",
                Title = "Batch load file",
                Filter = "Json(*.json) | *.json"
            };

            if (browseDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;

            BatchLoadFile.Text = browseDialog.FileName;

            var batchSave = JsonConvert.DeserializeObject<SavedParam>(File.ReadAllText(BatchLoadFile.Text));

            Categories.SelectedItem = Categories.Items.Cast<PARAMRef>().FirstOrDefault(pd => pd.FancyDisplayName == batchSave.ParamName);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var param in PARAMDATA.Params)
            {
                Categories.Items.Add(param);
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var batchSave = JsonConvert.DeserializeObject<SavedParam>(File.ReadAllText(BatchLoadFile.Text));
             
            var selectedParam = Categories.SelectedItem as PARAMRef;

            if (selectedParam == null)
                return;

            foreach (var row in batchSave.SavedParams)

            {
                var newParamRow = new ParamRow();

                var index = selectedParam.Value.Count();

                try
                {
                    var hexBytes = row.RawData.Split(' ')
                    .Select(x => byte.Parse(x, System.Globalization.NumberStyles.HexNumber))
                    .ToArray();

                    if (hexBytes.Length == selectedParam.Value.EntrySize)
                    {
                        PARAMDATA.IsParamRowClipboardValid = true;
                    }

                    newParamRow.ID = long.Parse(row.ID);
                    newParamRow.Name = row.Name;
                    newParamRow.RawData = hexBytes;
                    newParamRow.LoadValuesFromRawData(selectedParam.Value);

                }
                catch
                {

                }

                selectedParam.Value.Entries.Insert(index, newParamRow);

                Close();
            }
        }
    }
}

using MeowDSIO.DataTypes.PARAM;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MeowsBetterParamEditor
{
    /// <summary>
    /// Interaction logic for BatchSave.xaml
    /// </summary>
    public partial class BatchSave : Window
    {
        ParamDataContext PARAMDATA;
        MainWindow Parent;

        public BatchSave(ParamDataContext paramdata, MainWindow parent)
        {
            InitializeComponent();
            PARAMDATA = paramdata;
            Parent = parent;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var param in PARAMDATA.Params)
            {
                Categories.Items.Add(param);
            }

            Categories.SelectedIndex = Parent.MainTabs.SelectedIndex;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var index = Categories.SelectedIndex;
            var selectedParamRow = PARAMDATA.Params[index].Value.Entries;
            var selectedParam = PARAMDATA.Params[index];
            var start = string.IsNullOrEmpty(StartParam.Text) ? 0 : long.Parse(StartParam.Text);
            var finish = string.IsNullOrEmpty(EndParam.Text) ? 0 : long.Parse(EndParam.Text);

            var browseDialog = new SaveFileDialog()
            {
                AddExtension = true,
                FileName = $"{selectedParam.FancyDisplayName} {start}-{finish}",
                DefaultExt = ".json",
                Title = "Batch save location/filename",
                Filter = "Json(*.json) | *.json"
            };

            if (browseDialog.ShowDialog() == false)
                return;
            

            //var selectedParamRow = ParamEntryList.SelectedItem as ParamRow;
            //var selectedParam = MainTabs.SelectedItem as PARAMRef;

            //if (selectedParam == null || selectedParamRow == null)
            //    return;
            var list = new List<SavedParamRow>();

            for (int i = 0; i < selectedParamRow.Count; i++)
            {
                if (selectedParamRow[i].ID >= start && selectedParamRow[i].ID <= finish)
                {
                    selectedParamRow[i].ReInitRawData(selectedParam.Value);
                    selectedParamRow[i].SaveValuesToRawData(selectedParam.Value);
                    var sb = new StringBuilder();
                    for (int j = 0; j < selectedParamRow[i].RawData.Length; j++)
                    {
                        if (j > 0)
                            sb.Append(" ");

                        sb.Append(selectedParamRow[i].RawData[j].ToString("X2"));
                    }

                    list.Add(new SavedParamRow(selectedParamRow[i].ID.ToString(), selectedParamRow[i].Name, sb.ToString()));
                }
            }

            var savedParam = new SavedParam(selectedParam.FancyDisplayName, list);

            var json = JsonConvert.SerializeObject(savedParam);

            File.WriteAllText(browseDialog.FileName, json);

            Close();
        }
    }
}

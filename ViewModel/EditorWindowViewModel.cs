using DataAnalyzer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DataAnalyzer.ViewModel
{
    public class EditorWindowViewModel
    {
        public ObservableCollection<Variaty> Variaties { get; set; }

        public EditorWindowViewModel(List<double> variaties)
        {
            Variaties = new ObservableCollection<Variaty>();
            variaties.Sort();

            for (int i = 0; i < variaties.Count; i++)
            {
                Variaty? item = Variaties.FirstOrDefault(item => item.Value == variaties[i]);

                if (item != null)
                {
                    int freq = ++Variaties[item.Id - 1].Frequency;
                    Variaties[item.Id - 1].RelativeFrequency = (double)freq / variaties.Count;
                }
                else
                {
                    Variaties.Add(new Variaty()
                    {
                        Id = Variaties.Count + 1,
                        Value = variaties[i],
                        Frequency = 1,
                        RelativeFrequency = (double)1 / variaties.Count,
                        ECDFValue = 0.0
                    });
                }
            }

            double step = 0.0;
            foreach (var item in Variaties)
            {
                item.ECDFValue = item.RelativeFrequency + step;
                step += item.RelativeFrequency;

                item.RelativeFrequency = Math.Round(item.RelativeFrequency, 4);
                item.ECDFValue = Math.Round(item.ECDFValue, 4);
            }
        }
    }
}

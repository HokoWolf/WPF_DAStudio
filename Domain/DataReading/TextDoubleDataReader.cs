using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace DataAnalyzer.Domain.DataReading
{
    public class TextDoubleDataReader : IDataReader<double>
    {
        public IList<double> GetData(string filename)
        {
            IList<double> data = new List<double>();

            try
            {
                using (TextReader reader = File.OpenText(filename))
                {
                    string? input;

                    while ((input = reader.ReadLine()) != null)
                    {
                        data.Add(double.Parse(input));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return data;
        }
    }
}

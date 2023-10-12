using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace DataAnalyzer.Domain
{
    public class BinaryDoubleDataReader : IDataReader<double>
    {
        public IList<double> GetData(string filename)
        {
            IList<double> data = new List<double>();

            try
            {
                using (BinaryReader reader = new(File.OpenRead(filename)))
                {
                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        try
                        {
                            data.Add(reader.ReadDouble());
                        }
                        catch (EndOfStreamException)
                        {
                            break;
                        }
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

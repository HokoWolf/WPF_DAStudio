using System.Collections.Generic;

namespace DataAnalyzer.Domain.DataReading
{
    public interface IDataReader<T>
    {
        IList<T> GetData(string filename);
    }
}

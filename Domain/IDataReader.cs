using System.Collections.Generic;

namespace DataAnalyzer.Domain
{
    public interface IDataReader<T>
    {
        IList<T> GetData(string filename);
    }
}

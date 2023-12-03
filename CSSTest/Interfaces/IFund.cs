using CSSTest.Models;

namespace CSSTest.Interfaces
{
    public interface IFund
    {
        public List<Fund> CreateFunds(int start, int finish);

    }
}

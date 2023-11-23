using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace CSSTest.Models
{
    public class Fund
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Value> FundValues { get; set; }

        public Fund() { }

        public Fund(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }

    public class Value
    {
        [Key]
        public int ValueId { get; set; }
        public DateTime ValueDate { get; set; }
        public double ValueDouble { get; set; }
        public int FundId { get; set; }
        public Fund Fund { get; set; }
        public Value() { }
    }

    public class DataHelper
    {
        public static IEnumerable<Fund> CreateFunds(List<string> listOfFields)
        {
            if(listOfFields.Count > 0)
            {

            }
            IEnumerable<Fund> result = new List<Fund>();

            result = new Fund[]
            {
                new Fund
                {
                  Id = 1,
                  Name = "Fund 1",
                  Description = "Description 1"

                }
            };

            return result;
        }

        public void CreateRandonData<T> (T data)
        {
            var type = typeof (T);
            string className = type.Name;
            switch (className)
            {
                case "Funds":
                    List<string> listOfFields = new List<string>();
                    FieldInfo[] fields = type.GetFields();
                    foreach (FieldInfo field in fields)
                    {
                        listOfFields.Add(field.Name);
                    }
                    CreateFunds(listOfFields);

                    break;

                    
                case "Value":
                    //do something
                    break;
                default: break;
            }
        }
    }
}

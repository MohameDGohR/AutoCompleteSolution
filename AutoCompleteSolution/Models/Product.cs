using Nest;

namespace AutoCompleteSolution.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
         public CompletionField Suggest { get; set; }
    }
}

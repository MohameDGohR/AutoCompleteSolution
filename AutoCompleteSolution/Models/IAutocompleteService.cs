using Nest;

namespace AutoCompleteSolution.Models
{
    public interface IAutocompleteService
    {
        Task<bool> CreateIndexAsync(string indexName);
        Task IndexAsync(string indexName, List<Product> products);
        Task<ProductSuggestResponse> SuggestAsync(string indexName, string keyword,string context);

        Task<IList<Product>> GetSearch(string keyword);
        Task DeleteDocument(Product pd);

        Task UpdateDocument(Product pd);

        Task Create(Product pd);

        Task<Product> GetDetails(int id);
        Task<ProductSuggestResponse> SuggestTerms(string keyword, string indexName);




    }
}

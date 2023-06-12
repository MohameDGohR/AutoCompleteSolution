using Elasticsearch.Net;
using Nest;
using System.Runtime.CompilerServices;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace AutoCompleteSolution.Models
{
    public class AutocompleteService : IAutocompleteService
    {
        readonly IElasticClient _elasticClient;

        public AutocompleteService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }
        public async Task<bool> CreateIndexAsync(string indexName)
        {
          

            if (_elasticClient.Indices.Exists(indexName.ToLowerInvariant()).Exists)
            {
                _elasticClient.Indices.Delete(indexName.ToLowerInvariant());
            }

            var createIndexResponse = await _elasticClient.Indices.CreateAsync(/*createIndexDescriptor*/indexName
                ,x=> x.Map<Product>(s=>s.Properties(ps => ps.Completion(c => c.Contexts(ctx => ctx.Category(c=>c.Name("country").Path("c"))).Name(p => p.Suggest))).Properties(ps => ps.Text(c => c.Name(p => p.Id)))
                .Properties(ps => ps.Text(c => c.Name(p => p.Name)))

                ));

            return createIndexResponse.IsValid;
        }

        public async  Task IndexAsync(string indexName, List<Product> products)
        {
            await _elasticClient.IndexManyAsync(products, indexName);
        }

        public async Task<ProductSuggestResponse> SuggestAsync(string indexName, string keyword, string context)
        {
            ISearchResponse<Product> searchResponse = await _elasticClient.SearchAsync<Product>(s => s
                                                .Index(indexName)
                                                .Suggest(su => su
                                                     .Completion("suggestions", c => c
                                                         
                                                         
                                                          .Field(f => f.Suggest)
                                                           .Prefix(keyword)
                                                            .Fuzzy(f => f
                                                              .Fuzziness(Fuzziness.EditDistance(4))
                                                        
                                                             // .PrefixLength(4)
                                                            // .MinLength(6)
                                                              
                                                          )
                                                         .SkipDuplicates(true)
                                                         .Contexts(co => co.Context("country",cd => cd.Context(context)))
                                                        
                                                          .Size(5))
                                                        ));

            var suggests = from suggest in searchResponse.Suggest["suggestions"]
                           from option in suggest.Options
                           select new ProductSuggest
                           {
                               Id = option.Source.Id,
                               Name = option.Source.Name,
                               SuggestedName = option.Text,
                               Score = option.Score
                           };

            return new ProductSuggestResponse
            {
                Suggests = suggests
            };
        }



        public async Task<IList<Product>> GetSearch(string keyword)
        {
            var result = await  _elasticClient.SearchAsync<Product>(s => s.Query(q => q.QueryString(d => d.Query('*' + keyword + '*'))).Size(5000));
            var finalResult = result;
            var finalContent = finalResult.Documents.ToList();
            return finalContent;
        }


     

       public async Task DeleteDocument(Product pd)
        {
            await _elasticClient.DeleteAsync<Product>(pd);
        }

        public async Task UpdateDocument(Product pd)
        {
            await _elasticClient.UpdateAsync<Product>(pd.Id, u => u
                .Index("product_suggest")
                 .Doc(pd));
        }

        public async Task Create(Product pd)
        {
           await   _elasticClient.IndexDocumentAsync<Product>(pd);
            
        }

        public async Task<Product> GetDetails(int id)
        {
            var res = await _elasticClient.GetAsync<Product>(id.ToString(), g => g.Index("product_suggest"));
            return res.Source;
        }


        public async Task<ProductSuggestResponse> SuggestTerms(string keyword,string indexName)
        {
            var searchResponse = await _elasticClient.SearchAsync<Product>(s => s.Index(indexName)
            .Suggest(su => su.Term("suggestions", f => f.Text(keyword).Field(s => s.Name).SuggestMode(SuggestMode.Popular).Size(5))));

           


            var suggests = from suggest in searchResponse.Suggest["suggestions"].ToList()
                           from option in suggest.Options.ToList()
                           select new ProductSuggest
                           {
                               SuggestedName = option.Text,
                               Score = option.Score
                           };

            return new ProductSuggestResponse
            {
                Suggests = suggests
            };
        }
    }
}

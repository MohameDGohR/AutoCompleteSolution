using AutoCompleteSolution.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace AutoCompleteSolution.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SuggestingController : ControllerBase
    {
        private readonly IAutocompleteService _auto;

        public SuggestingController(IAutocompleteService auto)
        {
            _auto = auto;
        }
        [HttpPost]
        public async Task<IActionResult> add()
        {
            List<Product> products = new List<Product>();

            products.Add(new Product()
            {
                Id = 1,
                Name = "Redmi 15",
                Suggest = new CompletionField()
                {
                    Input = new[] {"note 10"  },
                    Contexts = new Dictionary<string, IEnumerable<string>>
                {
                    { "country", new [] { "egypt", "china" } }
                }

                   
                }
            });

            products.Add(new Product()
            {
                Id = 2,
                Name = "Samsung Galaxy S8",
                Suggest = new CompletionField()
                {
                    Input = new[] { "Samsung Galaxy S8", "Galaxy S8", "S8" },
                    Contexts = new Dictionary<string, IEnumerable<string>>
                {
                    { "country", new [] {  "china" } }
                }
                }
            });

            products.Add(new Product()
            {
                Id = 3,
                Name = "Apple Iphone 8",
                Suggest = new CompletionField()
                {
                    Input = new[] { "Apple Iphone 8", "Iphone 8" },
                    Contexts = new Dictionary<string, IEnumerable<string>>
                {
                    { "country", new [] {  "china" } }
                }
                }
            });

            products.Add(new Product()
            {
                Id = 4,
                Name = "Apple Iphone X",
                Suggest = new CompletionField()
                {
                    Input = new[] { "Apple Iphone X", "Iphone X" },
                    Contexts = new Dictionary<string, IEnumerable<string>>
                {
                    { "country", new [] {"china" } }
                }
                }
            });

            products.Add(new Product()
            {
                Id = 5,
                Name = "Apple iPad Pro",
                Suggest = new CompletionField()
                {
                    Input = new[] { "Apple iPad Pro", "iPad Pro" },
                    Contexts = new Dictionary<string, IEnumerable<string>>
                {
                    { "country", new [] { "egypt" } }
                }
                }
            });
            var res = await  _auto.CreateIndexAsync("product_suggest");
            await  _auto.IndexAsync("product_suggest", products);

            return Ok();
        }

        [HttpGet]
        public async Task<ProductSuggestResponse> Get(string keyword, string context)
        {
            return await _auto.SuggestAsync("product_suggest", keyword, context);
        }


        [HttpGet]
        public async Task<IActionResult> GetDetails(int id)
        { 
        
         return Ok(await _auto.GetDetails(id));
        }


        [HttpGet]
        public async Task<IActionResult> GetAll(string KeyWord)
        {

            return Ok(await _auto.GetSearch(KeyWord));
        }



        [HttpGet]
        public async Task<ProductSuggestResponse> GetSuggestTerms(string keyword)
        {
            return await _auto.SuggestTerms(keyword, "product_suggest");
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            Product prod = await _auto.GetDetails(id);
             await  _auto.DeleteDocument(prod);

            return Ok();    
        }


        [HttpPut]
        public async Task<IActionResult> UpdateDocument(Product product)
        {
           await  _auto.UpdateDocument(product);
            return Ok();    
        
        }

    }
}

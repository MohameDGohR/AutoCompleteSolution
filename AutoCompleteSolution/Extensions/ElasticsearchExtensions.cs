using Nest;

namespace AutoCompleteSolution.Extensions
{
    public static class ElasticsearchExtensions
    {
        public static void AddElasticSearch(this IServiceCollection services,
           IConfiguration configuration)
        {
            var url = configuration["ELKConfiguration:url"];
            var defaultIndex = configuration["ELKConfiguration:index"];
            var settings = new ConnectionSettings(new Uri(url)).PrettyJson()
                .DefaultIndex(defaultIndex);
            var client = new ElasticClient(settings);
            services.AddSingleton<IElasticClient>(client);
        }
    }
}

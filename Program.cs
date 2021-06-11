using System;
using System.Threading.Tasks;
using NRediSearch;
using NRediSearch.Aggregation;
using StackExchange.Redis;

namespace SimplePagingExample
{
    class Program
    {
        static ConnectionMultiplexer _redis = ConnectionMultiplexer.Connect("localhost");

        static async Task Main(string[] args)
        {
            await WithSearch();
            await WithAggregation();            
        }

        /// <summary>
        /// Uses the Search API and the LIMIT field to paginate the results.
        /// Usage this way is not immune to new records messing up the results of pagination
        /// So it may be wise to design schema's in such a way to prevent newer records interfering with
        /// a paging query (e.g. have a created at field and exlude all records created after the start of the cursor).
        /// </summary>
        /// <returns></returns>
        public static async Task WithSearch()
        {
            int offset = 0;
            const int limit = 50;

            var db = _redis.GetDatabase();
            var client = new Client("books-idx", db);

            var query = new Query("*");
            query.Limit(offset, limit);

            var result = await client.SearchAsync(query);
            while (result.Documents.Count > 0)
            {
                Console.WriteLine(result.Documents[0]["title"]);
                offset += limit;
                query.Limit(offset, limit);
                result = await client.SearchAsync(query);
            }
        }

        /// <summary>
        /// Uses aggregation with <see href="https://oss.redislabs.com/redisearch/Aggregations/#cursor_api">cursor API</see>
        /// the cursor maintains the query in redis for 30000 milliseconds and spoon feeds it back
        /// to you as you consume the cursor. The LOAD parameter used can be less than ideal under heavier loads as it requires 
        /// that the query essentially perform an HREAD on each record
        /// </summary>
        /// <returns></returns>        
        public static async Task WithAggregation()
        {
            var db = _redis.GetDatabase();
            var bookClient = new Client("books-idx", db);
            var aggregation = new AggregationBuilder("*")
                .Load(new[] { "title" })
                .Cursor(50, 30000);
            var result = await bookClient.AggregateAsync(aggregation);
            while (result.CursorId != 0)
            {
                Console.WriteLine(result.GetRow(0).Value["title"]);
                result = await bookClient.CursorReadAsync(result.CursorId, 50);
            }
        }        
    }
}

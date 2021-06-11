# Simple Paging Examples with Redis Search

Provided in here are two simple pagination examples using [RediSearch](https://oss.redislabs.com/redisearch/) in .NET with the StackExcahnge [NRediSearch]https://github.com/StackExchange/StackExchange.Redis) library. They both opperate off of the [data set](https://github.com/redislabs-training/ru203/blob/main/README.md) used by Redis University's [ru203](https://university.redislabs.com/courses/ru203/) class.

## Sample 1 WithSeach

With [Search](https://oss.redislabs.com/redisearch/Commands/#ftsearch) uses a standard query + limit operation. this will work for relatively static data sets, or if there's an additional field that's indexed to further limit your query to block out new records

## Sample 2 WithAggregation

This uses the [Aggregate](https://oss.redislabs.com/redisearch/Commands/#ftaggregate) function within [RediSearch](https://oss.redislabs.com/redisearch/)
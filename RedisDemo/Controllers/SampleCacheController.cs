using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using RedisDemo.Models;

namespace RedisDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleCacheController : ControllerBase
    {
        private static readonly List<DemoData> demoDatas = new List<DemoData>
        {
            new DemoData{ EmployeeName="Donald Trump", Country="USA", PassportNo="USA001", Status="True", Enrolled=DateTime.Now},
            new DemoData{ EmployeeName="Boris Johansen", Country="UK", PassportNo="UK001", Status="True", Enrolled=DateTime.Now},
            new DemoData{ EmployeeName="Narendara Modi", Country="India", PassportNo="IND001", Status="True", Enrolled=DateTime.Now},
            new DemoData{ EmployeeName="Emmanuel Macron", Country="France", PassportNo="FR001", Status="True", Enrolled=DateTime.Now},
        };

        private IDistributedCache _redisCache;
        public SampleCacheController(IDistributedCache redisCache)
        {
            _redisCache = redisCache;
        }
        public IActionResult Index()
        {
            //1. Get data from the redis cache
            string sampleData = _redisCache.GetString("DemoData");

            //if cache is not present
            if (sampleData==null)
            {
                //Get Data from database here - used sample data for now.
                sampleData = JsonSerializer.Serialize(demoDatas);

                var cacheOptions = new DistributedCacheEntryOptions();
                //Set expiry time
                cacheOptions.SetAbsoluteExpiration(DateTimeOffset.Now.AddMinutes(2));
                //Add data to Redis Cache
                _redisCache.SetString("DemoData", sampleData, cacheOptions);
            }
            //If cache present it will return sampleData

            return Ok(sampleData);
        }
    }
}

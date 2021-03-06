using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OneLine.Extensions
{
    public static class ActionResultExtensions
    {
        public static IActionResult OutputJson(this ContentResult result, object data, bool ConverCamelCaseNaming = false)
        {
            if (ConverCamelCaseNaming)
            {
                DefaultContractResolver contractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };
                result.Content = JsonConvert.SerializeObject(data, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, ContractResolver = contractResolver });
            }
            else
            {
                result.Content = JsonConvert.SerializeObject(data, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            }
            result.ContentType = "application/json";
            result.StatusCode = 200;
            return result;
        }
    }
}


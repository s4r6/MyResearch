using System.Collections.Generic;
using Newtonsoft.Json;
using NUnit.Framework;
using UnityEngine;

namespace Infrastructure
{
    public class JsonListSerializer<T> : ISerializer<List<T>>
    {
        public List<T> Load(string json)
        {
            return JsonConvert.DeserializeObject<List<T>>(json);
        }
    }
}
using Domain.Action;
using Domain.Stage.Object;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

namespace Infrastructure.Repository
{
    public static class ChoiceFactory
    {
        public static Choice FromJson(JObject json)
        {
            var choice = new Choice
            {
                Label = json["Label"]?.ToString(),
                RiskId = json["RiskId"]?.ToString(),
                IsCorrect = json["IsCorrect"]?.ToObject<bool>() ?? true,
                OverrideActions = ParseOverrideActions(json["OverrideActions"]),
                ActionAttributes = ParseActionAttributes(json["ActionAttributes"]),
                ObjectAttributes = ParseObjectAttributes(json["ObjectAttributes"])
            };

            return choice;
        }

        public static List<Choice> FromJsonArray(JArray array)
        {
            return array.Select(token => FromJson((JObject)token)).ToList();
        }

        private static List<ActionEntity> ParseOverrideActions(JToken token)
        {
            var list = new List<ActionEntity>();
            if (token is not JArray array) return list;

            foreach (var item in array)
            {
                var target = item["target"]?.ToString() switch
                {
                    "Held" => TargetType.HeldItem,
                    _ => TargetType.HeldItem,
                };

                list.Add(new ActionEntity()
                {
                    id = item["id"]?.ToString(),
                    label = item["label"]?.ToString(),
                    riskChange = item["riskChange"]?.ToObject<int>() ?? 0,
                    actionPointCost = item["ActionPointCost"]?.ToObject<int>() ?? 0,
                    target = target,
                    ObjectAttributes = item["ObjectAttributes"]?.ToObject<List<string>>() ?? new(),
                });
            }

            return list;
        }

        private static List<ChoiceAttribute> ParseActionAttributes(JToken token)
        {
            var list = new List<ChoiceAttribute>();
            if (token is not JArray array) return list;

            foreach (var item in array)
            {
                list.Add(new ChoiceAttribute
                {
                    Name = item["name"]?.ToString(),
                    Target = Enum.TryParse(item["target"]?.ToString(), out TargetType parsed)
                                ? parsed
                                : TargetType.Self
                });
            }

            return list;
        }

        private static List<string> ParseObjectAttributes(JToken token)
        {
            return token is JArray array
                ? array.Select(x => x?.ToString()).Where(x => !string.IsNullOrEmpty(x)).ToList()
                : new List<string>();
        }
    }
}
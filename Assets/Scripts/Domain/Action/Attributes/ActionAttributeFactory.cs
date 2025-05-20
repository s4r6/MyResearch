using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Domain.Action
{
    public class ActionAttributeFactory
    {
        /// <summary>
        /// 文字列で指定された属性名を、NeedAttributeで評価するActionAttributeとして生成
        /// </summary>
        public static ActionAttribute CreateFromName(string name, TargetType target = TargetType.Self, int Priority = 5)
        {
            return new NeedAttribute(
                requiredAttributes: new List<string> { name },
                attributeSource: target, // デフォルトはSelf。Shredderなどは明示的に指定する
                priority: Priority
            );
        }

        /// <summary>
        /// JSONからtypeを見て複雑なActionAttributeを生成（NeedAttribute, DestroyOtherなど）
        /// </summary>
        public static ActionAttribute CreateFromJson(JObject json)
        {
            var type = json["type"]?.ToString();
            var priority = json["priority"]?.ToObject<int>() ?? 0;

            return type switch
            {
                "NeedAttribute" => CreateNeedAttribute(json, priority),
                "DestroyOther" => null,
                _ => throw new NotSupportedException($"Unknown ActionAttribute type: {type}")
            };
        }

        private static ActionAttribute CreateNeedAttribute(JObject json, int priority)
        {
            var attrList = json["param"]?["attributes"]?.ToObject<List<string>>() ?? new();
            var target = json["param"]?["target"]?.ToString() ?? "Self";
            var source = Enum.TryParse(target, out TargetType parsed)
                ? parsed
                : TargetType.Self;

            return new NeedAttribute(attrList, source, priority);
        }
    }
}
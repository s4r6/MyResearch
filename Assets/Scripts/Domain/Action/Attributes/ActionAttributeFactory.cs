using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Domain.Action
{
    public class ActionAttributeFactory
    {
        /// <summary>
        /// ������Ŏw�肳�ꂽ���������ANeedAttribute�ŕ]������ActionAttribute�Ƃ��Đ���
        /// </summary>
        public static ActionAttribute CreateFromName(string name, TargetType target = TargetType.Self, int Priority = 5)
        {
            return new NeedAttribute(
                requiredAttributes: new List<string> { name },
                attributeSource: target, // �f�t�H���g��Self�BShredder�Ȃǂ͖����I�Ɏw�肷��
                priority: Priority
            );
        }

        /// <summary>
        /// JSON����type�����ĕ��G��ActionAttribute�𐶐��iNeedAttribute, DestroyOther�Ȃǁj
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
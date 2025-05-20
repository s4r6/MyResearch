using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Component;
using Domain.Stage.Object;
using UnityEngine;

namespace Domain.Action
{
    public class ActionExecuter
    {
        //ActionAttributeを評価する
        public List<ActionAttribute> Evaluate(ObjectEntity target, ObjectEntity heldItem)
        {
            if (!target.TryGetComponent<ActionableComponent>(out var component))
                return new List<ActionAttribute>();

            return component.Attributes
                .OrderByDescending(attr => attr.Priority)
                .Where(attr => attr.IsMatch(target, heldItem))
                .ToList();
        }

        public void ExecuteFirstMatched(ObjectEntity target, ObjectEntity heldItem)
        {
            var matched = Evaluate(target, heldItem);
            if (matched.Count > 0)
            {
                matched.First().ApplyEffect(target, heldItem);
            }
        }

        public void ExecuteMatced(ObjectEntity target, ObjectEntity heldItem)
        {
            var matched = Evaluate(target, heldItem);
            // 優先度の高い順に評価し、最初にマッチしたものを実行
            foreach (var attr in matched)
            {

                Console.WriteLine($"[ActionExecutor] 条件一致 → 実行: {attr.GetType().Name}");
                attr.ApplyEffect(target, heldItem);
                return;

            }

            Console.WriteLine("[ActionExecutor] 実行可能なアクションが見つかりませんでした");
        }
    }
}
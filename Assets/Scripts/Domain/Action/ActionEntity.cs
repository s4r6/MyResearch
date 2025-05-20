using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Domain.Action
{
    public class ActionEntity
    {
        public string id;
        public string label;
        public int riskChange;
        public int actionPointCost;

        public TargetType target = TargetType.Self;
        public List<string> ObjectAttributes = new();
        public override string ToString()
        {
            return $"ActionEntity: {id}, {riskChange}, {actionPointCost}";
        }
    }
}
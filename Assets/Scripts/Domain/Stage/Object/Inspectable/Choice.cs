using System.Collections.Generic;
using UnityEngine;
using Domain.Action;

namespace Domain.Stage.Object
{
    public class Choice
    {
        public string Label { get; set; }
        public string RiskId { get; set; }
        public bool IsCorrect { get; set; } = true;

        public List<ActionEntity> OverrideActions { get; set; } = new();
    }
}
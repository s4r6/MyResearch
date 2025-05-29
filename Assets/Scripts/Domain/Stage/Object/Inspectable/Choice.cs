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
        public List<ChoiceAttribute> ActionAttributes { get; set; } = new(); // optional
        public List<string> ObjectAttributes { get; set; } = new(); // ??’Ç‰Á
    }

    public class ChoiceAttribute
    {
        public string Name { get; set; }
        public TargetType Target { get; set; } = TargetType.Self;
    }
}
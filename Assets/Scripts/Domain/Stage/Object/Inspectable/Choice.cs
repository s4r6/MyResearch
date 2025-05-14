using System.Collections.Generic;
using UnityEngine;
using Domain.Action;

namespace Domain.Stage.Object
{
    public class Choice
    {
        public string label;
        public string riskId;
        public bool isCorrect;
        public List<ActionEntity> OverrideActions;
    }
}
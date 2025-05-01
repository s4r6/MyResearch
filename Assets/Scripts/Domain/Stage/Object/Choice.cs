using System.Collections.Generic;
using UnityEngine;
using Domain.Action;

namespace Domain.Stage.Object
{
    public class Choice
    {
        public readonly string label;
        public readonly string riskId;
        public readonly bool isCorrect;
        public readonly List<ActionEntity> availableActions; 

        public Choice(string label, string riskId, bool isCorrect, List<ActionEntity> availableActions)
        {
            this.label = label;
            this.riskId = riskId;
            this.isCorrect = isCorrect;
            this.availableActions = availableActions;
        }
    }
}
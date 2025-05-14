using System.Collections.Generic;
using System;
using UnityEngine;
using Domain.Stage.Object;

namespace Infrastructure
{
    [Serializable]
    public class ActionEffect
    {
        public string ActionEffectId;
        public string BaseActionId;
        public int riskChange;
        public int ActionPointCost;
        public bool IsSuccess;
    }

    [Serializable]
    public class StageObject
    {
        public string ObjectId;
        public string DisplayName;
        public string Description;
        public List<string> ChoiceIds;
    }
}

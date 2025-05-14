using System.Collections.Generic;
using System.Linq;
using Domain.Player;
using Domain.Stage.Object;
using NUnit.Framework;
using UnityEngine;

namespace Domain.Action
{
    public class ActionRuleService
    {

        public List<string> FindAvailableActionIds(List<string> actionIds, ObjectEntity obj, ObjectEntity lookObjctId)
        {
            List<string> result = new List<string>();
            foreach (var actionId in actionIds) 
            { 
                switch(actionId)
                {
                    case "ShredderUse":
                        if(obj == null) break;
                        if(obj.ObjectId == "Memo")
                        {
                            if(obj.availableActionIds.Contains("ShredderUse"))
                                result.Add(actionId);
                        }
                        break;
                }
            }

            return result;
        }
    }
}


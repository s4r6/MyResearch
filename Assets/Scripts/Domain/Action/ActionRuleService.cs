using UnityEngine;

namespace Domain.Action
{
    public enum ResultIds
    { 
        Unknown,
        Destroy,
        Lock,
        Unavailable
    }
    public struct ExecuteResultData
    {
        public readonly ResultIds result;
        public readonly string? targetObjectId;
        public readonly string? executeObjectId;

        public ExecuteResultData(ResultIds actionId, string? executeId, string? targetId)
        {
            result = actionId;
            targetObjectId = targetId;
            executeObjectId = executeId;
        }
    }

    public class ActionRuleService
    {
        public ActionRuleService() 
        {
            
        }

        public ExecuteResultData Execute(string actionId, string? executeObjectId, string? targetObjectId) 
        {
            switch (actionId) 
            {
                case "ShredderUse":
                    var shredderResult = new ExecuteResultData(ResultIds.Destroy, executeObjectId, targetObjectId);
                    return shredderResult;

                case "TrashBin":
                    var trashResult = new ExecuteResultData(ResultIds.Destroy, executeObjectId, targetObjectId);
                    return trashResult;

                case "LockPC":
                    var lockPCResult = new ExecuteResultData(ResultIds.Lock, executeObjectId, targetObjectId);
                    return lockPCResult;
            }

            return new ExecuteResultData(ResultIds.Unknown, executeObjectId, targetObjectId);
        }

        
    }
}


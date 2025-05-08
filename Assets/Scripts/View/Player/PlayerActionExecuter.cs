using Domain.Action;
using Unity.VisualScripting;
using UnityEngine;

namespace View.Player
{
    public class PlayerActionExecuter : MonoBehaviour
    {
        //--------------------VIEW---------------------
        public void DestroyObject(string objectId)
        {
            var obj = GameObject.Find(objectId);
            if (obj != null) 
            {
                Destroy(obj.gameObject);
            }

        }

        //--------------------PRESENTER--------------------
        public void ActionExecute(ResultIds result, string? executeId, string? targetId) 
        { 
            switch(result)
            {
                case ResultIds.Destroy:
                    DestroyObject(targetId);
                    break;
            }
        }

    }
}
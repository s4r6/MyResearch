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
        public void ActionExecute(string actionId, string targetId) 
        { 
            switch(actionId)
            {
                case "ShredderUse":
                    DestroyObject(targetId);
                    break;
            }
        }
    }
}
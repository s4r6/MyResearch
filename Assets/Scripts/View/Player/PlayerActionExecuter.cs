using Domain.Action;
using Unity.VisualScripting;
using UnityEngine;
using View.Stage;

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

        public void OffMonitor(string objectId)
        {
            var obj = GameObject.Find(objectId);
            if (obj != null)
            {
                obj.GetComponent<Monitor>().TurnOff();
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

                case "Shutdown":
                    OffMonitor(targetId);
                    break;
                    
            }
        }
    }
}
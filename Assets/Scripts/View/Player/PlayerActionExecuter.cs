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
        public void ActionExecute(string actionId, string heldId, string lookingId) 
        {
            Debug.Log("“K—p‚·‚éAction:"+ actionId);
            switch(actionId)
            {
                case "ShredderUse":
                    DestroyObject(heldId);
                    break;
                case "TrashBin":
                    DestroyObject(heldId);  
                    break;
                case "Shutdown":
                    OffMonitor(lookingId);
                    break;
                case "StoreCabinet":
                    DestroyObject(heldId);
                    break;
                case "PlaceCamera":
                    break;
                    
            }
        }
    }
}
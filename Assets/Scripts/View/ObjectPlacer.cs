using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace View.Stage
{
    public class ObjectPlacer : MonoBehaviour
    {
        [SerializeField]
        List<GameObject> Objects;

        [SerializeField]
        Transform PlacePosition;

        void Start()
        {
            foreach (var obj in Objects) 
            { 
                obj.SetActive(false);
            }  
        }

        public void PlaceObject(string objectId)
        {
            var obj = Objects.Find(p => p.name == objectId);
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }
}
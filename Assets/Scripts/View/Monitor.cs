using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace View.Stage
{
    public class Monitor : MonoBehaviour
    {
        [SerializeField]
        Renderer render;

        public void TurnOff()
        {
            var offMonitorMaterial = Resources.Load<Material>("ThirdParty/Office Room Furniture/Office/Materials/Monitor_1_off");
            Debug.Log(offMonitorMaterial.name);
            var materials = render.materials;
            for (int i = 0; i < materials.Length; i++) 
            { 
                Debug.Log(materials[i].name);
                if(materials[i].name.Contains("Monitor_1_on (Instance)"))
                {
                    Debug.Log("”­Œ©");
                    materials[i] = offMonitorMaterial;
                }
            }

            render.materials = materials;
        }
    }
}
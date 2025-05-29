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
            var materials = render.materials;
            for (int i = 0; i < materials.Length; i++) 
            { 
                if(materials[i].name.Contains("Monitor_1_on (Instance)"))
                {
                    materials[i] = offMonitorMaterial;
                }
            }

            render.materials = materials;
        }
    }
}
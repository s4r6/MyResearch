using UnityEngine;

namespace View.Stage.Object
{
    public class ActionableObject : MonoBehaviour
    {
        [SerializeField]
        string actionId;

        public string Id => actionId;
    }
}


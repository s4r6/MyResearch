using System.Collections.Generic;
using UnityEngine;

namespace Domain.Stage.Object
{


    /// <summary>
    /// �����\�ȃI�u�W�F�N�g��\��
    /// </summary>
    public class InspectableObject
    {
        public readonly string id;
        public readonly string name;
        public readonly string description;
        public readonly Dictionary<string, Choice> choices;
        public Choice selectedChoice = null;
        
        public InspectableObject(string id, string name, string description, Dictionary<string, Choice> choices)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.choices = choices;
        }

        public void SelectChoice(Choice choice)
        {
            selectedChoice = choice;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
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

        public void SelectChoice(string choiceText)
        {
            if (selectedChoice != null)
                return;

            var choice = choices.Values.ToList()
                                .Find(x => x.label == choiceText);
            
            Debug.Log(choice.availableActions.Count);

            selectedChoice = choice;
            Debug.Log(selectedChoice);
        }
    }
}
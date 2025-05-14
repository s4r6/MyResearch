using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Domain.Stage.Object
{
    /// <summary>
    /// �����\�ȃI�u�W�F�N�g��\��
    /// </summary>
    public class InspectableObject : ObjectEntity
    {
        public List<Choice> choices;
        public Choice selectedChoice = null;
        
        public InspectableObject(string id, string name, string description, List<Choice> choices)
            :base(id, name, description, new List<string>())
        {
            this.choices = choices;
        }

        public void SelectChoice(string choiceText)
        {
            if (selectedChoice != null)
                return;

            var choice = choices.Find(x => x.label == choiceText);

            selectedChoice = choice;
            availableActionIds = selectedChoice.OverrideActions.Select(x => x.id).ToList();
            Debug.Log(selectedChoice);
        }

        public override bool IsInspectable => true;
    }
}
using Domain.Stage.Object;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Domain.Component
{
    public class InspectableComponent : GameComponent
    {
        public string DisplayName;
        public string Description;
        public List<Choice> Choices = new();

        // 選択されたChoiceを保持（UseCaseで更新される）
        public Choice SelectedChoice;

        public bool IsActioned = false;

        internal object FindAll()
        {
            throw new NotImplementedException();
        }
    }
}
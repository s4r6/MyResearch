using Domain.Stage.Object;
using System.Collections.Generic;
using UnityEngine;

namespace Domain.Component
{
    public class ChoicableComponent : GameComponent
    {
        public List<Choice> Choices = new();
        // 選択されたChoiceを保持（UseCaseで更新される）
        public Choice SelectedChoice;
    }
}
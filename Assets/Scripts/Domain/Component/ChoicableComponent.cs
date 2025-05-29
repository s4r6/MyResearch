using Domain.Stage.Object;
using System.Collections.Generic;
using UnityEngine;

namespace Domain.Component
{
    public class ChoicableComponent : GameComponent
    {
        public List<Choice> Choices = new();
        // �I�����ꂽChoice��ێ��iUseCase�ōX�V�����j
        public Choice SelectedChoice;
    }
}
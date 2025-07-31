using Domain.Stage.Object;
using Domain.Player;
using View.UI;
using System.Linq;
using System;
using Domain.Component;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace UseCase.Player
{
    public struct InspectData
    {
        public string DisplayName {  get; set; }
        public string Description { get; set; }
        public List<string> ChoiceLabels { get; set; }
        public string SelectedLabel {  get; set; }
        public bool IsSelectable {  get; set; }
    }

    public interface IInspectPresenter
    {
        public UniTask StartInspect(InspectData data, Action<string> onEnd);
    }

    public class PlayerInspectUseCase : IInspectUseCase
    {
        IObjectRepository repository;
        PlayerEntity entity;
        IInspectPresenter presenter;
        InspectService inspectService;

        ObjectEntity currentInspectObject;

        public PlayerInspectUseCase(PlayerEntity entity, IInspectPresenter presenter, InspectService inspectService, IObjectRepository repository)
        {
            this.entity = entity;
            this.presenter = presenter;
            this.inspectService = inspectService;
            this.repository = repository;
        }

        public bool CanInspect(string objectId)
        {
            var entity = repository.GetById(objectId);
            return inspectService.CanInspect(entity);
        }

        Action OnCompleteInspect;
        public bool TryInspect(string objectId, Action onComplete)
        {
            OnCompleteInspect = onComplete;

            //Entity取得
            ObjectEntity obj = repository.GetById(objectId);

            //調査可能か確認
            if (!inspectService.CanInspect(obj)) return false;

            currentInspectObject = obj;

            //リスク候補を取得
            var choicable = inspectService.TryGetChoice(obj);
            
            var Inspectable = obj.GetComponent<InspectableComponent>();
            var dto = new InspectData
            {
                DisplayName = Inspectable.DisplayName,
                Description = Inspectable.Description,
                ChoiceLabels = choicable?.Choices?.Select(x => x.Label).ToList() ?? null,
                SelectedLabel = choicable?.SelectedChoice?.Label ?? string.Empty,
                IsSelectable = !Inspectable.IsActioned
            };

            //調査画面を表示
            presenter.StartInspect(dto, result => OnEndInspect(result)).Forget();

            return true;
        }
        
        public UniTask OnEndInspect(string choiceText)
        {
            if (!string.IsNullOrEmpty(choiceText))
            {
                inspectService.ApplySelectedChoice(currentInspectObject, choiceText);
            }

            OnCompleteInspect?.Invoke();
            OnCompleteInspect = null;

            Debug.Log("Inspect終了");
            return UniTask.CompletedTask;
        }
    }
}
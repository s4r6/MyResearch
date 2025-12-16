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
using UniRx;
using Domain.Tutorial;

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
        IGameRestriction restriction;

        ObjectEntity currentInspectObject;

        Subject<string> onInspect = new();
        Subject<string> onRiskSelected= new();

        List<string> AllowOnlyObject = new();
        public IObservable<string> OnInspected => onInspect;
        public IObservable<string> OnRiskSelected => onRiskSelected;

        public PlayerInspectUseCase(PlayerEntity entity, IInspectPresenter presenter, InspectService inspectService, IObjectRepository repository, IGameRestriction restriction = null)
        {
            this.entity = entity;
            this.presenter = presenter;
            this.inspectService = inspectService;
            this.repository = repository;

            this.restriction = restriction == null ? new NoRestriction() : restriction;
        }

        public void LimitInspectableObject(string id)
        {
            AllowOnlyObject.Clear();
            AllowOnlyObject.Add(id);
        }

        public void AllowAllInspctableObject()
        {
            AllowOnlyObject.Clear();
        }

        public bool CanInspect(string objectId)
        {
            if(!restriction.CanInspect())
            {
                return false;
            }

            if (AllowOnlyObject.Count >= 1 && !AllowOnlyObject.Contains(objectId))
            {
                return false;
            }

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
            if (!inspectService.CanInspect(obj) || !restriction.CanInspect()) return false;

            if (AllowOnlyObject.Count >= 1 && !AllowOnlyObject.Contains(objectId)) return false;

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

            onInspect.OnNext(Inspectable.DisplayName);
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

            onRiskSelected.OnNext("");
            OnCompleteInspect?.Invoke();
            OnCompleteInspect = null;

            Debug.Log("Inspect終了");
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// チュートリアル用など、UIを通さずにリスクを確定させたいときに使う
        /// </summary>
        public void ForceSelectRisk(string objectId, string choiceLabel)
        {
            // 対象オブジェクト取得
            var obj = repository.GetById(objectId);
            if (obj == null)
            {
                Debug.LogWarning($"ForceSelectRisk: object not found: {objectId}");
                return;
            }

            // リスクを直接適用
            inspectService.ApplySelectedChoice(obj, choiceLabel);

            // 必要であれば currentInspectObject も更新しておく
            currentInspectObject = obj;

            // 通常の選択完了と同じイベントを流す（チュートリアル側がこれを見る）
            onRiskSelected.OnNext(objectId);

            Debug.Log($"ForceSelectRisk: {objectId} に '{choiceLabel}' を適用しました。");
        }
    }
}
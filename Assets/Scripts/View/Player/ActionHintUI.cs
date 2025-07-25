using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Presenter.Player;
using UnityEngine;
using UseCase.Network.DTO;

namespace View.Player
{

    public class ActionHintUI : MonoBehaviour, IActionHintView
    {
        [SerializeField]
        GameObject Move;
        [SerializeField]
        GameObject Inspect;
        [SerializeField]
        GameObject Action;
        [SerializeField]
        GameObject Document;
        [SerializeField]
        GameObject SelectRisk;
        [SerializeField]
        GameObject SelectAction;
        [SerializeField]
        GameObject Select;
        [SerializeField]
        GameObject Cancel;
        [SerializeField]
        GameObject Interact;

        [SerializeField] float spacing = 30f;
        [SerializeField] RectTransform KeyConfigSpace;

        Dictionary<ActionHintId, GameObject> hintMap;
        List<RectTransform> activeHints = new();
        private void Awake()
        {
            hintMap = new Dictionary<ActionHintId, GameObject>
            {
                { ActionHintId.Move, Move },
                { ActionHintId.Interact, Interact },
                { ActionHintId.Inspect, Inspect },
                { ActionHintId.Action, Action },
                { ActionHintId.Document, Document },
                { ActionHintId.Select, Select },
                { ActionHintId.Cancel, Cancel },
                { ActionHintId.SelectRisk, SelectRisk },
                { ActionHintId.SelectAction, SelectAction },
            };
        }

        public void ShowHintList(IEnumerable<ActionHint> hints)
        {
            var activeIds = hints.Select(h => h.Id).ToList();

            activeHints.Clear();

            foreach(var pair in hintMap)
            {
                bool shouldShow = activeIds.Contains(pair.Key);
                pair.Value.SetActive(shouldShow);

                if(shouldShow)
                {
                    var rect = pair.Value.GetComponent<RectTransform>();
                    activeHints.Add(rect);
                }
            }

            LayoutHints();
        }

        void LayoutHints()
        {
            if (activeHints.Count == 0) return;

            // ‘S‘Ì•‚ÌŒvŽZ
            float totalWidth = activeHints.Sum(rt => rt.rect.width);
            float totalSpacing = spacing * (activeHints.Count - 1);
            float layoutWidth = KeyConfigSpace.rect.width;

            float startX = -layoutWidth / 2f;
            float currentX = startX;

            foreach (var rect in activeHints)
            {
                float width = rect.rect.width;
                rect.anchoredPosition = new Vector2(currentX + width / 2f, rect.anchoredPosition.y);
                currentX += width + spacing;
            }
        }

    }
}
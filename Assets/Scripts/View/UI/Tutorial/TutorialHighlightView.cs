using NUnit.Framework;
using Presenter.Tutorial;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TutorialHighlightView : MonoBehaviour, ITutorialHighlightView
{
    [SerializeField] private List<GameObject> targets = new();
    GameObject current;
    public void Highlight(string targetId)
    {
        // 既存ハイライトを解除
        ClearHighlight();

        if (string.IsNullOrEmpty(targetId)) return;

        // IDで該当ターゲットを検索
        var target = targets.Find(obj => obj.name == targetId);
        if (target == null) return;

        target.SetActive(true);
        current = target;
    }

    public void ClearHighlight()
    {
        if (current != null)
        {
            current.SetActive(false);
            current = null;
        }
    }
}

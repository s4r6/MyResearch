using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace Presenter.Tutorial
{
    public interface ITutorialWindowView
    {
        /// <summary>チュートリアル用メッセージとUIヒントを表示</summary>
        void Show(string message, string uiHint);
        void HideHint();
        /// <summary>ウィンドウを閉じる（アニメーションがあればここで）</summary>
        UniTask HideAsync(CancellationToken token = default);

    }
}
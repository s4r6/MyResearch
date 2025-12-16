using System;
using UnityEngine;

public interface ITutorialGameEvents
{
    /// <summary>
    /// チュートリアル用の「環境資料」が開かれたときに発火するイベント
    /// </summary>
    event Action EnvironmentDocumentOpened;
    event Action EnvironmentDocumentClosed;
    event Action<string> ObjectInspected;
    event Action<string> RiskSelected; // 引数: 対象オブジェクトID
    event Action<string> ActionListOpened;
    event Action<string> ActionSelected;
    event Action<string> ObjectHeld;
    event Action EndGame;
}

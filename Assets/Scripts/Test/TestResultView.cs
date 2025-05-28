using System.Collections.Generic;
using Domain.Stage;
using NUnit.Framework;
using UnityEngine;
using View.UI;

public class TestResultView : MonoBehaviour
{
    [SerializeField]
    ResultView view;
    List<RiskAssessmentHistory> histories = new()
{
    // 初回：最大50 → -15 減って 35
    new RiskAssessmentHistory("PC01", "パスワード貼付", "パスワードを剥がした", -15, 35, 50, 3, 7, 10),

    // 次：35 → -10 減って 25
    new RiskAssessmentHistory("サーバールーム扉", "開放状態", "施錠した", -10, 25, 50, 2, 5, 10),

    // 次：25 → -8 減って 17
    new RiskAssessmentHistory("USBメモリ", "持ち出し制限なし", "回収して保管した", -8, 17, 50, 1, 4, 10),

    // 次：17 → -5 減って 12
    new RiskAssessmentHistory("書庫01", "赤本が無施錠", "鍵をかけた", -5, 12, 50, 2, 2, 10),

    // 次：12 → -6 減って 6（最終状態）
    new RiskAssessmentHistory("管理PC", "画面ロックなし", "画面ロックを設定した", -6, 6, 50, 1, 1, 10)
};

    private void Start()
    {
        view.ShowResultWindow(histories);
    }
}

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
    new RiskAssessmentHistory(
        "PC01",
        "パスワード貼付",
        "パスワードを剥がした",
        -15, 35, 50,
        3, 7, 10,
        "PC本体に貼られていたパスワードを剥がすことで、第三者による不正ログインリスクを大幅に軽減した。"),

    // 次：35 → -10 減って 25
    new RiskAssessmentHistory(
        "サーバールーム扉",
        "開放状態",
        "施錠した",
        -10, 25, 50,
        2, 5, 10,
        "誰でも立ち入れる状態だったサーバールームを施錠し、物理的な不正アクセスのリスクを低下させた。"),

    // 次：25 → -8 減って 17
    new RiskAssessmentHistory(
        "USBメモリ",
        "持ち出し制限なし",
        "回収して保管した",
        -8, 17, 50,
        1, 4, 10,
        "制限なく持ち出されていたUSBメモリを適切に回収・保管することで、情報漏洩のリスクを軽減した。"),

    // 次：17 → -5 減って 12
    new RiskAssessmentHistory(
        "書庫01",
        "赤本が無施錠",
        "鍵をかけた",
        -5, 12, 50,
        2, 2, 10,
        "重要書類である赤本が保管された書庫を施錠し、内部情報の不正取得リスクを減少させた。"),

    // 次：12 → -6 減って 6（最終状態）
    new RiskAssessmentHistory(
        "管理PC",
        "画面ロックなし",
        "画面ロックを設定した",
        -6, 6, 50,
        1, 1, 10,
        "無防備だった管理PCに画面ロックを設定し、席を離れた際の不正操作リスクを抑えた。")
};

    private void Start()
    {
        view.ShowResultWindow(histories);
    }
}

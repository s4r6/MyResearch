using System;
using System.Collections.Generic;
using Domain.Stage;
using UnityEngine;
using View.UI;

public class TestResultView : MonoBehaviour
{
    [SerializeField]
    ResultView view;
    SurmmaryDTO surmmary = new()
    {
        time = TimeSpan.FromMinutes(12.4),
        FindRiskNum = 6,
        MaxRiskNum = 30,
        ExecuteCorrectActionNum = 5,
        MaxCorrectActionNum = 8,
        CurrentRisk = 20,
        MaxRisk = 60,
        CurrentActionPoint = 8,
        MaxActionPoint = 20,

        Actions = new List<SurmmaryDetailDTO>
        {
            new SurmmaryDetailDTO
            {
                DisplayName = "一般教務用PC01",
                RiskLabel = "不正操作・内部不正のリスク",
                ActionLabel = "自動ロックを設定",
                RiskChange = -6,
                ActionCost = 1,
                Explanation = "自動ロックは、一定時間操作がない場合に端末を自動でロックすることで、不正操作や内部不正のリスクを確実に軽減できる効果的な対応策です。設定さえ行えば常に機能するため、手動操作に依存せず、シャットダウンと比較して現実的かつ継続的に端末を保護できる点で、より適切な対応策といえます。",
                Description = "一般業務用に使われているPC。起動には8桁のパスワードが必要である。電源はついたまま。",
                RiskLabels = new List<string>()
                {
                    "不正操作・内部不正のリスク",
                    "全端末乗っ取りのリスク",
                    "盗難時の情報漏えいリスク",
                    "既知の脆弱性からの侵入リスク"
                },
                /*ActionLabels = new List<string>()
                {
                    "シャットダウン",
                    "自動ロックを設定"
                }*/
            },
            new SurmmaryDetailDTO
            {
                DisplayName = "会議室端末",
                RiskLabel = "自動ロックが無効",
                ActionLabel = "自動ロックを有効にする",
                RiskChange = -8,
                ActionCost = 3,
                Explanation = "一定時間操作がない場合に自動でロックされるよう設定した。"
            },
            new SurmmaryDetailDTO
            {
                DisplayName = "文書庫ドア",
                RiskLabel = "解錠状態で放置されている",
                ActionLabel = "オートロック機能を設定する",
                RiskChange = -12,
                ActionCost = 5,
                Explanation = "施錠忘れを防止するために自動施錠機能を導入した。"
            },
            new SurmmaryDetailDTO
            {
                DisplayName = "USBメモリ",
                RiskLabel = "個人情報ファイルが暗号化されていない",
                ActionLabel = "ディスク暗号化を有効にする",
                RiskChange = -10,
                ActionCost = 3,
                Explanation = "持ち出し時の情報漏えいリスクを軽減した。"
            },
            new SurmmaryDetailDTO
            {
                DisplayName = "ごみ箱",
                RiskLabel = "機密文書がシュレッダーされていない",
                ActionLabel = "シュレッダーを使用する",
                RiskChange = -10,
                ActionCost = 2,
                Explanation = "重要書類を安全に破棄した。"
            }
        }

    };



    private void Start()
    {
        view.ShowResultWindow(surmmary);
    }
}

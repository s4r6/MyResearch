using Cysharp.Threading.Tasks;
using Domain.Tutorial;
using System.Threading;
using UnityEngine;
using UseCase.Player;

namespace Presenter.Tutorial
{
    public interface ITutorialHighlightView
    {
        void Highlight(string targetId);   // 対象を光らせる
        void ClearHighlight();            // 全ハイライト解除
    }

    public class TutorialFlowController
    {
        readonly TutorialPhaseState _phase;
        readonly ITutorialWindowView _windowView;
        readonly ITutorialInput _tutorialInput;
        readonly IActionMapController _actionMapController;
        readonly ITutorialGameEvents _gameEvents;
        readonly ITutorialHighlightView _highlightView;
        readonly PlayerInspectUseCase _inspectUseCase;

        string _cachedActionMapName;
        // Phase2 で調べさせたい対象のID（SOから注入してもよい）
        readonly string _phase2TargetId;
        readonly string _secondObjectTargetId; // 別オブジェクト用
        readonly string _finalRiskObjectId;

        public TutorialFlowController(
            TutorialPhaseState phase,
            ITutorialWindowView windowView,
            ITutorialInput tutorialInput,
            IActionMapController actionMapController,
            ITutorialGameEvents gameEvents,
            ITutorialHighlightView highlightView,
            PlayerInspectUseCase inspectUseCase,
            string firstObjectTargetId,
            string secondObjectTargetId,
            string finalRiskObjectId
        )
        {
            _phase = phase;
            _windowView = windowView;
            _tutorialInput = tutorialInput;
            _actionMapController = actionMapController;
            _gameEvents = gameEvents;
            _highlightView = highlightView;
            _inspectUseCase = inspectUseCase;
            _phase2TargetId = firstObjectTargetId;
            _secondObjectTargetId = secondObjectTargetId;
            _finalRiskObjectId = finalRiskObjectId;

            _phase.AdvanceTo(TutorialPhase.Phase0_Intro);
        }

        public async UniTask RunAllAsync(CancellationToken token = default)
        {
            await RunPhase0Async(token);
            await RunPhase1Async(token);
            await RunPhase2Async(token);
            await RunPhase3Async(token);
            await RunPhase4Async(token);
            await RunPhase5Async(token);
            await RunPhase6Async(token);
            await RunPhase7Async(token);
            await RunPhase8Async(token);
            await RunPhase9Async(token);
            await RunPhase10Async(token);
            await RunPhase11Async(token);
            await RunPhase12Async(token);
            await RunPhase13Async(token);
            // この後に Phase2? を足していく
        }

        async UniTask RunPhase0Async(CancellationToken token)
        {
            //現在のActionMapを退避
            _cachedActionMapName = _actionMapController.CurrentActionMapName;

            //Tutorialマップに切り替え
            _actionMapController.SwitchTo("Tutorial");

            //説明文とUIヒントを表示
            var message =
                "ここでは、リスクアセスメントとリスク対応の進め方を練習します。\n" +
                "まずは基本操作と考え方の流れを、段階的に体験していきましょう。";

            var uiHint = "";

            _windowView.Show(message, uiHint);

            //Next（ボタン or キー）を待つ
            var tcs = new UniTaskCompletionSource();

            void OnNext()
            {
                if (!tcs.Task.Status.IsCompleted())
                {
                    tcs.TrySetResult();
                }
            }

            _tutorialInput.NextRequested += OnNext;

            try
            {
                using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token))
                {
                    linkedCts.Token.Register(() =>
                    {
                        if (!tcs.Task.Status.IsCompleted())
                        {
                            tcs.TrySetCanceled();
                        }
                    });

                    await tcs.Task;
                }
            }
            finally
            {
                _tutorialInput.NextRequested -= OnNext;
            }

            //ウィンドウを閉じる
            await _windowView.HideAsync(token);

            //ActionMapを元に戻す
            if (!string.IsNullOrEmpty(_cachedActionMapName))
            {
                _actionMapController.SwitchTo(_cachedActionMapName);
            }

            _phase.AdvanceTo(TutorialPhase.Phase1_ReadDocument);
        }

        async UniTask RunPhase1Async(CancellationToken token)
        {
            //
            // サブステップA：説明ウィンドウ＋Tutorial操作
            //
            var previousMap = _actionMapController.CurrentActionMapName;
            _actionMapController.SwitchTo("Tutorial");

            var message =
                "最初のステップは“資料を読むこと”です。\n" +
                "実際の作業現場では、事前の情報が判断の土台になります。\n" +
                "まずはTabキーで環境資料を開いて、内容を確認してみましょう。";

            var uiHintInWindow = "環境資料を確認する";

            _windowView.Show(message, uiHintInWindow);

            // ウィンドウ上のNextボタン or Tutorial/Next入力を待つ
            var nextTcs = new UniTaskCompletionSource();

            void OnNext()
            {
                if (!nextTcs.Task.Status.IsCompleted())
                    nextTcs.TrySetResult();
            }

            _tutorialInput.NextRequested += OnNext;

            try
            {
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token);
                linkedCts.Token.Register(() =>
                {
                    if (!nextTcs.Task.Status.IsCompleted())
                        nextTcs.TrySetCanceled();
                });

                await nextTcs.Task;
            }
            finally
            {
                _tutorialInput.NextRequested -= OnNext;
            }

            await _windowView.HideAsync(token);

            // ActionMap を通常（元の）に戻す
            _actionMapController.SwitchTo(previousMap);

            //
            // ① 資料が「開かれる」のを待つ
            //
            var docOpenedTcs = new UniTaskCompletionSource();

            void OnEnvironmentDocumentOpened()
            {
                if (!docOpenedTcs.Task.Status.IsCompleted())
                    docOpenedTcs.TrySetResult();
            }

            _gameEvents.EnvironmentDocumentOpened += OnEnvironmentDocumentOpened;

            try
            {
                using var linkedCts2 = CancellationTokenSource.CreateLinkedTokenSource(token);
                linkedCts2.Token.Register(() =>
                {
                    if (!docOpenedTcs.Task.Status.IsCompleted())
                        docOpenedTcs.TrySetCanceled();
                });

                await docOpenedTcs.Task;
            }
            finally
            {
                _gameEvents.EnvironmentDocumentOpened -= OnEnvironmentDocumentOpened;
            }

            //
            // ② 資料が「閉じられる」のを待つ
            //
            var docClosedTcs = new UniTaskCompletionSource();

            void OnEnvironmentDocumentClosed()
            {
                if (!docClosedTcs.Task.Status.IsCompleted())
                    docClosedTcs.TrySetResult();
            }

            _gameEvents.EnvironmentDocumentClosed += OnEnvironmentDocumentClosed;

            try
            {
                using var linkedCts3 = CancellationTokenSource.CreateLinkedTokenSource(token);
                linkedCts3.Token.Register(() =>
                {
                    if (!docClosedTcs.Task.Status.IsCompleted())
                        docClosedTcs.TrySetCanceled();
                });

                await docClosedTcs.Task;
            }
            finally
            {
                _gameEvents.EnvironmentDocumentClosed -= OnEnvironmentDocumentClosed;
            }

            // 資料が閉じられたタイミングでヒントを消す
            _windowView.HideHint();

            // ここで Phase1 完了
            _phase.AdvanceTo(TutorialPhase.Phase2_InspectPC1);
        }

        async UniTask RunPhase2Async(CancellationToken token)
        {
            //
            // サブステップA：説明ウィンドウ＋Tutorial操作
            //
            var previousMap = _actionMapController.CurrentActionMapName;
            _actionMapController.SwitchTo("Tutorial");

            var message =
                "資料を見たら、今度は環境内を調査してみましょう。\n" +
                "PCに近づき、左クリックで調べてみてください。";

            var uiHintInWindow = "PC1を調べる";

            _inspectUseCase.LimitInspectableObject("PCSet01");
            _windowView.Show(message, uiHintInWindow);

            var nextTcs = new UniTaskCompletionSource();

            void OnNext()
            {
                if (!nextTcs.Task.Status.IsCompleted())
                    nextTcs.TrySetResult();
            }

            _tutorialInput.NextRequested += OnNext;

            try
            {
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token);
                linkedCts.Token.Register(() =>
                {
                    if (!nextTcs.Task.Status.IsCompleted())
                        nextTcs.TrySetCanceled();
                });

                await nextTcs.Task;
            }
            finally
            {
                _tutorialInput.NextRequested -= OnNext;
            }

            await _windowView.HideAsync(token);

            // ActionMap を通常に戻す
            _actionMapController.SwitchTo(previousMap);


            //
            // サブステップB：対象をハイライト＋ヒントだけで待機
            //
            // 対象をハイライト
            _highlightView.Highlight("PC1_Outline");

            var inspectTcs = new UniTaskCompletionSource();

            void OnObjectInspected(string inspectedId)
            {
                // チュートリアルで指定した対象だけを受け付ける
                if (inspectedId != _phase2TargetId)
                    return;

                if (!inspectTcs.Task.Status.IsCompleted())
                    inspectTcs.TrySetResult();
            }

            _gameEvents.ObjectInspected += OnObjectInspected;

            try
            {
                using var linkedCts2 = CancellationTokenSource.CreateLinkedTokenSource(token);
                linkedCts2.Token.Register(() =>
                {
                    if (!inspectTcs.Task.Status.IsCompleted())
                        inspectTcs.TrySetCanceled();
                });

                await inspectTcs.Task;
            }
            finally
            {
                _gameEvents.ObjectInspected -= OnObjectInspected;
            }

            // 対象が調べられたら、ヒントとハイライトを消す
            _windowView.HideHint();
            _highlightView.ClearHighlight();

            // 必要ならウィンドウ自体も隠す（ヒント窓ごと閉じる）
            await _windowView.HideAsync(token);

            _phase.AdvanceTo(TutorialPhase.Phase3_RiskExplanation);
            // ここで Phase2 完了
        }

        async UniTask RunPhase3Async(CancellationToken token)
        {
            //
            // サブステップA：説明ウィンドウ＋Tutorial操作
            //
            var previousMap = _actionMapController.CurrentActionMapName;
            _actionMapController.SwitchTo("Tutorial");

            var message =
                "対象を調べると、以下の情報が表示されます。\n" +
                "・対象の状態\n" +
                "・想定されるリスク\n" +
                "これらの情報と環境資料、オブジェクトの配置などからリスクを選択しましょう。\n"+
                "リスクは対応を行うまで何度でも変更できます。";

            var uiHintInWindow = "リスクを選択する";

            _windowView.Show(message, uiHintInWindow);

            // Enter(Tutorial/Next)待ち
            var nextTcs = new UniTaskCompletionSource();

            void OnNext()
            {
                nextTcs.TrySetResult(); // 複数回呼ばれても TrySetResult なので安全
            }

            _tutorialInput.NextRequested += OnNext;

            try
            {
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token);
                linkedCts.Token.Register(() =>
                {
                    nextTcs.TrySetCanceled();
                });

                await nextTcs.Task;
            }
            finally
            {
                _tutorialInput.NextRequested -= OnNext;
            }

            await _windowView.HideAsync(token);

            // ActionMap を通常に戻す
            _actionMapController.SwitchTo(previousMap);


            //
            // サブステップB：ヒントだけ表示＋リスク選択待ち
            //

            var riskTcs = new UniTaskCompletionSource();

            void OnRiskSelected(string targetId)
            {
                // チュートリアルでリスク選択させたい対象だけを見る
                // Phase2 と同じオブジェクトなら _phase2TargetId を再利用
                /*if (targetId != _phase2TargetId)
                    return;*/

                riskTcs.TrySetResult();
            }

            _gameEvents.RiskSelected += OnRiskSelected;

            try
            {
                using var linkedCts2 = CancellationTokenSource.CreateLinkedTokenSource(token);
                linkedCts2.Token.Register(() =>
                {
                    riskTcs.TrySetCanceled();
                });

                await riskTcs.Task;
            }
            finally
            {
                _gameEvents.RiskSelected -= OnRiskSelected;
            }

            // リスクが選ばれたらヒントを消す
            _windowView.HideHint();

            // 必要ならウィンドウごと閉じる
            await _windowView.HideAsync(token);

            _phase.AdvanceTo(TutorialPhase.Phase4_OpenActionList);
            // ここで Phase3 完了
        }

        async UniTask RunPhase4Async(CancellationToken token)
        {
            //
            // サブステップA：説明ウィンドウ＋Tutorial操作
            //
            var previousMap = _actionMapController.CurrentActionMapName;
            _actionMapController.SwitchTo("Tutorial");

            var message =
                "リスクを特定したら、次は“どう対応するか”を考えるステップです。\n" +
                "ハイライトされている対象に近づいて、Eキーを押すと対応策の一覧が表示されます。";

            var uiHintInWindow = "対応策を確認する";

            _windowView.Show(message, uiHintInWindow);

            var nextTcs = new UniTaskCompletionSource();

            void OnNext()
            {
                if (!nextTcs.Task.Status.IsCompleted())
                    nextTcs.TrySetResult();
            }

            _tutorialInput.NextRequested += OnNext;

            try
            {
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token);
                linkedCts.Token.Register(() =>
                {
                    if (!nextTcs.Task.Status.IsCompleted())
                        nextTcs.TrySetCanceled();
                });

                await nextTcs.Task;
            }
            finally
            {
                _tutorialInput.NextRequested -= OnNext;
            }

            await _windowView.HideAsync(token);

            // ActionMap を通常に戻す
            _actionMapController.SwitchTo(previousMap);


            //
            // サブステップB：通常入力＋ヒントだけ表示＋一覧オープン待ち
            //

            // 対象をハイライト（Phase2で使ったIDを使い回す想定）
            _highlightView?.Highlight("PC1_Outline");

            var listTcs = new UniTaskCompletionSource();

            void OnCountermeasureListOpened(string targetId)
            {
                /*if (targetId != _phase2TargetId)
                    return; // 他のオブジェクトなら無視*/

                if (!listTcs.Task.Status.IsCompleted())
                    listTcs.TrySetResult();
            }

            _gameEvents.ActionListOpened += OnCountermeasureListOpened;

            try
            {
                using var linkedCts2 = CancellationTokenSource.CreateLinkedTokenSource(token);
                linkedCts2.Token.Register(() =>
                {
                    if (!listTcs.Task.Status.IsCompleted())
                        listTcs.TrySetCanceled();
                });

                await listTcs.Task;
            }
            finally
            {
                _gameEvents.ActionListOpened -= OnCountermeasureListOpened;
            }

            // 一覧が開いたら、ひとまずヒントは消す
            _windowView.HideHint();
            // ハイライトは「一覧中も対象を分かりやすくしておきたい」なら残してもOK
            // とりあえずここでは残したままにしておく

            // ウィンドウ本体を閉じたいならここで
            await _windowView.HideAsync(token);

            _phase.AdvanceTo(TutorialPhase.Phase5_SelectAction);
            // ここで Phase4 完了（一覧UIは開いたままの想定）
        }

        async UniTask RunPhase5Async(CancellationToken token)
        {
            //
            // サブステップA：対処法一覧の説明（Tutorial操作）
            //
            var previousMap = _actionMapController.CurrentActionMapName;
            _actionMapController.SwitchTo("Tutorial");

            var message =
                "対応策の一覧には、以下の情報が表示されます。\n" +
                "・現在のActionPoint(AP)\n" +
                "・消費ActionPoint\n" +
                "・どう対処するか\n" +
                "ActionPointは行動できるスタミナのようなものです。これが足りないと対処ができず、リスクへの対応が十分に行えません。\n" +
                "また、対応策を選択してしまうとリスクが確定してしまい、リスクの変更ができないので十分に考えて選択しましょう。";

            var uiHintInWindow = "対応策を選択する";

            _windowView.Show(message, uiHintInWindow);

            var nextTcs = new UniTaskCompletionSource();

            void OnNext()
            {
                if (!nextTcs.Task.Status.IsCompleted())
                    nextTcs.TrySetResult();
            }

            _tutorialInput.NextRequested += OnNext;

            try
            {
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token);
                linkedCts.Token.Register(() =>
                {
                    if (!nextTcs.Task.Status.IsCompleted())
                        nextTcs.TrySetCanceled();
                });

                await nextTcs.Task;
            }
            finally
            {
                _tutorialInput.NextRequested -= OnNext;
            }

            await _windowView.HideAsync(token);

            // ActionMap を通常に戻す（一覧は開いたまま）
            _actionMapController.SwitchTo(previousMap);


            //
            // サブステップB：ヒントだけ表示＋対処法選択を待つ
            //

            var cmTcs = new UniTaskCompletionSource();

            void OnCountermeasureSelected(string targetId)
            {
                // 対象オブジェクトを指定するならチェック
                /*if (targetId != _phase2TargetId)
                    return;*/

                if (!cmTcs.Task.Status.IsCompleted())
                    cmTcs.TrySetResult();
            }

            _gameEvents.ActionSelected += OnCountermeasureSelected;

            try
            {
                using var linkedCts2 = CancellationTokenSource.CreateLinkedTokenSource(token);
                linkedCts2.Token.Register(() =>
                {
                    if (!cmTcs.Task.Status.IsCompleted())
                        cmTcs.TrySetCanceled();
                });

                await cmTcs.Task;
            }
            finally
            {
                _gameEvents.ActionSelected -= OnCountermeasureSelected;
            }

            // 対処法が選ばれたらヒントを消す
            _windowView.HideHint();
            await _windowView.HideAsync(token);

            // 必要ならここでハイライトも消してしまう
            _highlightView?.ClearHighlight();

            _inspectUseCase.AllowAllInspctableObject();
            _phase.AdvanceTo(TutorialPhase.Phase6_DescribeRiskAssessment);
            // ここで Phase5 完了（対処法も決定済み）
        }

        async UniTask RunPhase6Async(CancellationToken token)
        {
            // 1. ActionMapをTutorialに切り替え
            var previousMap = _actionMapController.CurrentActionMapName;
            _actionMapController.SwitchTo("Tutorial");

            // 2. 説明文＋ヒントを表示
            var message =
                "このようにリスクアセスメントとリスク対応では大まかに\n" +
                "1. 前提条件や環境の確認\n" +
                "2. リスクの発見\n" +
                "3. リスクへの対応\n" +
                "という3段階に分かれます。";

            var uiHint = "";

            _windowView.Show(message, uiHint);

            // 3. Enter（Tutorial/Next入力）を待つ
            var nextTcs = new UniTaskCompletionSource();

            void OnNext()
            {
                if (!nextTcs.Task.Status.IsCompleted())
                    nextTcs.TrySetResult();
            }

            _tutorialInput.NextRequested += OnNext;

            try
            {
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token);
                linkedCts.Token.Register(() =>
                {
                    if (!nextTcs.Task.Status.IsCompleted())
                        nextTcs.TrySetCanceled();
                });

                await nextTcs.Task;
            }
            finally
            {
                _tutorialInput.NextRequested -= OnNext;
            }

            // 4. ウィンドウを閉じる
            await _windowView.HideAsync(token);

            // 5. ActionMapを元に戻す
            _actionMapController.SwitchTo(previousMap);

            _phase.AdvanceTo(TutorialPhase.Phase7_InspectMemo);
            // ここで「リスクアセスメント概要説明フェーズ」は完了
        }

        async UniTask RunPhase7Async(CancellationToken token)
        {
            //
            // サブステップA：説明ウィンドウ＋Tutorial操作
            //
            var previousMap = _actionMapController.CurrentActionMapName;
            _actionMapController.SwitchTo("Tutorial");

            var message =
                "次は別のオブジェクトに対してリスクアセスメントとリスク対応を行いましょう。\n" +
                "ハイライトされたオブジェクトを調査してください。";

            var uiHintInWindow = "メモを調べる";

            _inspectUseCase.LimitInspectableObject("Memo");
            _windowView.Show(message, uiHintInWindow);

            var nextTcs = new UniTaskCompletionSource();

            void OnNext()
            {
                if (!nextTcs.Task.Status.IsCompleted())
                    nextTcs.TrySetResult();
            }

            _tutorialInput.NextRequested += OnNext;

            try
            {
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token);
                linkedCts.Token.Register(() =>
                {
                    if (!nextTcs.Task.Status.IsCompleted())
                        nextTcs.TrySetCanceled();
                });

                await nextTcs.Task;
            }
            finally
            {
                _tutorialInput.NextRequested -= OnNext;
            }

            await _windowView.HideAsync(token);

            // ActionMap を通常に戻す
            _actionMapController.SwitchTo(previousMap);


            //
            // サブステップB：別オブジェクトをハイライト＋ヒントだけで待機
            //
            _highlightView?.Highlight("Memo_Outline");

            var inspectTcs = new UniTaskCompletionSource();

            void OnObjectInspected(string inspectedId)
            {
                /*// 別オブジェクトだけを対象にする
                if (inspectedId != _secondObjectTargetId)
                    return;*/

                if (!inspectTcs.Task.Status.IsCompleted())
                    inspectTcs.TrySetResult();
            }

            _gameEvents.ObjectInspected += OnObjectInspected;

            try
            {
                using var linkedCts2 = CancellationTokenSource.CreateLinkedTokenSource(token);
                linkedCts2.Token.Register(() =>
                {
                    if (!inspectTcs.Task.Status.IsCompleted())
                        inspectTcs.TrySetCanceled();
                });

                await inspectTcs.Task;
            }
            finally
            {
                _gameEvents.ObjectInspected -= OnObjectInspected;
            }

            // 調査が終わったらヒントとハイライトを消す
            _windowView.HideHint();
            _highlightView?.ClearHighlight();

            await _windowView.HideAsync(token);

            _phase.AdvanceTo(TutorialPhase.Phase8_PickupMemo);
            // ここで「別オブジェクトの調査フェーズ」は完了
        }

        async UniTask RunPhase8Async(CancellationToken token)
        {
            //
            // 0. リスクを自動で “不正利用・内部不正のリスク” に確定させておく
            //
            const string memoRiskLabel = "不正利用・内部不正のリスク";
            _inspectUseCase.ForceSelectRisk(_secondObjectTargetId, memoRiskLabel);

            //
            // 1. 説明ウィンドウ（Tutorial ActionMap）
            //
            var previousMap = _actionMapController.CurrentActionMapName;
            _actionMapController.SwitchTo("Tutorial");

            var message =
                "オブジェクトによっては手に持って運ぶことが出来ます。\n" +
                "「パスワードの漏洩リスク」を選択してからこのメモに対して F キーを押して、手に持ってみましょう。";

            var uiHintInWindow = "メモを手に持つ";

            _windowView.Show(message, uiHintInWindow);

            var nextTcs = new UniTaskCompletionSource();

            void OnNext()
            {
                if (!nextTcs.Task.Status.IsCompleted())
                    nextTcs.TrySetResult();
            }

            _tutorialInput.NextRequested += OnNext;

            try
            {
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token);
                linkedCts.Token.Register(() =>
                {
                    if (!nextTcs.Task.Status.IsCompleted())
                        nextTcs.TrySetCanceled();
                });

                await nextTcs.Task;
            }
            finally
            {
                _tutorialInput.NextRequested -= OnNext;
            }

            await _windowView.HideAsync(token);

            // ActionMap を通常に戻す
            _actionMapController.SwitchTo(previousMap);


            //
            // 2. メモをハイライトして「F で持つ」動作をさせる
            //
            _highlightView?.Highlight("Memo_Outline");


            var holdTcs = new UniTaskCompletionSource();

            void OnObjectHeld(string objectId)
            {
                if (objectId != "Memo")
                    return;

                if (!holdTcs.Task.Status.IsCompleted())
                    holdTcs.TrySetResult();
            }

            _gameEvents.ObjectHeld += OnObjectHeld;

            try
            {
                using var linkedCts2 = CancellationTokenSource.CreateLinkedTokenSource(token);
                linkedCts2.Token.Register(() =>
                {
                    if (!holdTcs.Task.Status.IsCompleted())
                        holdTcs.TrySetCanceled();
                });

                await holdTcs.Task;
            }
            finally
            {
                _gameEvents.ObjectHeld -= OnObjectHeld;
            }

            // メモを手に持ったらヒントとハイライトを消す
            _windowView.HideHint();
            _highlightView?.ClearHighlight();

            await _windowView.HideAsync(token);

            _phase.AdvanceTo(TutorialPhase.Phase9_UseShredder);
            // ここで Phase8（メモを手に持つフェーズ）完了
        }

        async UniTask RunPhase9Async(CancellationToken token)
        {
            //
            // 1. 説明ウィンドウ（Tutorial ActionMap）
            //
            var previousMap = _actionMapController.CurrentActionMapName;
            _actionMapController.SwitchTo("Tutorial");

            var message =
                "手に持った状態で、ハイライトされたオブジェクトのもとに行ってEキーを押してみましょう。";

            var uiHintInWindow = "対処法を確認";

            _windowView.Show(message, uiHintInWindow);

            var nextTcs = new UniTaskCompletionSource();

            void OnNext()
            {
                if (!nextTcs.Task.Status.IsCompleted())
                    nextTcs.TrySetResult();
            }

            _tutorialInput.NextRequested += OnNext;

            try
            {
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token);
                linkedCts.Token.Register(() =>
                {
                    if (!nextTcs.Task.Status.IsCompleted())
                        nextTcs.TrySetCanceled();
                });

                await nextTcs.Task;
            }
            finally
            {
                _tutorialInput.NextRequested -= OnNext;
            }

            await _windowView.HideAsync(token);

            // ActionMap を通常に戻す
            _actionMapController.SwitchTo(previousMap);


            //
            // 2. メモを使う対象をハイライト＆「Eで対処法確認」ヒントだけ表示
            //
            _highlightView?.Highlight("Shredder_Outline");

            var useTcs = new UniTaskCompletionSource();

            void OnObjectUsedOnTarget(string objectId)
            {
                /*// 「メモを、このターゲットに対して使ったとき」だけ受け付ける
                if (heldObjectId != _memoObjectId) return;
                if (targetObjectId != _memoUseTargetId) return;*/

                if (!useTcs.Task.Status.IsCompleted())
                    useTcs.TrySetResult();
            }

            _gameEvents.ActionListOpened += OnObjectUsedOnTarget;

            try
            {
                using var linkedCts2 = CancellationTokenSource.CreateLinkedTokenSource(token);
                linkedCts2.Token.Register(() =>
                {
                    if (!useTcs.Task.Status.IsCompleted())
                        useTcs.TrySetCanceled();
                });

                await useTcs.Task;
            }
            finally
            {
                _gameEvents.ActionListOpened -= OnObjectUsedOnTarget;
            }

            // メモを対象に使ったら、ヒントとハイライトを消す
            _windowView.HideHint();
            _highlightView?.ClearHighlight();

            await _windowView.HideAsync(token);

            // ここで Phase9（メモを他オブジェクトに使うフェーズ）完了
        }

        async UniTask RunPhase10Async(CancellationToken token)
        {
            // 1. ActionMap を Tutorial に切り替え
            var previousMap = _actionMapController.CurrentActionMapName;
            _actionMapController.SwitchTo("Tutorial");

            // 2. 説明文＋ヒントを表示
            var message =
                "手に持ったオブジェクトに対する対処法が表示されましたね。\n" +
                "このように、オブジェクトやリスクによっては、\n" +
                "手に持って他のオブジェクトを使用するという対処法があるものもあります。\n" +
                "では、対応策を選択してこのオブジェクトに対するリスクアセスメントと対応を終わらせましょう。";

            var uiHint = "";

            _windowView.Show(message, uiHint);

            // 3. Enter（Tutorial/Next入力）を待つ
            var nextTcs = new UniTaskCompletionSource();

            void OnNext()
            {
                if (!nextTcs.Task.Status.IsCompleted())
                    nextTcs.TrySetResult();
            }

            _tutorialInput.NextRequested += OnNext;

            try
            {
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token);
                linkedCts.Token.Register(() =>
                {
                    if (!nextTcs.Task.Status.IsCompleted())
                        nextTcs.TrySetCanceled();
                });

                await nextTcs.Task;
            }
            finally
            {
                _tutorialInput.NextRequested -= OnNext;
            }

            // 4. ウィンドウを閉じる
            await _windowView.HideAsync(token);

            // 5. ActionMap を元に戻す
            _actionMapController.SwitchTo(previousMap);

            var cmTcs = new UniTaskCompletionSource();

            void OnCountermeasureSelected(string targetId)
            {
                // 対象オブジェクトを指定するならチェック
                /*if (targetId != _phase2TargetId)
                    return;*/

                if (!cmTcs.Task.Status.IsCompleted())
                    cmTcs.TrySetResult();
            }

            _gameEvents.ActionSelected += OnCountermeasureSelected;

            try
            {
                using var linkedCts2 = CancellationTokenSource.CreateLinkedTokenSource(token);
                linkedCts2.Token.Register(() =>
                {
                    if (!cmTcs.Task.Status.IsCompleted())
                        cmTcs.TrySetCanceled();
                });

                await cmTcs.Task;
            }
            finally
            {
                _gameEvents.ActionSelected -= OnCountermeasureSelected;
            }
            // ここで Phase10（ゲーム固有アクション説明2）完了
            _inspectUseCase.AllowAllInspctableObject();
            _phase.AdvanceTo(TutorialPhase.Phase10_FreeAssessment);
        }

        async UniTask RunPhase11Async(CancellationToken token)
        {
            await UniTask.Yield();
            //
            // 1. 説明ウィンドウ（Tutorial ActionMap）
            //
            var previousMap = _actionMapController.CurrentActionMapName;
            _actionMapController.SwitchTo("Tutorial");

            var message =
                "この環境には、あと一つリスクがあります。\n" +
                "リスクがあるオブジェクトを自分で探して、調査し、\n" +
                "リスクの選択と対処まで行ってみましょう。\n" +
                "操作で迷ったときは下の操作説明バーで確認できます。";

            var uiHintInWindow = "リスクアセスメントを行う";

            _windowView.Show(message, uiHintInWindow);

            var nextTcs = new UniTaskCompletionSource();

            void OnNext()
            {
                if (!nextTcs.Task.Status.IsCompleted())
                    nextTcs.TrySetResult();
            }

            _tutorialInput.NextRequested += OnNext;

            try
            {
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token);
                linkedCts.Token.Register(() =>
                {
                    if (!nextTcs.Task.Status.IsCompleted())
                        nextTcs.TrySetCanceled();
                });

                await nextTcs.Task;
            }
            finally
            {
                _tutorialInput.NextRequested -= OnNext;
            }

            await _windowView.HideAsync(token);

            // 2. ActionMap を通常に戻す
            _actionMapController.SwitchTo(previousMap);


            //
            // 3. 自力でリスクアセスメントしてもらうフェーズ
            //
            // ハイライトは出さない。ヒントだけを表示して自由に動いてもらう。

            var completeTcs = new UniTaskCompletionSource();

            void OnCountermeasureSelected(string targetId)
            {
                // 最後のリスク対象オブジェクトに対して対処が行われたら完了
                /*if (targetId != _finalRiskObjectId)
                    return;*/

                if (!completeTcs.Task.Status.IsCompleted())
                    completeTcs.TrySetResult();
            }

            _gameEvents.ActionSelected += OnCountermeasureSelected;

            try
            {
                using var linkedCts2 = CancellationTokenSource.CreateLinkedTokenSource(token);
                linkedCts2.Token.Register(() =>
                {
                    if (!completeTcs.Task.Status.IsCompleted())
                        completeTcs.TrySetCanceled();
                });

                await completeTcs.Task;
            }
            finally
            {
                _gameEvents.ActionSelected -= OnCountermeasureSelected;
            }

            // 4. 最後のリスクに対処できたので、ヒントとウィンドウを消す
            _windowView.HideHint();
            await _windowView.HideAsync(token);

            _phase.AdvanceTo(TutorialPhase.Phase11_EndGameInstruction);
            // ここで Phase11（独力でリスクアセスメント）完了
        }

        async UniTask RunPhase12Async(CancellationToken token)
        {
            // 1. ActionMap を Tutorial に切り替え
            var previousMap = _actionMapController.CurrentActionMapName;
            _actionMapController.SwitchTo("Tutorial");

            // 2. 説明文＋ヒントを表示
            var message =
                "すべてのリスクに対処できたと判断したら、\n" +
                "Spaceキーを入力してゲームを終了しましょう。\n\n" +
                "本番のモードでも同様に、\n" +
                "「もう十分にリスク対策ができた」と思ったタイミングで\n" +
                "Spaceキーでゲームを終了できます。";

            var uiHint = "ゲームを終了する";

            _windowView.Show(message, uiHint);

            // 3. Enter（Tutorial/Next入力）を待つ
            var nextTcs = new UniTaskCompletionSource();

            void OnNext()
            {
                if (!nextTcs.Task.Status.IsCompleted())
                    nextTcs.TrySetResult();
            }

            _tutorialInput.NextRequested += OnNext;

            try
            {
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token);
                linkedCts.Token.Register(() =>
                {
                    if (!nextTcs.Task.Status.IsCompleted())
                        nextTcs.TrySetCanceled();
                });

                await nextTcs.Task;
            }
            finally
            {
                _tutorialInput.NextRequested -= OnNext;
            }

            // 4. ウィンドウを閉じる
            await _windowView.HideAsync(token);

            // 5. ActionMap を元に戻す
            _actionMapController.SwitchTo(previousMap);

            _phase.AdvanceTo(TutorialPhase.Phase12_Result);
            // ここで「ゲーム終了方法説明フェーズ」完了
        }

        async UniTask RunPhase13Async(CancellationToken token)
        {
            var endGameTcs = new UniTaskCompletionSource();

            void OnEndGame()
            {
                if (!endGameTcs.Task.Status.IsCompleted())
                    endGameTcs.TrySetResult();
            }

            _gameEvents.EndGame += OnEndGame;

            try
            {
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token);
                linkedCts.Token.Register(() =>
                {
                    if (!endGameTcs.Task.Status.IsCompleted())
                        endGameTcs.TrySetCanceled();
                });

                await endGameTcs.Task;
            }
            finally
            {
                _gameEvents.EndGame -= OnEndGame;
            }

            //
            // 2. ここから「リザルト画面説明フェーズ」
            //

            // 1. ActionMap を Tutorial に切り替え
            var previousMap = _actionMapController.CurrentActionMapName;
            _actionMapController.SwitchTo("Tutorial");

            // 2. 説明文＋ヒントを表示
            var message =
                "ゲームを終了すると、これまで行ったリスクアセスメントの詳細結果が表示されます。\n" +
                "Summary画面では、\n" +
                "・どのオブジェクトに対して\n" +
                "・どのような判断・行動を行ったか\n" +
                "・その結果、リスクがどう変化したか\n" +
                "を確認できます。\n" +
                "各行動をクリックするとより詳しい情報も確認できます。";

            var uiHint = "";

            _windowView.Show(message, uiHint);

            // 3. Enter（Tutorial/Next入力）を待つ
            var nextTcs = new UniTaskCompletionSource();

            void OnNext()
            {
                if (!nextTcs.Task.Status.IsCompleted())
                    nextTcs.TrySetResult();
            }

            _tutorialInput.NextRequested += OnNext;

            try
            {
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token);
                linkedCts.Token.Register(() =>
                {
                    if (!nextTcs.Task.Status.IsCompleted())
                        nextTcs.TrySetCanceled();
                });

                await nextTcs.Task;
            }
            finally
            {
                _tutorialInput.NextRequested -= OnNext;
            }

            // 4. ウィンドウを閉じる
            await _windowView.HideAsync(token);

            // 5. ActionMap を元に戻す
            _actionMapController.SwitchTo(previousMap);

            _phase.AdvanceTo(TutorialPhase.Completed);
            // ここでチュートリアル完了
        }
    }
}
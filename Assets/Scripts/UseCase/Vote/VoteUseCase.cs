using Cysharp.Threading.Tasks;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UniRx;
using UseCase.Network;
using Debug = UnityEngine.Debug;


public enum VoteChoice { Undefined, Yes, No }
public enum VoteResult { Pending, Passed, Failed }
/// <summary>
/// “Š•[‚Ì“r’†Œo‰ß‚ğƒrƒ…[‘w‚Ö’Ê’m‚·‚é‚½‚ß‚Ì VO
/// </summary>
public readonly struct VoteProgress
{
    public readonly int Yes;
    public readonly int No;
    public readonly int Total;
    public float RatioYes => Total == 0 ? 0f : (float)Yes / Total;
    public float RatioNo => Total == 0 ? 0f : (float)No / Total;
    public VoteProgress(int yes, int no, int total)
    {
        Yes = yes; No = no; Total = total;
    }
}

public interface IVotePresenter
{
    void StartVote(int playerNum, Action<bool> onComplete);
    void UpdateVote(VoteProgress progress, Action<VoteChoice> onSelect);
    void OnVotePassed();
    void OnVoteFailed();
}

public class VoteUseCase
{
    readonly IPacketSender sender;
    readonly IPacketReceiver receiver;
    readonly ISessionRepository session;

    readonly IVotePresenter presenter;

    readonly Subject<int> onVoteStarted = new();
    readonly Subject<VoteProgress> onUpdate = new();
    readonly Subject<Unit> onFail = new();
    readonly Subject<Unit> onPass = new();

    public IObservable<int> OnVoteStarted => onVoteStarted;
    public IObservable<VoteProgress> OnVoteUpdated => onUpdate;
    public IObservable<Unit> OnVoteFailed => onFail;
    public IObservable<Unit> OnVotePassed => onPass;

    CompositeDisposable disposables = new CompositeDisposable();

    public VoteUseCase(IPacketSender sender,IPacketReceiver receiver, ISessionRepository session, IVotePresenter presenter)
    {
        this.sender = sender;
        this.receiver = receiver;
        this.session = session;
        this.presenter = presenter;
        Bind();
    }

    // „Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ VoteStart „Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ
    public async UniTask StartVote()
    {
        var tcs = new UniTaskCompletionSource<bool>();

        var room = session.GetRoomSession();
        var playerNum = room.Players.Count;
        presenter.StartVote(playerNum, (isYes) =>
        {
            tcs.TrySetResult(isYes);
        });

        var result = await tcs.Task;

        if(result == true)
        {
            await sender.SendStartVoteData(new StartVoteData
            {
                PlayerId = session.GetPlayerSession().Id,
                RoomId = session.GetRoomSession().Id
            });
            onVoteStarted.OnNext(playerNum);
        }
        else
        {
            presenter.OnVoteFailed();
            onFail.OnNext(Unit.Default);
        }
  
    }

    // „Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ VoteNotifier „Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ
    public void HandleVoteNotifier(VoteNotifierData data)
    {
        Debug.Log("GetVoteNotifier");
        var progress = new VoteProgress(data.Yes, data.No, data.Total);

        presenter.UpdateVote(progress, (result) =>
        {
            SendVoteChoice(result).Forget();            
        });
    }

    // „Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ VoteEnd „Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ
    public void HandleVoteEnd(VoteEndData data)
    {
        Debug.Log("GetVoteEndNotifier");
        if (data.Result == VoteResult.Passed)
        {
            presenter.OnVotePassed();
            onPass.OnNext(Unit.Default);
        }
        else
        {
            presenter.OnVoteFailed();
            onFail.OnNext(Unit.Default);
        }
    }

    // „Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ ChoiceVote „Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ„Ÿ
    public async UniTask SendVoteChoice(VoteChoice choice)
    {
        var playerId = session.GetPlayerSession().Id;
        var roomId = session.GetRoomSession().Id;   

        await sender.SendVoteChoiceData(new VoteChoiceData
        {
            RoomId = roomId,
            PlayerId = playerId,
            Choice = choice,
        });
    }

    void Bind()
    {
        receiver.OnReceiveVoteNotifierData
            .Subscribe(x =>
            {
                HandleVoteNotifier(x);
            }).AddTo(disposables);

        receiver.OnReceiveVoteResult
            .Subscribe(x =>
            {
                HandleVoteEnd(x);
            }).AddTo(disposables);
    }

}

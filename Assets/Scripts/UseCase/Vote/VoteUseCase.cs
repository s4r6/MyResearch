using Cysharp.Threading.Tasks;
using System;
using System.Diagnostics;
using UniRx;
using UseCase.Network;
using Debug = UnityEngine.Debug;


public enum VoteChoice { Undefined, Yes, No }
public enum VoteResult { Pending, Passed, Failed }
/// <summary>
/// ���[�̓r���o�߂��r���[�w�֒ʒm���邽�߂� VO
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

public class VoteUseCase
{
    readonly IPacketSender sender;
    readonly IPacketReceiver receiver;
    readonly ISessionRepository session;

    readonly Subject<int> onVoteStarted = new();
    readonly Subject<VoteProgress> onUpdate = new();
    readonly Subject<Unit> onFail = new();
    readonly Subject<Unit> onPass = new();

    public IObservable<int> OnVoteStarted => onVoteStarted;
    public IObservable<VoteProgress> OnVoteUpdated => onUpdate;
    public IObservable<Unit> OnVoteFailed => onFail;
    public IObservable<Unit> OnVotePassed => onPass;

    CompositeDisposable disposables = new CompositeDisposable();

    public VoteUseCase(IPacketSender sender,IPacketReceiver receiver , ISessionRepository session)
    {
        this.sender = sender;
        this.receiver = receiver;
        this.session = session;

        Bind();
    }

    // �������������������������� VoteStart ��������������������������
    public void StartVote()
    {
        sender.SendStartVoteData(new StartVoteData
        {
            PlayerId = session.GetPlayerSession().Id,
            RoomId = session.GetRoomSession().Id
        });

        var room = session.GetRoomSession();
        var playerNum = room.Players.Count;

        onVoteStarted.OnNext(playerNum);
    }

    // ������������������������ VoteNotifier ����������������������
    public void HandleVoteNotifier(VoteNotifierData data)
    {
        Debug.Log("GetVoteNotifier");
        var progress = new VoteProgress(data.Yes, data.No, data.Total);
        onUpdate.OnNext(progress);
    }

    // �������������������������� VoteEnd ������������������������������
    public void HandleVoteEnd(VoteEndData data)
    {
        Debug.Log("GetVoteEndNotifier");
        if (data.Result == VoteResult.Passed)
        {
            onPass.OnNext(Unit.Default);
        }
        else
        {
            onFail.OnNext(Unit.Default);
        }
    }

    // ���������������������� ChoiceVote ��������������������
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

using System;
using Cysharp.Threading.Tasks;
using Domain.Player;
using UnityEngine;
using UseCase.Network;
using View.Player;

public class RemotePlayerMoveController : IMoveController
{
    PlayerEntity model;
    PlayerView view;

    IPacketSender sender;
    ISessionRepository sessionRepository;

    Vector3 lastSentPosition = Vector3.zero;
    float lastSentTime = 0;

    public RemotePlayerMoveController(PlayerEntity model, PlayerView view, IPacketSender sender, ISessionRepository sessionRepository)
    {
        this.model = model;
        this.view = view;
        this.sender = sender;
        this.sessionRepository = sessionRepository;
    }

    void TryMove(Vector2 inputDirection)
    {
        var position = view.Move(inputDirection.normalized, model.speed);
        model.SetPosition(position);
    }

    public async UniTask OnMoveInput(Vector2 direction)
    {
        //移動処理
        TryMove(direction);

        await SendPosition();
    }

    void TryRotate(float yaw, float pitch)
    {
        var rotation = view.Rotate(yaw, pitch);
        model.SetRotation(rotation);
    }

    public UniTask OnLookInput(Vector2 delta)
    {
        float yaw = delta.x * model.lookSensitivity.x;
        float pitch = delta.y * model.lookSensitivity.y;

        TryRotate(yaw, pitch);
        return UniTask.CompletedTask;
    }

    int frameCounter = 0;
    public async UniTask SendPosition()
    {
        var position = model.position;
        if(position != lastSentPosition)
        {
            frameCounter++;
            if (frameCounter >= 10)
            {
                frameCounter = 0;
                lastSentPosition = position;
                lastSentTime = Time.time;
                try
                {
                    await sender.SendPlayerPosition(new PositionSyncData
                    {
                        PlayerId = sessionRepository.GetPlayerSession().Id,
                        RoomId = sessionRepository.GetRoomSession().Id,
                        Position = position
                    });
                }
                catch (Exception ex)
                {
                    Debug.LogError($"位置送信中にエラー: {ex}");
                }
            }
            else if(Time.time - lastSentTime >= 2f)
            {
                lastSentTime = Time.time;
                await sender.SendPlayerPosition(new PositionSyncData
                {
                    PlayerId = sessionRepository.GetPlayerSession().Id,
                    RoomId = sessionRepository.GetRoomSession().Id,
                    Position = position
                });
            }
        }
    }
}

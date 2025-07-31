using System.Collections.Generic;
using System;
using Domain.Network;
using Presenter.Network;
using UnityEngine;
using UniRx;

namespace UseCase.Network
{
    public interface IRemotePlayerViewFactory
    {
        public RemotePlayerPresenter SpawnRemotePlayer(IRemotePlayerRepository repository, string id);
    }

    public class RemotePlayerSyncUseCase : IDisposable
    {
        readonly IRemotePlayerRepository repository;
        readonly IRemotePlayerViewFactory factory;
        readonly ISessionRepository session;
        readonly IPacketReceiver receiver;

        Dictionary<string, RemotePlayerPresenter> presenters = new();

        CompositeDisposable _disposables = new();

        public RemotePlayerSyncUseCase(IRemotePlayerRepository repository, IRemotePlayerViewFactory factory, ISessionRepository session, IPacketReceiver receiver)
        {
            this.repository = repository;
            this.factory = factory;
            this.session = session;
            this.receiver = receiver;

            Bind();

            CreateRemotePlayer();
        }

        public void CreateRemotePlayer()
        {
            var player = session.GetPlayerSession();
            var room = session.GetRoomSession();

            foreach(var remote in room.Players)
            {
                if (remote.Id == player.Id)
                    continue;

                var presenter = factory.SpawnRemotePlayer(repository, remote.Id);
                presenters.Add(remote.Id, presenter);
            }
            
        }

        public void OnDisconnectPlayer(string id)
        {
            var room = session.GetRoomSession();
            var player = session.GetPlayerSession();
            room.RemovePlayer(player);

            var presenter = presenters[id];
            presenter.Destroy();

            presenters.Remove(id);
        }

        public void OnJoinPlayer(string id, string name)
        {
            var room = session.GetRoomSession();
            room.AddPlayer(new PlayerSession(id, name));

            var presenter = factory.SpawnRemotePlayer(repository, id);
            presenters.Add(id, presenter);
        }

        public void Update()
        {
            //毎フレームのRemotePlayer更新処理
            //補間や予測など
        }

        public void SyncPosition(string id, Position position)
        {
            var player = repository.Get(id);
            if (player != null)
            {
                player.SetPosition(position);
            }

            if(!presenters.TryGetValue(id, out var presenter))
            {
                throw new Exception($"Id;{id}のPresenterが存在しません");
            }
            
            presenter.Move(player.GetPosition());
        }

        void Bind()
        {
            receiver.OnMove
                .Subscribe(x =>
                {
                    SyncPosition(x.PlayerId, x.Pos);
                }).AddTo(_disposables);

            receiver.OnReceiveJoinPlayerData
                .Subscribe(x =>
                {
                    OnJoinPlayer(x.JoinedPlayerConncetionId, x.JoinedPlayerName);
                }).AddTo(_disposables);

            receiver.OnReceiveDisconnectData
                .Subscribe(x =>
                {
                    OnDisconnectPlayer(x.DisconnectedId);
                }).AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Domain.Network;
using Domain.Stage;
using Infrastracture.Network;
using Infrastructure.Network;
using UniRx;
using UnityEngine;
using UseCase.Network;

namespace Infrastracture.Repository
{
    public class RemoteStageRepository : IStageRepository
    {
        IWebSocketService server;

        StageEntity currentStageEntity;

        public RemoteStageRepository(IWebSocketService server)
        {
            this.server = server;
            Bind();
        }

        public StageEntity GetCurrentStageEntity()
        {
            return currentStageEntity;
        }

        public void Save(StageEntity stage)
        {
            currentStageEntity = stage;
        }

        public StageEntity CreateStage(int stageNumber)
        {
            //currentStageEntity = new StageEntity(MaxRiskMap[stageNumber], MaxRiskMap[stageNumber]);
            return currentStageEntity;
        }

        void Bind()
        {
            server.OnMessage
                .Where(tuple => tuple.Item1 == PacketId.CreateRoomResponse)
                .Subscribe(tuple =>
                {
                    var payload = tuple.Item2["Payload"];
                    var riskAmount = payload["MaxRiskAmount"].ToObject<int>();
                    var actionPointAmount = payload["MaxActionPointAmount"].ToObject<int>();

                    Debug.Log($"Current Risk Amount: {riskAmount}");
                    Debug.Log($"Current Action Point Amount: {actionPointAmount}");

                    currentStageEntity = new StageEntity(riskAmount, actionPointAmount);
                });

            server.OnMessage
                .Where(tuple => tuple.Item1 == PacketId.ActionResponse)
                .Subscribe(tuple =>
                {
                    var payload = tuple.Item2["Payload"];
                    var result = payload["Result"].ToObject<ActionResultType>();
                    if (result != ActionResultType.Success)
                        return;

                    var currentRiskAmount = payload["currentRiskAmount"].ToObject<int>();
                    var currentActionPointAmount = payload["currentActionPointAmount"].ToObject<int>();
                    var histories = payload["histories"].ToObject<List<RiskAssessmentHistory>>();

                    currentStageEntity.Update(currentRiskAmount, currentActionPointAmount, histories);
                });
        }
    }
}
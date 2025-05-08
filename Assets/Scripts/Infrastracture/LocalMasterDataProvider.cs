using Domain.Stage.Object;
using Infrastructure.Master;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.IO;
using Domain.Action;
using UnityEngine;
using System.Linq;

namespace Infrastructure.Master
{
    public class StageObjectData
    {
        public string ObjectId;
        public string DisplayName;
        public string Description;
        public List<ChoiceData> Choices;
    }

    public class ChoiceData
    {
        public string Label;
        public string RiskId;
        public bool IsCorrect;
        public List<ActionEntityData> OverrideActions;
    }

    public class ActionEntityData
    {
        public string Id;
        public int RiskChange;
        public int ActionPointCost;
        public bool IsSuccess;
        public string TargetObjectId;
        public string ExecuteOnObjectId;
    }

    public class ActionLabelData
    {
        public string ActionId { get; set; }
        public bool IsOnly { get; set; }
        public string Label { get; set; }

        public override string ToString()
        {
            return $"ActionLabelData: {ActionId}, {IsOnly}, {Label}";
        }
    }

    public class RiskLabelData
    {
        public string RiskId { get; set; }
        public string DisplayName { get; set; }
    }

    public class LocalMasterDataProvider : IObjectDataProvider, IActionDataProvider
    {

        string filePath = "C:/Users/kouta/Desktop/UnityProjects/MyResearch/Assets/Resources/Master/";
        Dictionary<string, InspectableObject> InspectableObjectMap;
        Dictionary<string, ActionLabelData> ActionLabels;    //ActionLabelActionId擾邽߂Dictionary
        Dictionary<string, string> RiskNames;
        HashSet<string> CarriableObjects;
        Dictionary<string, List<string>> ActionTargets;

        public LocalMasterDataProvider() 
        { 
            FetchAllMasterData();
        }

        public InspectableObject GetInspectableObject(string id)
        {
            if(InspectableObjectMap.TryGetValue(id, out InspectableObject obj))
            {
                return obj;
            }

            return null;
        }

        public InspectableObject GetCarryableObject(string id)
        {
            if(CarriableObjects.Contains(id))
            {
                return InspectableObjectMap[id];
            }

            return null;
        }

        //Objectがデータ上可能なアクションのリストを取得
        public List<string> GetAvailableActionIds(string id)
        {
            //ObjectIdを要素に含むすべてのActionを取得
            var actionTargetKeys = GetActionTargetKeysByObjectId(id);
            if(actionTargetKeys.Count == 0)
            {
                return null;
            }

            return actionTargetKeys;
        }

        public List<ActionEntity> GetActionEntities(string objectId)
        {
            if(InspectableObjectMap.TryGetValue(objectId, out InspectableObject obj))
            {
                return obj.selectedChoice.availableActions;
            }

            return null;
        }

        public List<InspectableObject> GetActionableObjectsByObjectId(string id)
        {
            Debug.Log($"[LocalMasterDataProvider] GetActionableObjectsByObjectId開始: id={id}");
            var result = new List<InspectableObject>();
            
            Debug.Log($"[LocalMasterDataProvider] 検索対象のオブジェクト数: {InspectableObjectMap.Count}");
            foreach (var obj in InspectableObjectMap.Values)
            {
                Debug.Log($"[LocalMasterDataProvider] オブジェクト処理中: id={obj.id}, selectedChoice={obj.selectedChoice}");
                
                // selectedChoiceが存在しない場合はスキップ
                if (obj.selectedChoice == null)
                {
                    Debug.Log($"[LocalMasterDataProvider] selectedChoiceが存在しないためスキップ: id={obj.id}");
                    continue;
                }
                
                Debug.Log($"[LocalMasterDataProvider] 利用可能なアクション数: {obj.selectedChoice.availableActions.Count}");
                
                // selectedChoiceのavailableActionsの中から、executeOnObjectIdが一致するActionEntityがあるかチェック
                bool hasMatchingAction = obj.selectedChoice.availableActions
                    .Any(action => action.executeOnObjectId == id);
                
                if (hasMatchingAction)
                {
                    Debug.Log($"[LocalMasterDataProvider] マッチするアクションを発見: objectId={obj.id}");
                    result.Add(obj);
                }
                else
                {
                    Debug.Log($"[LocalMasterDataProvider] マッチするアクションなし: objectId={obj.id}");
                }
            }
            
            Debug.Log($"[LocalMasterDataProvider] 検索結果: {result.Count}件のオブジェクトが見つかりました");
            return result;
        }

        public string GetActionId(string label)
        {
            var actionData = ActionLabels.Values.ToList()
                                                .Find(x => x.Label == label);

            if(actionData == null)
                return null;

            return actionData.ActionId;
        }

        void FetchAllMasterData()
        {
            LoadCarriableObjectData();
            LoadRiskMaster();
            LoadActionMaster();
            LoadStageObjectData();
            LoadActionTargets();
        }

        void LoadCarriableObjectData(string fileName = "CarriableObjects.json")
        {
            string fullPath = Path.Combine(filePath, fileName);
            if (!File.Exists(fullPath))
            {
                Console.WriteLine($"[ActionLabelLoader] File not found: {fullPath}");
                CarriableObjects = new HashSet<string>();
            }

            try
            {
                var json = File.ReadAllText(fullPath);
                var objectIds = JsonConvert.DeserializeObject<List<string>>(json);
                CarriableObjects = new HashSet<string>(objectIds); // 肪ɂȂ

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ActionLabelLoader] Failed to load or parse: {ex.Message}");
                CarriableObjects = new HashSet<string>();
            }
        }

        //�w�肵���t�@�C������filePath����擾���āAAction��id�ƕ\�����ŕۊ�
        void LoadActionMaster(string fileName = "ActionMaster.json")
        {
            string fullPath = Path.Combine(filePath, fileName);
            if (!File.Exists(fullPath))
            {
                Console.WriteLine($"[ActionLabelLoader] File not found: {fullPath}");
                Debug.Log($"[ActionLabelLoader] File not found: {fullPath}");
                ActionLabels = new Dictionary<string, ActionLabelData>();
            }

            try
            {
                var json = File.ReadAllText(fullPath);
                Debug.Log($"[ActionLabelLoader] Read JSON: {json}");

                var labelMap = JsonConvert.DeserializeObject<Dictionary<string, ActionLabelData>>(json);
                Debug.Log($"[ActionLabelLoader] Deserialized labelMap. Count: {labelMap.Count}");

                var result = new Dictionary<string, ActionLabelData>();
                foreach (var pair in labelMap)
                {
                    result[pair.Key] = pair.Value;
                    Debug.Log($"[ActionLabelLoader] {pair.Value}");
                }

                Console.WriteLine($"[ActionLabelLoader] Loaded {result.Count} action labels.");
                Debug.Log($"[ActionLabelLoader] Loaded {result.Count} action labels.");
                ActionLabels = result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ActionLabelLoader] Failed to load or parse: {ex.Message}");
                Debug.Log($"[ActionLabelLoader] Failed to load or parse: {ex.Message}");
                ActionLabels = new Dictionary<string, ActionLabelData>();
            }
        }

        //�w�肵���t�@�C������filePath����擾���āARisk��id�ƕ\�����ŕۊ�
        void LoadRiskMaster(string fileName = "RiskMaster.json")
        {
            string fullPath = Path.Combine(filePath, fileName);
            if (!File.Exists(fullPath))
            {
                Console.WriteLine($"[RiskLabelLoader] �t�@�C����������܂���: {fullPath}");
                RiskNames = new Dictionary<string, string>();
            }

            try
            {
                string json = File.ReadAllText(fullPath);
                var rawDict = JsonConvert.DeserializeObject<Dictionary<string, RiskLabelData>>(json);

                var result = new Dictionary<string, string>();

                foreach (var pair in rawDict)
                {
                    result[pair.Key] = pair.Value.DisplayName;
                }

                Console.WriteLine($"[RiskLabelLoader] {result.Count} ���̃��X�N���x����ǂݍ��݂܂����B");
                RiskNames = result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RiskLabelLoader] �ǂݍ��ݎ��s: {ex.Message}");
                RiskNames = new Dictionary<string, string>();
            }
        }

        //�w�肵���t�@�C������filePath����擾���āAInspectableObject��id�ƃC���X�^���X�ŕۊ�
        void LoadStageObjectData(string fileName = "StageObjects_re.json")
        {
            string fullPath = Path.Combine(filePath, fileName);
            if (!File.Exists(fullPath))
            {
                Console.WriteLine($"[StageObjectLoader] ファイルが見つかりません: {fullPath}");
                Debug.Log($"[StageObjectLoader] ファイルが見つかりません: {fullPath}");
                InspectableObjectMap = new Dictionary<string, InspectableObject>();
            }

            try
            {
                string json = File.ReadAllText(fullPath);
                var objectList = JsonConvert.DeserializeObject<List<StageObjectData>>(json);

                var result = new Dictionary<string, InspectableObject>();

                foreach (var objData in objectList)
                {
                    var choiceDict = new Dictionary<string, Choice>();

                    foreach (var choiceData in objData.Choices)
                    {
                        var actionList = new List<ActionEntity>();
                        if (choiceData.OverrideActions != null)
                        {
                            foreach (var action in choiceData.OverrideActions)
                            {
                                Debug.Log("[LocalMasterDataProvider] ActionLabelData: " + ActionLabels[action.Id]);
                                var entity = new ActionEntity(
                                    action.Id,
                                    ActionLabels[action.Id].Label,
                                    action.RiskChange,
                                    action.ActionPointCost,
                                    action.IsSuccess,
                                    action.TargetObjectId,
                                    action.ExecuteOnObjectId
                                );
                                actionList.Add(entity);
                                Debug.Log("[LocalMasterDataProvider] アクション追加: " + entity.ToString());
                            }
                        }

                        var choice = new Choice(
                            choiceData.Label,
                            choiceData.RiskId,
                            choiceData.IsCorrect,
                            actionList
                        );

                        choiceDict[choice.riskId] = choice;
                    }

                    var inspectable = new InspectableObject(
                        objData.ObjectId,
                        objData.DisplayName,
                        objData.Description,
                        choiceDict
                    );

                    result[objData.ObjectId] = inspectable;
                }

                Console.WriteLine($"[StageObjectLoader] {result.Count} 件のオブジェクトを読み込みました。");
                Debug.Log($"[StageObjectLoader] {result.Count} 件のオブジェクトを読み込みました。");
                InspectableObjectMap = result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[StageObjectLoader] 読み込み失敗: {ex.Message}");
                Debug.Log($"[StageObjectLoader] 読み込み失敗: {ex.Message}");
                InspectableObjectMap = new Dictionary<string, InspectableObject>();
            }
        }

        void LoadActionTargets(string fileName = "ActionTargets.json")
        {
            string fullPath = Path.Combine(filePath, fileName);
            if (!File.Exists(fullPath))
            {
                Console.WriteLine($"[ActionTargetsLoader] File not found: {fullPath}");
                ActionTargets = new Dictionary<string, List<string>>();
                return;
            }

            try
            {
                var json = File.ReadAllText(fullPath);
                var rawDict = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);

                Console.WriteLine($"[ActionTargetsLoader] {rawDict.Count} 件のActionTargetを読み込みました。");
                ActionTargets = rawDict;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ActionTargetsLoader] Failed to load or parse: {ex.Message}");
                ActionTargets = new Dictionary<string, List<string>>();
            }
        }
    
        /// <summary>
        /// 指定したidをvalue(List<string>)に含む全てのkeyを返す
        /// </summary>
        List<string> GetActionTargetKeysByObjectId(string id)
        {
            var result = new List<string>();
            foreach (var pair in ActionTargets)
            {
                if (pair.Value != null && pair.Value.Contains(id))
                {
                    result.Add(pair.Key);
                }
            }
            return result;
        }

        
    }
}


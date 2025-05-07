using Domain.Stage.Object;
using Infrastructure.Master;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.IO;
using Domain.Action;
using UnityEngine;

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
    }

    public class ActionLabelData
    {
        public string ActionId { get; set; }
        public string Label { get; set; }
    }

    public class RiskLabelData
    {
        public string RiskId { get; set; }
        public string DisplayName { get; set; }
    }

    //���[�J���ŃT�[�o�[�����Fetch���Č����邽�߂̃��[�J�������N���X
    public class LocalMasterDataProvider : IMasterDataProvider
    {

        string filePath = "C:/Users/kouta/Desktop/UnityProjects/MyResearch/Assets/Resources/Master/";
        Dictionary<string, InspectableObject> InspectableObjectMap;
        Dictionary<string, string> ActionLabels;    //ActionLabel��ActionId����擾���邽�߂�Dictionary
        Dictionary<string, string> RiskNames;
        HashSet<string> CarriableObjects;

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


        void FetchAllMasterData()
        {
            LoadCarriableObjectData();
            LoadRiskMaster();
            LoadActionMaster();
            LoadStageObjectData();
        }

        void LoadCarriableObjectData(string fileName = "CarriableObjects.json")
        {
            string fullPath = Path.Combine(filePath, fileName);
            if (!File.Exists(fullPath))
            {
                Console.WriteLine($"[ActionLabelLoader] File not found: {fullPath}");
                ActionLabels = new Dictionary<string, string>();
            }

            try
            {
                var json = File.ReadAllText(fullPath);
                var objectIds = JsonConvert.DeserializeObject<List<string>>(json);
                CarriableObjects = new HashSet<string>(objectIds); // ���肪�����ɂȂ�

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ActionLabelLoader] Failed to load or parse: {ex.Message}");
                ActionLabels = new Dictionary<string, string>();
            }
        }

        //�w�肵���t�@�C������filePath����擾���āAAction��id�ƕ\�����ŕۊ�
        void LoadActionMaster(string fileName = "ActionMaster.json")
        {
            string fullPath = Path.Combine(filePath, fileName);
            if (!File.Exists(fullPath))
            {
                Console.WriteLine($"[ActionLabelLoader] File not found: {fullPath}");
                ActionLabels = new Dictionary<string, string>();
            }

            try
            {
                var json = File.ReadAllText(fullPath);
                var labelMap = JsonConvert.DeserializeObject<Dictionary<string, ActionLabelData>>(json);

                var result = new Dictionary<string, string>();
                foreach (var pair in labelMap)
                {
                    result[pair.Key] = pair.Value.Label;
                }

                Console.WriteLine($"[ActionLabelLoader] Loaded {result.Count} action labels.");
                ActionLabels = result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ActionLabelLoader] Failed to load or parse: {ex.Message}");
                ActionLabels = new Dictionary<string, string>();
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
        void LoadStageObjectData(string fileName = "StageObjects.json")
        {
            string fullPath = Path.Combine(filePath, fileName);
            if (!File.Exists(fullPath))
            {
                Console.WriteLine($"[StageObjectLoader] �t�@�C����������܂���: {fullPath}");
                Debug.Log($"[StageObjectLoader] �t�@�C����������܂���: {fullPath}");
                InspectableObjectMap =  new Dictionary<string, InspectableObject>();
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
                                var entity = new ActionEntity(
                                    action.Id,
                                    action.RiskChange,
                                    action.ActionPointCost,
                                    action.IsSuccess
                                );
                                actionList.Add(entity);
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

                Console.WriteLine($"[StageObjectLoader] {result.Count} �̃I�u�W�F�N�g��ǂݍ��݂܂����B");
                Debug.Log($"[StageObjectLoader] {result.Count} �̃I�u�W�F�N�g��ǂݍ��݂܂����B");
                InspectableObjectMap =  result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[StageObjectLoader] �ǂݍ��ݎ��s: {ex.Message}");
                Debug.Log($"[StageObjectLoader] �ǂݍ��ݎ��s: {ex.Message}");
                InspectableObjectMap = new Dictionary<string, InspectableObject>();
            }
        }
    
    }
}


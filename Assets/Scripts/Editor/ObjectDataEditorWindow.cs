using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class ActionData
{
    public string id;
    public string label;
    public int riskChange;
    public int ActionPointCost;
    public string target;
    public List<string> ObjectAttributes = new();
}

[System.Serializable]
public class ChoiceData
{
    public string Label;
    public string RiskId;
    public List<ActionData> OverrideActions = new();
}

[System.Serializable]
public class ObjectData
{
    public string ObjectId;
    public string DisplayName;
    public string Description;
    public List<ChoiceData> Choices = new();
}

public class ObjectDataEditorWindow : EditorWindow
{
    private List<ObjectData> objectDataList = new();
    private FoldoutManager foldoutManager = new();
    private Vector2 scroll;
    private string saveFolderPath = "Assets/Resources/Master";
    private string fileName = "StageObjects.json";

    [MenuItem("Tools/Object Data Editor (Newtonsoft)")]
    public static void ShowWindow()
    {
        var window = GetWindow<ObjectDataEditorWindow>("Object Data Editor");
        window.LoadExistingJsonAtStartup();
    }

    private void LoadExistingJsonAtStartup()
    {
        string fullPath = Path.Combine(saveFolderPath, fileName);
        if (!File.Exists(fullPath)) return;

        Debug.Log("読み込み");
        string json = File.ReadAllText(fullPath);
        var data = JsonConvert.DeserializeObject<List<ObjectData>>(json);
        if (data != null)
        {
            objectDataList = data;
            foldoutManager = new FoldoutManager();
            for (int i = 0; i < data.Count; i++)
            {
                foldoutManager.Ensure(i);
            }
        }
    }

    private void OnGUI()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll);

        DrawSavePathSettings();

        EditorGUILayout.Space(10);
        GUILayout.Label("オブジェクトデータ", EditorStyles.boldLabel);

        if (GUILayout.Button("追加", GUILayout.Width(80), GUILayout.Height(30)))
        {
            var newObj = new ObjectData();
            objectDataList.Add(newObj);
            foldoutManager.Ensure(objectDataList.Count - 1);
        }

        int? removeObjectIndex = null;

        for (int i = 0; i < objectDataList.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");
            DrawObject(objectDataList[i], i, out bool remove);
            if (remove) removeObjectIndex = i;
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        if (removeObjectIndex.HasValue)
        {
            int i = removeObjectIndex.Value;
            objectDataList.RemoveAt(i);
            foldoutManager.RemoveAt(i);
        }

        if (GUILayout.Button("保存 (上書き保存)"))
        {
            SaveToJsonOverwrite();
        }

        if (GUILayout.Button("追記保存 (Incremental Save)"))
        {
            SaveToJsonIncrementally();
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawSavePathSettings()
    {
        GUILayout.Label("保存設定", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("保存先フォルダ", GUILayout.Width(100));
        EditorGUILayout.SelectableLabel(saveFolderPath, GUILayout.Height(18));
        if (GUILayout.Button("選択", GUILayout.Width(50)))
        {
            string selected = EditorUtility.OpenFolderPanel("保存先フォルダを選択", Application.dataPath, "");
            if (!string.IsNullOrEmpty(selected))
            {
                if (selected.StartsWith(Application.dataPath))
                {
                    saveFolderPath = "Assets" + selected.Substring(Application.dataPath.Length);
                }
                else
                {
                    EditorUtility.DisplayDialog("エラー", "Assetsフォルダ内を選択してください", "OK");
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        fileName = EditorGUILayout.TextField("ファイル名（.json）", fileName);
    }

    private void DrawObject(ObjectData obj, int i, out bool remove)
    {
        remove = false;
        foldoutManager.Ensure(i);

        foldoutManager.ObjectFoldouts[i] = EditorGUILayout.Foldout(foldoutManager.ObjectFoldouts[i], $"Object {i + 1}: {obj.ObjectId}", true);
        if (!foldoutManager.ObjectFoldouts[i]) return;

        EditorGUILayout.BeginHorizontal();
        obj.ObjectId = EditorGUILayout.TextField("ObjectId", obj.ObjectId, GUILayout.Width(300));
        if (GUILayout.Button("← 選択オブジェクト名", GUILayout.Width(130)))
        {
            if (Selection.activeGameObject != null)
            {
                obj.ObjectId = Selection.activeGameObject.name;
            }
        }
        EditorGUILayout.EndHorizontal();

        obj.Description = EditorGUILayout.TextField("Description", obj.Description, GUILayout.Width(500), GUILayout.Height(100));

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("選択肢を追加", GUILayout.Width(100)))
        {
            obj.Choices.Add(new ChoiceData());
            foldoutManager.Ensure(i, obj.Choices.Count - 1);
        }
        EditorGUILayout.EndHorizontal();

        int? removeChoiceIndex = null;
        for (int j = 0; j < obj.Choices.Count; j++)
        {
            DrawChoice(obj.Choices[j], i, j, out bool removeChoice);
            if (removeChoice) removeChoiceIndex = j;
        }

        if (removeChoiceIndex.HasValue)
        {
            obj.Choices.RemoveAt(removeChoiceIndex.Value);
            foldoutManager.RemoveChoiceAt(i, removeChoiceIndex.Value);
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("オブジェクトを削除", GUILayout.Width(100)))
        {
            remove = true;
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawChoice(ChoiceData choice, int i, int j, out bool remove)
    {
        remove = false;
        foldoutManager.Ensure(i, j);

        foldoutManager.ChoiceFoldouts[i][j] = EditorGUILayout.Foldout(foldoutManager.ChoiceFoldouts[i][j], $"Choice {j + 1}: {choice.Label}", true);
        if (!foldoutManager.ChoiceFoldouts[i][j]) return;

        choice.Label = EditorGUILayout.TextField("Label", choice.Label);
        choice.RiskId = EditorGUILayout.TextField("RiskId", choice.RiskId);

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("アクションを追加", GUILayout.Width(100)))
        {
            choice.OverrideActions.Add(new ActionData());
            foldoutManager.Ensure(i, j, choice.OverrideActions.Count - 1);
        }
        EditorGUILayout.EndHorizontal();

        int? removeActionIndex = null;
        for (int k = 0; k < choice.OverrideActions.Count; k++)
        {
            DrawAction(choice.OverrideActions[k], i, j, k, out bool removeAction);
            if (removeAction) removeActionIndex = k;
        }

        if (removeActionIndex.HasValue)
        {
            choice.OverrideActions.RemoveAt(removeActionIndex.Value);
            foldoutManager.RemoveActionAt(i, j, removeActionIndex.Value);
        }

        if (GUILayout.Button("選択肢を削除"))
        {
            remove = true;
        }
    }

    private void DrawAction(ActionData action, int i, int j, int k, out bool remove)
    {
        remove = false;
        foldoutManager.Ensure(i, j, k);

        foldoutManager.ActionFoldouts[i][j][k] = EditorGUILayout.Foldout(foldoutManager.ActionFoldouts[i][j][k], $"Action {k + 1}: {action.id}", true);
        if (!foldoutManager.ActionFoldouts[i][j][k]) return;

        action.id = EditorGUILayout.TextField("ID", action.id);
        action.label = EditorGUILayout.TextField("Label", action.label);
        action.riskChange = EditorGUILayout.IntField("Risk Change", action.riskChange);
        action.ActionPointCost = EditorGUILayout.IntField("Action Point Cost", action.ActionPointCost);
        action.target = EditorGUILayout.TextField("Target", action.target);
        DrawStringList("Object Attributes", action.ObjectAttributes);

        if (GUILayout.Button("Remove Action"))
        {
            remove = true;
        }
    }

    private void DrawStringList(string label, List<string> list)
    {
        EditorGUILayout.LabelField(label);
        for (int i = 0; i < list.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            list[i] = EditorGUILayout.TextField(list[i]);
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                list.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add " + label, GUILayout.Width(200)))
        {
            list.Add("");
        }
        EditorGUILayout.EndHorizontal();
    }

    private void SaveToJsonIncrementally()
    {
        if (string.IsNullOrEmpty(saveFolderPath) || string.IsNullOrEmpty(fileName))
        {
            EditorUtility.DisplayDialog("エラー", "保存先フォルダとファイル名を指定してください。", "OK");
            return;
        }

        string fullPath = Path.Combine(saveFolderPath, fileName);

        List<ObjectData> existing = new();
        if (File.Exists(fullPath))
        {
            string oldJson = File.ReadAllText(fullPath);
            existing = JsonConvert.DeserializeObject<List<ObjectData>>(oldJson) ?? new List<ObjectData>();
        }

        existing.AddRange(objectDataList);

        string newJson = JsonConvert.SerializeObject(existing, Formatting.Indented);
        File.WriteAllText(fullPath, newJson);
        Debug.Log($"JSONファイルに追記保存しました: {fullPath}");
        AssetDatabase.Refresh();
    }

    private void SaveToJsonOverwrite()
    {
        if (string.IsNullOrEmpty(saveFolderPath) || string.IsNullOrEmpty(fileName))
        {
            EditorUtility.DisplayDialog("エラー", "保存先フォルダとファイル名を指定してください。", "OK");
            return;
        }

        string fullPath = Path.Combine(saveFolderPath, fileName);
        string json = JsonConvert.SerializeObject(objectDataList, Formatting.Indented);
        File.WriteAllText(fullPath, json);
        Debug.Log($"JSONファイルを上書き保存しました: {fullPath}");
        AssetDatabase.Refresh();
    }
}
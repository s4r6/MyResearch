using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectMasterDataEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;
    List<ObjectData> objectEntities = new();

    VisualElement scrollView;

    VisualTreeAsset ObjectFoldoutTemplate;
    VisualTreeAsset ChoiceFoldoutTemplate;
    List<ObjectFoldoutElement> foldoutElements = new();

    [MenuItem("Window/UI Toolkit/ObjectMasterDataEditor")]
    public static void ShowExample()
    {
        ObjectMasterDataEditor wnd = GetWindow<ObjectMasterDataEditor>();
        wnd.titleContent = new GUIContent("ObjectMasterDataEditor");
    }

    public void CreateGUI()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/ObjectMasterDataEditor.uxml");
        visualTree.CloneTree(rootVisualElement);

        ObjectFoldoutTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Assets/Scripts/Editor/Templates/ObjectFoldoutTemplate.uxml"
        );

        ChoiceFoldoutTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Assets/Scripts/Editor/Templates/ChoiceFoldoutTemplate.uxml"
        );

        var AddObject = rootVisualElement.Q<Button>("AddObject");
        var SaveButton = rootVisualElement.Q<Button>("SaveJson");
        var LoadButton = rootVisualElement.Q<Button>("LoadJson");
        scrollView = rootVisualElement.Q<ScrollView>("ObjectScrollView");

        AddObject.clicked += AddNewObjectFoldout;
        SaveButton.clicked += SaveToJson;
        LoadButton.clicked += LoadFromJson;
    }

    void AddNewObjectFoldout()
    {
        var foldout = new ObjectFoldoutElement(ObjectFoldoutTemplate, ChoiceFoldoutTemplate);

        foldout.OnRequestDelete += (self) =>
        {
            scrollView.Remove(self);
            foldoutElements.Remove(self);
        };

        foldoutElements.Add(foldout);
        scrollView.Add(foldout);
    }

    void SaveToJson()
    {
        string path = EditorUtility.SaveFilePanel(
            "Save Object Master JSON",
            "Assets/Resources/Master",       // 初期ディレクトリ
            "StageObjects",                  // デフォルトファイル名
            "json"                           // 拡張子
         );

        if (string.IsNullOrEmpty(path))
        {
            Debug.Log("保存キャンセルされました。");
            return;
        }

        var objectList = new List<ObjectData>();

        foreach (var foldout in foldoutElements)
        {
            objectList.Add(foldout.ToData());
        }

        string json = JsonConvert.SerializeObject(objectList, Formatting.Indented);

        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, json);
        AssetDatabase.Refresh();

        Debug.Log("保存完了: " + path);
    }

    void LoadFromJson()
    {
        string path = EditorUtility.OpenFilePanel(
            "Load Object Master JSON",
            "Assets/Resources/Master",       // 初期ディレクトリ
            "json"                           // 拡張子
        );

        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            Debug.LogWarning("ファイルが存在しないか、読み込みがキャンセルされました。");
            return;
        }

        if (!File.Exists(path))
        {
            Debug.LogWarning("ファイルが存在しません: " + path);
            return;
        }

        string json = File.ReadAllText(path);
        var objectList = JsonConvert.DeserializeObject<List<ObjectData>>(json);

        scrollView.Clear();
        foldoutElements.Clear();

        foreach (var data in objectList)
        {
            var foldout = new ObjectFoldoutElement(ObjectFoldoutTemplate, ChoiceFoldoutTemplate);
            foldout.LoadFromData(data);

            foldout.OnRequestDelete += (self) =>
            {
                scrollView.Remove(self);
                foldoutElements.Remove(self);
            };

            foldoutElements.Add(foldout);
            scrollView.Add(foldout);
        }

        Debug.Log("読み込み完了: " + path);
    }
}

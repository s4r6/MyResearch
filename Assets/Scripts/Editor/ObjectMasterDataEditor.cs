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
            "Assets/Resources/Master",       // �����f�B���N�g��
            "StageObjects",                  // �f�t�H���g�t�@�C����
            "json"                           // �g���q
         );

        if (string.IsNullOrEmpty(path))
        {
            Debug.Log("�ۑ��L�����Z������܂����B");
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

        Debug.Log("�ۑ�����: " + path);
    }

    void LoadFromJson()
    {
        string path = EditorUtility.OpenFilePanel(
            "Load Object Master JSON",
            "Assets/Resources/Master",       // �����f�B���N�g��
            "json"                           // �g���q
        );

        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            Debug.LogWarning("�t�@�C�������݂��Ȃ����A�ǂݍ��݂��L�����Z������܂����B");
            return;
        }

        if (!File.Exists(path))
        {
            Debug.LogWarning("�t�@�C�������݂��܂���: " + path);
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

        Debug.Log("�ǂݍ��݊���: " + path);
    }
}

using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

public class ObjectDataMasterEditor : EditorWindow
{
    [MenuItem("Tools/ObjectMasterEditor")]
    public static void ShowWindow()
    {
        ObjectDataMasterEditor window = GetWindow<ObjectDataMasterEditor>();
        window.titleContent = new GUIContent("ObjectData Master Editor");
    }

    public void CreateGUI()
    {
        
    }
}

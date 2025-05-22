using System;
using System.Collections.Generic;
using Domain.Stage.Object;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectFoldoutElement : VisualElement
{
    private VisualTreeAsset choiceTemplate;
    private VisualElement choiceContainer;
    private Foldout objectFoldout;

    public event Action<ObjectFoldoutElement> OnRequestDelete;

    VisualTreeAsset ActionFoldoutTemplate;

    List<ChoiceFoldoutElement> foldoutElements = new();
    public ObjectFoldoutElement(VisualTreeAsset foldoutTemplate, VisualTreeAsset choiceTemplate)
    {
        ActionFoldoutTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Assets/Scripts/Editor/Templates/ActionFoldoutTemplate.uxml"
        );

        this.choiceTemplate = choiceTemplate;
        foldoutTemplate.CloneTree(this);

        var addChoiceButton = this.Q<Button>("AddChoice");
        objectFoldout = this.Q<Foldout>("ObjectFoldout");

        addChoiceButton.clicked += AddChoice;

        var objectId = this.Q<TextField>("ObjectIdTextField");
        var objectName = this.Q<TextField>("NameTextField");
        var description = this.Q<TextField>("DescriptionTextField");
        objectId.RegisterValueChangedCallback(evt =>
        {
            objectFoldout.text = evt.newValue;
        });


        var deleteButton = new Button(() =>
        {
            OnRequestDelete?.Invoke(this); // 外へ削除リクエスト
        })
        {
            text = "×"
        };

        deleteButton.style.alignSelf = Align.FlexEnd;
        deleteButton.style.marginTop = 4;
        deleteButton.style.marginBottom = 4;

        objectFoldout.Add(deleteButton);
    }

    public void AddChoice()
    {
        //ChoiceをまとめるFoldoutを作成
        if(choiceContainer == null)
        {
            choiceContainer = new Foldout { text = "Choices", value = true };
            choiceContainer.name = "ChoiceContainer";
            objectFoldout.Add(choiceContainer);
        }

        var choiceElement = new ChoiceFoldoutElement(choiceTemplate, ActionFoldoutTemplate);

        choiceElement.OnRequestDelete += (self) =>
        {
            choiceContainer.Remove(self);
            foldoutElements.Remove(self);

            if (choiceContainer.childCount == 0)
            {
                objectFoldout.Remove(choiceContainer);
                choiceContainer = null;
            }
        };

        foldoutElements.Add(choiceElement);
        choiceContainer.Add(choiceElement);
    }

    public ObjectData ToData()
    {
        var objectId = this.Q<TextField>("ObjectIdTextField").value;
        var displayName = this.Q<TextField>("NameTextField").value;
        var description = this.Q<TextField>("DescriptionTextField").value;

        var data = new ObjectData
        {
            ObjectId = objectId,
            DisplayName = displayName,
            Description = description,
            Choices = new List<ChoiceData>()
        };

        if (choiceContainer == null)
            return data;

        foreach (var choice in foldoutElements)
        {
            data.Choices.Add(choice.ToData());
        }

        return data;
    }

    public void LoadFromData(ObjectData data)
    {
        var objectId = this.Q<TextField>("ObjectIdTextField");
        var displayName = this.Q<TextField>("NameTextField");
        var description = this.Q<TextField>("DescriptionTextField");

        objectId.value = data.ObjectId;
        displayName.value = data.DisplayName;
        description.value = data.Description;

        objectFoldout.text = objectId.value;

        foreach (var choiceData in data.Choices)
        {
            AddChoiceFromData(choiceData);
        }
    }

    private void AddChoiceFromData(ChoiceData choiceData)
    {
        if (choiceContainer == null)
        {
            choiceContainer = new Foldout { text = "Choices", value = true };
            choiceContainer.name = "ChoiceContainer";
            objectFoldout.Add(choiceContainer);
        }

        var choice = new ChoiceFoldoutElement(choiceTemplate, ActionFoldoutTemplate);
        choice.LoadFromData(choiceData);

        choice.OnRequestDelete += (self) =>
        {
            choiceContainer.Remove(self);
            foldoutElements.Remove(self);

            if (choiceContainer.childCount == 0)
            {
                objectFoldout.Remove(choiceContainer);
                choiceContainer = null;
            }
        };

        foldoutElements.Add(choice);
        choiceContainer.Add(choice);
    }
}

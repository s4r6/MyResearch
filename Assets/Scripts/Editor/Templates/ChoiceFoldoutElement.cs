using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ChoiceFoldoutElement : VisualElement
{
    private Foldout rootFoldout;
    private VisualTreeAsset ActionTemplate;
    private Foldout ActionsFoldout;

    public event Action<ChoiceFoldoutElement> OnRequestDelete;

    List<ActionFoldoutElement> foldoutElements = new();
    public ChoiceFoldoutElement(VisualTreeAsset choiceTemplate, VisualTreeAsset ActionTemplate)
    {
        this.ActionTemplate = ActionTemplate;
        choiceTemplate.CloneTree(this);

        rootFoldout = this.Q<Foldout>("ChoiceFoldout");
        var addButton = this.Q<Button>("AddAction");

        addButton.clicked += AddAction;

        var deleteButton = new Button(() =>
        {
            OnRequestDelete?.Invoke(this);
        })
        {
            text = "Å~"
        };

        deleteButton.style.alignSelf = Align.FlexEnd;
        deleteButton.style.marginTop = 4;
        deleteButton.style.marginBottom = 4;
        rootFoldout.Add(deleteButton);
    }

    public void SetFoldoutName(string name)
    {
        rootFoldout.name = name;
    }

    public void AddAction()
    {
        if (ActionsFoldout == null)
        {
            ActionsFoldout = new Foldout { text = " Actions", value = true };
            ActionsFoldout.name = "ActionFoldout";
            rootFoldout.Add(ActionsFoldout);
        }

        var entry = new ActionFoldoutElement(ActionTemplate);

        entry.OnRequestDelete += (self) =>
        {
            ActionsFoldout.Remove(self);
            Debug.Log("ActionçÌèú");
            if (ActionsFoldout.childCount == 0)
            {
                rootFoldout.Remove(ActionsFoldout);
                ActionsFoldout = null;
            }
        };

        foldoutElements.Add(entry);
        ActionsFoldout.Add(entry);
    }

    public ChoiceData ToData()
    {
        var label = this.Q<TextField>("LabelTextField")?.value ?? "";
        var riskId = this.Q<TextField>("RiskIdTextField")?.value ?? "";

        var data = new ChoiceData
        {
            Label = label,
            RiskId = riskId,
            OverrideActions = new List<ActionData>()
        };

        if (ActionsFoldout == null)
            return data;
        
        foreach(var action in foldoutElements)
        {
            data.OverrideActions.Add(action.ToData());
        }

        return data;
    }

    public void LoadFromData(ChoiceData data)
    {
        this.Q<TextField>("LabelTextField").value = data.Label;
        this.Q<TextField>("RiskIdTextField").value = data.RiskId;

        foreach (var action in data.OverrideActions)
        {
            AddActionFromData(action);
        }
    }

    private void AddActionFromData(ActionData data)
    {
        if (ActionsFoldout == null)
        {
            ActionsFoldout = new Foldout { text = "Actions", value = true };
            ActionsFoldout.name = "ActionFoldout";
            rootFoldout.Add(ActionsFoldout);
        }

        var action = new ActionFoldoutElement(ActionTemplate);
        action.LoadFromData(data);

        action.OnRequestDelete += (self) =>
        {
            ActionsFoldout.Remove(self);
            Debug.Log("ActionçÌèú");
            if (ActionsFoldout.childCount == 0)
            {
                rootFoldout.Remove(ActionsFoldout);
                ActionsFoldout = null;
            }
        };

        foldoutElements.Add(action);
        ActionsFoldout.Add(action);
    }
}

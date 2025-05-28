using System;
using System.Collections.Generic;
using Codice.Utils;
using UnityEngine;
using UnityEngine.UIElements;

public class ActionFoldoutElement : VisualElement
{
    Foldout rootFoldout;
    Foldout attrContainer;
    public event Action<ActionFoldoutElement> OnRequestDelete;

    List<TextField> attrElemensts = new();
    public ActionFoldoutElement(VisualTreeAsset template)
    {
        template.CloneTree(this);

        rootFoldout = this.Q<Foldout>("ActionFoldout");
        var addAttributeButton = this.Q<Button>("AddObjectAttribute");


        addAttributeButton.clicked += AddAttribute;

        var deleteButton = new Button(() =>
        {
            Debug.Log("削除イベント");
            OnRequestDelete?.Invoke(this); // 親に削除を依頼
        })
        {
            text = "×"
        };

        deleteButton.style.alignSelf = Align.FlexEnd;
        deleteButton.style.marginTop = 4;
        deleteButton.style.marginBottom = 4;

        rootFoldout.Add(deleteButton);
    }

    public void AddAttribute()
    {
        if (attrContainer == null)
        {
            attrContainer = new Foldout { text = "Object Attributes", value = true };
            attrContainer.name = "ObjectAttributeContainer";
            rootFoldout.Add(attrContainer);
        }

        var row = new VisualElement();
        row.style.flexDirection = FlexDirection.Row;
        row.style.alignItems = Align.Center;

        var attrField = new TextField
        {
            label = "",
            style = { flexGrow = 1 }
        };

        var deleteButton = new Button(() =>
        {
            attrContainer.Remove(row);

            if (attrContainer.childCount == 0)
            {
                rootFoldout.Remove(attrContainer);
                attrContainer = null;
            }
        })
        {
            text = "×"
        };

        deleteButton.style.width = 24;
        deleteButton.style.unityTextAlign = TextAnchor.MiddleCenter;
        deleteButton.style.marginLeft = 4;

        row.Add(attrField);
        row.Add(deleteButton);
        attrElemensts.Add(attrField);
        attrContainer.Add(row);
    }

    public ActionData ToData()
    {
        var actionId = this.Q<TextField>("ActionIdField")?.value ?? "";
        var label = this.Q<TextField>("LabelField")?.value ?? "";
        var riskChange = this.Q<IntegerField>("RiskChangeField")?.value ?? 0;
        var apCost = this.Q<IntegerField>("ActionPointCostField")?.value ?? 0;
        var target = this.Q<TextField>("TargetTextField")?.value ?? "";

        Debug.Log(actionId);
        Debug.Log(label);
        Debug.Log(riskChange);
        Debug.Log(apCost);
        Debug.Log(target);

        var data = new ActionData
        {
            id = actionId,
            label = label,
            riskChange = riskChange,
            ActionPointCost = apCost,
            target = target,
            ObjectAttributes = new List<string>()
        };

        if (attrContainer == null)
            return data;

        foreach (var field in attrElemensts)
        {
            data.ObjectAttributes.Add(field.value);
        }

        return data;
    }

    public void LoadFromData(ActionData data)
    {
        this.Q<TextField>("ActionIdField").value = data.id;
        this.Q<TextField>("LabelField").value = data.label;
        this.Q<IntegerField>("RiskChangeField").value = data.riskChange;
        this.Q<IntegerField>("ActionPointCostField").value = data.ActionPointCost;
        this.Q<TextField>("TargetTextField").value = data.target;

        foreach (var attr in data.ObjectAttributes)
        {
            AddAttribute(attr);
        }
    }

    public void AddAttribute(string value)
    {
        if (attrContainer == null)
        {
            attrContainer = new Foldout { text = "Object Attributes", value = true };
            attrContainer.name = "ObjectAttributeContainer";
            rootFoldout.Add(attrContainer);
        }

        var row = new VisualElement();
        row.style.flexDirection = FlexDirection.Row;
        row.style.alignItems = Align.Center;

        var attrField = new TextField
        {
            label = "",
            value = value,
            style = { flexGrow = 1 }
        };

        var deleteButton = new Button(() =>
        {
            attrContainer.Remove(row);

            if (attrContainer.childCount == 0)
            {
                rootFoldout.Remove(attrContainer);
                attrContainer = null;
            }
        })
        {
            text = "×"
        };

        deleteButton.style.width = 24;
        deleteButton.style.marginLeft = 4;

        row.Add(attrField);
        row.Add(deleteButton);
        attrElemensts.Add(attrField);
        attrContainer.Add(row);
    }
}

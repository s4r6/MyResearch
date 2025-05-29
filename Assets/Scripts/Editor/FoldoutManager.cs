using System.Collections.Generic;
using UnityEngine;

public class FoldoutManager
{
    public List<bool> ObjectFoldouts = new();
    public List<List<bool>> ChoiceFoldouts = new();
    public List<List<List<bool>>> ActionFoldouts = new();

    public void Ensure(int i, int j = -1, int k = -1)
    {
        while (ObjectFoldouts.Count <= i)
            ObjectFoldouts.Add(true);
        while (ChoiceFoldouts.Count <= i)
            ChoiceFoldouts.Add(new List<bool>());
        while (ActionFoldouts.Count <= i)
            ActionFoldouts.Add(new List<List<bool>>());

        if (j >= 0)
        {
            while (ChoiceFoldouts[i].Count <= j)
                ChoiceFoldouts[i].Add(true);
            while (ActionFoldouts[i].Count <= j)
                ActionFoldouts[i].Add(new List<bool>());
        }

        if (j >= 0 && k >= 0)
        {
            while (ActionFoldouts[i][j].Count <= k)
                ActionFoldouts[i][j].Add(true);
        }
    }

    public void RemoveAt(int i)
    {
        ObjectFoldouts.RemoveAt(i);
        ChoiceFoldouts.RemoveAt(i);
        ActionFoldouts.RemoveAt(i);
    }

    public void RemoveChoiceAt(int i, int j)
    {
        ChoiceFoldouts[i].RemoveAt(j);
        ActionFoldouts[i].RemoveAt(j);
    }

    public void RemoveActionAt(int i, int j, int k)
    {
        ActionFoldouts[i][j].RemoveAt(k);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsNear : MonoBehaviour {


    #region Singleton

    public static ItemsNear instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one instance of ItemNear is trying to active");
            return;
        }

        instance = this;
    }

    #endregion

    public List<Interactionable> targets = new List<Interactionable>();
	
    public void AddNewElement(Interactionable element)
    {
        targets.Add(element);
    }

    public void DeleteElement(Interactionable element)
    {
        targets.Remove(element);
    }

    public void EmptyList()
    {
        targets.Clear();
    }

    public int Size()
    {
        return targets.Count;
    }

    public Interactionable GetInteractionable(int i)
    {
        return targets[i];
    }
}

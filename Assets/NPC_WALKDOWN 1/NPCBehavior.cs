using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events; 

public class NPCBehavior : MonoBehaviour
{
    public StandCategory WantedCategory { get; private set; }

    public float Happiness { get; private set; } = 0.5f; // npc happiness

    
    public StandController TargetStand { get; private set; } 

    public void SetWantedCategory(StandCategory category)
    {
        WantedCategory = category;
    }

    public void SetTargetStand(StandController stand)
    {
        TargetStand = stand;
    }

    public void DebugWantedCategoryAndTargetStand()
    {
        string standName = TargetStand != null ? TargetStand.gameObject.name : "none";
        Debug.Log($"NPC wants category: {WantedCategory} and is heading to stand: {standName}");
    }

    // sets happiness to full when NPC reaches the desired stand
    public void SetHappinessFull()
    {
        Happiness = 1.0f; // full happiness
        Debug.Log(gameObject.name + " reached their desired stand and is now fully happy!"); //debug out - ernest 
    }

    [HideInInspector]
    public UnityEvent OnReachedTarget = new UnityEvent(); // [ hide  ] 

    //call the happiness to change to full and invoke the onreach unity target event. -ernest
    public void OnReachedTargetStand()
    {
        SetHappinessFull();
        OnReachedTarget.Invoke();
    }
}
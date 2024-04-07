using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class NPCBehavior : MonoBehaviour
{
    public StandCategory WantedCategory { get; private set; }

    //stand npc wants to visit.
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
}

using UnityEngine;

using System.Collections.Generic;

[System.Serializable]
public class NPCData
{
    public int ID;
    public string Name;
    public List<int> shopItemIDs;
    public NPCType Type;
}

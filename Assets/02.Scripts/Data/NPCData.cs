using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCData
{
    public int ID;
    public string Name;
    public List<int> shopItemIDs;
    public NPCType Type;
}

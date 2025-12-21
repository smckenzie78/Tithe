using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCData", menuName = "Scriptable Objects/NPCData")]
public class NPCData : ScriptableObject
{
    public enum NPC_Class { Trader, Farmer, Deity, Villager };
    public NPC_Class NPC_class;
    public List<string> baseDialogue;
    
}

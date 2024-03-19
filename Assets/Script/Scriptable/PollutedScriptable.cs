using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPolluted", menuName = "Polluted ScriptableObject")]

public class PollutedScriptable : ScriptableObject
{
    public string nameBuilding;
    public int id;
    public Material mat;
    public List<RessourceScriptable> input;
    public List<int> requireAmount;
    public IndustryScriptable industryScriptable;
    //for later use
    public float pollutionLevel;
    public string description;
}

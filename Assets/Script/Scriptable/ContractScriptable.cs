using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//pour un effet moins répétitif mettre un random léger pour tweak les qté requises

[CreateAssetMenu(fileName = "NewContract", menuName = "Contract ScriptableObject")]
public class ContractScriptable : ScriptableObject
{
    public int id;
    public string nameContract;
    public string reward;
    public int rewardQty;
    public RessourceScriptable requiredRessource;
    public int requiredQty;

    public int difficultyLevel;
}

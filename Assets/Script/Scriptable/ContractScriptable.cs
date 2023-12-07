using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//pour un effet moins r�p�titif mettre un random l�ger pour tweak les qt� requises

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

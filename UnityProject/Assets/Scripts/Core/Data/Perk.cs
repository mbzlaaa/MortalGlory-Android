using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Perk", menuName = "MortalGlory/Perk")]
public class Perk : ScriptableObject
{
    public string name;
    public int Id;
    public Rarity PerkRarity;
    public Sprite icon;
    public string PrimaryEffect;
    public int PrimaryEffectValue;
    public string SecondaryEffect;
    public int SecondaryEffectValue;
    public string Description;

    public void InitializePerk() { throw new NotImplementedException("InitializePerk"); }
    public string GetTooltip() { throw new NotImplementedException("GetTooltip"); }
    private string GetTranslatedStat(string stat) { throw new NotImplementedException("GetTranslatedStat"); }
}

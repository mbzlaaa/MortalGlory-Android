using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Spellbook", menuName = "MortalGlory/Spellbook")]
public class Spellbook : Item
{
    public Skill TeachesSkill;
    public int itemValue;
    public int shopValue;

    public override void Initialize(int initRarity, int rarityBonus, int maxRarity) { throw new NotImplementedException("Initialize"); }
    public override int GetRealValue() { throw new NotImplementedException("GetRealValue"); }
    public override int GetShopValue() { throw new NotImplementedException("GetShopValue"); }
    public override void DiscountThis() { throw new NotImplementedException("DiscountThis"); }
    public override bool Use(CharacterTemplate User) { throw new NotImplementedException("Use"); }
    public override string GetTooltip() { throw new NotImplementedException("GetTooltip"); }
}

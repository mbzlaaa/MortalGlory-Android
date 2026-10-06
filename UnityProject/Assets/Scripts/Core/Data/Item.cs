using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "MortalGlory/Item")]
public class Item : ScriptableObject
{
    public Sprite icon;
    public string title;
    public Rarity itemRarity;
    public bool discounted;

    // TODO(method-body): restore from cloud ILSpy dump phase
    public virtual bool Use(CharacterTemplate User) { throw new NotImplementedException("Use"); }
    public virtual void Initialize(int Rarity, int rarityBonus, int maxRarity) { throw new NotImplementedException("Initialize"); }
    public virtual int GetRealValue() { throw new NotImplementedException("GetRealValue"); }
    public virtual int GetShopValue() { throw new NotImplementedException("GetShopValue"); }
    public virtual void DiscountThis() { throw new NotImplementedException("DiscountThis"); }
    public virtual string GetTooltip() { throw new NotImplementedException("GetTooltip"); }
}

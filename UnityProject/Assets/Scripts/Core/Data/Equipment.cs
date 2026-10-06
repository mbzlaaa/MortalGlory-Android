using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "MortalGlory/Equipment")]
public class Equipment : Item
{
    public EquipmentSlot equipSlot;
    public classType EquipmentMainType;
    public int itemLevel;
    public string MainStat;
    public string MainStat2;

    public int InitSTRmax;
    public int InitAGImax;
    public int InitWISmax;
    public int InitVITmax;
    public int InitARMORmax;
    public int InitPhysicalDamagemax;
    public int InitKnockbackChancemax;
    public int InitCritChancemax;
    public int InitDodgeChancemax;
    public int InitMPmax;
    public int InitMagicDamagemax;
    public int InitMagicDefensemax;
    public int InitHPmax;
    public int InitDebuffResistancemax;
    public int InitMaxAPmax;
    public int InitArcheryDamagemax;
    public int InitKnockbackResistancemax;
    public int InitDebuffChancemax;

    public int STRmodifier;
    public int AGImodifier;
    public int WISmodifier;
    public int VITmodifier;
    public int ARMORmodifier;
    public int PhysicalDamagemodifier;
    public int KnockbackChancemodifier;
    public int CritChancemodifier;
    public int DodgeChancemodifier;
    public int MaxAPmodifier;
    public int MPmodifier;
    public int MagicDamagemodifier;
    public int MagicDefensemodifier;
    public int HPmodifier;
    public int DebuffResistancemodifier;
    public int ArcheryDamagemodifier;
    public int KnockbackResistancemodifier;
    public int DebuffChancemodifier;

    public Skill givesSkill;
    public Perk givesPerk;

    private int raritylvlbuff;
    private int skillGainChance;
    private int skillGainRarity;
    private int raritystatmaxbonus;
    private float priceRarityModifier;

    public int itemValue;
    public int shopValue;

    private List<string> ChosenPrimaryStats;
    private List<string> ChosenSecondaryStats;
    private int StatPoints;

    public bool Bow;

    // TODO(method-body): restore from cloud ILSpy dump phase
    public override bool Use(CharacterTemplate user) { throw new NotImplementedException("Use"); }
    public override void Initialize(int initRarity, int rarityBonus, int maxRarity) { throw new NotImplementedException("Initialize"); }
    private void SetRarity(int predetermined, int rarityBonus, int maxRarity) { throw new NotImplementedException("SetRarity"); }
    public List<string> FilterStat(string stat, int maxvalue, List<string> list) { throw new NotImplementedException("FilterStat"); }
    public void FillModifier(string stat, float minimumallocation) { throw new NotImplementedException("FillModifier"); }
    public void ArcherizeEquipment() { throw new NotImplementedException("ArcherizeEquipment"); }
    public override int GetRealValue() { throw new NotImplementedException("GetRealValue"); }
    public override int GetShopValue() { throw new NotImplementedException("GetShopValue"); }
    public override void DiscountThis() { throw new NotImplementedException("DiscountThis"); }
    public override string GetTooltip() { throw new NotImplementedException("GetTooltip"); }
}

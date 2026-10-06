using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterTemplate", menuName = "MortalGlory/CharacterTemplate")]
public class CharacterTemplate : ScriptableObject
{
    public string Charname;
    public CharacterRace Race;
    public classType RaceMainType;
    public Sprite Portrait;
    public Sprite Sprite;
    public int CharLevel;

    public int InitSTRmin;
    public int InitSTRmax;
    public int InitAGImin;
    public int InitAGImax;
    public int InitWISmin;
    public int InitWISmax;
    public int InitVITmin;
    public int InitVITmax;
    public int InitARMORmin;
    public int InitARMORmax;

    public Stat STR;
    public Stat AGI;
    public Stat WIS;
    public Stat VIT;
    public Stat ARMOR;

    public int InitPhysicalDamage;
    public int InitKnockbackChance;
    public int InitCritChance;
    public int InitDodgeChance;
    public int InitMaxAP;
    public int InitMP;
    public int InitMagicDamage;
    public int InitMagicDefense;
    public int InitHP;
    public int InitDebuffResistance;
    public int InitKnockbackResistance;
    public int InitDebuffChance;
    public int InitArcheryDamage;

    public Stat PhysicalDamage;
    public Stat KnockbackChance;
    public Stat CritChance;
    public Stat DodgeChance;
    public Stat MaxAP;
    public Stat MP;
    public Stat MagicDamage;
    public Stat MagicDefense;
    public Stat HP;
    public Stat DebuffResistance;
    public Stat KnockbackResistance;
    public Stat DebuffChance;
    public Stat ArcheryDamage;

    public int CurrentHP;
    public int CurrentMP;
    public int WinCount;
    public int LossCount;
    public int KillCount;
    public int InjuryCount;

    public Skill Skill1;
    public Skill Skill2;
    public Skill Skill3;
    public Skill Skill4;
    public Perk Perk1;
    public Perk Perk2;
    public Perk Perk3;
    public Perk Perk4;

    public int characterValue;
    public int shopValue;
    public float priceRarityModifier;
    public Rarity characterRarity;
    private int skillGainChance;

    public bool Boss;
    public bool AIChar;
    public string Title;
    public Perk PerkFromTitle;
    public string StatGainFromTitle;

    public List<Skill> CharacterSkills;
    public List<Perk> CharacterPerks;
    public List<Skill> CharacterSkillsFromGear;
    public List<Perk> CharacterPerksFromGear;
    public List<Equipment> CharacterEquipList;

    public EnemyBattler BattleUnit;

    public int Wage;
    public float InherentWageScalingModifier;
    public int GamesSatInRoster;

    // TODO(method-body): restore from cloud ILSpy dump phase
    public void Initialize(int initRarity, bool AI, int rarityBonus, int maxRarity, float statmodifier, int minRarity) { throw new NotImplementedException("Initialize"); }
    private void SetRarity(int predetermined, int rarityBonus, int maxRarity, int minRarity) { throw new NotImplementedException("SetRarity"); }
    public void UpdateSecondaryStats() { throw new NotImplementedException("UpdateSecondaryStats"); }
    public int GetRealValue() { throw new NotImplementedException("GetRealValue"); }
    public int GetShopValue() { throw new NotImplementedException("GetShopValue"); }
    public void DiscountThis() { throw new NotImplementedException("DiscountThis"); }
    public int CheckLuckAmount() { throw new NotImplementedException("CheckLuckAmount"); }
    public void WageIncrease() { throw new NotImplementedException("WageIncrease"); }
    public int RecalculateWage() { throw new NotImplementedException("RecalculateWage"); }
    public bool LearnSkill(Skill skill) { throw new NotImplementedException("LearnSkill"); }
    public void ForgetSkill(int slot) { throw new NotImplementedException("ForgetSkill"); }
    public void LearnGearSkill(Skill skill) { throw new NotImplementedException("LearnGearSkill"); }
    public void ForgetGearSkill(Skill skill) { throw new NotImplementedException("ForgetGearSkill"); }
    public void LearnGearPerk(Perk perk) { throw new NotImplementedException("LearnGearPerk"); }
    public void RemoveGearPerkEffect(Perk perk) { throw new NotImplementedException("RemoveGearPerkEffect"); }
    public void ClearGearPerks() { throw new NotImplementedException("ClearGearPerks"); }
    public void ClearGearSkills() { throw new NotImplementedException("ClearGearSkills"); }
    public void RearrangeSkills() { throw new NotImplementedException("RearrangeSkills"); }
    public bool DoesCharHaveSkill(Skill skill) { throw new NotImplementedException("DoesCharHaveSkill"); }
    public bool DoesCharHavePerk(Perk perk) { throw new NotImplementedException("DoesCharHavePerk"); }
    public bool IsThereRoomForPerk() { throw new NotImplementedException("IsThereRoomForPerk"); }
    public bool LearnPerk(Perk perk) { throw new NotImplementedException("LearnPerk"); }
    public void ApplyCurrentPerkEffects() { throw new NotImplementedException("ApplyCurrentPerkEffects"); }
    public void ApplyPerkEffect(string PerkEffect, int value) { throw new NotImplementedException("ApplyPerkEffect"); }
    public void RemovePerkEffect(string PerkEffect, int value) { throw new NotImplementedException("RemovePerkEffect"); }
    public string GetTooltip() { throw new NotImplementedException("GetTooltip"); }
    public void CreateListOfCharacterSkills() { throw new NotImplementedException("CreateListOfCharacterSkills"); }
    public void CreateListOfCharacterPerks() { throw new NotImplementedException("CreateListOfCharacterPerks"); }
    public int HowManySkills() { throw new NotImplementedException("HowManySkills"); }
    public int HowManyPerks() { throw new NotImplementedException("HowManyPerks"); }
    public bool CheckTitleValidity() { throw new NotImplementedException("CheckTitleValidity"); }
    public string GainRandomStat(int amount, int presetstat) { throw new NotImplementedException("GainRandomStat"); }
    public string LoseRandomStat(int amount, int presetstat) { throw new NotImplementedException("LoseRandomStat"); }
    public void ShuffleMainStats(int bonus) { throw new NotImplementedException("ShuffleMainStats"); }
    public void RandomizeSkills() { throw new NotImplementedException("RandomizeSkills"); }
    public void SuperSkills() { throw new NotImplementedException("SuperSkills"); }
    public void GainTitle(bool Random) { throw new NotImplementedException("GainTitle"); }
    public Perk GainPerk(int chancebonus, bool PreventAmnesiac) { throw new NotImplementedException("GainPerk"); }
    public string GetTranslatedTitleStatGain() { throw new NotImplementedException("GetTranslatedTitleStatGain"); }
    public string GetTranslatedPerkName() { throw new NotImplementedException("GetTranslatedPerkName"); }
    public void CauseInjuryInTown(int amount) { throw new NotImplementedException("CauseInjuryInTown"); }
    public void HealInjury(int amount) { throw new NotImplementedException("HealInjury"); }
    public int FullRecovery() { throw new NotImplementedException("FullRecovery"); }
    public void UpdateHPMP() { throw new NotImplementedException("UpdateHPMP"); }
    public int RegainHP(int amount) { throw new NotImplementedException("RegainHP"); }
    public int ReduceHP(int amount, bool deathpossible) { throw new NotImplementedException("ReduceHP"); }
    public int RegainMP(int amount) { throw new NotImplementedException("RegainMP"); }
    public int ReduceMP(int amount) { throw new NotImplementedException("ReduceMP"); }
    public int CalculateTrainingCost(int stat, int baseprice) { throw new NotImplementedException("CalculateTrainingCost"); }
    public float EstimateCharacterQuality() { throw new NotImplementedException("EstimateCharacterQuality"); }
    public bool DoesThisCharacterHaveABow() { throw new NotImplementedException("DoesThisCharacterHaveABow"); }
}

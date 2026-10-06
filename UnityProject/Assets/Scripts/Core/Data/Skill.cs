using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "MortalGlory/Skill")]
public class Skill : ScriptableObject
{
    public string name;
    public int Id;
    public Sprite icon;
    public SkillType SkillType;
    public SkillTargets SkillTargets;
    public int Range;
    public int ExclusionRange;
    public int ConsumesTurns;
    public int ExtraDamage;
    public int SkillNumberVariable;
    public int SpecialVariable;
    public int ManaCost;
    public int ShowInLog;
    public string Description;
    public Rarity SkillRarity;
    public bool AIfriendly;
    public classType ClassType;
    public classType BookCategory;
    public DamageType DamageType;
    public int MaxCooldown;
    public Buff CausesBuff;
    public int BuffChance;
    public string HitAnimation;
    public string HitSFX;
    public bool BowSkill;

    // TODO(method-body): restore from cloud ILSpy dump phase
    public string GetTooltip() { throw new NotImplementedException("GetTooltip"); }
    public string GetBuffText() { throw new NotImplementedException("GetBuffText"); }
    public string GetTranslatedTitle() { throw new NotImplementedException("GetTranslatedTitle"); }
}

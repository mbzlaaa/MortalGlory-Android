// MortalGlory - Game Enums (reconstructed from reverse-engineered metadata)
// Namespace-free to match original Assembly-CSharp layout.

public enum classType { Warrior = 0, Mage = 1, Balanced = 2, Archer = 3 }

public enum CharacterRace
{
    Minotaur = 0, Werewolf = 1, Elf = 2, Dryad = 3, Vampire = 4,
    Fiendling = 5, Wizard = 6, Whisperer = 7, Gnome = 8, Troll = 9,
    Undine = 10, Lizard = 11, Dwarf = 12, Slitherer = 13, Prowler = 14,
    Umibozu = 15, Angel = 16
}

public enum DamageType
{
    normal = 0, magic = 1, piercing = 2, collision = 3, scrape = 4,
    NoDamage = 5, MagicHalfPiercing = 6, NormalHalfPiercing = 7,
    MagicPiercing = 8, NormalPiercing = 9
}

public enum DIRECTION { UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3 }

public enum DraggableType { Character = 0, Item = 1 }

public enum EquipmentSlot { Chest = 0, Weapon = 1, Accessory = 2, Random = 3 }

public enum CharacterEquipmentBar { H1 = 0, H2 = 1, H3 = 2, H4 = 3, E1 = 4, E2 = 5, E3 = 6, E4 = 7 }

public enum Rarity { Common = 0, Uncommon = 1, Rare = 2, Epic = 3, Legendary = 4 }

public enum RewardType { Equipment = 0, Item = 1, Character = 2, Gold = 3 }

public enum SkillTargets { Enemy = 0, Ally = 1, Friendly = 2, Anyone = 3, Anything = 4, Self = 5 }

public enum KeyAction
{
    none = 0, upleft = 1, up = 2, upright = 3, right = 4, downright = 5,
    down = 6, downleft = 7, left = 8, pass = 9, skill1 = 10, skill2 = 11,
    skill3 = 12, skill4 = 13, submit = 14, cancel = 15, closetooltips = 16,
    togglegrid = 17
}

public enum SkillType
{
    Melee = 0, MeleeSkill = 1, MagicStraigth = 2, MagicStraigthScaling = 3,
    Ranged = 4, RangedStraigth = 5, RangedStraigthScaling = 6, RangedPotion = 7,
    Magic = 8, Environmental = 9, Knockback = 10, KnockbackNoDamage = 11,
    KnockbackMagic = 12, KnockbackStraigthRanged = 13, KnockbackStraigthRangedScaling = 14,
    KnockbackStraigthMagic = 15, BurnBuffs = 16, DoubleBuffs = 17, BurnBuffsAll = 18,
    DoubleBuffsAll = 19, Buff = 20, BuffAll = 21, DebuffAll = 22, RandomBuffAll = 23,
    RandomDebuffAll = 24, MeleeSelfBuff = 25, MagicSelfBuff = 26, Charge = 27,
    ChargeScaling = 28, ChargeKnockback = 29, ChargeKnockbackScaling = 30, ChargeCont = 31,
    ChargeKnockbackCont = 32, ChargeContScaling = 33, ChargeKnockbackContScaling = 34,
    PacifistCharge = 35, MeleeHitVampiric = 36, MagicHitVampiric = 37, MeleeHitHealing = 38,
    MagicHitHealing = 39, MeleeRegainMP = 40, MeleeDestroyMP = 41, MagicDestroyMP = 42,
    MagicPercentual = 43, BerserkMelee = 44, BerserkBuff = 45, MeleeIcarus = 46,
    MagicIcarus = 47, Immolation = 48, PoisonExplosion = 49, Obliteration = 50,
    Bow = 51, BowSelfBuff = 52, BowStraigth = 53, BowStraigthSpell = 54,
    RandomEnemyTargetSpell = 55, BloodDrinkerMelee = 56, BloodDrinkerSpell = 57,
    BloodDrinkerHeal = 58, BurnBuffsMagic = 59, BurnBuffsMagicStraigth = 60,
    ManaBoostedSpell = 61, ManaBoostedSelfHeal = 62, ManaBoostedKnockback = 63,
    EffectBoostedMelee = 64, EffectBoostedSpell = 65, APBoostedMelee = 66,
    APBoostedSpell = 67, RestoreMP = 68
}

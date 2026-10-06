using System;

[Serializable]
public class BaseCharacter
{
    public string charname;
    public string race;
    public int perk1;
    public int perk2;
    public int perk3;
    public int perk4;
    public float baseHP;
    public float curHP;
    public float baseMP;
    public float curMP;
    public int strength;
    public int intellect;
    public int agility;
    public int LifetimeWins;
    public int LifetimeKills;
    public int LifetimeLosses;
}

[Serializable]
public class BaseHero
{
    public string name;
    public float baseHP;
    public float curHP;
    public float baseMP;
    public float curMP;
    public int stamina;
    public int intellect;
    public int dexterity;
    public int agility;
}

[Serializable]
public class BaseEnemy
{
    public string name;
    public Type EnemyType;
    public Rarity rarity;
    public float baseHP;
    public float curHP;
    public float baseMP;
    public float curMP;
    public float baseATK;
    public float curATK;
    public float baseDEF;
    public float curDEF;
}

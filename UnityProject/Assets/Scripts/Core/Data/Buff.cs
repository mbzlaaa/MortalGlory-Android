using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff", menuName = "MortalGlory/Buff")]
public class Buff : ScriptableObject
{
    public Sprite Icon;
    public bool IsItBuff;
    public string title;
    public string Description;
    public string BuffType;
    public int BuffValue;
    public int Duration;
    public ActivationType Activation;
    public string OnApplyText;
    public string OnApplyAnimation;
    public string OnApplySFX;

    public string GetTooltip() { throw new NotImplementedException("GetTooltip"); }
    public string GetTranslatedTitle() { throw new NotImplementedException("GetTranslatedTitle"); }
    public string GetTranslatedDescription() { throw new NotImplementedException("GetTranslatedDescription"); }
}

// Activation trigger for buffs (referenced by Buff.Activation)
// TODO(enum-verify): value set pending full IL dump (was: System.Enum)
public enum ActivationType { OnTurnStart = 0, OnTurnEnd = 1, OnHit = 2, OnApply = 3, Passive = 4 }

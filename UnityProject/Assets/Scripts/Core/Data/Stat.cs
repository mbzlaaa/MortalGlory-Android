using System;
using System.Collections.Generic;

[Serializable]
public class Stat
{
    private int baseValue;
    private string description;
    private List<int> modifiers;
    private List<int> gearmodifiers;
    private List<int> racemodifiers;
    private List<int> perkmodifiers;
    private List<int> buffmodifiers;
    private List<int> titlemodifiers;
    private int MaxValue;

    // TODO(method-body): restore from cloud ILSpy dump phase
    public void SetMaxValue(int value) { throw new NotImplementedException("SetMaxValue"); }
    public int GetValue() { throw new NotImplementedException("GetValue"); }
    public void ChangeValue(int modifier) { throw new NotImplementedException("ChangeValue"); }
    public void AddModifier(int modifier) { throw new NotImplementedException("AddModifier"); }
    public void RemoveModifier(int modifier) { throw new NotImplementedException("RemoveModifier"); }
    public void AddGearModifier(int modifier) { throw new NotImplementedException("AddGearModifier"); }
    public void RemoveGearModifier(int modifier) { throw new NotImplementedException("RemoveGearModifier"); }
    public void AddTitleModifier(int modifier) { throw new NotImplementedException("AddTitleModifier"); }
    public void RemoveTitleModifier(int modifier) { throw new NotImplementedException("RemoveTitleModifier"); }
    public void AddRaceModifier(int modifier) { throw new NotImplementedException("AddRaceModifier"); }
    public void RemoveRaceModifier(int modifier) { throw new NotImplementedException("RemoveRaceModifier"); }
    public void AddPerkModifier(int modifier) { throw new NotImplementedException("AddPerkModifier"); }
    public void RemovePerkModifier(int modifier) { throw new NotImplementedException("RemovePerkModifier"); }
    public void AddBuffModifier(int modifier) { throw new NotImplementedException("AddBuffModifier"); }
    public void RemoveBuffModifier(int modifier) { throw new NotImplementedException("RemoveBuffModifier"); }
    public void RemovePositiveBuffModifiers() { throw new NotImplementedException("RemovePositiveBuffModifiers"); }
    public void RemoveNegativeBuffModifiers() { throw new NotImplementedException("RemoveNegativeBuffModifiers"); }
    public int GetBaseValue() { throw new NotImplementedException("GetBaseValue"); }
    public int GetGearModifiers() { throw new NotImplementedException("GetGearModifiers"); }
    public int GetTitleModifiers() { throw new NotImplementedException("GetTitleModifiers"); }
    public int GetRaceModifiers() { throw new NotImplementedException("GetRaceModifiers"); }
    public int GetPerkModifiers() { throw new NotImplementedException("GetPerkModifiers"); }
    public int GetBuffModifiers() { throw new NotImplementedException("GetBuffModifiers"); }
    public int GetMiscModifiers() { throw new NotImplementedException("GetMiscModifiers"); }
    public int DoesItHaveModifiers() { throw new NotImplementedException("DoesItHaveModifiers"); }
    public void ClearGearModifiers() { throw new NotImplementedException("ClearGearModifiers"); }
    public void ClearPerkModifiers() { throw new NotImplementedException("ClearPerkModifiers"); }
    public void ClearBuffModifiers() { throw new NotImplementedException("ClearBuffModifiers"); }
    public void InitializeStat(int min, int max) { throw new NotImplementedException("InitializeStat"); }
    public void SetStat(int Amount) { throw new NotImplementedException("SetStat"); }
    public void IncreaseBaseValue(int Amount) { throw new NotImplementedException("IncreaseBaseValue"); }
    public void DecreaseBaseValue(int Amount) { throw new NotImplementedException("DecreaseBaseValue"); }
}

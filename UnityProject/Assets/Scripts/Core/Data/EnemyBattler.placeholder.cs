// MortalGlory - placeholder forward-declaration for the runtime battle unit.
// TODO(battle-phase): replace with the real reconstructed EnemyBattler/HeroBattler
//   once the combat state machines are ported. Field type kept as-is so that
//   CharacterTemplate.cs compiles against the original metadata layout.
using System;
using UnityEngine;

[Serializable]
public class EnemyBattler : MonoBehaviour
{
    // TODO(fields): populate from archived metadata of the original EnemyBattler.
}

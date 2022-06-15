using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyController : MonoBehaviour, IDamageable
{
    public void TakeDamage(int attackerId, float damage)
    {
        Debug.Log($"Dummy dealt {damage} in damage by player {GameManager.instance.GetPlayer(attackerId).photonPlayer.NickName}");
    }
}

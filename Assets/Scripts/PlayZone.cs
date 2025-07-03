using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayZone : MonoBehaviour
{
    public void Hit(int damage)
    {
        if (gameObject.GetComponentInChildren<Card>() != null)
        {
            gameObject.GetComponentInChildren<Card>().Attacked(damage);
        }
    }
    public void Heal(int healValue)
    {
        if (gameObject.GetComponentInChildren<Card>() != null)
        {
            gameObject.GetComponentInChildren<Card>().Healed(healValue);
        }
    }
}

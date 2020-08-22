using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A trap damage area variant for water. Its a trigger this time and applies recoil in up direction.
/// </summary>
public class TrapDamageAreaWater : MonoBehaviour
{

    #region Fields
    [SerializeField] float recoilStrength;
    [SerializeField] float waterDamage;
    #endregion

    /// <summary>
    /// This trigger applies damage and recoil in up direction to the player if he enters.
    /// </summary>
    /// <param name="collision">Checks for the player.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>() != null)
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            player.ApplyRecoil(Vector3.up, recoilStrength, null, true);
            collision.gameObject.GetComponent<CharacterResources>().ReduceHealth(waterDamage);
        }
    }
}

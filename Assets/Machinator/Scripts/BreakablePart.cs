using UnityEngine;

/// <summary>
/// 2026 02 03
/// </summary>
public class BreakablePart : MonoBehaviour, IDamageable
{
    // REference to a main script 

    public BreakableController bc;

    public void ApplyDamage(int amount)
    {
        // Call cab for cal
        // cab.calcdamage();
        bc.CalcDamage(amount);
    }
}
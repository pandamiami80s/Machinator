using UnityEngine;

/// <summary>
/// 2026 02 18
/// Can receive exact damage amount
/// </summary>
public class BreakablePart : MonoBehaviour, IDamageable
{
    public BreakableController breakableController;



    public void TakeDamage(int damage)
    {
        breakableController.DamagePart();
    }
}
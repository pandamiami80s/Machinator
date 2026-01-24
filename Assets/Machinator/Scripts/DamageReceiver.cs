using UnityEngine;

public class DamageReceiver : MonoBehaviour, IDamageable
{
    public int partIndex;
    private TruckCabinController controller;

    void Awake() => controller = GetComponentInParent<TruckCabinController>();

    public void ApplyDamage(float damage)
    {
        if (controller != null) controller.DamagePart(partIndex, damage);
    }
}
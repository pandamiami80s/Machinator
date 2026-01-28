using UnityEngine;

public class VehicleArmorPart : MonoBehaviour, IDamageable
{
    public VehiclePartsController vehiclePartsController;
    public int armor;

    public void ApplyDamage(int damage)
    {
        vehiclePartsController.DamagePart(damage);
    }
}
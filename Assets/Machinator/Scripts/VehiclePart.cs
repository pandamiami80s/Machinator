using UnityEngine;

public class VehiclePart : MonoBehaviour, IDamageable
{
    public VehiclePartsController vehiclePartsController;
    public int armor;

    public void ApplyDamage(int damage)
    {
        vehiclePartsController.DamagePart(damage);
    }
}
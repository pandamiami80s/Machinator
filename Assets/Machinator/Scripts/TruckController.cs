using UnityEngine;
using System.Collections.Generic;

public class TruckController : MonoBehaviour
{
    public List<TurretSlot> turretSlots = new List<TurretSlot>();
    public LayerMask aimLayer; // Слой для наведения (земля/враги)

    // DALNOST = 1000 f 


    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, aimLayer))
        {
            foreach (var slot in turretSlots)
            {
                // Наведение
                slot.AimAt(hit.point);

                // Стрельба на левую кнопку мыши
                if (Input.GetMouseButton(0) && slot.currentWeapon != null)
                {
                    slot.currentWeapon.TryFire(hit.point);
                }
            }
        }
    }
}

using System.Drawing;
using UnityEngine;

// act as waepon
public class Turret : MonoBehaviour
{
    public float fireRate = 0.5f;     // Скорость стрельбы
    public GameObject bulletPrefab;   // Пуля
    public Transform firePoint;       // Откуда вылетает пуля

    private float _nextFireTime;

    public void TryFire(Vector3 hit)
    {
        if (Time.time >= _nextFireTime)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            Debug.DrawLine(firePoint.position, hit, UnityEngine.Color.red, 1.0f);

            _nextFireTime = Time.time + fireRate;
        }
    }
}
using UnityEngine;

/// <summary>
/// 2026 03 01
/// </summary>
public class EntityHealth : MonoBehaviour
{
    public float health = 100;

    public void DecreaseHealth(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(transform.root.gameObject);
        }
    }
}

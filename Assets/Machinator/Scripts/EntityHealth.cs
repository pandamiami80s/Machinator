using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EntityHealth : MonoBehaviour, IDamageable
{
    public int health = 100;
    ///int currentHealth;



    public void TakeDamage(int amount)
    {
        health -= amount;


        Debug.Log("Health damage taken " + amount);

        if (health <= 0)
        {
            Debug.Log("Health depleted");
        }
    }
}

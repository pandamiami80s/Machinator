using UnityEngine;

//[RequireComponent(typeof(Collider))]
public class EntityHealth : MonoBehaviour, IDamageable
{
    public float health = 100;
    ///int currentHealth;



    public void AddGlobalDamage(float amount)
    {
        health -= amount;


        Debug.Log("Health damage taken " + amount);

        if (health <= 0)
        {
            Debug.Log("Health depleted");
        }
    }

    public void ApplyDamage(float amount)
    {
        throw new System.NotImplementedException();
    }
}

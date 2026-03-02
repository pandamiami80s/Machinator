using UnityEngine;

public class TargetAI : MonoBehaviour
{
    public WeaponController wp;
    public Transform target;



    void Start()
    {
        
    }

    void Update()
    {
        if (target != null)
        {
            wp.SetFiring(true);
            wp.SetTarget(true, target.position);
        }
        else
        {
            wp.SetFiring(false);
            wp.SetTarget(true, transform.position + transform.forward * 100.0f);
        }
    }
}

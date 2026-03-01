using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 2026 03 01
///     isPiercing =  Go through enemies, not walls!
/// </summary>
public class RaycastType : MonoBehaviour
{
    [SerializeField] float range = 100.0f;
    [SerializeField] int damage = 10;
    [SerializeField] LayerMask hitLayerMask = 1;

    [Header("Piercing")]
    // Go through enemies, not walls!
    [SerializeField] bool isPiercing = false;

    [Header("Bounce")]
    [SerializeField] int maxBounce = 0;
    int currentBounce;
    [SerializeField] LayerMask bounceLayerMask;
    // Case: When bounceMask and hitMask contains same layer (Sometimes ignores hit layer when bounce)
    float hitOffset = 0.01f;
    [Min(0.1f)]
    [SerializeField] float distanceMultiplier = 0.5f;

    [Header("Path")]
    [SerializeField] float drawDuration = 1.0f;
    List<Vector3> path = new List<Vector3>();
    [SerializeField] OnDraw onDraw;
    [System.Serializable] class OnDraw : UnityEvent<Vector3[], float> { }



    void Start()
    {
        // If object is pooled
        path.Clear();
        currentBounce = 0;

        Vector3 startPosition = transform.position;
        path.Add(startPosition);

        Vector3 direction = transform.forward;
        Raycast(startPosition, direction, range);

        onDraw.Invoke(path.ToArray(), drawDuration);
    }

    void Raycast(Vector3 origin, Vector3 direction, float distance)
    {
        // Hit everything because we have IDamageable and dont want to set layers
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, distance, hitLayerMask);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.TryGetComponent(out IDamageable damageable))
            {
                // Enemy
                damageable.TakeDamage(10);
                if (!isPiercing)
                {
                    path.Add(hit.point);
                    
                    return;
                }
            }
            else
            {
                // Bounce
                path.Add(hit.point);
                if (currentBounce < maxBounce)
                {
                    currentBounce++;

                    float remainingDistance = (distance - hit.distance) * distanceMultiplier;
                    Vector3 reflectedDirection = Vector3.Reflect(direction, hit.normal);
                    Raycast(hit.point + hit.normal * hitOffset, reflectedDirection, remainingDistance);
                }

                return; 
            }
        }
        
        path.Add(origin + direction * distance);
    }
}
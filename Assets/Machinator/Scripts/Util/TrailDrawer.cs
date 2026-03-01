using System.Collections;
using UnityEngine;

/// <summary>
/// 2026 03 01
///     Material starts from RIGHT
/// </summary>
[RequireComponent(typeof(TrailRenderer))]
public class TrailDrawer : MonoBehaviour
{
    [SerializeField] TrailRenderer trailRenderer;

    public void OnDrawTrail(Vector3[] path, float time)
    {
        // Keep drawing untill time is out
        //trailRenderer.time = time;

        StartCoroutine(IEDrawTrail(path, time));
    }

    IEnumerator IEDrawTrail(Vector3[] path, float time)
    {
        // Calculate how much time each segment gets
        float timePerSegment = time / (path.Length - 1);

        for (int i = 1; i < path.Length; i++)
        {
            Vector3 startPos = transform.position;
            Vector3 targetPos = path[i];
            float elapsed = 0;

            // Rotate towards the next point
            Vector3 direction = targetPos - startPos;
            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
            
            while (elapsed < timePerSegment)
            {
                elapsed += Time.deltaTime;

                float normalizedTime = elapsed / timePerSegment;
                // Move the object based on percentage of completion
                transform.position = Vector3.Lerp(startPos, targetPos, normalizedTime);

                yield return null;
            }

            transform.position = targetPos;
        }

        Destroy(gameObject);
    }
}
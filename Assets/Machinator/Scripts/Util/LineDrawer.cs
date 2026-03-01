using System.Collections;
using UnityEngine;

/// <summary>
/// 2026 03 01
///     Material starts from LEFT
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class LineDrawer : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;

    public void OnDrawLine(Vector3[] path, float time)
    {
        StartCoroutine(IEDrawLine(path, time));
    }

    IEnumerator IEDrawLine(Vector3[] path, float time)
    {
        lineRenderer.positionCount = path.Length;
        lineRenderer.SetPositions(path);

        yield return new WaitForSeconds(time);

        Destroy(gameObject);
    }
}

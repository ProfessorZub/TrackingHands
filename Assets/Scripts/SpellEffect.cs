using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SpellEffect : MonoBehaviour
{
    LineRenderer lineRenderer;

    public HandTracking hand;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.01f; 
        lineRenderer.endWidth = 0.01f; 
    }

    void Update()
    {
        int sum = hand.HandSum;

        // Calculate hand position
        Vector3 pos = hand.transform.position;

        // Get forward direction of the player's headset
        Vector3 forward = OVRManager.instance.transform.forward;

        // Offset the position d meters forward from the midpoint
        float d = 1;
        Vector3 offset = forward * d;


        // Update GameObject's position
        transform.position = pos + offset;

        GenerateSpellEffect(sum);
    }

    void GenerateSpellEffect(int points)
    {
        if (points == 0)
        {
            // Clear the line renderer if there are no points
            lineRenderer.positionCount = 0;
            return;
        }
        else if (points == 1)
        {
            // Draw a small dash
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, new Vector3(0, 0, 0));
            lineRenderer.SetPosition(1, new Vector3(0.1f, 0, 0));
            return;
        }

        float radius = 0.5f;
        int steps = FindFirstRelativePrime(points);
        lineRenderer.positionCount = points + 1;

        for (int i = 0; i <= points; i++)
        {
            float radian = Mathf.Deg2Rad * 360f * (i * steps % points) / points;
            float x = Mathf.Cos(radian) * radius;
            float y = Mathf.Sin(radian) * radius;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
    }


    int GCD(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    int FindFirstRelativePrime(int n)
    {
        for (int i = 2; i < n; i++)
        {
            if (GCD(n, i) == 1)
            {
                return i;
            }
        }
        return -1; // no relative prime found, should never happen for n > 2
    }

}

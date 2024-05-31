using UnityEngine;

[System.Serializable]
public class Annulus
{
    public float distance;
    public float innerRadius;
    public float outerRadius;

    public Vector3 GetRandomPoint()
    {
        float angle = Random.Range(0f, Mathf.PI);

        float radius = Random.Range(innerRadius, outerRadius);

        float x = radius * Mathf.Cos(angle);
        float y = radius * Mathf.Sin(angle);

        return new Vector3(x, y, distance);
    }
}
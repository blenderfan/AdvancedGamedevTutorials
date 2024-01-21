using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : MonoBehaviour
{

    public int points;

    public LineRenderer lineRenderer;

    public World world;

    public void Start()
    {
        Vector3[] positions = new Vector3[this.points];

        float radius = this.world.radius;
        float currentAngle = 0.0f;
        float angleIncrease = 360.0f / this.points;
        for(int i = 0; i < this.points; i++)
        {
            float sin = Mathf.Sin(currentAngle * Mathf.Deg2Rad);
            float cos = Mathf.Cos(currentAngle * Mathf.Deg2Rad);

            positions[i] = new Vector3(sin * radius, cos * radius, 0.0f);

            currentAngle += angleIncrease;
        }

        this.lineRenderer.positionCount = this.points;
        this.lineRenderer.SetPositions(positions);
    }

}

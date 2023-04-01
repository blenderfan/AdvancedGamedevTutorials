using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon2D
{
    public List<Vector2> points = new List<Vector2>();

    public Polygon2D(List<Vector2> points)
    {
        this.points = points;
    }

    public bool IsInside(Vector2 position)
    {
        float windingNumber = 0.0f;

        //Going round in a circle
        for(int i = 0; i < this.points.Count; i++)
        {
            var a = this.points[i];
            var b = this.points[(i + 1) % this.points.Count];

            //Calculate the positions relative to the point
            var pointA = position - a;
            var pointB = position - b;

            //If one point is above and one point is below, only one of them has a negative value. Therefore if we multiply them together and
            //the number is negative, the edge crosses the horizontal line
            if(pointA.y * pointB.y < 0.0f)
            {
                //r represents the X-Coordinate relative to our position (name r was chosen in literature, it is not my doing ^^)
                //Calculating the crossing point would be
                //p = a + Mathf.InverseLerp(b.y, a.y, 0) * (b - a);
                //So calculating r is the same as:
                //r = a.x + Mathf.InverseLerp(b.y, a.y, 0) * (b.x - a.x);
                //If you write it out you'd get the code below:
                float r = pointA.x + (pointA.y * (pointB.x - pointA.x)) / (pointA.y - pointB.y);
                if(r < 0)
                {
                    if(pointA.y < 0.0f)
                    {
                        windingNumber += 1.0f;
                    } else
                    {
                        windingNumber -= 1.0f;
                    }
                }
            } else if(pointA.y == 0.0f && pointA.x > 0.0f)
            {
                if(pointB.y > 0.0f)
                {
                    windingNumber += 0.5f;
                } else
                {
                    windingNumber -= 0.5f;
                }
            }
            else if(pointB.y == 0.0f && pointB.x > 0.0f)
            {
                if(pointA.y < 0.0f)
                {
                    windingNumber += 0.5f;
                } else
                {
                    windingNumber -= 0.5f;
                }
            }
        }

        return ((int)windingNumber % 2) != 0;
    }
}

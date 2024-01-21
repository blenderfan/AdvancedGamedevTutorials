using UnityEngine;

public class VoronoiLookupTable
{

    public int[] table;

    public Vector2Int dimension;

    public Rect bounds;


    public int GetTableIndex(Vector2 position)
    {
        if(this.bounds.Contains(position))
        {
            float percentX = Mathf.InverseLerp(this.bounds.xMin, this.bounds.xMax, position.x);
            float percentY = Mathf.InverseLerp(this.bounds.yMin, this.bounds.yMax, position.y);

            int indexX = (int)(percentX * this.dimension.x);
            int indexY = (int)(percentY * this.dimension.y);

            return indexY * this.dimension.x + indexX;
        }
        return -1;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphLR : MonoBehaviour
{
    #region Public Variables

    public float scrollSpeed = 1.0f;
    public float posAddInterval = 0.1f;
    public Vector3 offset;

    public GameObject followedObject;

    public int positionBufferSize = 500;

    public LineRenderer lr;

    #endregion

    #region Private Variables

    private float posIntervalTimer = 0.0f;

    private List<Vector3> positionBuffer = new List<Vector3>();

    #endregion

    private void Start()
    {
        
    }


    private void Update()
    {
        float scrollOffset = Time.deltaTime * this.scrollSpeed;

        for (int i = 0; i < this.positionBuffer.Count; i++)
        {
            this.positionBuffer[i] -= scrollOffset * Vector3.right;
        }

        var pos = transform.position;
        pos.y = this.followedObject.transform.position.y;
        pos += this.offset;

        this.posIntervalTimer += Time.deltaTime;

        if (this.posIntervalTimer > this.posAddInterval) {

            this.positionBuffer.Add(pos);
            this.posIntervalTimer -= this.posAddInterval;
        }

        if(this.positionBuffer.Count > this.positionBufferSize)
        {
            this.positionBuffer.RemoveAt(0);
        }

        this.lr.positionCount = this.positionBuffer.Count;
        this.lr.SetPositions(this.positionBuffer.ToArray());

    }
}

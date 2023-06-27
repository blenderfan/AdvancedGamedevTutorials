using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class FishSwarm : MonoBehaviour
{

    #region Public Variables

    public int instances = 10000;

    public Material fishMaterial = null;

    public Mesh fishMesh = null;

    #endregion

    #region Private Variables

    private GraphicsBuffer fishTransformsBuffer;

    private NativeArray<Matrix4x4> fishTransforms;

    private RenderParams renderParams;

    #endregion


    private static readonly int MAX_INSTANCES_PER_DRAW_CALL = 1023;

    public void Start()
    {
        this.Init();
    }

    private unsafe void InitData()
    {
        this.fishTransforms = new NativeArray<Matrix4x4>(this.instances, Allocator.Persistent);



        this.fishTransformsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, this.instances, sizeof(Matrix4x4));

    }

    public void Init()
    {
        this.InitData();

        this.renderParams = new RenderParams()
        {
            material = this.fishMaterial,
        };
    }


    private void Update()
    {
        int loopSize = this.instances / MAX_INSTANCES_PER_DRAW_CALL;
        if (loopSize % MAX_INSTANCES_PER_DRAW_CALL != 0) loopSize++;

        for(int i = 0; i < loopSize; i++)
        {
            int idx = i * MAX_INSTANCES_PER_DRAW_CALL;
            int remaining = MAX_INSTANCES_PER_DRAW_CALL;
            if(i == loopSize - 1 && this.instances % MAX_INSTANCES_PER_DRAW_CALL != 0)
            {
                remaining = this.instances % MAX_INSTANCES_PER_DRAW_CALL;
            }

            Graphics.RenderMeshPrimitives(this.renderParams, this.fishMesh, 0, remaining);
        }
        

    }
}

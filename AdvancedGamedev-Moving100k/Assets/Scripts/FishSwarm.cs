using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class FishSwarm : MonoBehaviour, IDisposable
{

    #region Public Variables

    public ComputeShader positionShader;

    public float swarmRadius = 5.0f;
    public float swarmSpeed = 1.0f;
    public float swarmRotationSpeed = 5.0f;

    public Hook hook = null;

    public int instances = 10000;

    public Material fishMaterial = null;

    public Mesh fishMesh = null;

    public Vector2 velocityRange = Vector2.zero;
    public Vector2 timeOffsetRange = Vector2.zero;
    public Vector2 scaleRange = Vector2.zero;

    #endregion

    #region Private Variables

    private ComputeShader positionShaderInstance = null;

    private GraphicsBuffer fishTransformsBuffer;
    private GraphicsBuffer fishDataBuffer;

    private Unity.Mathematics.Random rnd;

    private int moveKernel = 0;
    private int threadGroupsX = 0;

    private RenderParams renderParams;

    #endregion



    public void Start()
    {
        this.Init();
    }

    [BurstCompile]
    private struct FishTransformsInitJob : IJobParallelFor
    {
        public float radius;

        [WriteOnly, NoAlias]
        public NativeArray<Matrix4x4> matrices;

        [WriteOnly, NoAlias]
        public NativeArray<FishData> fishData;

        public float3 center;
        public float2 velocityRange;
        public float2 timeOffsetRange;
        public float2 scaleRange;

        public Unity.Mathematics.Random random;

        public void Execute(int index)
        {
            var offset = this.random.NextFloat3Direction() * this.radius * (1.0f - this.random.NextFloat() * this.random.NextFloat());
            float scale = Mathf.Lerp(this.scaleRange.x, this.scaleRange.y, this.random.NextFloat());

            this.matrices[index] = Matrix4x4.identity;
            this.fishData[index] = new FishData()
            {
                offset = offset,
                velocity = (this.random.NextFloat() * (this.velocityRange.y - this.velocityRange.x)) + this.velocityRange.x,
                color = this.random.NextFloat4(),
                timeOffset = (this.random.NextFloat() * (this.timeOffsetRange.y - this.timeOffsetRange.x)) + this.timeOffsetRange.x,
                scale = scale,
            };
        }
    }

    private unsafe void InitData()
    {
        var fishTransforms = new NativeArray<Matrix4x4>(this.instances, Allocator.TempJob);
        var fishData = new NativeArray<FishData>(this.instances, Allocator.TempJob);

        var fishTransformsInitJob = new FishTransformsInitJob()
        {
            center = this.transform.position,
            matrices = fishTransforms,
            radius = this.swarmRadius,
            random = this.rnd,
            fishData = fishData,
            velocityRange = this.velocityRange,
            timeOffsetRange = this.timeOffsetRange,
            scaleRange = this.scaleRange
        };

        fishTransformsInitJob.Schedule(this.instances, 64).Complete();

        this.fishTransformsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, this.instances, sizeof(Matrix4x4));
        this.fishTransformsBuffer.SetData(fishTransforms);

        this.fishDataBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, this.instances, sizeof(FishData));
        this.fishDataBuffer.SetData(fishData);

        fishTransforms.Dispose();
        fishData.Dispose();

    }

    private void SetMaterialBuffers()
    {
        this.positionShaderInstance.SetBuffer(this.moveKernel, ShaderConstants.FISH_SWARM_TRANSFORMS_ID, this.fishTransformsBuffer);
        this.positionShaderInstance.SetBuffer(this.moveKernel, ShaderConstants.FISH_SWARM_DATA_ID, this.fishDataBuffer);

        this.fishMaterial.SetBuffer(ShaderConstants.FISH_SWARM_TRANSFORMS_ID, this.fishTransformsBuffer);
        this.fishMaterial.SetBuffer(ShaderConstants.FISH_SWARM_DATA_ID, this.fishDataBuffer);
    }

    public void Init()
    {
        this.positionShaderInstance = Instantiate(this.positionShader);

        uint x, y, z;
        this.positionShaderInstance.GetKernelThreadGroupSizes(this.moveKernel, out x, out y, out z);

        this.threadGroupsX = this.instances / (int)x;
        if (this.instances % x != 0) this.threadGroupsX++;



        this.rnd = new Unity.Mathematics.Random();
        this.rnd.InitState();

        this.moveKernel = this.positionShaderInstance.FindKernel("Move");

        this.InitData();
        this.SetMaterialBuffers();

        //Deactivate frustum culling for now, as we adjust the positions in the shader itself
        var worldBounds = new Bounds();
        worldBounds.center = Vector3.zero;
        worldBounds.extents = new Vector3(10e8f, 10e8f, 10e8f);

        this.renderParams = new RenderParams()
        {
            material = this.fishMaterial,
            worldBounds = worldBounds,
        };
    }


    private void MoveSwarm()
    {

        this.positionShaderInstance.SetVector(ShaderConstants.FISH_SWARM_TARGET_ID, this.hook.transform.position);
        this.positionShaderInstance.SetMatrix(ShaderConstants.SWARM_LOCAL_TO_WORLD, this.transform.localToWorldMatrix);
        this.positionShaderInstance.SetFloat(ShaderConstants.TIME_ID, Time.time);

        this.positionShaderInstance.Dispatch(this.moveKernel, this.threadGroupsX, 1, 1);

        var dir = (this.hook.transform.position - this.transform.position).normalized;
        this.transform.position += dir * this.swarmSpeed * Time.deltaTime;

        if (dir != Vector3.zero)
        {
            var lookDirection = Quaternion.LookRotation(dir.normalized);
            var currentDirection = Quaternion.LookRotation(this.transform.forward);

            this.transform.rotation = Quaternion.Slerp(currentDirection, lookDirection, this.swarmRotationSpeed * Time.deltaTime);
        }
    }


    private void Update()
    {
        this.MoveSwarm();

        Graphics.RenderMeshPrimitives(this.renderParams, this.fishMesh, 0, this.instances);
        

    }

    private void OnDestroy()
    {
        this.Dispose();
    }

    public void Dispose()
    {
        if(this.positionShaderInstance != null)
        {
            Destroy(this.positionShaderInstance);
        }

        if(this.fishTransformsBuffer != null)
        {
            this.fishTransformsBuffer.Release();
        }

        if(this.fishDataBuffer != null)
        {
            this.fishDataBuffer.Release();
        }
    }
}

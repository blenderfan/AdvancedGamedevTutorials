using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

public class SpaceshipSpawner : MonoBehaviour
{
    #region Public Fields

    public ComputeShader voronoi;

    public float spaceshipSize;

    public GameObject spaceshipPrefab;

    public int numberOfSpaceships;

    public Material voronoiMaterial;

    public Vector2Int tableSize;

    public World world;

    #endregion

    #region Private Fields

    private bool usingLookupTable = false;
    private bool calculatingTableInProgress = false;

    private float voronoiAlpha = 0.0f;

    private GraphicsBuffer sitesBuffer;
    private GraphicsBuffer tableBuffer;

    private List<Spaceship> spaceships = new List<Spaceship>();

    private Sampler spaceshipSampler = null;

    private Texture2D voronoiTexture = null;

    private VoronoiLookupTable lookupTable = null;

    private static ProfilerMarker spaceshipMarker = new ProfilerMarker("Spaceships");

    #endregion

    public bool IsUsingLookupTable() => this.usingLookupTable;

    public float CurrentVoronoiAlpha() => this.voronoiAlpha;

    public Sampler GetSpaceshipSampler() => this.spaceshipSampler;

    public void SetVoronoiAlpha(float alpha)
    {
        var color = Color.white;
        color.a = alpha;
        this.voronoiMaterial.SetColor(ShaderConstants.COLOR_ID, color);
        this.voronoiAlpha = alpha;
    }

    private void FillAndSetTexture(int sites, VoronoiLookupTable table)
    {
        if(this.voronoiTexture == null)
        {
            this.voronoiTexture = new Texture2D(this.tableSize.x, this.tableSize.y, TextureFormat.ARGB32, false);
        }

        for (int x = 0; x < this.tableSize.x; x++)
        {
            for (int y = 0; y < this.tableSize.y; y++)
            {
                int index = table.table[y * this.tableSize.x + x];

                int prevX = (x - 1);
                if (prevX < 0) prevX = 0;

                int nextX = (x + 1);
                if (nextX >= this.tableSize.x) nextX = this.tableSize.x - 1;

                int prevY = (y - 1);
                if (prevY < 0) prevY = 0;

                int nextY = (y + 1);
                if (nextY >= this.tableSize.y) nextY = this.tableSize.y - 1;

                int prevXIndex = table.table[y * this.tableSize.x + prevX];
                int nextXIndex = table.table[y * this.tableSize.x + nextX];
                int prevYIndex = table.table[prevY * this.tableSize.x + x];
                int nextYIndex = table.table[nextY * this.tableSize.x + x];

                float red = 0.0f;
                float green = 0.0f;
                float blue = (index / (float)sites) * 0.5f;
                if (prevXIndex != nextXIndex || prevYIndex != nextYIndex)
                {
                    red = 1.0f;
                    green = 1.0f;
                    blue = 1.0f;
                }

                this.voronoiTexture.SetPixel(x, y, new Color(red, green, blue, 1.0f));
            }
        }
        this.voronoiTexture.Apply();
        this.voronoiMaterial.SetTexture(ShaderConstants.MAIN_TEX_ID, this.voronoiTexture);
    }

    private IEnumerator CalculateVoronoiLookupTable()
    {
        this.calculatingTableInProgress = true;

        int kernel = this.voronoi.FindKernel(ShaderConstants.VORONOI_KERNEL);

        var bounds = new Rect(-this.world.radius, -this.world.radius, this.world.radius * 2.0f, this.world.radius * 2.0f);

        this.voronoi.SetVector(ShaderConstants.BOUNDS_ID, new Vector4(bounds.xMin, bounds.yMin, bounds.xMax, bounds.yMax));
        this.voronoi.SetInt(ShaderConstants.TABLE_SIZE_X_ID, this.tableSize.x);
        this.voronoi.SetInt(ShaderConstants.TABLE_SIZE_Y_ID, this.tableSize.y);

        var asteroidSpawner = GameObject.FindObjectOfType<AsteroidSpawner>();
        var asteroidPositions = asteroidSpawner.GetAsteroidPositions();
        float4[] sites = new float4[asteroidPositions.Count];
        for(int i = 0; i < asteroidPositions.Count; i++)
        {
            sites[i] = new float4(asteroidPositions[i].x, asteroidPositions[i].y, 0.0f, 0.0f);
        }

        if(this.sitesBuffer != null && this.sitesBuffer.IsValid())
        {
            this.sitesBuffer.Release();
        }

        this.sitesBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, asteroidPositions.Count, sizeof(float) * 4);
        this.sitesBuffer.SetData(sites);

        this.voronoi.SetBuffer(kernel, ShaderConstants.SITES_ID, this.sitesBuffer);

        if(this.tableBuffer != null && this.tableBuffer.IsValid())
        {
            this.tableBuffer.Release();
        }

        this.tableBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, this.tableSize.x * this.tableSize.y, sizeof(int) * 4);

        this.voronoi.SetBuffer(kernel, ShaderConstants.RESULT_ID, this.tableBuffer);

        this.voronoi.GetKernelThreadGroupSizes(kernel, out uint threadsX, out uint threadsY, out uint threadsZ);

        int groupsX = this.tableSize.x / (int)threadsX;
        if (this.tableSize.x % threadsX != 0) groupsX++;

        int groupsY = this.tableSize.y / (int)threadsY;
        if(this.tableSize.y % threadsY != 0) groupsY++;

        this.voronoi.Dispatch(kernel, groupsX, groupsY, 1);

        var request = AsyncGPUReadback.Request(this.tableBuffer);

        while (!request.done) yield return null;

        this.lookupTable = new VoronoiLookupTable();

        var tableData = request.GetData<int4>();

        int[] table = new int[this.tableSize.x * this.tableSize.y];
        for(int i = 0; i < tableData.Length; i++)
        {
            table[i] = tableData[i].x;
        }

        this.lookupTable.table = table;
        this.lookupTable.dimension = this.tableSize;
        this.lookupTable.bounds = bounds;

        this.FillAndSetTexture(asteroidPositions.Count, this.lookupTable);

        this.calculatingTableInProgress = false;
    }

    private void OnDestroy()
    {
        if (this.tableBuffer != null && this.tableBuffer.IsValid()) this.tableBuffer.Release();
        if (this.sitesBuffer != null && this.sitesBuffer.IsValid()) this.sitesBuffer.Release();


        if(this.voronoiTexture != null)
        {
            GameObject.Destroy(this.voronoiTexture);
        }
    }

    public void UseVoronoiLookupTable(bool enable)
    {
        this.usingLookupTable = enable;
        if(this.usingLookupTable && !this.calculatingTableInProgress)
        {
            this.StartCoroutine(this.CalculateVoronoiLookupTable());
        } else
        {
            this.SetVoronoiAlpha(0.0f);
        }
    }

    public void SpawnSpaceships(int nr)
    {
        for (int i = 0; i < nr; i++)
        {
            var spaceship = GameObject.Instantiate(this.spaceshipPrefab);

            var pos = this.transform.position;
            var radius = Mathf.Sqrt(UnityEngine.Random.value) * (this.world.radius - this.spaceshipSize);
            var angle = UnityEngine.Random.Range(0.0f, 360.0f);

            var offset = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right;

            pos = pos + offset * radius;

            spaceship.transform.position = pos;
            spaceship.transform.localScale = Vector3.one * this.spaceshipSize;
            spaceship.transform.parent = this.transform;

            var spaceshipComponent = spaceship.GetComponentInChildren<Spaceship>();
            spaceshipComponent.world = this.world;
            spaceshipComponent.spaceshipSize = this.spaceshipSize;

            this.spaceships.Add(spaceshipComponent);
        }
    }

    void Start()
    {
        this.SetVoronoiAlpha(0.0f);
        this.SpawnSpaceships(this.numberOfSpaceships);
    }

    private void Update()
    {
        for(int i = 0; i < this.spaceships.Count; i++)
        {
            if (this.usingLookupTable)
            {
                this.spaceships[i].UpdatePositionAndVelocity(spaceshipMarker, lookupTable);
            }
            else
            {
                this.spaceships[i].UpdatePositionAndVelocity(spaceshipMarker);
            }
        }

        if (this.spaceshipSampler == null || !this.spaceshipSampler.isValid)
        {
            this.spaceshipSampler = Sampler.Get("Spaceships");
        }
    }
}

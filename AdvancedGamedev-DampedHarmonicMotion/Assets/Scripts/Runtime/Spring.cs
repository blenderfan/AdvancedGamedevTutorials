using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    #region Public Variables

    public bool dampened = false;

    public float amplitude = 1.0f;
    public float frequency = 1.0f;
    public float dampening = 0.1f;

    public GameObject cylinder;
    public GameObject weight;

    public int verticesPerSection;
    public int sectionsPerWinding;

    public SpringParameters parameters;

    #endregion

    #region Private Variables

    private float equilibriumHeight;
    private float phase = 0.0f;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    #endregion

    private void Start()
    {
        this.meshRenderer = GetComponent<MeshRenderer>();
        this.meshFilter = GetComponent<MeshFilter>();

        this.equilibriumHeight = this.parameters.height;
    }

    private void MoveSHM()
    {
        this.parameters.height = this.equilibriumHeight + Mathf.Cos(this.frequency * Mathf.PI * 2.0f * this.phase) * this.amplitude;

        this.phase += Time.deltaTime;
    }

    private void MoveDHM()
    {
        float amplitudeScale = Mathf.Exp(-this.dampening * 0.5f * this.phase);
        this.parameters.height = this.equilibriumHeight + Mathf.Cos(this.frequency * Mathf.PI * 2.0f * this.phase) * this.amplitude * amplitudeScale;

        this.phase += Time.deltaTime;
    }

    private void Update()
    {
        if (this.dampened)
        {
            this.MoveDHM();
        }
        else
        {
            this.MoveSHM();
        }

        float cylinderHeight = this.cylinder.transform.localScale.y;

        this.cylinder.transform.position = this.transform.position + Vector3.down * (this.parameters.height + cylinderHeight * 0.5f);
        this.weight.transform.position = this.transform.position + Vector3.down * (this.parameters.height + cylinderHeight);

        var currentSpringMesh = this.meshFilter.sharedMesh;
        if (currentSpringMesh != null)
        {
            GameObject.Destroy(currentSpringMesh);
        }

        var spring = SpringMeshGenerator.GenerateSpring(this.parameters, this.sectionsPerWinding, this.verticesPerSection);
        

        this.meshFilter.sharedMesh = spring;
    }

}

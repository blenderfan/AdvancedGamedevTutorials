using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public static class SpringMeshGenerator
{
    

    public static Mesh GenerateSpring(SpringParameters springParams, int sectionsPerWinding, int verticesPerSection)
    {
        var triangles = new NativeList<int>(Allocator.TempJob);
        var vertices = new NativeList<Vector3>(Allocator.TempJob);

        var springJob = new SpringGenerateJob()
        {
            crossSectionRadius = springParams.crossSectionRadius,
            crossSectionVertices = verticesPerSection,
            height = springParams.height,
            sectionsPerWinding = sectionsPerWinding,
            springRadius = springParams.springRadius,
            triangles = triangles,
            vertices = vertices,
            windings = springParams.windings,
        };

        springJob.Schedule().Complete();

        var springMesh = new Mesh();

        springMesh.SetVertices(vertices.AsArray());
        springMesh.SetTriangles(triangles.ToArray(), 0);

        triangles.Dispose();
        vertices.Dispose();

        springMesh.RecalculateBounds();
        springMesh.RecalculateNormals();

        return springMesh;
    }

    [BurstCompile]
    public struct SpringGenerateJob : IJob
    {

        public float height;
        public float windings;
        public float springRadius;
        public float crossSectionRadius;

        public int sectionsPerWinding;
        public int crossSectionVertices;

        [NoAlias]
        public NativeList<Vector3> vertices;

        [WriteOnly, NoAlias]
        public NativeList<int> triangles;

        private void FillCrossSection(NativeArray<Vector3> crossSection)
        {
            float anglePerVertex = (Mathf.PI * 2.0f) / (float)this.crossSectionVertices;
            float currentAngle = 0.0f;
            for(int i = 0; i < this.crossSectionVertices; i++)
            {
                math.sincos(currentAngle, out float sin, out float cos);

                Vector3 pos = new Vector3(0.0f, sin * this.crossSectionRadius, cos * this.crossSectionRadius);

                crossSection[i] = pos;

                currentAngle += anglePerVertex;
            }
        }

        private void TranslateSection(ref NativeArray<Vector3> section, NativeArray<Vector3> original,  Vector3 offset, float angle)
        {
            for(int i = 0; i < section.Length; i++)
            {
                section[i] = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.up) * original[i];
                section[i] += offset;
            }
        }

        private void AddTubeTriangles(int start)
        {
            for(int i = 0; i < this.crossSectionVertices; i++)
            {
                int nextIdx = ((i + 1) % this.crossSectionVertices);

                this.triangles.Add(start + i + this.crossSectionVertices);
                this.triangles.Add(start + nextIdx);
                this.triangles.Add(start + i);


                this.triangles.Add(start + i + this.crossSectionVertices);
                this.triangles.Add(start + nextIdx + this.crossSectionVertices);
                this.triangles.Add(start + nextIdx);
            }
        }

        public void Execute()
        {
            this.vertices.Clear();
            this.triangles.Clear();

            float regularWindings = this.windings - 2.0f;

            float anglePerPosition = (Mathf.PI * 2.0f) / this.sectionsPerWinding;
            float heightPerPosition = this.height / (this.windings * this.sectionsPerWinding);

            NativeArray<Vector3> crossSection = new NativeArray<Vector3>(this.crossSectionVertices, Allocator.Temp);
            this.FillCrossSection(crossSection);

            NativeArray<Vector3> currentSection = new NativeArray<Vector3>(this.crossSectionVertices, Allocator.Temp);
            this.AddTubeTriangles(this.vertices.Length);

            for (int i = 0; i < crossSection.Length; i++)
            {
                currentSection[i] = crossSection[i];
                this.vertices.Add(crossSection[i]);
            }

            float currentHeight = 0.0f;
            float currentSpringAngle = 0.0f;
            //Start
            for(int i = 0; i <  this.sectionsPerWinding; i++)
            {
                if(i > 0)
                {
                    this.AddTubeTriangles(this.vertices.Length);

                    float currentRadius = (i / (float)this.sectionsPerWinding) * this.springRadius;
                    math.sincos(currentSpringAngle, out float sin, out float cos);
                    var offset = new Vector3(sin * currentRadius, currentHeight, cos * currentRadius);

                    this.TranslateSection(ref currentSection, crossSection, offset, currentSpringAngle);

                    for(int j = 0; j < currentSection.Length; j++)
                    {
                        this.vertices.Add(currentSection[j]);
                    }
                }

                currentSpringAngle += anglePerPosition;
                currentHeight -= heightPerPosition;
            }

            int regularPositions = (int)regularWindings * this.sectionsPerWinding;
            for(int i = 0; i < regularPositions; i++)
            {
                this.AddTubeTriangles(this.vertices.Length);

                math.sincos(currentSpringAngle, out float sin, out float cos);
                var offset = new Vector3(sin * this.springRadius, currentHeight, cos * this.springRadius);

                this.TranslateSection(ref currentSection, crossSection, offset, currentSpringAngle);

                for (int j = 0; j < currentSection.Length; j++)
                {
                    this.vertices.Add(currentSection[j]);
                }

                currentSpringAngle += anglePerPosition;
                currentHeight -= heightPerPosition;
            }

            //End
            for(int i = 0; i < this.sectionsPerWinding; i++)
            {
                if (i < this.sectionsPerWinding - 1)
                {
                    this.AddTubeTriangles(this.vertices.Length);
                }

                float currentRadius = (1.0f - (i / (float)this.sectionsPerWinding)) * this.springRadius;
                math.sincos(currentSpringAngle, out float sin, out float cos);
                var offset = new Vector3(sin * currentRadius, currentHeight, cos * currentRadius);

                this.TranslateSection(ref currentSection, crossSection, offset, currentSpringAngle);

                for (int j = 0; j < currentSection.Length; j++)
                {
                    this.vertices.Add(currentSection[j]);
                }

                currentSpringAngle += anglePerPosition;
                currentHeight -= heightPerPosition;
            }
        }
    }

}

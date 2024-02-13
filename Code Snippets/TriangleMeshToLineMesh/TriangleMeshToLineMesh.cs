

//You can simply save a mesh with line topology like this:
[ContextMenu("Save Line Mesh")]
public void SaveLineMesh()
{
    var lineMesh = this.TriangleMeshToLineMesh(this.target.sharedMesh, 0);
    AssetDatabase.CreateAsset(lineMesh, "Assets/LineMesh.asset");
    AssetDatabase.SaveAssets();
}


//Creates a line mesh from a triangle mesh, where for each triangle three
//lines are created (three pairs of vertices)
//You could do better with first collecting neighboring triangles, but it
//would not have fitted as easily on the screen for the short video ^^
private Mesh TriangleMeshToLineMesh(Mesh triangleMesh, int submeshIndex)
{
    var lineMesh = new Mesh();

    var vertices = triangleMesh.vertices;
    var triangles = triangleMesh.GetTriangles(submeshIndex);
    var uv = triangleMesh.uv;

    int indexCount = triangles.Length * 2;
    var lineVertices = new Vector3[indexCount];
    var lineUV = new Vector2[indexCount];
    var indicesRange = Enumerable.Range(0, indexCount);
    var lineIndices = indicesRange.ToArray();

    for (int i = 0; i < triangles.Length; i += 3)
    {
        int a = triangles[i + 0];
        int b = triangles[i + 1];
        int c = triangles[i + 2];

        lineVertices[i * 2 + 0] = vertices[a];
        lineVertices[i * 2 + 1] = vertices[b];

        lineVertices[i * 2 + 2] = vertices[b];
        lineVertices[i * 2 + 3] = vertices[c];

        lineVertices[i * 2 + 4] = vertices[c];
        lineVertices[i * 2 + 5] = vertices[a];

        lineUV[i * 2 + 0] = uv[a];
        lineUV[i * 2 + 1] = uv[b];

        lineUV[i * 2 + 2] = uv[b];
        lineUV[i * 2 + 3] = uv[c];

        lineUV[i * 2 + 4] = uv[c];
        lineUV[i * 2 + 5] = uv[a];
    }

    lineMesh.vertices = lineVertices;
    lineMesh.uv = lineUV;
    lineMesh.SetIndices(lineIndices, MeshTopology.Lines, 0);
    return lineMesh;
}

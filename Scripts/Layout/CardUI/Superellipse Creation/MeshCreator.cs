/// <summary>
/// Credit - ryanslikesocool 
/// Sourced from - https://github.com/ryanslikesocool/Unity-Card-UI
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnityEngine.UI.Extensions
{

///Credit where credit is due
///https://wiki.unity3d.com/index.php?title=Triangulator  
[ExecuteInEditMode]
public class MeshCreator : MonoBehaviour
{
    public void CreateMesh(List<Vector2> points)
    {
        // Create Vector2 vertices
        Vector2[] vertices2D = points.ToArray();

        // Use the triangulator to get indices for creating triangles
        Triangulator tr = new Triangulator(vertices2D);
        int[] indices = tr.Triangulate();

        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[vertices2D.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
        }

        // Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        // Set up game object with mesh;
        GetComponent<MeshFilter>().mesh = msh;
    }
}
}
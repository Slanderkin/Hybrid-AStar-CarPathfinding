using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]

public class GridMesh : MonoBehaviour
{

    private int GridSizeX = (int)Parameters.worldSizeX / Parameters.cellWidth;
    private int GridSizeY = (int)Parameters.worldSizeY / Parameters.cellWidth;
    
    private int squareSize = Pathfinding.Parameters.cellWidth;

    /*
     World size is 3000 x 1500 y as of rn
         */

    private int indexOffset;

    void Awake()
    {
        MeshFilter filter = gameObject.GetComponent<MeshFilter>();
        var mesh = new Mesh();
        var verticies = new List<Vector3>();

        var indicies = new List<int>();
        for (int i = 0; i <= GridSizeX; i++)
        {
            verticies.Add(new Vector3(i * squareSize, 0, 0));
            verticies.Add(new Vector3(i * squareSize, GridSizeY * squareSize, 0));

            indicies.Add(2 * i + 0);
            indicies.Add(2 * i + 1);

        }
        indexOffset = (GridSizeX + 1) * 2;
        for (int i=0; i <= GridSizeY; i++)
        {
            verticies.Add(new Vector3(0, i * squareSize, 0));
            verticies.Add(new Vector3(GridSizeX * squareSize, i * squareSize, 0));

            indicies.Add(2 * i + 0 + indexOffset);
            indicies.Add(2 * i + 1 + indexOffset);
        }

        
        mesh.vertices = verticies.ToArray();
        mesh.SetIndices(indicies.ToArray(), MeshTopology.Lines, 0);
        filter.mesh = mesh;

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer.material.color = Color.white;

        meshRenderer.enabled = false;
    }
}
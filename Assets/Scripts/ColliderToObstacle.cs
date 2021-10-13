using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEditor;
using UnityEngine;

public class ColliderToObstacle : MonoBehaviour
{
    public MeshCollider     meshCollider;
    public GraphUpdateScene graphUpdateScene;

    [ContextMenu("Generate")]
    public void Generate()
    {
        graphUpdateScene.points = new Vector3[meshCollider.sharedMesh.vertexCount];
        for (int i = 0; i < graphUpdateScene.points.Length; i++)
        {
            graphUpdateScene.points[i] = meshCollider.sharedMesh.vertices[i];
        }
    }
}

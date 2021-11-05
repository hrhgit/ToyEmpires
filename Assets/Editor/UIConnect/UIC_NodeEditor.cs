using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIC_Node))]
public class UIC_NodeEditor : UnityEditor.Editor
{
    UIC_Node uicNode;

    public override void OnInspectorGUI()
    {
        if (uicNode == null)
            uicNode = (UIC_Node)target;
        nodePosition = uicNode.transform.position;
        spLineControlPosition = uicNode.spLineControlPointTranform.position;
        DrawDefaultInspector();
    }

    Vector3 nodePosition;
    Vector3 spLineControlPosition;
    void OnSceneGUI()
    {
        Handles.DrawLine(nodePosition, spLineControlPosition);
    }
}

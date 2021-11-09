using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UIC_Manager : MonoBehaviour
{
    // v2.0 - entity list not static to localize UIC to children
    // list of entities in the scene, used to improve performance of detections 
    List<UIC_Entity>        _entityList = new List<UIC_Entity>();
    public List<UIC_Entity> EntityList { get => _entityList; }

    // v2.0 - made not static to localize UIC to children
    // list of connections in the scene, used to improve performance of detections
    List<UIC_Connection>        _connectionsList =new List<UIC_Connection>();
    public List<UIC_Connection> ConnectionsList { get => _connectionsList; }

    // v2.0 - made not static to localize UIC to children
    public UIC_LineRenderer uiLineRenderer;
    public List<UIC_Line> UILinesList { get => uiLineRenderer.UILines; }

    // v2.0 - made not static to localize UIC to children
    public Canvas canvas;
    public RectTransform canvasRectTransform;

    // v2.0 - made not static to localize UIC to children
    // v1.3 - reference to rendermode
    public RenderMode CanvasRenderMode
    {
        get
        {
            if (!canvas)
                canvas = GetComponent<Canvas>();

            return canvas.renderMode;
        }
    }
    public Camera mainCamera;

    // list of selected uic objects, used for single or multi selection
    public List<I_UIC_Selectable> selectedUIObjectsList = new List<I_UIC_Selectable>();
    public I_UIC_Object clickedUIObject;

    public T GetClickedObjectOfType<T>()
    {
        if (clickedUIObject is T)
            return (T)clickedUIObject;
        else
            return default(T);
    }

    // v2.0 - made public and not static
    public UIC_Pointer pointer;

    [Header("Logic")]
    public bool replaceConnectionByReverse;
    // v2.0 - using detect nodes from raycast instead of distance to pointer
    //float _maxPointerDetectionDistance = 30;
    //public float MaxPointerDetectionDistance
    //{
    //    get
    //    {
    //        return _maxPointerDetectionDistance * canvasRectTransform.localScale.x;
    //    }
    //    set
    //    {
    //        _maxPointerDetectionDistance = value;
    //    }
    //}

    [Header("Connection Settings (for new liens)")]
    public bool disableConnectionClick = false;
    public int globalLineWidth = 2;
    public Color globalLineDefaultColor = Color.white;
    public UIC_Connection.LineTypeEnum globalLineType;
    [Header("- line caps")]
    public UIC_Line.CapTypeEnum globalCapStartType;
    public float globalCapStartSize;
    public Color globalCapStartColor;
    public float globalCapStartAngleOffset;
    public UIC_Line.CapTypeEnum globalCapEndType;
    public float globalCapEndSize;
    public Color globalCapEndColor;
    public float globalCapEndAngleOffset;

    [Header("- line animation")]
    public UIC_LineAnimation globalLineAnimation = new UIC_LineAnimation();

    private void OnEnable()
    {
        InitUILineRenderer();
    }

    public void OnValidate()
    {
        Awake();
        UpdateEntityList();
        InitUILineRenderer();
    }

    public void Awake()
    {
        // canvas = GetComponent<Canvas>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        pointer = GetComponentInChildren<UIC_Pointer>();
    }

    public void Start()
    {
        UpdateEntityList();
        InitUILineRenderer();
    }

    void InitUILineRenderer()
    {
        uiLineRenderer = GetComponentInChildren<UIC_LineRenderer>();
        if (!uiLineRenderer)
        {
            uiLineRenderer = GetComponentInChildren<UIC_LineRenderer>();

            if (!uiLineRenderer)
            {
                uiLineRenderer = Instantiate(Resources.Load("UIC_LineRenderer") as UIC_LineRenderer, transform.position, Quaternion.identity, transform);
                uiLineRenderer.name = "UIC_LineRenderer";
            }
        }
    }

    public void AddLine(UIC_Line line)
    {
        if (!uiLineRenderer.UILines.Contains(line))
            uiLineRenderer.UILines.Add(line);
    }

    public void RemoveLine(UIC_Line line)
    {
        if (uiLineRenderer.UILines.Contains(line))
            uiLineRenderer.UILines.Remove(line);
    }

    // v2.0 - made not static
    // v1.3 - new method instantiate Entity at position
    public void InstantiateEntityAtPosition(UIC_Entity entityToInstantiate, Vector3 position)
    {
        GameObject go = Instantiate(entityToInstantiate.gameObject, new Vector3(200, 100), Quaternion.identity, canvas.transform);
        AddEntity(go.GetComponent<UIC_Entity>());

        // v1.5 - instantiate entity at a convenient world space position 
        if (CanvasRenderMode == RenderMode.ScreenSpaceOverlay)
        {
            go.transform.position = position + new Vector3(15, 15, 0);
        }
        else if (CanvasRenderMode == RenderMode.ScreenSpaceCamera)
        {
            position.z = 0;
            go.transform.localPosition = position + new Vector3(1, 1, 0);
        }
        else if (CanvasRenderMode == RenderMode.WorldSpace)
        {
            position.z = 0;
            go.transform.localPosition = position + new Vector3(1, 1, 0);
            go.transform.localRotation = Quaternion.identity;
        }
    }

    // v2.0 - made not static
    public void AddEntity(UIC_Entity entityToAdd)
    {
        if (!EntityList.Contains(entityToAdd))
        {
            EntityList.Add(entityToAdd);
        }
    }

    // v2.0 - made not static and get entities in children
    public void UpdateEntityList()
    {
        _entityList = new List<UIC_Entity>();
        _entityList.AddRange(GetComponentsInChildren<UIC_Entity>());
    }

    // v1.3 - UIC_Manager.AddConnection return UIC_Connection
    public UIC_Connection AddConnection(UIC_Node node0, UIC_Node node1, UIC_Connection.LineTypeEnum lineType = UIC_Connection.LineTypeEnum.Spline)
    {
        UIC_Connection previousConnectionWithSameNode = NodesAreConnected(node0, node1);
        if (previousConnectionWithSameNode != null)
        {
            if (replaceConnectionByReverse)
            {
                previousConnectionWithSameNode.Remove();
                return previousConnectionWithSameNode;
            }
            else
            {
                return previousConnectionWithSameNode;
            }
        }

        UIC_Connection _connection = CreateConnection(node0, node1, lineType);
        ConnectionsList.Add(_connection);

        node0.connectionsList.Add(_connection);
        node1.connectionsList.Add(_connection);

        AddLine(_connection.line);

        _connection.line.width = globalLineWidth;
        _connection.line.defaultColor = globalLineDefaultColor;
        _connection.line.color = globalLineDefaultColor;

        _connection.line.SetCap(UIC_Line.CapIDEnum.Start, globalCapStartType, globalCapStartSize, globalCapStartColor, globalCapStartAngleOffset);
        _connection.line.SetCap(UIC_Line.CapIDEnum.End, globalCapEndType, globalCapEndSize, globalCapEndColor, globalCapEndAngleOffset);

        CopyAnimation(globalLineAnimation, _connection.line.animation);

        _connection.UpdateLine();

        return _connection;
    }

    public static void CopyAnimation(UIC_LineAnimation from, UIC_LineAnimation to)
    {
        to.Type = from.Type;
        to.isActive = from.isActive;
        to.pointCount = from.pointCount;
        to.size = from.size;
        to.color = from.color;
        to.DrawType = from.DrawType;
        to.speed = from.speed;
        to.time = from.time;
    }

    // v1.4 - NodesAreConnected verification method made public 
    public UIC_Connection NodesAreConnected(UIC_Node node0, UIC_Node node1)
    {
        foreach (UIC_Connection connection in ConnectionsList)
        {
            if ((node0 == connection.node0 && node1 == connection.node1) ||
                    (node0 == connection.node1 && node1 == connection.node0))
                return connection;
        }
        return null;
    }

    UIC_Connection CreateConnection(UIC_Node node0, UIC_Node node1, UIC_Connection.LineTypeEnum lineType)
    {
        UIC_Connection _connection = new UIC_Connection(node0, node1, lineType);
        _connection.line = new UIC_Line();
        _connection.line.width = 2;

        return _connection;
    }

    public void RemoveUIObject()
    {
        for (int i = selectedUIObjectsList.Count - 1; i >= 0; i--)
        {
            selectedUIObjectsList[i].Remove();
        }
    }

    public UIC_Connection FindClosestConnectionToPosition(Vector3 position, float maxDistance)
    {
        float minDist = Mathf.Infinity;
        UIC_Connection closestConnection = null;
        foreach (UIC_Connection connection in ConnectionsList)
        {
            int connectionPointsCount = connection.line.points.Count;
            if (connectionPointsCount > 0)
            {
                // v1.2 - changed from DistanceToSpline(obsolete) to DistanceToConnection, a general and more precise way to find distance to connections independent of the lineType
                for (int i = 1; i < connectionPointsCount; i++)
                {
                    float distance = UIC_Utility.DistanceToConnectino(connection, position, maxDistance);
                    if (distance < minDist)
                    {
                        closestConnection = connection;
                        minDist = distance;
                    }
                }
            }
        }

        return closestConnection;
    }

}


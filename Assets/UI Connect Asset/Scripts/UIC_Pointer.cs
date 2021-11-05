using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class UIC_Pointer : MonoBehaviour
{
    UIC_Manager _uicManager;
    public KeyCode clickKey;
    public KeyCode secondaryKey;

    Image _image;
    Canvas _pointerCanvas;

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    public Sprite iconDefault;
    public Sprite iconHold;

    Vector3 _initialMousePos;

    List<I_UIC_Object> _orderedObjectsList = new List<I_UIC_Object>();

    // v1.3 - corrected pointer position for canvas render mode overlay and camera
    public Vector3 PointerPosition
    {
        get
        {
            return Input.mousePosition;
        }
    }

    public UnityEvent e_OnPointerDownFirst;
    public UnityEvent e_OnPointerDownLast;

    public UnityEvent e_OnDragFirst;
    public UnityEvent e_OnDragLast;

    public UnityEvent e_OnPointerUpFirst;
    public UnityEvent e_OnPointerUpLast;
    // OBS.: event First and Last means that it is called prior or after all the actions on the event

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            UnityEditor.SceneView.RepaintAll();
        }
#endif
    }

    void OnValidate()
    {
        Init();

        Awake();
    }

    void Awake()
    {
        _uicManager = GetComponentInParent<UIC_Manager>();
    }

    void Start()
    {
        e_OnPointerDownFirst = e_OnPointerDownFirst ?? new UnityEvent();
        e_OnPointerDownLast = e_OnPointerDownLast ?? new UnityEvent();
        e_OnDragFirst = e_OnDragFirst ?? new UnityEvent();
        e_OnDragLast = e_OnDragLast ?? new UnityEvent();
        e_OnPointerUpFirst = e_OnPointerUpFirst ?? new UnityEvent();
        e_OnPointerUpLast = e_OnPointerUpLast ?? new UnityEvent();

        Cursor.visible = false;
        FollowMouse();
        Init();

        m_Raycaster = FindObjectOfType<GraphicRaycaster>();
        m_EventSystem = FindObjectOfType<EventSystem>();
    }

    public void Init()
    {
        _image = _image ? _image : GetComponent<Image>() ? GetComponent<Image>() : gameObject.AddComponent<Image>();
        _image.raycastTarget = false;
        _pointerCanvas = _pointerCanvas ? _pointerCanvas : GetComponent<Canvas>() ? GetComponent<Canvas>() : gameObject.AddComponent<Canvas>();
        _pointerCanvas.overrideSorting = true;
        _pointerCanvas.sortingOrder = 999; // pointer on top of everything makes dragged entity being also on top of everithing
    }

    void Update()
    {
        FollowMouse();

        if (Input.GetKeyDown(clickKey))
        {
            OnPointerDown();
        }

        if (Input.GetKey(clickKey))
        {
            if (_initialMousePos != transform.position)
                OnDrag();
        }

        if (Input.GetKeyUp(clickKey))
        {
            OnPointerUp();
        }

    }

    private void OnDisable()
    {
        e_OnPointerDownFirst.RemoveAllListeners();
        e_OnPointerDownLast.RemoveAllListeners();
        e_OnDragFirst.RemoveAllListeners();
        e_OnDragLast.RemoveAllListeners();
        e_OnPointerUpFirst.RemoveAllListeners();
        e_OnPointerUpLast.RemoveAllListeners();
    }

    void FollowMouse()
    {
        Camera mainCamera = _uicManager.mainCamera;
        if (_uicManager.CanvasRenderMode == RenderMode.ScreenSpaceOverlay)
        {
            transform.position = PointerPosition;
        }
        else if (_uicManager.CanvasRenderMode == RenderMode.ScreenSpaceCamera)
        {
            var screenPoint = PointerPosition;
            screenPoint.z = _uicManager.transform.position.z - mainCamera.transform.position.z; //distance of the plane from the camera
            transform.position = mainCamera.ScreenToWorldPoint(screenPoint);
        }
        // v1.5 - set pointer on the world space canvas
        else if (_uicManager.CanvasRenderMode == RenderMode.WorldSpace)
        {
            var screenPoint = PointerPosition;
            screenPoint.z = _uicManager.transform.position.z - mainCamera.transform.position.z; //distance of the plane from the camera
            transform.position = mainCamera.ScreenToWorldPoint(screenPoint);
        }
    }

    public void OnPointerDown()
    {
        e_OnPointerDownFirst.Invoke();

        _image.sprite = iconHold;

        SelectCloserUIObject();
        UIC_ContextMenu.UpdateContextMenu();
        _initialMousePos = transform.position;

        e_OnPointerDownLast.Invoke();
    }

    public void OnDrag()
    {
        e_OnDragFirst.Invoke();

        if (_uicManager.clickedUIObject is I_UIC_Draggable)
            (_uicManager.clickedUIObject as I_UIC_Draggable).OnDrag();

        if (_uicManager.clickedUIObject is UIC_Entity)
        {
            foreach (I_UIC_Selectable obj in _uicManager.selectedUIObjectsList)
            {
                if (obj is UIC_Entity)
                {
                    (obj as I_UIC_Draggable).OnDrag();
                }
            }
        }

        e_OnDragLast.Invoke();
    }

    public void OnPointerUp()
    {
        e_OnPointerUpFirst.Invoke();

        _image.sprite = iconDefault;

        if (_uicManager.clickedUIObject is I_UIC_Clickable)
            (_uicManager.clickedUIObject as I_UIC_Clickable).OnPointerUp();

        foreach (I_UIC_Object uiObject in _uicManager.selectedUIObjectsList)
        {
            if (uiObject is I_UIC_Clickable)
                (uiObject as I_UIC_Clickable).OnPointerUp();
        }

        e_OnPointerUpLast.Invoke();
    }

    public void UnselectAllUIObjects()
    {
        if (!Input.GetKey(secondaryKey))
        {
            for (int i = _uicManager.selectedUIObjectsList.Count - 1; i >= 0; i--)
            {
                _uicManager.selectedUIObjectsList[i].Unselect();
            }
        }

    }

    [System.Obsolete]
    public List<RaycastResult> RaycastUI()
    {
        // v2.0 - by using more than one canvases, raycast system needs to know search the objects in every graphicRaycaster
        List<RaycastResult> results = new List<RaycastResult>();
        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = PointerPosition;
        List<RaycastResult> resultsLocal = new List<RaycastResult>();

        // you can cache this array for performance
        GraphicRaycaster[] m_RaycasterArray = FindObjectsOfType<GraphicRaycaster>();
        foreach (GraphicRaycaster gr in m_RaycasterArray)
        {
            gr.Raycast(m_PointerEventData, resultsLocal);
            results.AddRange(resultsLocal);
        }

        return results;
    }

    // v2.0 - fix: wrong raycaster found when using more than one canvas
    public List<RaycastResult> RaycastUIAll()
    {
        List<RaycastResult> results = new List<RaycastResult>();
        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = PointerPosition;
        List<RaycastResult> resultsLocal = new List<RaycastResult>();

        // if the GraphicRaycasters are fixed, its possible to cache this array for performance 
        GraphicRaycaster[] m_RaycasterArray = FindObjectsOfType<GraphicRaycaster>();
        foreach (GraphicRaycaster gr in m_RaycasterArray)
        {
            gr.Raycast(m_PointerEventData, resultsLocal);
            results.AddRange(resultsLocal);
        }

        return results;
    }

    private static int SortByPriority(I_UIC_Object o1, I_UIC_Object o2)
    {
        return o2.Priority.CompareTo(o1.Priority);
    }

    public List<I_UIC_Object> ReorderFoundObjectsToPriority(List<I_UIC_Object> objectsList)
    {
        objectsList.Sort(SortByPriority);
        return objectsList;
    }

    public List<I_UIC_Object> OrderedObjectsUnderPointer()
    {
        List<I_UIC_Object> orderedObjects = new List<I_UIC_Object>();

        List<RaycastResult> results = RaycastUIAll();

        I_UIC_Object uiObject = null;

        foreach (RaycastResult result in results)
        {
            uiObject = result.gameObject.GetComponent<I_UIC_Object>();

            if (uiObject != null)
            {
                if (!(uiObject is I_UIC_Clickable) || !(uiObject as I_UIC_Clickable).DisableClick)
                    orderedObjects.Add(uiObject);
            }
        }

        // v1.5 - find closest connection based on the world space
        if (_uicManager.CanvasRenderMode != RenderMode.WorldSpace)
        {
            uiObject = _uicManager.FindClosestConnectionToPosition(PointerPosition, 15);
        }
        else
        {
            uiObject = _uicManager.FindClosestConnectionToPosition(UIC_Utility.WorldToScreenPointInCanvas(transform.position, _uicManager), 15);
        }
        if (uiObject != null)
            if (!(uiObject as I_UIC_Clickable).DisableClick)
                orderedObjects.Add(uiObject);

        orderedObjects.Sort(SortByPriority);

        return orderedObjects;
    }

    public I_UIC_Object FindObjectCloserToPointer()
    {
        _orderedObjectsList = OrderedObjectsUnderPointer();

        if (_orderedObjectsList.Count > 0)
        {
            if (!(_orderedObjectsList[0] is I_UIC_ContextItem))
                UnselectAllUIObjects();

            return _orderedObjectsList[0];
        }
        else
        {
            UnselectAllUIObjects();
            return null;
        }
    }

    public void SelectCloserUIObject()
    {
        _uicManager.clickedUIObject = FindObjectCloserToPointer();
        if (_uicManager.clickedUIObject is I_UIC_Clickable)
        {
            (_uicManager.clickedUIObject as I_UIC_Clickable).OnPointerDown();
        }
    }

    // v2.0 - method to find closest node by distance made obsolete
    [System.Obsolete("Obsolete, use RaycastClosestNodeOfOppositPolarity instead.")]
    public static UIC_Node FindClosestNodeOfOppositPolarity(Vector2 position, float maxDistance, UIC_Node draggedNode, UIC_Manager uicManager)
    {
        float minDist = Mathf.Infinity;
        UIC_Node closestNode = null;
        foreach (UIC_Entity entity in uicManager.EntityList)
        {
            if ((entity == draggedNode.entity && entity.enableSelfConnection) || entity != draggedNode.entity)
            {
                foreach (UIC_Node node in entity.nodeList)
                {
                    if (draggedNode != node && node.haveSpots && draggedNode.haveSpots)
                    {
                        if (node.polarityType != draggedNode.polarityType || node.polarityType == UIC_Node.PolarityTypeEnum._all)
                        {
                            float distance = Vector2.Distance(position, node.transform.position);
                            if (distance < minDist && distance <= maxDistance / uicManager.uiLineRenderer.rectTransform.localScale.x)
                            {
                                closestNode = node;
                                minDist = distance;
                            }
                        }
                    }
                }
            }
        }
        
        return closestNode;
    }

    // v2.0 - new method to find node using raycast from pointer
    public UIC_Node RaycastClosestNodeOfOppositPolarity(UIC_Node draggedNode)
    {
        UIC_Node closestNode = null;

        List<RaycastResult> results = _uicManager.pointer.RaycastUIAll();
        I_UIC_Object uiObject = null;
        foreach (RaycastResult result in results)
        {
            uiObject = result.gameObject.GetComponent<I_UIC_Object>();

            if (uiObject != null)
            {
                if (!(uiObject is I_UIC_Clickable) || !(uiObject as I_UIC_Clickable).DisableClick)
                    if (uiObject is UIC_Node)
                    {
                        UIC_Node node = (uiObject as UIC_Node);
                        if (draggedNode != node && node.haveSpots && draggedNode.haveSpots)
                            if ((node.entity == draggedNode.entity && node.entity.enableSelfConnection) || node.entity != draggedNode.entity)
                                if (node.polarityType != draggedNode.polarityType || node.polarityType == UIC_Node.PolarityTypeEnum._all)
                                {
                                    return node;
                                }
                    }
            }
        }
        return closestNode;
    }

}

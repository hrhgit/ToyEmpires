/// Daniel C Menezes
/// Procedural UI Rounded Corners - https://danielcmcg.github.io/
/// 
/// Based on CiaccoDavide's Unity-UI-Polygon
/// Sourced from - https://github.com/CiaccoDavide/Unity-UI-Polygon

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// v2.1 - changed from Graphic to MaskableGraphic
// v1.2 - UIC_LineRenderer code optimization and refactoring
[ExecuteInEditMode]
public class UIC_LineRenderer : MaskableGraphic
{
    public UIC_Manager uicManager;

    public Sprite s1;

    [SerializeField] Texture m_Texture;

    RectTransform _rectTransform;
    float _rectScaleX;
    float _deg2Rad90 = Mathf.Deg2Rad * 90f;

    Canvas _canvas;

    //[HideInInspector]
    public List<UIC_Line> uIlines;
    // v1.0.1 - added property for UIC_LineRenderer.UILines with null check
    public List<UIC_Line> UILines
    {
        get
        {
            // v2.0 - fix: uILines list reset on enable
            //if (uIlines == null)
            //    uIlines = new List<UIC_Line>();
            return uIlines;
        }
    }

    [HideInInspector]
    public override Texture mainTexture
    {
        get => m_Texture == null ? s_WhiteTexture : m_Texture;
    }
    [HideInInspector]
    public Texture texture
    {
        get => m_Texture;
        set
        {
            if (m_Texture == value) return;
            m_Texture = value;
            SetVerticesDirty();
            SetMaterialDirty();
        }
    }

    protected override void OnEnable()
    {
        // v2.0 - fix: uILines list reset on enable
        //uIlines = uIlines == null ? uIlines : new List<UIC_Line>();
        _rectTransform = GetComponent<RectTransform>();
        _rectScaleX = _rectTransform.localScale.x;

        _canvas = GetComponentInParent<Canvas>();

        Awake();
    }

    void Awake()
    {
        // v2.0 - fix: uILines list reset on enable
        //uIlines = uIlines == null ? uIlines : new List<UIC_Line>();
        // _uicManager = GetComponentInParent<UIC_Manager>();
    }

    void Update()
    {
        // v1.5 - scale the line renderer for world space
        // Debug.Log(_uicManager);
        if (uicManager.CanvasRenderMode != RenderMode.WorldSpace)
        {
            _rectTransform.localScale = Vector3.one / _canvas.scaleFactor;
        }
        else
        {
            _rectTransform.localScale = Vector3.one;

            // v2.1 - fix: changes on the UIC_Manager pivot, cause offset on the LineRenderer, compensated by setting the LineRenderer pivot
            _rectTransform.pivot = new Vector2(uicManager.canvasRectTransform.pivot.x - 0.5f, uicManager.canvasRectTransform.pivot.y - 0.5f);
        }

        SetVerticesDirty();
        SetMaterialDirty();
    }

    // v1.2.1 - added uiline count limit
    [HideInInspector]
    public int vertCount = 0;
    public bool IsVertLimitReached { get => (vertCount >= 64000); }

    protected UIVertex[] SetVbo(Vector2[] vertices, Vector2[] uvs, Color _color)
    {
        UIVertex[] vbo = new UIVertex[4];
        for (int i = 0; i < vertices.Length; i++)
        {
            var vert = UIVertex.simpleVert;
            vert.color = _color;

            vert.position = vertices[i];
            vert.uv0 = uvs[i];
            vbo[i] = vert;
        }

        return vbo;
    }

    Vector2 lastPos2;
    Vector2 lastPos3;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        _vh = _vh ?? vh;

        if (uIlines != null)
        {
            foreach (UIC_Line line in uIlines)
            {
                if (line.points != null && line.points.Count > 0)
                {
                    if (!IsVertLimitReached)
                    {
                        // v1.2 - fix: excessive frame drop on spline drawing
                        DrawLine(line);
                    }
                    else
                    {
                        Debug.Log("<!> Vert limit reached. Some lines may not be drawn");
                    }

                    vertCount = _vh.currentVertCount;
                }
            }
        }

    }

    float animT = 0;
    Vector2 animPositionStart;
    Vector2 animPositionEnd;


    VertexHelper _vh;

    Vector2 uv0 = new Vector2(0, 0);
    Vector2 uv1 = new Vector2(0, 1);
    Vector2 uv2 = new Vector2(1, 1);
    Vector2 uv3 = new Vector2(1, 0);

    Vector2 pos0;
    Vector2 pos1;
    Vector2 pos2;
    Vector2 pos3;

    void DrawLine(UIC_Line line)
    {
        // v2.1 - Corrected lines position when LineRenderer is offset for Canvas Overlay
        Vector2 uicLineRendererOffset = Vector2.zero;
        if (uicManager.CanvasRenderMode == RenderMode.ScreenSpaceOverlay)
        {
            uicLineRendererOffset = transform.position;
        }

        int linePointsCount = line.points.Count;
        for (int i = 1; i < linePointsCount; i++)
        {
            Vector2 p0 = line.points[i - 1];
            Vector2 p1 = line.points[i];

            float angle = Mathf.Atan2(p0.y - p1.y, p0.x - p1.x) + (_deg2Rad90);

            float cos = Mathf.Cos(angle);   // calc cos
            float sin = Mathf.Sin(angle);   // calc sin

            float _halfLineWidth = ((line.width / _rectScaleX) / 2);
            float _wCos = (_halfLineWidth * cos);
            float _wSin = (_halfLineWidth * sin);

            // v2.1 - Corrected lines position when LineRenderer is offset
            pos0 = new Vector2((p0.x) + _wCos, (p0.y) + _wSin) - uicLineRendererOffset;
            pos1 = new Vector2((p0.x) - _wCos, (p0.y) - _wSin) - uicLineRendererOffset;
            pos2 = new Vector2((p1.x) - _wCos, (p1.y) - _wSin) - uicLineRendererOffset;
            pos3 = new Vector2((p1.x) + _wCos, (p1.y) + _wSin) - uicLineRendererOffset;

            _vh.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv0, uv1, uv2, uv3 }, line.color));

            if (i > 1)
            {
                _vh.AddUIVertexQuad(SetVbo(new[] { lastPos3, lastPos2, pos1, pos0 }, new[] { uv0, uv1, uv2, uv3 }, line.color));
            }

            if (i == 1)
                DrawCap(line.points[i - 1] - uicLineRendererOffset, angle + (line.capStartAngleOffset * Mathf.Deg2Rad), line.capStartSize, line.capStartType, line.capStartColor);
            if (i == linePointsCount - 1)
                DrawCap(line.points[i] - uicLineRendererOffset, angle + (line.capEndAngleOffset * Mathf.Deg2Rad), line.capEndSize, line.capEndType, line.capEndColor);

            lastPos2 = pos2;
            lastPos3 = pos3;
        }

        DrawLineAnimation(line, -uicLineRendererOffset);
    }

    public void DrawCap(Vector2 position, float angle, float size, UIC_Line.CapTypeEnum type, Color color)
    {
        if (type != UIC_Line.CapTypeEnum.none)
        {
            float cos = Mathf.Cos(angle);   // calc cos
            float sin = Mathf.Sin(angle);   // calc sin
            size /= _rectScaleX;

            float _sSin = (size * sin);
            float _sCos = (size * cos);

            if (type == UIC_Line.CapTypeEnum.Square)
            {
                Vector2 po0 = new Vector2((position.x - _sSin) + _sCos, (position.y + _sCos) + _sSin);
                Vector2 po1 = new Vector2((position.x - _sSin) - _sCos, (position.y + _sCos) - _sSin);
                Vector2 po2 = new Vector2((position.x + _sSin) - _sCos, (position.y - _sCos) - _sSin);
                Vector2 po3 = new Vector2((position.x + _sSin) + _sCos, (position.y - _sCos) + _sSin);

                _vh.AddUIVertexQuad(SetVbo(new[] { po0, po1, po2, po3 }, new[] { uv0, uv1, uv2, uv3 }, color));
            }
            else if (type == UIC_Line.CapTypeEnum.Triangle)
            {
                Vector2 po0 = new Vector2((position.x - _sSin), (position.y + _sCos));
                Vector2 po1 = new Vector2((position.x - _sSin), (position.y + _sCos));
                Vector2 po2 = new Vector2((position.x + _sSin) - _sCos, (position.y - _sCos) - _sSin);
                Vector2 po3 = new Vector2((position.x + _sSin) + _sCos, (position.y - _sCos) + _sSin);

                _vh.AddUIVertexQuad(SetVbo(new[] { po0, po1, po2, po3 }, new[] { uv0, uv1, uv2, uv3 }, color));
            }
            else if (type == UIC_Line.CapTypeEnum.Circle)
            {
                DrawCircle(position, size, color);
            }
        }
    }

    void DrawCircle(Vector2 position, float radius, Color color)
    {
        Vector2 p0 = position;

        Vector2 prevX = p0;
        float _circleSlice = (360 / 10);
        for (int j = 0; j <= 10; j++)
        {
            float angle = _circleSlice * j;
            angle = Mathf.Deg2Rad * angle;

            float cos = Mathf.Cos(angle);   // calc cos
            float sin = Mathf.Sin(angle);   // calc sin

            pos2 = p0;
            pos3 = p0;
            pos0 = prevX;
            pos1 = new Vector2((p0.x) + (radius * cos), (p0.y) + (radius * sin));

            prevX = pos1;

            // v1.4 - fix: circle cap with uv wrongly positioned
            _vh.AddUIVertexQuad(SetVbo(new[] { pos0, pos1, pos2, pos3 }, new[] { uv3, uv0, uv1, uv2 }, color));
        }
    }

    // v1.4 - method to draw line animations
    public void DrawLineAnimation(UIC_Line line, Vector2 offset)
    {
        UIC_LineAnimation animation = line.animation;
        if (animation.isActive)
        {
            for (int i = 0; i < animation.pointCount; i++)
            {
                System.Tuple<Vector2, float> pointInLine;
                if (animation.Type == UIC_LineAnimation.AnimationTypeEnum.lerp)
                {
                    // triangle wave
                    animation.time = Mathf.Abs(((Time.time * animation.speed + ((float)i / (float)animation.pointCount)) % 1));

                    if (animation.speed < 0)
                        animation.time = 1 - animation.time;

                    pointInLine = line.LerpLine(animation.time);
                }
                else
                {
                    float timePerPoint = 0;
                    animation.time += Mathf.Abs(((Time.deltaTime * animation.speed * 100)) / line.length);
                    if (animation.time > 1)
                        animation.time = 0;

                    timePerPoint = animation.time + ((float)i / (float)animation.pointCount);
                    if (timePerPoint > 1)
                        timePerPoint = timePerPoint - 1;

                    if (animation.speed < 0)
                        timePerPoint = 1 - timePerPoint;

                    pointInLine = line.LerpLine(timePerPoint);
                }
                DrawCap(pointInLine.Item1 + offset, pointInLine.Item2, animation.size, animation.DrawType, animation.color);
            }
        }
    }
}

[System.Serializable]
public class UISpline
{
    public Vector2[] controlPoints = new Vector2[4];
    public Vector2[] points = new Vector2[1];
    public int steps;
    public static int minPoints = 5;
    public static int maxPoints = 15;

    public void SetControlPoints(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        // v1.2 - sets the number of points in the line based on its curvature
        steps = CalculateNumberOfPointsInLine(p0, p1, p2, p3);

        controlPoints = new Vector2[] { p0, p1, p2, p3 };
        points = GetCurve(steps);
    }

    // v1.2 - added method that sets the number of points in the line based on its curvature
    // - adjust the values  minPoints and maxPoints to achieve the desired efficiency or smoothness of the lines
    public int CalculateNumberOfPointsInLine(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        float a0 = Mathf.Abs(Mathf.Sin(Vector2.Angle(p0 - p1, p3 - p1) * Mathf.Deg2Rad));
        float a1 = Mathf.Abs(Mathf.Sin(Vector2.Angle(p3 - p2, p0 - p2) * Mathf.Deg2Rad));
        float r0 = a0 > a1 ? a0 : a1;

        return (int)((r0 * (maxPoints - minPoints)) + minPoints);
    }

    Vector2[] GetCurve(int steps)
    {
        points = new Vector2[steps + 1];
        for (int i = 0; i < steps; i++)
        {
            points[i] = GetPoint(controlPoints[0], controlPoints[1], controlPoints[2], controlPoints[3], (float)i / (float)steps);
        }
        points[steps] = controlPoints[3];

        return points;
    }

    public Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return
            oneMinusT * oneMinusT * oneMinusT * p0 +
            3f * oneMinusT * oneMinusT * t * p1 +
            3f * oneMinusT * t * t * p2 +
            t * t * t * p3;
    }

}
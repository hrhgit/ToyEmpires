using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UIC_Line
{
    public CapTypeEnum capStartType;
    public float capStartSize;
    public Color capStartColor;
    public float capStartAngleOffset;
    public CapTypeEnum capEndType;
    public float capEndSize;
    public Color capEndColor;
    public float capEndAngleOffset;

    public string ID;
    public float width;

    public Color color;
    public List<Vector2> points;

    public Color selectedColor = new Color32(0x7f, 0x5a, 0xf0, 0xff);
    public Color hoverColor = new Color32(0x2c, 0xb6, 0x7d, 0xff);
    public Color defaultColor = new Color32(0xff, 0xff, 0xfe, 0xff);

    public UIC_Line()
    {
        color = defaultColor;
        width = 2;
        points = new List<Vector2>();
    }

    /// <summary>
    /// Clear all points and add new points to the line
    /// </summary>
    /// <param name="newPoints"></param>
    public void SetPoints(Vector2[] newPoints)
    {
        if (newPoints != null)
        {
            points.Clear();
            points.AddRange(newPoints);
        }

        length = GetLength();
    }

    public enum CapTypeEnum { none, Square, Circle, Triangle };
    public enum CapIDEnum { Start, End };
    public void SetCap(CapIDEnum capID, CapTypeEnum capType, float capSize = 5, Color? capColor = null, float angleOffset = 0)
    {
        if (capID == CapIDEnum.Start)
        {
            this.capStartSize = capSize;
            this.capStartType = capType;
            this.capStartColor = capColor ?? Color.white;
            this.capStartAngleOffset = angleOffset;
        }
        else
        {
            this.capEndSize = capSize;
            this.capEndType = capType;
            this.capEndColor = capColor ?? Color.white;
            this.capEndAngleOffset = angleOffset;
        }
    }

    // v1.4 - new methods and variables for enabling lerp line
    [Range(0, 1)]
    public float linePosition01;
    public float length;
    public UIC_LineAnimation animation = new UIC_LineAnimation();
    float _deg2Rad90 = Mathf.Deg2Rad * 90f;
    public float GetLength()
    {
        float l = 0;
        for (int i = 1; i < points.Count; i++)
        {
            l += Vector2.Distance(points[i - 1], points[i]);
        }
        return l;
    }
    System.Tuple<int, float> GetPreviousPointELength(float pos01)
    {
        System.Tuple<int, float> t = new System.Tuple<int, float>(0, 0);
        float l = 0;
        float prevL = 0;
        for (int i = 1; i < points.Count; i++)
        {
            l += Vector2.Distance(points[i - 1], points[i]);
            if (l >= pos01 * length)
            {
                t = new System.Tuple<int, float>(i - 1, prevL);
                break;
            }
            prevL = l;
        }
        return t;
    }

    // v1.4 - LerpLine method that returns point in the line, used to draw animations points
    public System.Tuple<Vector2, float> LerpLine(float pos01)
    {
        System.Tuple<int, float> t = GetPreviousPointELength(pos01);
        Vector2 p0 = points[t.Item1];
        Vector2 p1 = points[t.Item1 + 1];
        float ppDistance = Vector2.Distance(p0, p1);

        Vector2 position = Vector2.Lerp(p0, p1, ((pos01 * length) - t.Item2) / ppDistance);
        float angle = Mathf.Atan2(p0.y - p1.y, p0.x - p1.x) + _deg2Rad90;

        return new System.Tuple<Vector2, float>(position, angle);
    }
}

// v1.4 - added line animation class for uic_lines 
[System.Serializable]
public class UIC_LineAnimation
{
    public enum AnimationTypeEnum {lerp, constantSpeed}
    public AnimationTypeEnum Type = AnimationTypeEnum.lerp;
    public bool isActive = false;
    [Range(1, 10)]
    public int pointCount = 1;
    public float size = 10;
    public Color color = Color.white;
    public UIC_Line.CapTypeEnum DrawType = UIC_Line.CapTypeEnum.Triangle;
    public float speed = 0.5f;
    [HideInInspector]
    public float time = 0;
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIC_Utility
{
    // adapted from http://csharphelper.com/blog/2016/09/find-the-shortest-distance-between-a-point-and-a-line-segment-in-c/
    public static float FindDistanceToSegment(Vector2 pt, Vector2 p1, Vector2 p2)
    {
        Vector2 closest;
        float dx = p2.x - p1.x;
        float dy = p2.y - p1.y;
        if ((dx == 0) && (dy == 0))
        {
            // It's a point not a line segment.
            closest = p1;
            dx = pt.x - p1.x;
            dy = pt.y - p1.y;
            return Mathf.Sqrt(dx * dx + dy * dy);
        }

        // Calculate the t that minimizes the distance.
        float t = ((pt.x - p1.x) * dx + (pt.y - p1.y) * dy) /
            (dx * dx + dy * dy);

        // See if this represents one of the segment's
        // end points or a point in the middle.
        if (t < 0)
        {
            closest = new Vector2(p1.x, p1.y);
            dx = pt.x - p1.x;
            dy = pt.y - p1.y;
        }
        else if (t > 1)
        {
            closest = new Vector2(p2.x, p2.y);
            dx = pt.x - p2.x;
            dy = pt.y - p2.y;
        }
        else
        {
            closest = new Vector2(p1.x + t * dx, p1.y + t * dy);
            dx = pt.x - closest.x;
            dy = pt.y - closest.y;
        }

        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    // v1.2 - method DistanceToSpline made obsolete due to missing detection in some cases, changed to DistanceToConnection
    [Obsolete("Method obsolete, use DistanceToConnection instead")]
    public static float DistanceToSpline(UISpline spline, Vector3 point, int steps)
    {
        float minDist = Mathf.Infinity;

        for (int i = 0; i <= steps; i++)
        {
            Vector3 curvePoint = spline.GetPoint(spline.controlPoints[0], spline.controlPoints[1], spline.controlPoints[2],
                        spline.controlPoints[3], (float)i / (float)steps);

            float distance = Vector3.Distance(point, curvePoint);
            if (distance < minDist)
                minDist = distance;
        }

        return minDist;
    }

    // v1.2 - new precise method used to find distance from pointer to connection
    public static float DistanceToConnectino(UIC_Connection conn, Vector3 point, float maxDistance)
    {

        List<Vector2> connPoints = conn.line.points;
        int connectionPointsCount = connPoints.Count;
        float minDist = Mathf.Infinity;

        for (int i = 1; i < connectionPointsCount; i++)
        {
            float distance = UIC_Utility.FindDistanceToSegment(point, connPoints[i - 1], connPoints[i]);
            if (distance < minDist && distance <= maxDistance)
            {
                minDist = distance;
            }
        }

        return minDist;
    }

    // adapted from https://www.geeksforgeeks.org/check-if-two-given-line-segments-intersect/
    //---
    // Given three colinear points p, q, r, the function checks if 
    // point q lies on line segment 'pr' 
    static bool PointIsOnSegment(Vector2 p, Vector2 q, Vector2 r)
    {
        if (q.x <= Mathf.Max(p.x, r.x) && q.x >= Mathf.Min(p.x, r.x) &&
            q.y <= Mathf.Max(p.y, r.y) && q.y >= Mathf.Min(p.y, r.y))
            return true;

        return false;
    }

    // To find orientation of ordered triplet (p, q, r). 
    // The function returns following values 
    // 0 --> p, q and r are colinear 
    // 1 --> Clockwise 
    // 2 --> Counterclockwise 
    static int LineOrientation(Vector2 p, Vector2 q, Vector2 r)
    {
        // See https://www.geeksforgeeks.org/orientation-3-ordered-points/ 
        // for details of below formula. 
        float val = (q.y - p.y) * (r.x - q.x) -
                  (q.x - p.x) * (r.y - q.y);

        if (val == 0) return 0;  // colinear 

        return (val > 0) ? 1 : 2; // clock or counterclock wise 
    }

    // The main function that returns true if line segment 'p1q1' 
    // and 'p2q2' intersect. 
    public static bool LinesDoIntersect(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2)
    {
        // Find the four orientations needed for general and 
        // special cases 
        int o1 = LineOrientation(p1, q1, p2);
        int o2 = LineOrientation(p1, q1, q2);
        int o3 = LineOrientation(p2, q2, p1);
        int o4 = LineOrientation(p2, q2, q1);

        // General case 
        if (o1 != o2 && o3 != o4)
            return true;

        // Special Cases 
        // p1, q1 and p2 are colinear and p2 lies on segment p1q1 
        if (o1 == 0 && PointIsOnSegment(p1, p2, q1)) return true;

        // p1, q1 and q2 are colinear and q2 lies on segment p1q1 
        if (o2 == 0 && PointIsOnSegment(p1, q2, q1)) return true;

        // p2, q2 and p1 are colinear and p1 lies on segment p2q2 
        if (o3 == 0 && PointIsOnSegment(p2, p1, q2)) return true;

        // p2, q2 and q1 are colinear and q1 lies on segment p2q2 
        if (o4 == 0 && PointIsOnSegment(p2, q1, q2)) return true;

        return false; // Doesn't fall in any of the above cases 
    }
    //--- 

    public static bool ConnectionsIntersect(UIC_Connection conn1, UIC_Connection conn2)
    {
        bool intersect = false;

        if (conn1 != conn2)
        {
            List<Vector2> conn1Points = conn1.line.points;
            List<Vector2> conn2Points = conn2.line.points;
            int conn1PointsCount = conn1Points.Count;
            int conn2PointsCount = conn2Points.Count;
            for (int i = 1; i < conn1PointsCount; i++)
            {
                for (int j = 1; j < conn2PointsCount; j++)
                {
                    Vector2 p1 = conn1Points[i - 1];
                    Vector2 q1 = conn1Points[i];
                    Vector2 p2 = conn2Points[j - 1];
                    Vector2 q2 = conn2Points[j];
                    intersect = UIC_Utility.LinesDoIntersect(p1, q1, p2, q2);
                    if (intersect)
                        break;
                }
                if (intersect)
                    break;
            }
        }
        return intersect;
    }

    public static bool ConnectionIntersectRect(UIC_Connection conn1, RectTransform rt)
    {
        int intersectCount = 0;

        List<Vector2> conn1Points = conn1.line.points;
        int conn1PointsCount = conn1Points.Count;
        for (int i = 1; i < conn1PointsCount; i++)
        {
            Vector2 p1 = conn1Points[i - 1];
            Vector2 q1 = conn1Points[i];

            Vector3[] v = new Vector3[4];
            rt.GetWorldCorners(v);

            intersectCount = 0;

            for (int j = 1; j <= 4; j++)
            {
                bool intersect = false;

                int k = j;
                Vector2 p2 = v[k - 1];
                if (j == 4)
                {
                    k = 0;
                }
                Vector2 q2 = v[k];

                intersect = UIC_Utility.LinesDoIntersect(p1, q1, p2, q2);

                if (intersect)
                    intersectCount++;

                if (intersectCount >= 2)
                    break;
            }

            if (intersectCount >= 2)
                break;

        }
        return intersectCount >= 2 ? true : false;
    }

    // v2.0 - added local manager to parameters
    // v1.5 - transformation to world space
    public static Vector3 WorldToScreenPointInCanvas(Vector3 point, UIC_Manager uicManager)
    {
        Camera mainCamera = uicManager.mainCamera;
        RectTransform canvasRect = uicManager.canvasRectTransform;

        // v2.1 - Corrected lines position when LineRenderer is offset for Canvas World Space
        Vector3 uicManagerOffset = mainCamera.WorldToScreenPoint(uicManager.transform.position);
        Vector3 uicLineRendererOffset = mainCamera.WorldToScreenPoint(uicManager.uiLineRenderer.transform.position);

        Vector3 screenPos = mainCamera.WorldToScreenPoint(point) - uicLineRendererOffset + uicManagerOffset;
        Vector2 screenPos2D = new Vector2(screenPos.x, screenPos.y);
        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos2D, mainCamera, out anchoredPos);
        return anchoredPos;// + new Vector2(canvasRect.sizeDelta.x / 2, canvasRect.sizeDelta.y / 2);
    }

    // v2.1 - Method added to corrected lines position when LineRenderer is offset for Canvas Screen Space Camera
    public static Vector3 WorldToScreenPoint(Vector3 point, UIC_Manager uicManager)
    {
        Camera mainCamera = uicManager.mainCamera;

        // v2.1 - Corrected lines position when LineRenderer is offset for Canvas Screen Space Camera
        Vector3 uicLineRendererOffset = mainCamera.WorldToScreenPoint(uicManager.uiLineRenderer.transform.position);

        return mainCamera.WorldToScreenPoint(point) - uicLineRendererOffset;
    }
}

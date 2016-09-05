using UnityEngine;
using HoloToolkit.Unity;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// mananger all lines in the scene
/// </summary>
public class ARMakeLineManager : Singleton<ARMakeLineManager>
{
    // save all lines in scene
    private Stack<ARMakeLine> Lines = new Stack<ARMakeLine>();

    public GameObject LinePrefab;
    public GameObject TextPrefab;
    public GameObject PointPrefab;

    private ARMakePoint lastPoint;
    private const float defaultLineScale = 0.005f;
    private const float metersToInches = 39.3701f;

    // place point and lines
    public void AddPoint()
    {
        ARMakeManager.Instance.mode = ARMakeMode.AddLine;

        var point = (GameObject)Instantiate(PointPrefab, GazeManager.Instance.HitInfo.point, Quaternion.identity);
        point.name = "POINT_"+Path.GetRandomFileName();
        bool endOnPoint = false;

        if (GazeManager.Instance.HitInfo.collider != null)
        {
            ARMakePoint oldPoint = GazeManager.Instance.HitInfo.collider.transform.parent.GetComponent<ARMakePoint>();
            if (oldPoint != null)
            {
                if(oldPoint == lastPoint)
                {
                    Close();
                    Destroy(point);
                    return;
                }

                point.transform.position = oldPoint.transform.position;
                foreach (ARMakeLine oldLine in oldPoint.lineList)
                {
                    point.GetComponent<ARMakePoint>().AddLine(oldLine, oldPoint);
                }
                Destroy(oldPoint.gameObject);
                endOnPoint = true;

                if (lastPoint == null && oldPoint.ifLocked())
                    point.GetComponent<ARMakePoint>().Lock(true);
            }
        }

        if (lastPoint != null)
        {
            //Debug.Log(lastPoint.position + " " + point.transform.position);
            var centerPos = (lastPoint.position + point.transform.position) * 0.5f;

            var directionFromCamera = centerPos - Camera.main.transform.position;

            var distanceA = Vector3.Distance(lastPoint.position, Camera.main.transform.position);
            var distanceB = Vector3.Distance(point.transform.position, Camera.main.transform.position);

            //Debug.Log("A: " + distanceA + ",B: " + distanceB);
            Vector3 direction;
            if (distanceB > distanceA || (distanceA > distanceB && distanceA - distanceB < 0.1))
            {
                direction = point.transform.position - lastPoint.position;
            }
            else
            {
                direction = lastPoint.position - point.transform.position;
            }

            var distance = Vector3.Distance(lastPoint.position, point.transform.position);
            var line = (GameObject)Instantiate(LinePrefab, centerPos, Quaternion.LookRotation(direction));
            line.transform.localScale = new Vector3(distance, defaultLineScale, defaultLineScale);
            line.transform.Rotate(Vector3.down, 90f);

            string id = Path.GetRandomFileName();
            line.name = "LINE_" + id;

            var normalV = Vector3.Cross(direction, directionFromCamera);
            var normalF = Vector3.Cross(direction, normalV) * -1;

            var dimension = (GameObject)Instantiate(TextPrefab, centerPos, Quaternion.LookRotation(normalF));
            dimension.transform.Translate(Vector3.up * 0.05f);
            dimension.name = "DIMENSION_" + id;

            lastPoint.transform.parent = ARMakeManager.Instance.ARMakeObjects.transform;
            line.transform.parent = ARMakeManager.Instance.ARMakeObjects.transform;
            point.transform.parent = ARMakeManager.Instance.ARMakeObjects.transform;
            dimension.transform.parent = ARMakeManager.Instance.ARMakeObjects.transform;

            ARMakeLine tempLine = line.GetComponent<ARMakeLine>();
            Debug.Log("ADDING POINT " + tempLine.gameObject.name + " Adding " + lastPoint + " " + point.GetComponent<ARMakePoint>());
            tempLine.pointList.Add(lastPoint);
            tempLine.pointList.Add(point.GetComponent<ARMakePoint>());
            tempLine.Distance = distance;
            tempLine.SetDimension(dimension.GetComponent<ARMakeDimension>());
            dimension.GetComponent<ARMakeDimension>().SetLine(line);

            Debug.Log(" " + Vector3.Angle(direction, Vector3.up));
            Vector3 _dimensionPos = Vector3.Angle(direction, Vector3.up) > 45f && Vector3.Angle(direction, Vector3.down) > 45f ? Vector3.up : Vector3.right;
            dimension.GetComponent<ARMakeDimension>().MoveAndRedraw(distance, line.transform.position, _dimensionPos);

            Lines.Push(tempLine);

            lastPoint.AddLine(tempLine);
            lastPoint = point.GetComponent<ARMakePoint>();
            lastPoint.position = point.transform.position;
            lastPoint.IsStart = false;
            lastPoint.AddLine(tempLine);

            if (endOnPoint)
                Close();
            else
            {
                ARMakeLineGuide.Instance.UpdateSource(lastPoint);
            }
        }
        else
        {
            lastPoint = point.GetComponent<ARMakePoint>();
            lastPoint.position = point.transform.position;
            lastPoint.IsStart = true;

            ARMakeLineGuide.Instance.UpdateSource(lastPoint);
        }
    }

    // delete latest placed lines
    public void Close()
    {
        ARMakeManager.Instance.mode = ARMakeMode.Free;
        ARMakeLineGuide.Instance.EndGuide();
        lastPoint = null;
    }

    // delete latest placed lines
    public void Delete()
    {
        if (Lines != null && Lines.Count > 0)
        {
            var lastLine = Lines.Pop();
            Destroy(lastLine.transform.parent);
        }

    }

    // delete all lines in the scene
    public void Clear()
    {
        if (Lines != null && Lines.Count > 0)
        {
            while (Lines.Count > 0)
            {
                var lastLine = Lines.Pop();
                Destroy(lastLine.transform.parent);
            }
        }
    }

    // reset current unfinished line
    public void Reset()
    {
        if (lastPoint != null && lastPoint.IsStart)
        {
            Destroy(lastPoint.transform.parent);
            lastPoint = null;
        }
    }
}
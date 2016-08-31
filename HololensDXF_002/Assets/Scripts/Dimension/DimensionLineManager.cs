using UnityEngine;
using HoloToolkit.Unity;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// mananger all lines in the scene
/// </summary>
public class DimensionLineManager : Singleton<DimensionLineManager>
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
        DimensionManager.Instance.mode = ARMakeMode.AddLine;

        var point = (GameObject)Instantiate(PointPrefab, GazeManager.Instance.HitInfo.point, Quaternion.identity);
        point.name = Path.GetRandomFileName();
        bool endOnPoint = false;

        if (GazeManager.Instance.HitInfo.collider != null)
        {
            ARMakePoint oldPoint = GazeManager.Instance.HitInfo.collider.gameObject.GetComponent<ARMakePoint>();
            if (oldPoint != null)
            {
                point.transform.position = oldPoint.transform.position;
                foreach (ARMakeLine oldLine in oldPoint.lineList)
                {
                    point.GetComponent<ARMakePoint>().AddLine(oldLine, oldPoint);
                }
                DestroyImmediate(oldPoint.gameObject);
                endOnPoint = true;
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

            var normalV = Vector3.Cross(direction, directionFromCamera);
            var normalF = Vector3.Cross(direction, normalV) * -1;
            var tip = (GameObject)Instantiate(TextPrefab, centerPos, Quaternion.LookRotation(normalF));

            //unit is meter
            tip.transform.Translate(Vector3.up * 0.05f);
            tip.GetComponent<TextMesh>().text = (distance* metersToInches) + "in";

            var root = new GameObject();
            lastPoint.transform.parent = root.transform;
            line.transform.parent = root.transform;
            point.transform.parent = root.transform;
            tip.transform.parent = root.transform;

            ARMakeLine tempLine = line.GetComponent<ARMakeLine>();
            tempLine.pointList.Add(lastPoint);
            tempLine.pointList.Add(point.GetComponent<ARMakePoint>());
            tempLine.Distance = distance;
            Lines.Push(tempLine);

            lastPoint = point.GetComponent<ARMakePoint>();
            lastPoint.position = point.transform.position;
            lastPoint.IsStart = false;

            if (endOnPoint)
                Close();
            else
                ARMakeLineGuide.Instance.UpdateSource(lastPoint);
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
        DimensionManager.Instance.mode = ARMakeMode.Free;
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
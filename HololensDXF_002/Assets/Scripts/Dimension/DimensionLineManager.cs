using UnityEngine;
using HoloToolkit.Unity;
using System.Collections.Generic;

/// <summary>
/// mananger all lines in the scene
/// </summary>
public class DimensionLineManager : Singleton<DimensionLineManager>, IDimensionGeometry
{
    // save all lines in scene
    private Stack<DimensionLine> Lines = new Stack<DimensionLine>();

    private DimensionPoint lastPoint;

    private const float defaultLineScale = 0.005f;

    // place point and lines
    public void AddPoint(GameObject LinePrefab, GameObject PointPrefab, GameObject TextPrefab)
    {

        var hitPoint = GazeManager.Instance.HitInfo.point;


        var point = (GameObject)Instantiate(PointPrefab, hitPoint, Quaternion.identity);
        if (lastPoint != null && lastPoint.IsStart)
        {
            var centerPos = (lastPoint.Position + hitPoint) * 0.5f;

            var directionFromCamera = centerPos - Camera.main.transform.position;

            var distanceA = Vector3.Distance(lastPoint.Position, Camera.main.transform.position);
            var distanceB = Vector3.Distance(hitPoint, Camera.main.transform.position);

            Debug.Log("A: " + distanceA + ",B: " + distanceB);
            Vector3 direction;
            if (distanceB > distanceA || (distanceA > distanceB && distanceA - distanceB < 0.1))
            {
                direction = hitPoint - lastPoint.Position;
            }
            else
            {
                direction = lastPoint.Position - hitPoint;
            }

            var distance = Vector3.Distance(lastPoint.Position, hitPoint);
            var line = (GameObject)Instantiate(LinePrefab, centerPos, Quaternion.LookRotation(direction));
            line.transform.localScale = new Vector3(distance, defaultLineScale, defaultLineScale);
            line.transform.Rotate(Vector3.down, 90f);

            var normalV = Vector3.Cross(direction, directionFromCamera);
            var normalF = Vector3.Cross(direction, normalV) * -1;
            var tip = (GameObject)Instantiate(TextPrefab, centerPos, Quaternion.LookRotation(normalF));

            //unit is meter
            tip.transform.Translate(Vector3.up * 0.05f);
            tip.GetComponent<TextMesh>().text = distance + "m";

            var root = new GameObject();
            lastPoint.Root.transform.parent = root.transform;
            line.transform.parent = root.transform;
            point.transform.parent = root.transform;
            tip.transform.parent = root.transform;

            Lines.Push(new DimensionLine
            {
                Start = lastPoint.Position,
                End = hitPoint,
                Root = root,
                Distance = distance
            });

            lastPoint = new DimensionPoint
            {
                Position = hitPoint,
                Root = point,
                IsStart = false
            };

        }
        else
        {
            lastPoint = new DimensionPoint
            {
                Position = hitPoint,
                Root = point,
                IsStart = true
            };
        }
    }


    // delete latest placed lines
    public void Delete()
    {
        if (Lines != null && Lines.Count > 0)
        {
            var lastLine = Lines.Pop();
            Destroy(lastLine.Root);
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
                Destroy(lastLine.Root);
            }
        }
    }

    // reset current unfinished line
    public void Reset()
    {
        if (lastPoint != null && lastPoint.IsStart)
        {
            Destroy(lastPoint.Root);
            lastPoint = null;
        }
    }
}


public struct DimensionLine
{
    public Vector3 Start { get; set; }

    public Vector3 End { get; set; }

    public GameObject Root { get; set; }

    public float Distance { get; set; }
}
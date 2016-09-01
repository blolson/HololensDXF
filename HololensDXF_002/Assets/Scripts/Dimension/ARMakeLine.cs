using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class ARMakeLine : MonoBehaviour {

    public List<ARMakePoint> pointList = new List<ARMakePoint>();
    public GameObject LinePrefab;
    public float Distance;

    private GameObject Line;
    private const float defaultLineScale = 0.005f;

    public ARMakeLine(Vector3 Point1, Vector3 Point2, float dist)
    {
        Debug.Log("ADDING POINT - New Line: " + gameObject.name + " " + Point1 + " " + Point2);
        pointList.Add(new ARMakePoint(Point1));
        pointList.Add(new ARMakePoint(Point2));
        Distance = dist;
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    // Update is called once per frame
    public void RemovePoint(ARMakePoint _point)
    {
        Debug.Log("RemovePoint " + _point.name);
        if (pointList.Contains(_point))
        {
            pointList.Remove(_point);
        }

        if (pointList.Count <= 0)
        {
            Debug.Log("All points have been removed and this line has conceivably already been marked for destruction.");
            return;
        }
        else if (pointList.Count < 2)
        {
            Debug.Log(gameObject.name + " Point List is too low, now destroying this line " + pointList.Count);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    public ARMakeLine AddPoint(ARMakePoint _point, ARMakePoint _deletePoint = null)
    {
        if (pointList.Contains(_point))
        {
            Debug.Log(gameObject.name + " This point was already added. " + _point);
            return this;
        }
        else if (pointList.Count > 1)
        {
            Debug.Log("There's already two points in this line's point list");
            if (_deletePoint == null)
            {
                Debug.LogError("Didn't provide a _deletePoint in a list that already has 2 points, bailing out");
                return null;
            }
            Debug.Log(gameObject.name + " " + _point.name + " " + _deletePoint.name);
            return GenerateNew(_point, _deletePoint);
        }
        else
        {
            Debug.Log("Adding new _point: " + _point + " " + _deletePoint);
            return this;
        }
    }

    // Update is called once per frame
    private ARMakeLine GenerateNew(ARMakePoint _point, ARMakePoint _deletePoint)
    {
        PrintPoints();
        if (pointList.Count < 2)
        {
            Debug.LogError("Not enough points to draw this line.");
            return null;
        }

        int savePoint = 0;
        if (!pointList.Contains(_deletePoint))
        {
            Debug.LogError("You're mistaken! This line doesn't contain the point you're trying to replace!");
            return null;
        }
        else
        {
            foreach (ARMakePoint point in pointList)
            {
                if (point != _deletePoint && point != null)
                {
                    savePoint = pointList.IndexOf(point);
                    //Debug.Log(savePoint);
                    //Debug.Log(pointList[savePoint]);
                    break;
                }
            }
        }

        //Debug.Log(pointList[savePoint]);

        //Debug.Log(_point.name + " " + _deletePoint.name + " " + savePoint);
        var centerPos = (pointList[savePoint].transform.position + _point.transform.position) * 0.5f;

        var directionFromCamera = centerPos - Camera.main.transform.position;

        var distanceA = Vector3.Distance(pointList[savePoint].transform.position, Camera.main.transform.position);
        var distanceB = Vector3.Distance(_point.transform.position, Camera.main.transform.position);

        //Debug.Log("A: " + distanceA + ",B: " + distanceB);
        Vector3 direction;
        if (distanceB > distanceA || (distanceA > distanceB && distanceA - distanceB < 0.1))
        {
            direction = _point.transform.position - pointList[savePoint].transform.position;
        }
        else
        {
            direction = pointList[savePoint].transform.position - _point.transform.position;
        }

        var distance = Vector3.Distance(pointList[savePoint].transform.position, _point.transform.position);
        var line = (GameObject)Instantiate(LinePrefab, centerPos, Quaternion.LookRotation(direction));
        line.name = "LINE_"+Path.GetRandomFileName();

        Debug.Log(gameObject.name + " just generated Line: " + line.name);
        var lineScript = line.GetComponent<ARMakeLine>();

        line.transform.localScale = new Vector3(distance, defaultLineScale, defaultLineScale);
        line.transform.Rotate(Vector3.down, 90f);
        lineScript.pointList.Clear();

        Debug.Log("ADDING POINT " + line.name + " Adding " + pointList[savePoint] + " " + _point);
        lineScript.pointList.Add(pointList[savePoint]);
        lineScript.pointList.Add(_point);

        pointList[savePoint].AddLine(lineScript);
        
        //Blade: The calling function is _point and it will add its line from this return value.
        //_point.AddLine(lineScript);

        return line.GetComponent<ARMakeLine>();
    }

    public void MoveAndRedraw()
    {
        //DpointListebug.Log(lastPoint.position + " " + point.transform.position);
        //PrintPoints();
        //Debug.Log("Move & Redraw: " + gameObject.name);
        var centerPos = (pointList[0].transform.position + pointList[1].transform.position) * 0.5f;

        var directionFromCamera = centerPos - Camera.main.transform.position;

        var distanceA = Vector3.Distance(pointList[0].transform.position, Camera.main.transform.position);
        var distanceB = Vector3.Distance(pointList[1].transform.position, Camera.main.transform.position);

        //Debug.Log("A: " + distanceA + ",B: " + distanceB);
        Vector3 direction;
        if (distanceB > distanceA || (distanceA > distanceB && distanceA - distanceB < 0.1))
        {
            direction = pointList[1].transform.position - pointList[0].transform.position;
        }
        else
        {
            direction = pointList[0].transform.position - pointList[1].transform.position;
        }

        var distance = Vector3.Distance(pointList[0].transform.position, pointList[1].transform.position);
        gameObject.transform.rotation = Quaternion.LookRotation(direction);
        gameObject.transform.position = centerPos;

        gameObject.transform.localScale = new Vector3(distance, defaultLineScale, defaultLineScale);
        gameObject.transform.Rotate(Vector3.down, 90f);
    }

    // Update is called once per frame
    public void PrintPoints()
    {
        Debug.Log(gameObject.name + " pointList:");
        foreach (ARMakePoint p in pointList)
        {
            Debug.Log("\t" + p.name + " " + p.transform.position);
        }
    }

    // Update is called once per frame
    void OnDestroy()
    {
        //Debug.Log("OnDestroy " + gameObject.name);
        foreach (ARMakePoint _point in pointList)
        {
            _point.RemoveLine(this);
        }
    }
}

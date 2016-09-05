using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class ARMakeLine : MonoBehaviour {

    public List<ARMakePoint> pointList = new List<ARMakePoint>();
    public GameObject LinePrefab;
    public GameObject DimensionPrefab;
    public float Distance;

    private GameObject Line;
    private const float defaultLineScale = 0.005f;
    private const float metersToInches = 39.3701f;
    public ARMakeDimension dimension;

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
#if ARMAKEDEBUG
        Debug.Log("RemovePoint " + _point.name);
#endif
        if (pointList.Contains(_point))
        {
            pointList.Remove(_point);
        }

        if (pointList.Count <= 0)
        {
#if ARMAKEDEBUG
            Debug.Log("All points have been removed and this line has conceivably already been marked for destruction.");
#endif
            return;
        }
        else if (pointList.Count < 2)
        {
#if ARMAKEDEBUG
            Debug.Log(gameObject.name + " Point List is too low, now destroying this line " + pointList.Count);
#endif
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    public ARMakeLine AddPoint(ARMakePoint _point, ARMakePoint _deletePoint = null)
    {
        if (pointList.Contains(_point))
        {
#if ARMAKEDEBUG
            Debug.Log(gameObject.name + " This point was already added. " + _point);
#endif
            return this;
        }
        else if (pointList.Count > 1)
        {
#if ARMAKEDEBUG
            Debug.Log("There's already two points in this line's point list");
#endif
            if (_deletePoint == null)
            {
                Debug.LogError("Didn't provide a _deletePoint in a list that already has 2 points, bailing out");
                return null;
            }
#if ARMAKEDEBUG
            Debug.Log(gameObject.name + " " + _point.name + " " + _deletePoint.name);
#endif
            return GenerateNew(_point, _deletePoint);
        }
        else
        {
#if ARMAKEDEBUG
            Debug.Log("Adding new _point: " + _point + " " + _deletePoint);
#endif
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
        string id = Path.GetRandomFileName();
        line.name = "LINE_"+ id;
        line.transform.parent = ARMakeManager.Instance.ARMakeObjects.transform;
#if ARMAKEDEBUG
        Debug.Log(gameObject.name + " just generated Line: " + line.name);
#endif
        var lineScript = line.GetComponent<ARMakeLine>();

        line.transform.localScale = new Vector3(distance, defaultLineScale, defaultLineScale);
        line.transform.Rotate(Vector3.down, 90f);

        var normalV = Vector3.Cross(direction, directionFromCamera);
        var normalF = Vector3.Cross(direction, normalV) * -1;

        lineScript.pointList.Clear();
#if ARMAKEDEBUG
        Debug.Log("ADDING POINT " + line.name + " Adding " + pointList[savePoint] + " " + _point);
#endif
        lineScript.pointList.Add(pointList[savePoint]);
        lineScript.pointList.Add(_point);
        lineScript.Distance = distance;

        var _dimension = (GameObject)Instantiate(DimensionPrefab, centerPos, Quaternion.LookRotation(normalF));
        _dimension.transform.Translate(Vector3.up * 0.05f);
        _dimension.GetComponent<TextMesh>().text = (Mathf.Round(distance * metersToInches * 100f) / 100f) + "<size=" + (_dimension.GetComponent<TextMesh>().fontSize * 5f / 6f) + ">in</size>";
        _dimension.GetComponent<ARMakeDimension>().SetLine(line);
        _dimension.name = "DIMENSION_" + id;
        _dimension.transform.parent = ARMakeManager.Instance.ARMakeObjects.transform;

        lineScript.SetDimension(_dimension.GetComponent<ARMakeDimension>());
        Debug.Log("NEW DIMENSION: " + lineScript.dimension.name + " " + dimension.name);

        pointList[savePoint].AddLine(lineScript);

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
        Distance = distance;

        //Debug.Log(" " + Vector3.Angle(direction, Vector3.up));
        Vector3 _dimensionPos = Vector3.Angle(direction, Vector3.up) > 45f && Vector3.Angle(direction, Vector3.down) > 45f ? Vector3.up : Vector3.right;
        dimension.MoveAndRedraw(distance, centerPos, _dimensionPos);
    }

    public void UpdateDistance(float _distance)
    {
        Debug.Log("UpdateDistance: " + gameObject.name);
        Vector3 direction;
        Vector3 centerPos;
        _distance /= metersToInches;
        Debug.Log(_distance * metersToInches);
        Debug.Log( Distance * metersToInches);

        if (pointList[0].ifLocked())
        {
            direction = pointList[1].transform.position - pointList[0].transform.position;
            pointList[1].transform.position += (direction.normalized * (_distance - Distance));

        }
        else if (pointList[1].ifLocked())
        {
            direction = pointList[0].transform.position - pointList[1].transform.position;
            pointList[0].transform.position += (direction.normalized * (_distance - Distance));
        }
        else
        {
            direction = pointList[0].transform.position - pointList[1].transform.position;
            pointList[0].transform.position += ((direction.normalized * (_distance - Distance)) *0.5f);

            direction = pointList[1].transform.position - pointList[0].transform.position;
            pointList[1].transform.position += ((direction.normalized * (_distance - Distance)) *0.5f);
        }
        centerPos = (pointList[0].transform.position + pointList[1].transform.position) * 0.5f;
        gameObject.transform.position = centerPos;

        var distance = Vector3.Distance(pointList[0].transform.position, pointList[1].transform.position);
        Debug.Log(distance*metersToInches);
        gameObject.transform.localScale = new Vector3(distance, defaultLineScale, defaultLineScale);
        Distance = distance;

        var distanceA = Vector3.Distance(pointList[0].transform.position, Camera.main.transform.position);
        var distanceB = Vector3.Distance(pointList[1].transform.position, Camera.main.transform.position);

        //Debug.Log("A: " + distanceA + ",B: " + distanceB);
        if (distanceB > distanceA || (distanceA > distanceB && distanceA - distanceB < 0.1))
            direction = pointList[1].transform.position - pointList[0].transform.position;
        else
            direction = pointList[0].transform.position - pointList[1].transform.position;

        Vector3 _dimensionPos = Vector3.Angle(direction, Vector3.up) > 45f && Vector3.Angle(direction, Vector3.down) > 45f ? Vector3.up : Vector3.right;
        dimension.MoveAndRedraw(distance, centerPos, _dimensionPos);


        if (pointList[0].ifLocked())
        {
            pointList[1].RedrawLines();
        }
        else if (pointList[1].ifLocked())
        {
            pointList[0].RedrawLines();
        }
        else
        {
            pointList[0].RedrawLines();
            pointList[1].RedrawLines();
        }
    }

    public void SetDimension(ARMakeDimension _dimension)
    {
        dimension = _dimension;
    }

    // Update is called once per frame
    public void PrintPoints()
    {
#if ARMAKEDEBUG
        Debug.Log(gameObject.name + " pointList:");
#endif
        foreach (ARMakePoint p in pointList)
        {
#if ARMAKEDEBUG
            Debug.Log("\t" + p.name + " " + p.transform.position);
#endif
        }
    }

    // Update is called once per frame
    void OnDestroy()
    {
        Debug.Log("OnDestroy " + gameObject.name);
        foreach (ARMakePoint _point in pointList)
        {
            _point.RemoveLine(this);
        }

        if (dimension != null && dimension.gameObject != null)
        {
            Debug.Log("OnDestroy " + dimension.name);
            Destroy(dimension.gameObject);
        }
    }
}

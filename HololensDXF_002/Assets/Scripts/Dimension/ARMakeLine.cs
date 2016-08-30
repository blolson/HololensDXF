using UnityEngine;
using System.Collections.Generic;

public class ARMakeLine : MonoBehaviour {

    public List<ARMakePoint> points = new List<ARMakePoint>();
    public GameObject Root;
    public float Distance;

    public ARMakeLine(Vector3 Point1, Vector3 Point2, GameObject root, float dist)
    {
        points.Add(new ARMakePoint(Point1));
        points.Add(new ARMakePoint(Point2));
        Root = root;
        Distance = dist;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

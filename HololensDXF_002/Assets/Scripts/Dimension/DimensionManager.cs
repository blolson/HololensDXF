using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;
using System.Collections.Generic;
using System;
using System.IO;

/// <summary>
/// manager all measure tools here
/// </summary>
public class DimensionManager : Singleton<DimensionManager>
{
    private DimensionLineManager manager;
    public ARMakeMode mode;

    // set up prefabs
    public GameObject LinePrefab;
    public GameObject PointPrefab;
    public GameObject ModeTipObject;
    public GameObject TextPrefab;
    public SurfaceMeshesToPlanes planeConverter;
    public float flattenPlaneDist = 0.05f;
    public float combinePointsDist = 0.3f;
    private static readonly float FrameTime = .008f;

    private const float defaultLineScale = 0.005f;
    private const float metersToInches = 39.3701f;

    private List<ARMakePoint> PointList = new List<ARMakePoint>();

    private struct CombinePoint
    {
        public Vector3 point;
        public bool combined;
        public GameObject pointObject;

        public CombinePoint(Vector3 newVert)
        {
            point = newVert;
            combined = false;
            pointObject = null;
        }

        public CombinePoint(Vector3 newVert, GameObject _pointObject, bool _combined)
        {
            point = newVert;
            combined = _combined;
            pointObject = _pointObject;
        }
    }

    private struct CombinableVert
    {
        public Vector3 vert;
        public List<CombinePoint> combineVerts;
        public bool combined;
        public int meshID;

        public CombinableVert(List<CombinePoint> _list)
        {
            vert = new Vector3();
            meshID = -1;
            combineVerts = _list;
            combined = false;
        }

        public CombinableVert(Vector3 newVert, int _meshID)
        {
            vert = newVert;
            meshID = _meshID;
            combineVerts = new List<CombinePoint>();
            combined = false;
        }
    }

    void Start()
    {
        // inti measure mode
        manager = DimensionLineManager.Instance;
    }

    // place spatial point
    public void OnSelect()
    {
        manager.AddPoint();
    }

    // if lastPoint exists, stop it, and cleanup
    public void Close()
    {
        manager.Close();

    }
    // delete latest line or geometry
    public void DeleteLine()
    {
        manager.Delete();
    }

    // delete all lines or geometry
    public void ClearAll()
    {
        manager.Clear();
    }

    public void ConvertPlanesToLines()
    {
        StartCoroutine("_ConvertPlanesToLines");
    }

    IEnumerator _ConvertPlanesToLines()
    {
        List<CombinableVert> allVerts = new List<CombinableVert>();
        float start = Time.realtimeSinceStartup;

        int meshID = -1;
        foreach (GameObject ap in planeConverter.ActivePlanes)
        {
            ++meshID;
            MeshFilter filter = ap.GetComponent<MeshFilter>();
            // Since this is amortized across frames, the filter can be destroyed by the time
            // we get here.
            if (filter == null)
            {
                continue;
            }

            Mesh mesh = filter.sharedMesh;
            if (mesh == null)
            {
                // We don't need to do anything to this mesh, move to the next one.
                continue;
            }

            foreach(Vector3 vert in mesh.vertices)
            {
                //Debug.Log(ap.transform.TransformPoint(vert));
                allVerts.Add(new CombinableVert(ap.transform.TransformPoint(vert), meshID));
            }
        }

        GameObject[] activePlanes = planeConverter.ActivePlanes.ToArray();
        for( int p = 0; p < activePlanes.Length; p++)
        {
            Destroy(activePlanes[p]);
        }

        CombinableVert[] compareVerts = new CombinableVert[allVerts.Count];
        allVerts.CopyTo(compareVerts);
        for (int i = 0; i < compareVerts.Length; i++)
        {
            foreach (CombinableVert vert in allVerts)
            {
                if (i != allVerts.IndexOf(vert) && !compareVerts[i].combined && !vert.combined && compareVerts[i].meshID == vert.meshID)
                {
                    if (Vector3.Distance(compareVerts[i].vert, vert.vert) < flattenPlaneDist)
                    {
                        compareVerts[i].combineVerts.Add(new CombinePoint(compareVerts[allVerts.IndexOf(vert)].vert));
                        compareVerts[allVerts.IndexOf(vert)].combined = true;
                    }
                }
                // If too much time has passed, we need to return control to the main game loop.
                if ((Time.realtimeSinceStartup - start) > FrameTime)
                {
                    // Pause our work here, and continue finding vertices to remove on the next frame.
                    yield return null;
                    start = Time.realtimeSinceStartup;
                }
            }
        }

        int ending = 0;
        meshID = 0;
        int initLength = compareVerts[compareVerts.Length - 1].meshID + 1;
        CombinableVert[] verts = new CombinableVert[initLength];
        verts[meshID] = new CombinableVert(new List<CombinePoint>());
    
        for (int i = 0; i < verts.Length; i++)
            verts[i].meshID = -1;

        verts[meshID].meshID = meshID;

        for (int i = 0; i < compareVerts.Length; i++)
        {
            if(!compareVerts[i].combined)
            {
                //Debug.Log(compareVerts[i].vert + " Combined: ");
                //foreach (Vector3 cv in compareVerts[i].combineVerts)
                //{
                //    Debug.Log("\t" + cv);
                //}
                //manager.AddPoint(LinePrefab, PointPrefab, TextPrefab);
                ending++;

                //Debug.Log(meshID + " " + compareVerts[i].meshID);
                if (compareVerts[i].meshID != meshID)
                {
                    meshID = compareVerts[i].meshID;
                    verts[meshID] = new CombinableVert(new List<CombinePoint>());
                    verts[meshID].meshID = meshID;
                }
                verts[meshID].combineVerts.Add(new CombinePoint(compareVerts[i].vert));
                //Debug.Log(meshID + " " + verts[meshID].combineVerts.Count);
            }
        }
        Debug.Log("Starting Value: " + compareVerts.Length + " and Ending Value: " + ending);

        for (int v = 0; v < verts.Length; v++)
        {
            if (verts[v].meshID < 0)
                continue;

            //Debug.Log(verts[v].meshID);
            for (int o=0; o < verts[v].combineVerts.Count; o++)
            {
                for (int i = 0; i < verts[v].combineVerts.Count; i++)
                {
                    if(i > o && verts[v].combineVerts[o].point != verts[v].combineVerts[i].point)
                    {
                        Vector3 vTotal = verts[v].combineVerts[o].point - verts[v].combineVerts[i].point;
                        if(Mathf.Abs(vTotal.normalized.x) <= 0.01f || Mathf.Abs(vTotal.normalized.y) <= 0.01f)
                        {
                            if(!verts[v].combineVerts[o].combined)
                            {
                                var newPoint = (GameObject)Instantiate(PointPrefab, verts[v].combineVerts[o].point, Quaternion.identity);
                                newPoint.name = Path.GetRandomFileName();
                                verts[v].combineVerts[o] = new CombinePoint(verts[v].combineVerts[o].point, newPoint, true);
                                PointList.Add(newPoint.GetComponent<ARMakePoint>());
                            }

                            if (!verts[v].combineVerts[i].combined)
                            {
                                var newPoint = (GameObject)Instantiate(PointPrefab, verts[v].combineVerts[i].point, Quaternion.identity);
                                newPoint.name = Path.GetRandomFileName();
                                verts[v].combineVerts[i] = new CombinePoint(verts[v].combineVerts[i].point, newPoint, true);
                                PointList.Add(newPoint.GetComponent<ARMakePoint>());
                            }

                            //Debug.Log(lastPoint.position + " " + point.transform.position);
                            var centerPos = (verts[v].combineVerts[o].point + verts[v].combineVerts[i].point) * 0.5f;

                            var directionFromCamera = centerPos - Camera.main.transform.position;

                            var distanceA = Vector3.Distance(verts[v].combineVerts[o].point, Camera.main.transform.position);
                            var distanceB = Vector3.Distance(verts[v].combineVerts[i].point, Camera.main.transform.position);

                            //Debug.Log("A: " + distanceA + ",B: " + distanceB);
                            Vector3 direction;
                            if (distanceB > distanceA || (distanceA > distanceB && distanceA - distanceB < 0.1))
                            {
                                direction = verts[v].combineVerts[i].point - verts[v].combineVerts[o].point;
                            }
                            else
                            {
                                direction = verts[v].combineVerts[o].point - verts[v].combineVerts[i].point;
                            }

                            var distance = Vector3.Distance(verts[v].combineVerts[o].point, verts[v].combineVerts[i].point);
                            var line = (GameObject)Instantiate(LinePrefab, centerPos, Quaternion.LookRotation(direction));
                            line.name = Path.GetRandomFileName();
                            line.transform.localScale = new Vector3(distance, defaultLineScale, defaultLineScale);
                            line.transform.Rotate(Vector3.down, 90f);

                            //TIP
                            var normalV = Vector3.Cross(direction, directionFromCamera);
                            var normalF = Vector3.Cross(direction, normalV) * -1;
                            var tip = (GameObject)Instantiate(TextPrefab, centerPos, Quaternion.LookRotation(normalF));
                            //unit is meter
                            tip.transform.Translate(Vector3.up * 0.05f);
                            tip.GetComponent<TextMesh>().text = (distance * metersToInches) + "in";

                            var root = new GameObject();
                            verts[v].combineVerts[o].pointObject.transform.parent = root.transform;
                            line.transform.parent = root.transform;
                            verts[v].combineVerts[i].pointObject.transform.parent = root.transform;
                            tip.transform.parent = root.transform;

                            ARMakeLine tempLine = line.GetComponent<ARMakeLine>();
                            tempLine.pointList.Add(verts[v].combineVerts[o].pointObject.GetComponent<ARMakePoint>());
                            tempLine.pointList.Add(verts[v].combineVerts[i].pointObject.GetComponent<ARMakePoint>());

                            verts[v].combineVerts[o].pointObject.GetComponent<ARMakePoint>().lineList.Add(tempLine);
                            verts[v].combineVerts[i].pointObject.GetComponent<ARMakePoint>().lineList.Add(tempLine);

                            // If too much time has passed, we need to return control to the main game loop.
                            if ((Time.realtimeSinceStartup - start) > FrameTime)
                            {
                                // Pause our work here, and continue finding vertices to remove on the next frame.
                                yield return null;
                                start = Time.realtimeSinceStartup;
                            }
                        }
                    }
                }
            }
        }

        ARMakePoint[] pointArray = new ARMakePoint[PointList.Count];
        PointList.CopyTo(pointArray);
        for(int i = 0; i < pointArray.Length; i++)
        {
            for (int j = i; j < pointArray.Length; j++)
            {
                if (j != i && pointArray[i] != null && pointArray[j] != null && Vector3.Distance(pointArray[i].transform.position, pointArray[j].transform.position) < combinePointsDist)
                {
                    ARMakePoint removePoint = PointList[PointList.IndexOf(pointArray[i])];
                    foreach (ARMakeLine _line in removePoint.lineList)
                    {
                        //Debug.Log("Starting on this line now: " + _line.name + " " + removePoint.lineList.Count);
                        pointArray[j].AddLine(_line, removePoint);
                    }
                    DestroyImmediate(removePoint.gameObject);
                }
            }
        }

        SceneStateManager.Instance.Progress();
    }
}

public enum ARMakeMode
{
    Free,
    AddLine
}
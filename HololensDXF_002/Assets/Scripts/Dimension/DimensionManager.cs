using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;
using System.Collections.Generic;
using System;

/// <summary>
/// manager all measure tools here
/// </summary>
public class DimensionManager : Singleton<DimensionManager>
{
    private IDimensionGeometry manager;
    public DimensionMode mode;

    // set up prefabs
    public GameObject LinePrefab;
    public GameObject PointPrefab;
    public GameObject ModeTipObject;
    public GameObject TextPrefab;
    public SurfaceMeshesToPlanes planeConverter;
    public float flattenPlaneDist = 0.05f;
    public float combinePointsDist = 0.3f;
    private static readonly float FrameTime = .016f;

    private List<ARMakePoint> PointList = new List<ARMakePoint>();

    private struct CombinableVert
    {
        public Vector3 vert;
        public List<Vector3> combineVerts;
        public bool combined;

        public CombinableVert(Vector3 newVert)
        {
            vert = newVert;
            combineVerts = new List<Vector3>();
            combined = false;
        }
    }

    void Start()
    {
        // inti measure mode
        switch (mode)
        {
            default:
                manager = DimensionLineManager.Instance;
                break;
        }
    }

    // place spatial point
    public void OnSelect()
    {
        manager.AddPoint(LinePrefab, PointPrefab, TextPrefab);
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

    // change measure mode
    public void OnModeChange()
    {
        try  
        {
            manager.Reset();
            if (mode == DimensionMode.Line)
            {
                mode = DimensionMode.Line;
                manager = DimensionLineManager.Instance;
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
        ModeTipObject.SetActive(true);
    }

    public void ConvertPlanesToLines()
    {
        StartCoroutine("_ConvertPlanesToLines");
    }

    IEnumerator _ConvertPlanesToLines()
    {
        List<CombinableVert> verts = new List<CombinableVert>();
        float start = Time.realtimeSinceStartup;

        foreach (GameObject ap in planeConverter.ActivePlanes)
        {
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
                verts.Add(new CombinableVert(ap.transform.TransformPoint(vert)));
            }
        }

        CombinableVert[] compareVerts = new CombinableVert[verts.Count];
        verts.CopyTo(compareVerts);
        for (int i = 0; i < compareVerts.Length; i++)
        {
            foreach (CombinableVert vert in verts)
            {
                if (i != verts.IndexOf(vert) && !compareVerts[i].combined && !vert.combined)
                {
                    if (Vector3.Distance(compareVerts[i].vert, vert.vert) < flattenPlaneDist)
                    {
                        compareVerts[i].combineVerts.Add(compareVerts[verts.IndexOf(vert)].vert);
                        compareVerts[verts.IndexOf(vert)].combined = true;
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
        for (int i = 0; i < compareVerts.Length; i++)
        {
            if(!compareVerts[i].combined)
            {
                //Debug.Log(compareVerts[i].vert + " Combined: ");
                //foreach (Vector3 cv in compareVerts[i].combineVerts)
                //{
                //    Debug.Log("\t" + cv);
                //}
                ending++;

                //manager.AddPoint(LinePrefab, PointPrefab, TextPrefab);

                var newPoint = (GameObject)Instantiate(PointPrefab, compareVerts[i].vert, Quaternion.identity);
                PointList.Add(newPoint.GetComponent<ARMakePoint>());
                // If too much time has passed, we need to return control to the main game loop.
                if ((Time.realtimeSinceStartup - start) > FrameTime)
                {
                    // Pause our work here, and continue finding vertices to remove on the next frame.
                    yield return null;
                    start = Time.realtimeSinceStartup;
                }
            }
        }
        Debug.Log("Starting Value: " + compareVerts.Length + " and Ending Value: " + ending);
    }
}

public enum DimensionMode
{
    Line
}
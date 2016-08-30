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
}

public class DimensionPoint
{
    public Vector3 Position { get; set; }

    public GameObject Root { get; set; }
    public bool IsStart { get; set; }
}

public enum DimensionMode
{
    Line,
    Triangle,
    Rectangle,
    Cube,
    Ploygon
}
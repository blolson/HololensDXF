using UnityEngine;
using System.Collections;

/// <summary>
/// interface for geometry class
/// </summary>
public interface IDimensionGeometry
{
    void AddPoint();

    void Delete();

    void Clear();

    void Reset();
}
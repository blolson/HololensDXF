using UnityEngine;
using System.Collections;
using System.Xml;
using System.Text;
using System.IO;
using netDxf;
using netDxf.Drawing;
using netDxf.Blocks;
using netDxf.Collections;
using netDxf.Entities;
using netDxf.Header;
using netDxf.Objects;
using netDxf.Tables;
using netDxf.Units;
using Attribute = netDxf.Entities.Attribute;
using Image = netDxf.Entities.Image;
using Point = netDxf.Entities.Point;
using Trace = netDxf.Entities.Trace;
using Vector3 = netDxf.Vector3;

public class testDxf : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        LinearDimension();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LinearDimension()
    {
        DxfDocument dxf = new DxfDocument(DxfVersion.AutoCad2010);
        DimensionStyle myStyle = CreateDimStyle();

        Vector3 p1 = new Vector3(0, -5, 0);
        Vector3 p2 = new Vector3(-5, 0, 0);

        Line line1 = new Line(p1, p2)
        {
            Layer = new Layer("Reference line")
            {
                Color = AciColor.Green
            }
        };
        dxf.AddEntity(line1);

        double offset = 4;
        LinearDimension dimX1 = new LinearDimension(line1, offset, 0, Vector3.UnitZ, myStyle);
        LinearDimension dimY1 = new LinearDimension(line1, offset, 90, Vector3.UnitZ, myStyle);
        LinearDimension dim5 = new LinearDimension(line1, offset, -30, Vector3.UnitZ, myStyle);
        LinearDimension dim6 = new LinearDimension(line1, offset, -60, Vector3.UnitZ, myStyle);

        Vector3 p3 = new Vector3(6, -5, 0);
        Vector3 p4 = new Vector3(11, 0, 0);
        Line line2 = new Line(p3, p4)
        {
            Layer = new Layer("Reference line")
            {
                Color = AciColor.Green
            }
        };
        dxf.AddEntity(line2);
        LinearDimension dimX2 = new LinearDimension(line2, offset, -30.0, Vector3.UnitZ, myStyle);
        LinearDimension dimY2 = new LinearDimension(line2, offset, -60.0, Vector3.UnitZ, myStyle);
        LinearDimension dim3 = new LinearDimension(line2, offset, 30.0, Vector3.UnitZ, myStyle);
        LinearDimension dim4 = new LinearDimension(line2, offset, 60.0, Vector3.UnitZ, myStyle);

        dxf.AddEntity(dimX1);
        dxf.AddEntity(dimY1);
        dxf.AddEntity(dimX2);
        dxf.AddEntity(dimY2);
        dxf.AddEntity(dim3);
        dxf.AddEntity(dim4);
        dxf.AddEntity(dim5);
        dxf.AddEntity(dim6);
        Debug.Log("Save Location: " + Application.persistentDataPath.ToString() + "/test.dxf");
        bool success = dxf.Save(Application.persistentDataPath.ToString() + "/test.dxf");

        Debug.Log(success);
        if (success)
        {
            gameObject.AddComponent<Email>().SendEmail();
        }
    }

    private static DimensionStyle CreateDimStyle()
    {
        DimensionStyle myStyle = new DimensionStyle("MyStyle");
        myStyle.DIMCLRD = AciColor.Yellow;
        myStyle.DIMSAH = true;
        myStyle.DIMBLK1 = DimensionArrowhead.Box;
        myStyle.DIMBLK2 = DimensionArrowhead.DotBlank;
        //myStyle.DIMSE1 = true;
        //myStyle.DIMSE2 = true;

        myStyle.DIMCLRT = AciColor.Red;

        return myStyle;
    }
}

using UnityEngine;
using System.Collections;
using System.Xml;
using System.Text;
using System.IO;
using SMTP.Async;
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
        LinearDimension dimX1 = new LinearDimension(line1, offset, 0, Vector3.UnitZ);
        LinearDimension dimY1 = new LinearDimension(line1, offset, 90, Vector3.UnitZ);
        LinearDimension dim5 = new LinearDimension(line1, offset, -30, Vector3.UnitZ);
        LinearDimension dim6 = new LinearDimension(line1, offset, -60, Vector3.UnitZ);

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
        LinearDimension dimX2 = new LinearDimension(line2, offset, -30.0, Vector3.UnitZ);
        LinearDimension dimY2 = new LinearDimension(line2, offset, -60.0, Vector3.UnitZ);
        LinearDimension dim3 = new LinearDimension(line2, offset, 30.0, Vector3.UnitZ);
        LinearDimension dim4 = new LinearDimension(line2, offset, 60.0, Vector3.UnitZ);

        dxf.AddEntity(dimX1);
        dxf.AddEntity(dimY1);
        dxf.AddEntity(dimX2);
        dxf.AddEntity(dimY2);
        dxf.AddEntity(dim3);
        dxf.AddEntity(dim4);
        dxf.AddEntity(dim5);
        dxf.AddEntity(dim6);
        bool success = dxf.Save("BladeUnityDimension.dxf");

        Debug.Log(success);
        if (success)
        {
            Email.SendEmail();
        }
    }

    IEnumerator UploadLevel()
    {
        //making a dummy xml level file
        XmlDocument map = new XmlDocument();
        map.LoadXml("<level></level>");

        //converting the xml to bytes to be ready for upload
        byte[] levelData = Encoding.UTF8.GetBytes(map.OuterXml);

        //generate a long random file name , to avoid duplicates and overwriting
        string fileName = Path.GetRandomFileName();
        fileName = fileName.Substring(0, 6);
        fileName = fileName.ToUpper();
        fileName = fileName + ".xml";

        //if you save the generated name, you can make people be able to retrieve the uploaded file, without the needs of listings
        //just provide the level code name , and it will retrieve it just like a qrcode or something like that, please read below the method used to validate the upload,
        //that same method is used to retrieve the just uploaded file, and validate it
        //this method is similar to the one used by the popular game bike baron
        //this method saves you from the hassle of making complex server side back ends which enlists available levels
        //this way you could enlist outstanding levels just by posting the levels code on a blog or forum, this way its easier to share, without the need of user accounts or install procedures
        WWWForm form = new WWWForm();

        print("form created ");

        form.AddField("action", "level upload");

        form.AddField("file", "file");

        form.AddBinaryData("file", levelData, fileName, "text/xml");

        print("binary data added ");
        //change the url to the url of the php file
        WWW w = new WWW("http://www.hololens.bladeolson.com/DXFupload.php", form);
        print("www created");

        yield return w;
        print("after yield w");
        if (w.error != null)
        {
            print("error");
            print(w.error);
        }
        else
        {
            //this part validates the upload, by waiting 5 seconds then trying to retrieve it from the web
            if (w.uploadProgress == 1 && w.isDone)
            {
                yield return new WaitForSeconds(5);
                //change the url to the url of the folder you want it the levels to be stored, the one you specified in the php file
                WWW w2 = new WWW("http://www.hololens.bladeolson.com/DXFuploads/" + fileName);
                yield return w2;
                if (w2.error != null)
                {
                    print("error 2");
                    print(w2.error);
                }
                else
                {
                    //then if the retrieval was successful, validate its content to ensure the level file integrity is intact
                    if (w2.text != null && w2.text != "")
                    {
                        if (w2.text.Contains("<level>") && w2.text.Contains("</level>"))
                        {
                            //and finally announce that everything went well
                            print("Level File " + fileName + " Contents are: \n\n" + w2.text);
                            print("Finished Uploading Level " + fileName);
                        }
                        else
                        {
                            print("Level File " + fileName + " is Invalid");
                        }
                    }
                    else
                    {
                        print("Level File " + fileName + " is Empty");
                    }
                }
            }
        }
    }
}


using UnityEngine;
using System.IO;
using System.Collections;

public class Email : MonoBehaviour
{
    public void SendEmail()
    {
        StartCoroutine("UploadLevel");
        Debug.Log("Email Started...");
    }

    IEnumerator UploadLevel()
    {
        //converting the xml to bytes to be ready for upload
        byte[] dxfData = UnityEngine.Windows.File.ReadAllBytes(Application.persistentDataPath.ToString() + "/test.dxf");

        //generate a long random file name , to avoid duplicates and overwriting
        string fileName = Path.GetRandomFileName();
        fileName = fileName.ToUpper();
        fileName = fileName + ".dxf";
        Debug.Log(fileName + " " + dxfData.Length);

        //if you save the generated name, you can make people be able to retrieve the uploaded file, without the needs of listings
        //just provide the level code name , and it will retrieve it just like a qrcode or something like that, please read below the method used to validate the upload,
        //that same method is used to retrieve the just uploaded file, and validate it
        //this method is similar to the one used by the popular game bike baron
        //this method saves you from the hassle of making complex server side back ends which enlists available levels
        //this way you could enlist outstanding levels just by posting the levels code on a blog or forum, this way its easier to share, without the need of user accounts or install procedures
        WWWForm form = new WWWForm();

        Debug.Log("form created ");

        form.AddField("action", "dxf upload");
        form.AddField("file", "file");
        form.AddBinaryData("file", dxfData, fileName, "application/dxf");

        Debug.Log("binary data added ");
        //change the url to the url of the php file
        WWW w = new WWW("http://hololens.bladeolson.com/upload.php", form);
        //WWW w = new WWW("http://localhost:46592/upload.php", form);
        Debug.Log("www created");

        yield return w;
        Debug.Log("after yield w: " + w.text);
        if (w.error != null)
        {
            Debug.Log("error");
            Debug.Log(w.error);
        }
        else
        {
            //this part validates the upload, by waiting 5 seconds then trying to retrieve it from the web
            Debug.Log("w.uploadProgress == 1 && w.isDone " + w.uploadProgress + " " + w.isDone);
            if (w.isDone)
            {
                yield return new WaitForSeconds(5);
                //change the url to the url of the folder you want it the dxfs to be stored, the one you specified in the php file
                WWW w2 = new WWW("http://hololens.bladeolson.com/dxf/" + fileName);
                yield return w2;
                if (w2.error != null)
                {
                    Debug.Log("error 2");
                    Debug.Log(w2.error);
                }
                else
                {
                    //then if the retrieval was successful, validate its content to ensure the level file integrity is intact
                    if (w2.text != null && w2.text != "")
                    {
                        if (w2.text.Contains("Dxf"))
                        {
                            //and finally announce that everything went well
                            //Debug.Log("DXF File " + fileName + " Contents are: \n\n" + w2.text);
                            Debug.Log("Finished Uploading DXF " + fileName);

                            form = new WWWForm();

                            Debug.Log("form created ");

                            form.AddField("action", "email");
                            form.AddField("file", "file");
                            form.AddBinaryData("file", dxfData, fileName, "application/dxf");

                            WWW w3 = new WWW("http://hololens.bladeolson.com/upload.php", form);
                            yield return w3;
                            Debug.Log("after yield w: " + w.text);
                            if (w3.error != null)
                            {
                                Debug.Log("error");
                                Debug.Log(w3.error);
                            }
                            else
                            {
                                //this part validates the upload, by waiting 5 seconds then trying to retrieve it from the web
                                Debug.Log("w3.uploadProgress == 1 && w3.isDone " + w3.uploadProgress + " " + w3.isDone);
                                if (w3.isDone)
                                {
                                    if (w3.text.Contains("Message has been sent"))
                                    {
                                        Debug.Log("DXF File " + fileName + " has been emailed");
                                    }
                                    else
                                    {
                                        Debug.Log("Server email complete");
                                    }
                                }

                            }
                        }
                        else
                        {
                            Debug.Log("DXF File " + fileName + " is Invalid");
                        }
                    }
                    else
                    {
                        Debug.Log("DXF File " + fileName + " is Empty");
                    }
                }
            }
        }
    }
}
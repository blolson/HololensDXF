using System;
using System.Net.Mail;
using System.ComponentModel;
using System.Net;
using UnityEngine;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace SMTP.Async
{
    public class Email
    {
        static bool mailSent = false;
        public static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            string token = (string)e.UserState;

            if (e.Cancelled)
            {
                Debug.Log("Send canceled. " + token);
            }
            if (e.Error != null)
            {
                Debug.Log(token + " " + e.Error.ToString());
            }
            else
            {
                Debug.Log("Message sent.");
            }
            mailSent = true;
        }
        public static void SendEmail()
        {
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress("blade@hololens.bladeolson.com", "Hololens DXF File");
            mail.To.Add("bladeolson@gmail.com");
            mail.Subject = "Hololens DXF Download #323";
            mail.Body = "Your AutoCad-ready .dxf file is attached to this email.";

            if (System.IO.File.Exists(Application.persistentDataPath + "/test.dxf"))
            {
                Debug.Log("Found File");
                Attachment attachment = new Attachment(Application.persistentDataPath + "/test.dxf");
                mail.Attachments.Add(attachment);
            }

            SmtpClient smtpServer = new SmtpClient("sub5.mail.dreamhost.com");
            smtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpServer.Port = 587;
            smtpServer.Credentials = new NetworkCredential("blade@hololens.bladeolson.com", "BodyShop") as ICredentialsByHost;
            smtpServer.EnableSsl = true;
            ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                return true;
            };

            // Send the e-mail asynchronously so the main thread wont hang when there's no internet available
            // The userState can be any object that allows your callback 
            // method to identify this send operation.

            smtpServer.SendCompleted += new
SendCompletedEventHandler(SendCompletedCallback);

            string userState = "test message 1";
            //smtpServer.SendAsync(mail, userState);

            // Clean up.
            mail.Dispose();
            Debug.Log("Goodbye.");
        }
    }
}
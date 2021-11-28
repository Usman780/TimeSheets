using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RestSharp.Authenticators;

namespace TimeCard.Helping_Classes
{
    public class MailSender
    {
        public static bool SendDownloadFileLinkEmail(string email, string path, string BaseUrl = "")
        {
            try
            {
                string MailBody = "<html><head></head><body><nav class='navbar navbar-default'><div class='container-fluid'>" +
                                  "</div> </nav><center><div><h1 class='text-center'>Completed Employee Assignments!</h1>" +
                                  "<p class='text-center'>To Download the File click on the button below</p><br>" +
                                  "<button style='background-color: rgb(0,174,239);'>" +
                                  "<a href='" + BaseUrl + "/ExcelReport/DownloadReport?FileAddress=" + path + "'>Click To Download</a>" +
                                  //"<a href='" + BaseUrl + "Home/AssignedEmployeesTimeSheet?email=" + StringCipher.Base64Encode(email) + "&time=" + StringCipher.Base64Encode(DateTime.Now.ToString("MM/dd/yyyy")) + "' style='text-decoration:none;font-size:15px;color:white;'>Reset Password</a>" +
                                  "</button></div></center>" +
                                  "<script src = 'https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js' ></ script ></ body ></ html >";
                MailBody += "<script src = 'https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js' ></ script ></ body ></ html >";

                RestClient client = new RestClient(); //intializing Rest client object 
                client.BaseUrl = new Uri("https://api.mailgun.net/v3"); // this is base url (remains same)
                client.Authenticator = new HttpBasicAuthenticator("api", "2b1ce341848c6fc996e43321365755ec-aff8aa95-f4645c49"); //copy Private Api Key from Api security (https://app.mailgun.com/app/account/security/api_keys)
                RestRequest request = new RestRequest();
                request.AddParameter("domain", "nodlays.co.uk", ParameterType.UrlSegment);  //Create a new domain from side bar "Sending -> Domains -> Add New Domain"
                request.Resource = "{domain}/messages";
                request.AddParameter("from", "nodlaysa@gmail.com"); //Can be used any email here, without any password requirements
                request.AddParameter("to", email); //email where you want to send mail
                request.AddParameter("subject", "Time Card | Employee Assignment"); //subject of mail
                request.AddParameter("html", MailBody); //send html code generated above
                request.Method = Method.POST;
                string response = client.Execute(request).Content.ToString();
                if (response != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
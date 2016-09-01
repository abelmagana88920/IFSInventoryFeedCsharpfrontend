using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Net;
using System.Net.Mail;
using System.Text;
 
using Excel = Microsoft.Office.Interop.Excel;
using Newtonsoft.Json.Linq;

 

namespace InventoryFeed.Controllers
{

     
    public class CustomerController : Controller
    {
        IFSReportingContext db = new IFSReportingContext();
        //
        // GET: /Customer/


        private static void GetPathParams(out string localPath, out string destinationPath, out string localPathwofile)
        {
            string path = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            //once you have the path you get the directory with:
            var directory = System.IO.Path.GetDirectoryName(path);
            localPath = new Uri(directory).LocalPath;
            destinationPath = localPath + "\\..\\App_Data\\created.csv";

            localPathwofile = localPath + "\\..\\App_Data";
        }

        public static void killProcessByName(string processName)
        {
            System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName(processName);
            foreach (System.Diagnostics.Process p in process)
            {
                if (!string.IsNullOrEmpty(p.ProcessName))
                {
                    try
                    {
                        p.Kill();
                    }
                    catch { }
                }
            }
        }

        public string convertCSVtoXLS(string destinationPath, string localPathwofile)
        {
            System.IO.File.Copy(@"" + destinationPath, @"" + destinationPath + ".xls");

            Random random = new Random();
            int randomNumber = random.Next(0, 10000);


            var _app = new Excel.Application();
            var _workbooks = _app.Workbooks;


            _workbooks.OpenText(@"" + destinationPath + ".xls",
                                     DataType: Excel.XlTextParsingType.xlDelimited,
                                     TextQualifier: Excel.XlTextQualifier.xlTextQualifierDoubleQuote,
                                     ConsecutiveDelimiter: true,
                                     Comma: true);
            // Convert To Excle 97 / 2003
            string createdNewFile = localPathwofile + "\\NewTest" + randomNumber + ".xls";
            _workbooks[1].SaveAs(createdNewFile, Excel.XlFileFormat.xlExcel5);

            _workbooks.Close();
            _app.Quit();

            System.Runtime.InteropServices.Marshal.ReleaseComObject(_app);

            return createdNewFile;
        }

        public ActionResult Index()
        {
            return View();
        }

        // GET: /Customer/Name/{customer_no}
        
        public JsonResult Name(string customer_name) {
           
            //return (Json(customer_no));
            return Json(db.tblInvoiceLinesMasters.Select(m => new { m.CUSTOMER_NO, m.CUSTOMER_NAME }).Where(m => m.CUSTOMER_NAME == customer_name).Distinct(), JsonRequestBehavior.AllowGet);
        }

        // GET: /Customer/Number/{customer_name}

        public JsonResult Number(string customer_no) {
            return Json(db.tblInvoiceLinesMasters.Select(m => new { m.CUSTOMER_NO, m.CUSTOMER_NAME }).Where(m => m.CUSTOMER_NO== customer_no).Distinct(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Emails(string customer_no, string filePath)
        {
            /*string localPath, destinationPath, localPathwofile;
            GetPathParams(out localPath, out destinationPath, out localPathwofile); */

            try
            {

                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress("abelmagana88920@gmail.com");
                mail.To.Add("abelmagana04426@gmail.com");
                mail.Subject = "Test Mail - 1";
                mail.Body = "mail with attachment";

                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment(filePath);
                mail.Attachments.Add(attachment);

                SmtpServer.Port = 587;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential("abelmagana88920@gmail.com", "magana04426");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);


                return Json(new { message = "Send Successfully" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
           
            }
        }


        public JsonResult FTPs(string customer_no, string filePath)
        {
           /*(  string localPath, destinationPath, localPathwofile;
            GetPathParams(out localPath, out destinationPath, out localPathwofile); */
            string fileName="";
            Uri uri = new Uri(filePath);
            if (uri.IsFile)
            {
                fileName = System.IO.Path.GetFileName(uri.LocalPath);
            }


            try
            {

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://abelmagana88920.orgfree.com/upload/" + fileName);
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // This example assumes the FTP site uses anonymous logon.
                request.Credentials = new NetworkCredential("abelmagana88920.orgfree.com", "magana88920");

                // Copy the contents of the file to the request stream.
               // StreamReader sourceStream = new StreamReader("C:\\Users\\Abel\\Desktop\\test.xls");
                //byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                byte[] fileContents = System.IO.File.ReadAllBytes(filePath);
                
               // sourceStream.Close();
                request.ContentLength = fileContents.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                //Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);

                response.Close();


                return Json(new { message = "Upload File Complete, status {0} " + response.StatusDescription }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);

            }
        }


        public JsonResult DataCSVXLS(string customer_no)
        {

            string localPath, destinationPath, localPathwofile;
            GetPathParams(out localPath, out destinationPath, out localPathwofile);

             
                var emList = db.tblInvoiceLinesMasters.Select(m => new { m.PART_NO, m.INVOICED_QTY, m.CUSTOMER_NO }).Where(m => m.CUSTOMER_NO == customer_no).ToList();
                if (emList.Count > 0)
                {
                    string header = @"""Part no"",""Invoice qty""";
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(header);

                    foreach (var i in emList)
                    {
                        sb.AppendLine(string.Join(",",
                            string.Format(@"""{0}""", i.PART_NO),
                            string.Format(@"""{0}""", i.INVOICED_QTY)
                            )
                        );
                    }

                    //creating csv file
                    using (System.IO.StreamWriter file = new
                    System.IO.StreamWriter(destinationPath))
                    {
                        file.WriteLine(sb.ToString());
                    }

                    // Convert CSV To xls

                    string createdNewFile =convertCSVtoXLS(destinationPath, localPathwofile); //static function

                    killProcessByName("Excel"); //static function

                    string fileName = destinationPath + ".xls";
                  
                    if (fileName != null || fileName != string.Empty)
                    {
                        if ((System.IO.File.Exists(fileName)))
                        {
                            System.IO.File.Delete(fileName);
                        }
                    }

                    return Json(new { message = "Create File Complete, status {0} " + Response.StatusDescription, filePath = createdNewFile }, JsonRequestBehavior.AllowGet);
                     
                }
                else
                {
                    return Json(new { message = "Error " + Response.StatusDescription }, JsonRequestBehavior.AllowGet);
                }  
        }


        [AllowAnonymous]
        [HttpPost]
        public JsonResult SendRequest(string customer_no)
        {
            try
            {
                var req = new {
                                 type = Request["type"],
                                 sendid = Request["sendid"],
                                 sendvia = Request["sendvia"],
                                 email = Request["email"],
                                 ftp =Request["ftp"],
                                 buyer = Request["buyer"],
                                 header = Request["header"],
                                time = Request["time"],
                                field = Request["field"]

                                // send = Request["sendid"],
                          };
                return Json(new { message = req});
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });

            }
        }


       /* public ActionResult RevaleeTest(string customer_no)
        {

            string revaleeServiceHost = "localhost:46200";
            try
            {
                //var TimeS=TimeSpan.FromSeconds(5.0);
                var TimeS = DateTimeOffset.Now.AddMinutes(0.25);
                Uri expirationMessageCallbackUri = new Uri(
                    string.Format("http://localhost:65369/Customer/SendRequest/NIC105", customer_no));

                Guid expirationMessageCallbackId = RevaleeRegistrar.ScheduleCallback(
                    revaleeServiceHost, TimeS, expirationMessageCallbackUri);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
                //return Json(new { message = TimeS }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK);
               // return Json(new {message = e.Message}, JsonRequestBehavior.AllowGet);
            }
        } */


    }
}

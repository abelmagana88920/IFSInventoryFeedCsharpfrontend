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

using InventoryFeed.Models.DB;
using System.Data;
using System.Data.Objects;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
//using InventoryFeed.Models.ViewModel;

 

namespace InventoryFeed.Controllers
{

    public class CustomerController : Controller
    {
        IFSReportingContext db = new IFSReportingContext();
        //
        // GET: /Customer/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Feed()
        {
            var feed = from m in db.tblInventoryFeeds
                          select m;

            return View(feed); 
        }

        public ActionResult Create()
        {

            return View();
        }

        public ActionResult Update(int id)
        {
           
                int if_id = id;
                var edit_query = db.tblInventoryFeeds.Where(m => m.if_id == if_id).FirstOrDefault();

                /*var edit_query = from m in db.tblInventoryFeeds
                                 where m.if_id == if_id
                                 select m; */
                return View(edit_query);
          
           
        }


        public ActionResult Process()
        {
          
            var process = from m in db.tblInventoryFeedProcesses
                         select m;  
 
             return View(process); 
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

        [AllowAnonymous]
        [HttpPost]
        public JsonResult SendRequest(string customer_no)
        {
            try
            {
                var protocol_addr="";


                if (Request["sendvia"] == "email") protocol_addr = Request["email"];
                else if (Request["sendvia"] == "ftp") protocol_addr = Request["ftp"];
                tblInventoryFeed req = new tblInventoryFeed {
                                customer_no = customer_no,
                                 filetype_requested = Request["type"],
                                 sendaaid_instead_brand_id = Request["sendid"],
                                 send_protocol = Request["sendvia"],
                                 protocol_address = protocol_addr.Replace(",",";"),
                                 
                                 sendbuyers_partno = Request["buyer"],
                                includeheaders = Request["header"],
                                sendtime = Request["time"].Replace(",",";"),
                                fields = Request["field"].Replace(",",";"),
                                sendday = "Mon,Tue,Wed,Thu,Fri"

                                // send = Request["sendid"],
                          };

                // Add the new object to the Orders collection.
                db.tblInventoryFeeds.Add(req);
                db.SaveChanges();

                int id = req.if_id;  //the last id inserted

                string[] separators = { ",", ";", " " };
                string sendtime = req.sendtime;
                string[] sendtime_array = sendtime.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                
                foreach (string individual_sendtime in sendtime_array)
                {
                    
                    tblInventoryFeedProcess subreq = new tblInventoryFeedProcess
                    {
                         if_id = id,
                         status="0",
                         time_split = TimeSpan.Parse(individual_sendtime, System.Globalization.CultureInfo.CurrentCulture),
                    };
                    db.tblInventoryFeedProcesses.Add(subreq);
                    db.SaveChanges();
                } 



                return Json(new { message = req});
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });

            }
        }

        [HttpPost]
        public JsonResult FeedDelete(int if_id)
        {
            IFSReportingContext db_local = new IFSReportingContext();

            tblInventoryFeed inv = new tblInventoryFeed() //selecting for update
            {
                if_id = if_id
            };

            db_local.tblInventoryFeeds.Attach(inv);  
            db_local.tblInventoryFeeds.Remove(inv);
            db_local.SaveChanges();


            Library.Execute("DELETE tblInventoryFeedProcess WHERE if_id = " + if_id);
   
            return Json(new { message = "deleted" });
        }


        [HttpPost]
        public JsonResult FeedEdit(int if_id)
        {
 
            IFSReportingContext db_local = new IFSReportingContext();
            tblInventoryFeed inv = new tblInventoryFeed() //selecting for update
            {
                if_id = if_id,
            };

            db_local.tblInventoryFeeds.Attach(inv);


            var protocol_addr = "";

            if (Request["sendvia"] == "email") protocol_addr = Request["email"];
            else if (Request["sendvia"] == "ftp") protocol_addr = Request["ftp"];


            inv.customer_no = Request["customer_no"];
             inv.filetype_requested = Request["type"];
             inv.sendaaid_instead_brand_id = Request["sendid"];
             inv.send_protocol = Request["sendvia"];
             inv.protocol_address = protocol_addr.Replace(",", ";");

              inv.sendbuyers_partno= Request["buyer"];
              inv.includeheaders = Request["header"];
             inv.sendtime = Request["time"].Replace(",", ";");
             inv.fields = Request["field"].Replace(",", ";");
             inv.sendday = "Mon,Tue,Wed,Thu,Fri";
            
            db_local.SaveChanges();

            Library.Execute("DELETE tblInventoryFeedProcess WHERE if_id = " + if_id);


            string[] separators = { ",", ";", " " };
            string sendtime = Request["time"].Replace(",", ";");
            string[] sendtime_array = sendtime.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            foreach (string individual_sendtime in sendtime_array)
            {

                tblInventoryFeedProcess subreq = new tblInventoryFeedProcess
                {
                    if_id = if_id,
                    status = "0",
                    time_split = TimeSpan.Parse(individual_sendtime, System.Globalization.CultureInfo.CurrentCulture),
                };
                db.tblInventoryFeedProcesses.Add(subreq);
                db.SaveChanges();
            } 


            return Json(new { message = "edit" });
        }


    }
}

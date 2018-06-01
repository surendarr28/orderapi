using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using orderapis.Core;

namespace orderapis.Controllers
{
    public class CustomerDetailsController : ApiController
    {

        [HttpPost]
        [ActionName("add")]
        public HttpResponseMessage insertMenuItem(CustomerDetails objItem)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, DbAccess.DbAInsert("tbl_customer_details", objItem));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [ActionName("order")]
        public HttpResponseMessage orderlist(Order objItem)
        {
            var customers = DbAccess.DbASelect("select o.vcOrderId, "
                                                + " o.vcInvoiceId,"
                                                + " o.id,"
                                                + " o.isQuatation,"
                                                + " o.iPack,"
                                                + " o.iPackPerPrice,"
                                                + " o.iOthers,"
                                                + " o.iGST,"
                                                + " o.vcFuncName,"
                                                + " o.vcFuncAddress,"
                                                + " o.vcFuncLocation,"
                                                + " o.vcFunctionDate,"
                                                + " o.vcNote,"
                                                + " o.isTax,"
                                                + " o.iConfirm,"
                                                + " o.iCustomerId, "
                                                + " o.iPackageId, "
                                                + " o.isCancel, "
                                                + " pack.vcPackagename, "
                                                + " cd.vcName as customername,"
                                                + " cd.iPhonenumber as customerphone,"
                                                + " cd.vcEmailid as customeremail"
                                                + "  from (select * from tbl_order where isActive = true order by id DESC) as o"
                                                + " left join tbl_package pack on pack.id = o.iPackageId"
                                                + " left join tbl_customer_details cd on cd.id = o.iCustomerId"
                                                + " where cd.vcEmailid LIKE '%" + objItem.searchkey + "%' or cd.vcName LIKE '%" + objItem.searchkey + "%' or cd.iPhonenumber LIKE '%" + objItem.searchkey + "%' OR o.vcOrderId LIKE '%" + objItem.searchkey + "%' OR o.vcInvoiceId LIKE '%" + objItem.searchkey + "%'");
            return Request.CreateResponse(HttpStatusCode.OK, customers);
        }

        [HttpPost]
        [ActionName("getorder")]
        public HttpResponseMessage getorder(Order objItem)
        {
            OrderDetail data = new OrderDetail();
            data.order = DbAccess.DbASelect("select o.vcOrderId, "
                                                + " o.vcInvoiceId,"
                                                + " o.id,"
                                                + " o.isQuatation,"
                                                + " o.iPack,"
                                                + " o.iPackPerPrice,"
                                                + " o.iOthers,"
                                                + " o.isCancel, "
                                                + " o.iGST,"
                                                + " o.vcFuncName,"
                                                + " o.vcFuncAddress,"
                                                + " o.vcFuncLocation,"
                                                + " o.vcFunctionDate,"
                                                + " o.vcNote,"
                                                + " o.isTax,"
                                                + " o.iConfirm,"
                                                + " o.iCustomerId, "
                                                + " o.iPackageId, "
                                                + " pack.vcPackagename, "
                                                + " cd.vcName as customername,"
                                                + " cd.iPhonenumber as customerphone,"
                                                + " cd.vcEmailid as customeremail"

                                                + "  from (select * from tbl_order where id = " + objItem.id + ") as o"
                                                + " left join tbl_package pack on pack.id = o.iPackageId"
                                                + " left join tbl_customer_details cd on cd.id = o.iCustomerId ");

            data.logisticitems = DbAccess.DbASelect("SELECT olm.*, li.dPrice as unchangeItemPrice FROM tbl_order_logistic_mapping as olm left join tbl_logistic_items li on li.id = olm.iLogisticId  where olm.iOrderId  = '" + objItem.id + "'");
            data.menuitems = DbAccess.DbASelect("SELECT olm.*, mi.dPrice as unchangeItemPrice  FROM tbl_order_item_mapping  as olm left join tbl_menu_item mi on mi.id = olm.iItemId where olm.iOrderId  = '" + objItem.id + "'");
            data.deposit = DbAccess.DbASelect("SELECT * FROM tbl_deposits where iOrderId  = '" + objItem.id + "'");

            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpPost]
        [ActionName("checkcustomer")]
        public HttpResponseMessage checkcustomer(CustomerDetails objItem)
        {
            var customers = DbAccess.DbASelect("select * from tbl_customer_details where isActive = true and ( iPhonenumber ='" + objItem.iPhonenumber + "' or vcEmailid = '" + objItem.vcEmailid + "')");
            return Request.CreateResponse(HttpStatusCode.OK, customers);
        }

        [HttpPost]
        [ActionName("payment")]
        public HttpResponseMessage payment(Recipt objItem)
        {
            var updateOrder = DbAccess.DbAQueryInsert("update tbl_order set iConfirm='1' where id = '" + objItem.order.id + "'");
            var payment = DbAccess.DbAQueryInsert("insert into tbl_deposits (recieptno, iOrderId, dAmt, vcType) values ('R-" + RandomNumber() +
             "', '" + objItem.deposit.iOrderId +
             "', '" + objItem.deposit.dAmt +
             "', '" + objItem.deposit.vcType + "')");
            var orders = DbAccess.DbASelect("select o.vcOrderId, "
                                                + " o.vcInvoiceId,"
                                                + " o.id,"
                                                + " o.createdAt,"
                                                + " o.isQuatation,"
                                                + " o.iPack,"
                                                + " o.iPackPerPrice,"
                                                + " o.iOthers,"
                                                + " o.iGST,"
                                                + " o.isCancel, "
                                                + " o.vcFuncName,"
                                                + " o.vcFuncAddress,"
                                                + " o.vcFuncLocation,"
                                                + " o.vcFunctionDate,"
                                                + " o.vcNote,"
                                                + " o.isTax,"
                                                + " o.iConfirm,"
                                                + " o.iCustomerId, "
                                                + " o.iPackageId, "
                                                + " pack.vcPackagename, "
                                                + " cd.vcName as customername,"
                                                + " cd.iPhonenumber as customerphone,"
                                                + " cd.vcEmailid as customeremail"

                                                + "  from (select * from tbl_order where id = " + objItem.order.id + ") as o"
                                                + " left join tbl_package pack on pack.id = o.iPackageId"
                                                + " left join tbl_customer_details cd on cd.id = o.iCustomerId ");
            var order = orders[0];
            var menuitems = DbAccess.DbASelect("SELECT * FROM tbl_order_item_mapping where iOrderId  = '" + objItem.order.id + "'");
            var logisticitems = DbAccess.DbASelect("SELECT * FROM tbl_order_logistic_mapping where iOrderId  = '" + objItem.order.id + "'");
            var deposit = DbAccess.DbASelect("SELECT * FROM tbl_deposits where iOrderId  = '" + objItem.order.id + "'");

            string menuItemString = string.Empty;
            string logisticItemString = string.Empty;
            string rowString = "<br/>";
            int row = 1;
            if (Convert.ToInt32(order["iPackageId"]) != 0)
            {
                row = 2;
            }

            decimal otherPrice = 0;
            decimal depositAmtPrice = 0;

            foreach (var item in menuitems)
            {
                menuItemString += "<br/> <span style='font-weight:normal; '>" + item["itemName"] + "</span>";
                rowString += "<br/>";
            }

            foreach (var item in deposit)
            {
                depositAmtPrice += Convert.ToDecimal(item["dAmt"]);
            }

            foreach (var item in logisticitems)
            {
                if (Convert.ToInt32(item["iQuantity"]) != 0)
                {
                    logisticItemString += "    <div style=\"clear:both;\"></div>" +
                    "    <div style=\"display:block;\">" +
                    "        <div style=\"border:1px solid #000; float:left; width:40px; text-align:center; font-weight:bold; \">" +
                    "            " + row++ + "" +
                    "        </div>" +
                    "        <div style=\"border:1px solid #000; float:left; width:240px; text-align:left; font-weight:bold; \">" +
                    "            <div style=\"padding-left:10px\">" + item["itemName"] + "" +
                    "            </div>" +
                    "        </div>" +
                    "        <div style=\"border:1px solid #000; float:left; width:180px; text-align:center; font-weight:bold; \">" +
                    "            " + item["iQuantity"] + "" +
                    "        </div>" +
                    "        <div style=\"border:1px solid #000; float:left; width:200px; text-align:center; font-weight:bold; \">" +
                    "            $" + (Convert.ToDecimal(item["dPrice"]) / Convert.ToInt32(item["iQuantity"])).ToString("F") + "" +
                    "        </div>" +
                    "        <div style=\"border:1px solid #000; float:left; width:200px; text-align:center; font-weight:bold; \">" +
                    "            $" + item["dPrice"] + "" +
                    "        </div>" +
                    "    </div>";

                    otherPrice += Convert.ToDecimal(item["dPrice"]);
                }
            }

            string grandTotal = ((Convert.ToDecimal(order["iPack"]) * Convert.ToDecimal(order["iPackPerPrice"])) + otherPrice).ToString("F"); ;

            Rectangle pageSize = new Rectangle(PageSize.A4);
            string menuItemTableString = string.Empty;
            if (Convert.ToInt32(order["iPackageId"]) != 0)
            {
                menuItemTableString = "<div style=\"display:block;\">" +
                    "        <div style=\"border:1px solid #000; float:left; width:40px; text-align:center; font-weight:bold; \">" +
                    "            1" + rowString +
                    "        </div>" +
                    "        <div style=\"border:1px solid #000; float:left; width:240px; text-align:left; font-weight:bold; \">" +
                    "            <div style=\"padding-left:10px\">" + order["vcPackagename"] + menuItemString +
                    "            </div>" +
                    "        </div>" +
                    "        <div style=\"border:1px solid #000; float:left; width:180px; text-align:center; font-weight:bold; \">" +
                    "            " + order["iPack"] + rowString +
                    "        </div>" +
                    "        <div style=\"border:1px solid #000; float:left; width:200px; text-align:center; font-weight:bold; \">" +
                    "            $" + order["iPackPerPrice"] + rowString +
                    "        </div>" +
                    "        <div style=\"border:1px solid #000; float:left; width:200px; text-align:center; font-weight:bold; \">" +
                    "            $" + (Convert.ToDecimal(order["iPack"]) * Convert.ToDecimal(order["iPackPerPrice"])) + rowString +
                    "        </div>" +
                    "    </div>";
            }

            string sb = "<!DOCTYPE html>" +
                "<html>" +
                "<head>" +
                "    <meta charset=\"UTF-8\">" +
                "    <title>Excel To HTML using codebeautify.org</title>" +
                "</head>" +
                "<body style='font-family: sans-serif; font-size:14px;'>" +
                "    <div>" +
                "        <div style='float:left;'></div>" +
                "        <div style='float:right; text-align:right;'>" +
                "            <h1 style='color:#333; font-size:24px; font-family: sans-serif; margin:0px; padding:px '>RECEIPT</h1>" +
                "            <h5 style='color:#666; font-family: sans-serif; margin:0px;'>GST Reg No. M90363451J" +
                "						</h5>" +
                "            <h5 style='color:#666; margin-top:20px; margin-bottom:0px; font-family: sans-serif;'>ORDER NO :" +
                "						<span style=\"color:#f01;\"> " + objItem.order.vcOrderId + "</span>" +
                "						</h5>" +
                "            <h5 style='color:#666; margin:5px 0px; font-family: sans-serif;'>INVOICE NO :" +
                "						<span style=\"color:#f01;\">  " + (objItem.order.vcInvoiceId != null ? objItem.order.vcInvoiceId : "--") + "</span>" +
                "						</h5>" +
                "        </div>" +
                "    </div>" +
                "    <div style=\"clear:both\"></div>" +
                "    <div>" +
                "        <label style='color:#666;'>Catering Hotline: 67627284</label>" +
                "        <label style='color:#666; float:right'>Order Date: <span style=\"color:#000; font-weight:bold\">" + Convert.ToDateTime(order["createdAt"]).ToString("MM/dd/yyyy hh:mm tt") + "</span>" +
                "        </label>" +
                "    </div>" +
                "    <div style=\"margin-top:10px; display:inline-block; width:48%; text-align:left;\">" +
                "        <label style='color:#000; display:block; font-weight:bold;'>" + order["customername"] + "</label>" +
                "        <label style='color:#666; display:block; margin-top:5px;'>Tel: <span style=\"color:#000; font-weight:bold;\">" + order["customerphone"] + "</span>" +
                "        </label>" +
                "    </div>" +
                "    <div style=\"margin-top:10px; display:inline-block; width:50%; text-align:left;\">" +
                "        <label style='color:#666; display:block; margin-top:5px;'>Function: <span style=\"color:#000; font-weight:bold;\">" + Convert.ToDateTime(objItem.order.vcFunctionDate).ToString("MM/dd/yyyy hh:mm tt") + "</span>" +
                "        </label>" +
                "        <label style='color:#666; display:block; margin-top:5px;'>Location: <span style=\"color:#000; font-weight:bold;\">" + objItem.order.vcFuncLocation + "</span>" +
                "        </label>" +
                "    </div>" +
                "    <div style=\"display:inline-block; width:50%; text-align:left;\">" +
                "        <label style='color:#666; display:block; margin-top:5px;'>Email: <span style=\"color:#000; font-weight:bold;\">" + order["customeremail"] + "</span>" +
                "        </label>" +
                "    </div>" +
                "    <div style=\"margin-top:10px; display:block;\">" +
                "        <div style=\"border:1px solid #000; float:left; width:40px; background:#ccc; text-align:center; font-weight:bold; \">" +
                "            No" +
                "        </div>" +
                "        <div style=\"border:1px solid #000; float:left; width:240px; background:#ccc; text-align:center; font-weight:bold; \">" +
                "            DESCRIPTION" +
                "        </div>" +
                "        <div style=\"border:1px solid #000; float:left; width:180px; background:#ccc; text-align:center; font-weight:bold; \">" +
                "            NO OF ITEMS" +
                "        </div>" +
                "        <div style=\"border:1px solid #000; float:left; width:200px; background:#ccc; text-align:center; font-weight:bold; \">" +
                "            UNIT PRICE" +
                "        </div>" +
                "        <div style=\"border:1px solid #000; float:left; width:200px; background:#ccc; text-align:center; font-weight:bold; \">" +
                "            TOTAL" +
                "        </div>" +
                "    </div>" +
                "    <div style=\"clear:both;\"></div>" +
"" + menuItemTableString + "" +
               "" + logisticItemString + "" +
                "    <div style=\"clear:both;\"></div>" +
                "    <div style=\"display:block;\">" +
                "        <div style=\"border:1px solid #000; float:left; width:666px; text-align:center; font-weight:bold; \">" +
                "            <div style='float:left;'>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; text-align:left; padding-left:2px\">" +
                "                        Notes:" +
                "                    </div>" +
                "                    <div style=\"display:block; font-weight:normal; font-size:13px;\">" + order["vcNote"] + "</div>" +
                "                </div>" +
                "                <div style=\"float:left; text-align:left; margin-top:12px; padding-left:2px\">" +
                "                    Deposit:" +
                "                    <div style=\"display:block; font-weight:normal; font-size:13px;\">" + DateTime.Now.ToString("MM/dd/yyyy") + ": $" + objItem.deposit.dAmt + "  Paid by " + objItem.deposit.vcType + "</div>" +
                "                </div>" +
                "            </div>" +
                "            <div style='float:right;'>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:10px; font-weight:bold; \">" +
                "                        Sub Total" +
                "                    </div>" +
                "                </div>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:10px; font-weight:bold; \">" +
                "                        7% GST" +
                "                    </div>" +
                "                </div>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:10px; font-weight:bold; \">" +
                "                        Grand Total" +
                "                    </div>" +
                "                </div>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:10px; font-weight:bold; \">" +
                "                        Deposit" +
                "                    </div>" +
                "                </div>" +
                "               <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:10px; font-weight:bold; \">" +
                "                        Credit" +
                "                    </div>" +
                "                </div>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:10px; font-weight:bold; \">" +
                "                        Balance Remining" +
                "                    </div>" +
                "                </div>" +
                "            </div>" +
                "        </div>" +
                "        <div style=\"border:1px solid #000; float:left; width:200px; text-align:center; font-weight:bold; \">" +
                "            <div style='float:right;'>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:30px; font-weight:bold; \">" +
                "                        " + grandTotal +
                "                    </div>" +
                "                </div>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:30px; font-weight:bold; \">" +
                "                        " + (Convert.ToDecimal(grandTotal) * Convert.ToDecimal(0.07)).ToString("F") + "" +
                "                    </div>" +
                "                </div>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:30px; font-weight:bold; \">" +
                "                       " + (Convert.ToDecimal(grandTotal) + (Convert.ToDecimal(grandTotal) * Convert.ToDecimal(0.07))).ToString("F") + "" +
                "                    </div>" +
                "                </div>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:30px; font-weight:bold; \">" +
                "                        " + objItem.deposit.dAmt + "" +
                "                    </div>" +
                "                </div>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:30px; font-weight:bold; \">" +
                "                        " + (depositAmtPrice - objItem.deposit.dAmt).ToString("F") + "" +
                "                    </div>" +
                "                </div>" +

                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:30px; font-weight:bold; \">" +
                "                        " + ((Convert.ToDecimal(grandTotal) + (Convert.ToDecimal(grandTotal) * Convert.ToDecimal(0.07))) - (depositAmtPrice)).ToString("F") + "" +
                "                    </div>" +
                "                </div>" +
                "            </div>" +
                "        </div>" +
                "    </div>" +
                "    <div style=\"clear:both;\"></div>" +
                "    <div style=\"display:block; margin-top:10px; font-weight:bold;\">" +
                "        Terms & Conditions:" +
                "    </div>" +
                "    <div style=\"clear:both;\"></div>" +
                "    <p style=\"display:block; margin-top:5px; \">" +
                "        Minimum 30 Pax*. Confirmation of order within 2 days in advance with 50% deposit and the balance payable at the time of delivery." +
                "    </p>" +
                "    <div style=\"clear:both;\"></div>" +
                "    <p style=\"display:block; margin-top:0px; \">" +
                "        The company reserves the right to change the terms and conditions." +
                "    </p>" +
                "    <div style=\"clear:both;\"></div>" +
                "    <p style=\"display:block; margin-top:0px; \">" +
                "        Cheque should be made payable to KARU'S INDIAN BANANA LEAF RESTAURANT" +
                "    </p>" +
                "    <div style=\"display:block; margin-top:80px;\">" +
                "        <div style=\"height:200px; width:200px; float:left;\">" +
                 "           <img src='" + objItem.reciver.imageData + "' height='100' width='200'/>" +
                "            <div>___________________</div>" +
                "            <div>Recipient Signature</div>" +
                "        </div>" +
                "        <div style=\"height:200px; width:200px; float:right;\">" +
                "           <img src='" + objItem.customer.imageData + "' height='100' width='200'/>" +
                "            <div>___________________</div>" +
                "            <div>Customer'sSignature</div>" +
                "        </div>" +
                "    </div>" +
                "</body>" +
                "</html>";

            StringReader sr = new StringReader(sb.ToString());

            try
            {
                var fileName = "Recipt_" + payment + ".pdf";
                string fileNameWitPath = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/"), fileName);
                FileStream fs = new FileStream(fileNameWitPath, FileMode.OpenOrCreate);
                var htmlContent = String.Format(sb);
                var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
                var pdfBytes = htmlToPdf.GeneratePdf(htmlContent);
                fs.Write(pdfBytes, 0, pdfBytes.Length);
                fs.Close();
                try
                {
                    string subject = order["vcOrderId"] + "RECIPT";
                    string body = "Hi, please see the recipt <a href=" + (HttpContext.Current.Request.Url.Host + ("/Uploads/") + fileName) + ">Open Recept</a> ";
                    string FromMail = "surendar28111989@gmail.com";
                    string emailTo = "surendar28ih@gmail.com";
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com", 587);
                    mail.From = new MailAddress(FromMail);
                    mail.To.Add(emailTo);
                    mail.Subject = subject;
                    mail.Body = body;
                    SmtpServer.Port = 25;
                    SmtpServer.Credentials = new System.Net.NetworkCredential("surendar28111989@gmail.com", "12rnsB6090");
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Send(mail);
                }
                catch
                {
                }
                return Request.CreateResponse(HttpStatusCode.Created, fileName);
            }
            catch
            {
                throw;
            }
            //finally
            //{

            //    doc.Close();
            //}
        }

        [HttpPost]
        [ActionName("cancel")]
        public HttpResponseMessage CancelOrder(Order objItem)
        {
            var updateOrder = DbAccess.DbAQueryInsert("update tbl_order set isCancel='1' where id = '" + objItem.id + "'");
            return Request.CreateResponse(HttpStatusCode.Created, updateOrder);
        }

        [HttpPost]
        [ActionName("invoice")]
        public HttpResponseMessage invoice(Recipt objItem)
        {
            var selectedOrder = DbAccess.DbASelect("select vcInvoiceId from tbl_order  where id = '" + objItem.order.id + "' AND vcInvoiceId IS NOT NULL " );

            if (selectedOrder.Count == 0)
            {     
                     var updateOrder = DbAccess.DbAQueryInsert("update tbl_order set iConfirm='1', vcInvoiceId ='I-" + RandomNumber() + "' where id = '" + objItem.order.id + "'");
            }
            var orders = DbAccess.DbASelect("select o.vcOrderId, "
                                                + " o.vcInvoiceId,"
                                                + " o.id,"
                                                + " o.createdAt,"
                                                + " o.isQuatation,"
                                                + " o.iPack,"
                                                + " o.iPackPerPrice,"
                                                + " o.iOthers,"
                                                + " o.iGST,"
                                                + " o.vcFuncName,"
                                                + " o.vcFuncAddress,"
                                                + " o.vcFuncLocation,"
                                                + " o.vcFunctionDate,"
                                                + " o.vcNote,"
                                                + " o.isCancel, "
                                                + " o.isTax,"
                                                + " o.iConfirm,"
                                                + " o.iCustomerId, "
                                                + " o.iPackageId, "
                                                + " pack.vcPackagename, "
                                                + " cd.vcName as customername,"
                                                + " cd.iPhonenumber as customerphone,"
                                                + " cd.vcEmailid as customeremail"

                                                + "  from (select * from tbl_order where id = " + objItem.order.id + ") as o"
                                                + " left join tbl_package pack on pack.id = o.iPackageId"
                                                + " left join tbl_customer_details cd on cd.id = o.iCustomerId ");
            var order = orders[0];
            var menuitems = DbAccess.DbASelect("SELECT * FROM tbl_order_item_mapping where iOrderId  = '" + objItem.order.id + "'");
            var logisticitems = DbAccess.DbASelect("SELECT * FROM tbl_order_logistic_mapping where iOrderId  = '" + objItem.order.id + "'");
            var deposit = DbAccess.DbASelect("SELECT * FROM tbl_deposits where iOrderId  = '" + objItem.order.id + "'");

            string menuItemString = string.Empty;
            string logisticItemString = string.Empty;
            string rowString = "<br/>";
            int row = 1;
            if (Convert.ToInt32(order["iPackageId"]) != 0)
            {
                row = 2;
            }

            decimal otherPrice = 0;
            decimal depositAmtPrice = 0;
            string menuItemTableString = string.Empty;

            foreach (var item in menuitems)
            {
                menuItemString += "<br/> <span style='font-weight:normal; '>" + item["itemName"] + "</span>";
                rowString += "<br/>";
            }

            foreach (var item in deposit)
            {
                depositAmtPrice += Convert.ToDecimal(item["dAmt"]);
            }

            foreach (var item in logisticitems)
            {
                if (Convert.ToInt32(item["iQuantity"]) != 0)
                {
                    logisticItemString += "    <div style=\"clear:both;\"></div>" +
                    "    <div style=\"display:block;\">" +
                    "        <div style=\"border:1px solid #000; float:left; width:40px; text-align:center; font-weight:bold; \">" +
                    "            " + row++ + "" +
                    "        </div>" +
                    "        <div style=\"border:1px solid #000; float:left; width:240px; text-align:left; font-weight:bold; \">" +
                    "            <div style=\"padding-left:10px\">" + item["itemName"] + "" +
                    "            </div>" +
                    "        </div>" +
                    "        <div style=\"border:1px solid #000; float:left; width:180px; text-align:center; font-weight:bold; \">" +
                    "            " + item["iQuantity"] + "" +
                    "        </div>" +
                    "        <div style=\"border:1px solid #000; float:left; width:200px; text-align:center; font-weight:bold; \">" +
                    "            $" + (Convert.ToDecimal(item["dPrice"]) / Convert.ToInt32(item["iQuantity"])).ToString("F") + "" +
                    "        </div>" +
                    "        <div style=\"border:1px solid #000; float:left; width:200px; text-align:center; font-weight:bold; \">" +
                    "            $" + item["dPrice"] + "" +
                    "        </div>" +
                    "    </div>";

                    otherPrice += Convert.ToDecimal(item["dPrice"]);
                }
            }

            string grandTotal = ((Convert.ToDecimal(order["iPack"]) * Convert.ToDecimal(order["iPackPerPrice"])) + otherPrice).ToString("F"); ;

            Rectangle pageSize = new Rectangle(PageSize.A4);

            if (Convert.ToInt32(order["iPackageId"]) != 0)
            {
                menuItemTableString = "<div style=\"display:block;\">" +
                    "        <div style=\"border:1px solid #000; float:left; width:40px; text-align:center; font-weight:bold; \">" +
                    "            1" + rowString +
                    "        </div>" +
                    "        <div style=\"border:1px solid #000; float:left; width:240px; text-align:left; font-weight:bold; \">" +
                    "            <div style=\"padding-left:10px\">" + order["vcPackagename"] + menuItemString +
                    "            </div>" +
                    "        </div>" +
                    "        <div style=\"border:1px solid #000; float:left; width:180px; text-align:center; font-weight:bold; \">" +
                    "            " + order["iPack"] + rowString +
                    "        </div>" +
                    "        <div style=\"border:1px solid #000; float:left; width:200px; text-align:center; font-weight:bold; \">" +
                    "            $" + order["iPackPerPrice"] + rowString +
                    "        </div>" +
                    "        <div style=\"border:1px solid #000; float:left; width:200px; text-align:center; font-weight:bold; \">" +
                    "            $" + (Convert.ToDecimal(order["iPack"]) * Convert.ToDecimal(order["iPackPerPrice"])) + rowString +
                    "        </div>" +
                    "    </div>";
            }
            string sb = "<!DOCTYPE html>" +
                "<html>" +
                "<head>" +
                "    <meta charset=\"UTF-8\">" +
                "    <title>Excel To HTML using codebeautify.org</title>" +
                "</head>" +
                "<body style='font-family: sans-serif; font-size:14px;'>" +
                "    <div>" +
                "        <div style='float:left;'></div>" +
                "        <div style='float:right; text-align:right;'>" +
                "            <h1 style='color:#333; font-size:24px; font-family: sans-serif; margin:0px; padding:px '>Tax Invoice</h1>" +
                "            <h5 style='color:#666; font-family: sans-serif; margin:0px;'>GST Reg No. M90363451J" +
                "						</h5>" +
                "            <h5 style='color:#666; margin-top:20px; margin-bottom:0px; font-family: sans-serif;'>ORDER NO :" +
                "						<span style=\"color:#f01;\"> " + order["vcOrderId"] + "</span>" +
                "						</h5>" +
                "            <h5 style='color:#666; margin:5px 0px; font-family: sans-serif;'>INVOICE NO :" +
                "						<span style=\"color:#f01;\">  " + (order["vcInvoiceId"] != null ? order["vcInvoiceId"] : "--") + "</span>" +
                "						</h5>" +
                "        </div>" +
                "    </div>" +
                "    <div style=\"clear:both\"></div>" +
                "    <div>" +
                "        <label style='color:#666;'>Catering Hotline: 67627284</label>" +
                "        <label style='color:#666; float:right'>Order Date: <span style=\"color:#000; font-weight:bold\">" + Convert.ToDateTime(order["createdAt"]).ToString("MM/dd/yyyy hh:mm tt") + "</span>" +
                "        </label>" +
                "    </div>" +
                "    <div style=\"margin-top:10px; display:inline-block; width:48%; text-align:left;\">" +
                "        <label style='color:#000; display:block; font-weight:bold;'>" + order["customername"] + "</label>" +
                "        <label style='color:#666; display:block; margin-top:5px;'>Tel: <span style=\"color:#000; font-weight:bold;\">" + order["customerphone"] + "</span>" +
                "        </label>" +
                "    </div>" +
                "    <div style=\"margin-top:10px; display:inline-block; width:50%; text-align:left;\">" +
                "        <label style='color:#666; display:block; margin-top:5px;'>Function: <span style=\"color:#000; font-weight:bold;\">" + Convert.ToDateTime(order["vcFunctionDate"]).ToString("MM/dd/yyyy hh:mm tt") + "</span>" +
                "        </label>" +
                "        <label style='color:#666; display:block; margin-top:5px;'>Location: <span style=\"color:#000; font-weight:bold;\">" + order["vcFuncLocation"] + "</span>" +
                "        </label>" +
                "    </div>" +
                "    <div style=\"display:inline-block; width:50%; text-align:left;\">" +
                "        <label style='color:#666; display:block; margin-top:5px;'>Email: <span style=\"color:#000; font-weight:bold;\">" + order["customeremail"] + "</span>" +
                "        </label>" +
                "    </div>" +
                "    <div style=\"margin-top:10px; display:block;\">" +
                "        <div style=\"border:1px solid #000; float:left; width:40px; background:#ccc; text-align:center; font-weight:bold; \">" +
                "            No" +
                "        </div>" +
                "        <div style=\"border:1px solid #000; float:left; width:240px; background:#ccc; text-align:center; font-weight:bold; \">" +
                "            DESCRIPTION" +
                "        </div>" +
                "        <div style=\"border:1px solid #000; float:left; width:180px; background:#ccc; text-align:center; font-weight:bold; \">" +
                "            NO OF ITEMS" +
                "        </div>" +
                "        <div style=\"border:1px solid #000; float:left; width:200px; background:#ccc; text-align:center; font-weight:bold; \">" +
                "            UNIT PRICE" +
                "        </div>" +
                "        <div style=\"border:1px solid #000; float:left; width:200px; background:#ccc; text-align:center; font-weight:bold; \">" +
                "            TOTAL" +
                "        </div>" +
                "    </div>" +
                "    <div style=\"clear:both;\"></div>" +
           "" + menuItemTableString + "" +
               "" + logisticItemString + "" +
                "    <div style=\"clear:both;\"></div>" +
                "    <div style=\"display:block;\">" +
                "        <div style=\"border:1px solid #000; float:left; width:666px; text-align:center; font-weight:bold; \">" +
                "            <div style='float:left;'>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; text-align:left; padding-left:2px\">" +
                "                        Notes:" +
                "                    </div>" +
                "                    <div style=\"display:block; font-weight:normal; font-size:13px;\">" + order["vcNote"] + "</div>" +
                "                </div>" +
                "            </div>" +
                "            <div style='float:right;'>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:10px; font-weight:bold; \">" +
                "                        Sub Total" +
                "                    </div>" +
                "                </div>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:10px; font-weight:bold; \">" +
                "                        7% GST" +
                "                    </div>" +
                "                </div>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:10px; font-weight:bold; \">" +
                "                        Grand Total" +
                "                    </div>" +
                "                </div>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:10px; font-weight:bold; \">" +
                "                        Deposit" +
                "                    </div>" +
                "                </div>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:10px; font-weight:bold; \">" +
                "                        Balance Remining" +
                "                    </div>" +
                "                </div>" +
                "            </div>" +
                "        </div>" +
                "        <div style=\"border:1px solid #000; float:left; width:200px; text-align:center; font-weight:bold; \">" +
                "            <div style='float:right;'>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:30px; font-weight:bold; \">" +
                "                        " + grandTotal +
                "                    </div>" +
                "                </div>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:30px; font-weight:bold; \">" +
                "                        " + (Convert.ToDecimal(grandTotal) * Convert.ToDecimal(0.07)).ToString("F") + "" +
                "                    </div>" +
                "                </div>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:30px; font-weight:bold; \">" +
                "                       " + (Convert.ToDecimal(grandTotal) + (Convert.ToDecimal(grandTotal) * Convert.ToDecimal(0.07))).ToString("F") + "" +
                "                    </div>" +
                "                </div>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:30px; font-weight:bold; \">" +
                "                        " + depositAmtPrice.ToString("F") + "" +
                "                    </div>" +
                "                </div>" +

                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:30px; font-weight:bold; \">" +
                "                        " + ((Convert.ToDecimal(grandTotal) + (Convert.ToDecimal(grandTotal) * Convert.ToDecimal(0.07))) - (depositAmtPrice)).ToString("F") + "" +
                "                    </div>" +
                "                </div>" +
                "            </div>" +
                "        </div>" +
                "    </div>" +
                "    <div style=\"clear:both;\"></div>" +
                "    <div style=\"display:block; margin-top:10px; font-weight:bold;\">" +
                "        Terms & Conditions:" +
                "    </div>" +
                "    <div style=\"clear:both;\"></div>" +
                "    <p style=\"display:block; margin-top:5px; \">" +
                "        Minimum 30 Pax*. Confirmation of order within 2 days in advance with 50% deposit and the balance payable at the time of delivery." +
                "    </p>" +
                "    <div style=\"clear:both;\"></div>" +
                "    <p style=\"display:block; margin-top:0px; \">" +
                "        The company reserves the right to change the terms and conditions." +
                "    </p>" +
                "    <div style=\"clear:both;\"></div>" +
                "    <p style=\"display:block; margin-top:0px; \">" +
                "        Cheque should be made payable to KARU'S INDIAN BANANA LEAF RESTAURANT" +
                "    </p>" +
                "    <div style=\"display:block; margin-top:80px;\">" +
                "        <div style=\"height:200px; width:200px; float:left;\">" +
                 "           <img src='" + objItem.reciver.imageData + "' height='100' width='200'/>" +
                "            <div>___________________</div>" +
                "            <div>Authorised Signature</div>" +
                "        </div>" +
                "        <div style=\"height:200px; width:200px; float:right;\">" +
                "           <img src='" + objItem.customer.imageData + "' height='100' width='200'/>" +
                "            <div>___________________</div>" +
                "            <div>Customer'sSignature</div>" +
                "        </div>" +
                "    </div>" +
                "</body>" +
                "</html>";

            StringReader sr = new StringReader(sb.ToString());
            try
            {
                var fileName = "Invoice_" + objItem.order.id + ".pdf";
                string fileNameWitPath = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/"), fileName);
                FileStream fs = new FileStream(fileNameWitPath, FileMode.OpenOrCreate);

                var htmlContent = String.Format(sb);
                var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
                var pdfBytes = htmlToPdf.GeneratePdf(htmlContent);
                fs.Write(pdfBytes, 0, pdfBytes.Length);
                fs.Close();

                try
                {
                    string subject = order["vcInvoiceId"] + " Invoice";
                    string body = "Hi, please see the Invoice <a href=" + (HttpContext.Current.Request.Url.Host + ("/Uploads/") + fileName) + ">Open Invoice</a> ";
                    string FromMail = "surendar28111989@gmail.com";
                    string emailTo = "surendar28ih@gmail.com";
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com", 587);
                    mail.From = new MailAddress(FromMail);
                    mail.To.Add(emailTo);
                    mail.Subject = subject;
                    mail.Body = body;
                    SmtpServer.Port = 25;
                    SmtpServer.Credentials = new System.Net.NetworkCredential("surendar28111989@gmail.com", "12rnsB6090");
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Send(mail);
                }
                catch
                {

                }

                return Request.CreateResponse(HttpStatusCode.Created, fileName);
            }
            catch
            {
                throw;
            }
        }

        [HttpPost]
        [ActionName("qutation")]
        public HttpResponseMessage qutation(Recipt objItem)
        {
            var updateOrder = DbAccess.DbAQueryInsert("update tbl_order set isQuatation='1' where id = '" + objItem.order.id + "'");
            var orders = DbAccess.DbASelect("select o.vcOrderId, "
                                                + " o.vcInvoiceId,"
                                                + " o.id,"
                                                + " o.createdAt,"
                                                + " o.isQuatation,"
                                                + " o.iPack,"
                                                + " o.iPackPerPrice,"
                                                + " o.iOthers,"
                                                + " o.iGST,"
                                                + " o.vcFuncName,"
                                                + " o.vcFuncAddress,"
                                                + " o.vcFuncLocation,"
                                                + " o.vcFunctionDate,"
                                                + " o.vcNote,"
                                                + " o.isCancel, "
                                                + " o.isTax,"
                                                + " o.iConfirm,"
                                                + " o.iCustomerId, "
                                                + " o.iPackageId, "
                                                + " pack.vcPackagename, "
                                                + " cd.vcName as customername,"
                                                + " cd.iPhonenumber as customerphone,"
                                                + " cd.vcEmailid as customeremail"

                                                + "  from (select * from tbl_order where id = " + objItem.order.id + ") as o"
                                                + " left join tbl_package pack on pack.id = o.iPackageId"
                                                + " left join tbl_customer_details cd on cd.id = o.iCustomerId ");
            var order = orders[0];
            var menuitems = DbAccess.DbASelect("SELECT * FROM tbl_order_item_mapping where iOrderId  = '" + objItem.order.id + "'");
            var logisticitems = DbAccess.DbASelect("SELECT * FROM tbl_order_logistic_mapping where iOrderId  = '" + objItem.order.id + "'");
            var deposit = DbAccess.DbASelect("SELECT * FROM tbl_deposits where iOrderId  = '" + objItem.order.id + "'");

            string menuItemString = string.Empty;
            string logisticItemString = string.Empty;
            string rowString = "<br/>";
            int row = 1;
            if (Convert.ToInt32(order["iPackageId"]) != 0)
            {
                row = 2;
            }
            string menuItemTableString = string.Empty;


            decimal otherPrice = 0;
            decimal depositAmtPrice = 0;

            foreach (var item in menuitems)
            {
                menuItemString += "<br/> <span style='font-weight:normal; '>" + item["itemName"] + "</span>";
                rowString += "<br/>";
            }

            foreach (var item in deposit)
            {
                depositAmtPrice += Convert.ToDecimal(item["dAmt"]);
            }

            foreach (var item in logisticitems)
            {
                if (Convert.ToInt32(item["iQuantity"]) != 0)
                {
                    logisticItemString += "    <div style=\"clear:both;\"></div>" +
                    "    <div style=\"display:block;\">" +
                    "        <div style=\"border:1px solid #000; float:left; width:40px; text-align:center; font-weight:bold; \">" +
                    "            " + row++ + "" +
                    "        </div>" +
                    "        <div style=\"border:1px solid #000; float:left; width:240px; text-align:left; font-weight:bold; \">" +
                    "            <div style=\"padding-left:10px\">" + item["itemName"] + "" +
                    "            </div>" +
                    "        </div>" +
                    "        <div style=\"border:1px solid #000; float:left; width:180px; text-align:center; font-weight:bold; \">" +
                    "            " + item["iQuantity"] + "" +
                    "        </div>" +
                    "        <div style=\"border:1px solid #000; float:left; width:200px; text-align:center; font-weight:bold; \">" +
                    "            $" + (Convert.ToDecimal(item["dPrice"]) / Convert.ToInt32(item["iQuantity"])).ToString("F") + "" +
                    "        </div>" +
                    "        <div style=\"border:1px solid #000; float:left; width:200px; text-align:center; font-weight:bold; \">" +
                    "            $" + item["dPrice"] + "" +
                    "        </div>" +
                    "    </div>";

                    otherPrice += Convert.ToDecimal(item["dPrice"]);
                }
            }

            string grandTotal = ((Convert.ToDecimal(order["iPack"]) * Convert.ToDecimal(order["iPackPerPrice"])) + otherPrice).ToString("F"); ;

            Rectangle pageSize = new Rectangle(PageSize.A4);

            if (Convert.ToInt32(order["iPackageId"]) != 0)
            {
                menuItemTableString = "<div style=\"display:block;\">" +
                    "        <div style=\"border:1px solid #000; float:left; width:40px; text-align:center; font-weight:bold; \">" +
                    "            1" + rowString +
                    "        </div>" +
                    "        <div style=\"border:1px solid #000; float:left; width:240px; text-align:left; font-weight:bold; \">" +
                    "            <div style=\"padding-left:10px\">" + order["vcPackagename"] + menuItemString +
                    "            </div>" +
                    "        </div>" +
                    "        <div style=\"border:1px solid #000; float:left; width:180px; text-align:center; font-weight:bold; \">" +
                    "            " + order["iPack"] + rowString +
                    "        </div>" +
                    "        <div style=\"border:1px solid #000; float:left; width:200px; text-align:center; font-weight:bold; \">" +
                    "            $" + order["iPackPerPrice"] + rowString +
                    "        </div>" +
                    "        <div style=\"border:1px solid #000; float:left; width:200px; text-align:center; font-weight:bold; \">" +
                    "            $" + (Convert.ToDecimal(order["iPack"]) * Convert.ToDecimal(order["iPackPerPrice"])) + rowString +
                    "        </div>" +
                    "    </div>";
            }

            string sb = "<!DOCTYPE html>" +
                "<html>" +
                "<head>" +
                "    <meta charset=\"UTF-8\">" +
                "    <title>Excel To HTML using codebeautify.org</title>" +
                "</head>" +
                "<body style='font-family: sans-serif; font-size:14px;'>" +
                "    <div>" +
                "        <div style='float:left;'></div>" +
                "        <div style='float:right; text-align:right;'>" +
                "            <h1 style='color:#333; font-size:24px; font-family: sans-serif; margin:0px; padding:px '>Quotation</h1>" +
                "            <h5 style='color:#666; font-family: sans-serif; margin:0px;'>GST Reg No. M90363451J" +
                "						</h5>" +
                "            <h5 style='color:#666; margin-top:20px; margin-bottom:0px; font-family: sans-serif;'>ORDER NO :" +
                "						<span style=\"color:#f01;\"> " + order["vcOrderId"] + "</span>" +
                "						</h5>" +
                "            <h5 style='color:#666; margin:5px 0px; font-family: sans-serif;'>INVOICE NO :" +
                "						<span style=\"color:#f01;\">  " + (order["vcInvoiceId"] != null ? order["vcInvoiceId"] : "--") + "</span>" +
                "						</h5>" +
                "        </div>" +
                "    </div>" +
                "    <div style=\"clear:both\"></div>" +
                "    <div>" +
                "        <label style='color:#666;'>Catering Hotline: 67627284</label>" +
                "        <label style='color:#666; float:right'>Order Date: <span style=\"color:#000; font-weight:bold\">" + Convert.ToDateTime(order["createdAt"]).ToString("MM/dd/yyyy hh:mm tt") + "</span>" +
                "        </label>" +
                "    </div>" +
                "    <div style=\"margin-top:10px; display:inline-block; width:48%; text-align:left;\">" +
                "        <label style='color:#000; display:block; font-weight:bold;'>" + order["customername"] + "</label>" +
                "        <label style='color:#666; display:block; margin-top:5px;'>Tel: <span style=\"color:#000; font-weight:bold;\">" + order["customerphone"] + "</span>" +
                "        </label>" +
                "    </div>" +
                "    <div style=\"margin-top:10px; display:inline-block; width:50%; text-align:left;\">" +
                "        <label style='color:#666; display:block; margin-top:5px;'>Function: <span style=\"color:#000; font-weight:bold;\">" + Convert.ToDateTime(order["vcFunctionDate"]).ToString("MM/dd/yyyy hh:mm tt") + "</span>" +
                "        </label>" +
                "        <label style='color:#666; display:block; margin-top:5px;'>Location: <span style=\"color:#000; font-weight:bold;\">" + order["vcFuncLocation"] + "</span>" +
                "        </label>" +
                "    </div>" +
                "    <div style=\"display:inline-block; width:50%; text-align:left;\">" +
                "        <label style='color:#666; display:block; margin-top:5px;'>Email: <span style=\"color:#000; font-weight:bold;\">" + order["customeremail"] + "</span>" +
                "        </label>" +
                "    </div>" +
                "    <div style=\"margin-top:10px; display:block;\">" +
                "        <div style=\"border:1px solid #000; float:left; width:40px; background:#ccc; text-align:center; font-weight:bold; \">" +
                "            No" +
                "        </div>" +
                "        <div style=\"border:1px solid #000; float:left; width:240px; background:#ccc; text-align:center; font-weight:bold; \">" +
                "            DESCRIPTION" +
                "        </div>" +
                "        <div style=\"border:1px solid #000; float:left; width:180px; background:#ccc; text-align:center; font-weight:bold; \">" +
                "            NO OF ITEMS" +
                "        </div>" +
                "        <div style=\"border:1px solid #000; float:left; width:200px; background:#ccc; text-align:center; font-weight:bold; \">" +
                "            UNIT PRICE" +
                "        </div>" +
                "        <div style=\"border:1px solid #000; float:left; width:200px; background:#ccc; text-align:center; font-weight:bold; \">" +
                "            TOTAL" +
                "        </div>" +
                "    </div>" +
                "    <div style=\"clear:both;\"></div>" +
              "" + menuItemTableString + "" +
               "" + logisticItemString + "" +
                "    <div style=\"clear:both;\"></div>" +
                "    <div style=\"display:block;\">" +
                "        <div style=\"border:1px solid #000; float:left; width:666px; text-align:center; font-weight:bold; \">" +
                "            <div style='float:left;'>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; text-align:left; padding-left:2px\">" +
                "                        Notes:" +
                "                    </div>" +
                "                    <div style=\"display:block; font-weight:normal; font-size:13px;\">" + order["vcNote"] + "</div>" +
                "                </div>" +
                "            </div>" +
                "            <div style='float:right;'>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:10px; font-weight:bold; \">" +
                "                        Sub Total" +
                "                    </div>" +
                "                </div>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:10px; font-weight:bold; \">" +
                "                        7% GST" +
                "                    </div>" +
                "                </div>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:10px; font-weight:bold; \">" +
                "                        Grand Total" +
                "                    </div>" +
                "                </div>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:10px; font-weight:bold; \">" +
                "                        Deposit" +
                "                    </div>" +
                "                </div>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:10px; font-weight:bold; \">" +
                "                        Balance Remining" +
                "                    </div>" +
                "                </div>" +
                "            </div>" +
                "        </div>" +
                "        <div style=\"border:1px solid #000; float:left; width:200px; text-align:center; font-weight:bold; \">" +
                "            <div style='float:right;'>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:30px; font-weight:bold; \">" +
                "                        " + grandTotal +
                "                    </div>" +
                "                </div>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:30px; font-weight:bold; \">" +
                "                        " + (Convert.ToDecimal(grandTotal) * Convert.ToDecimal(0.07)).ToString("F") + "" +
                "                    </div>" +
                "                </div>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:30px; font-weight:bold; \">" +
                "                       " + (Convert.ToDecimal(grandTotal) + (Convert.ToDecimal(grandTotal) * Convert.ToDecimal(0.07))).ToString("F") + "" +
                "                    </div>" +
                "                </div>" +
                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:30px; font-weight:bold; \">" +
                "                        " + depositAmtPrice.ToString("F") + "" +
                "                    </div>" +
                "                </div>" +

                "                <div style=\"display:block;\">" +
                "                    <div style=\"float:left; width:200px; text-align:right; padding-right:30px; font-weight:bold; \">" +
                "                        " + ((Convert.ToDecimal(grandTotal) + (Convert.ToDecimal(grandTotal) * Convert.ToDecimal(0.07))) - (depositAmtPrice)).ToString("F") + "" +
                "                    </div>" +
                "                </div>" +
                "            </div>" +
                "        </div>" +
                "    </div>" +
                "    <div style=\"clear:both;\"></div>" +
                "    <div style=\"display:block; margin-top:10px; font-weight:bold;\">" +
                "        Terms & Conditions:" +
                "    </div>" +
                "    <div style=\"clear:both;\"></div>" +
                "    <p style=\"display:block; margin-top:5px; \">" +
                "        Minimum 30 Pax*. Confirmation of order within 2 days in advance with 50% deposit and the balance payable at the time of delivery." +
                "    </p>" +
                "    <div style=\"clear:both;\"></div>" +
                "    <p style=\"display:block; margin-top:0px; \">" +
                "        The company reserves the right to change the terms and conditions." +
                "    </p>" +
                "    <div style=\"clear:both;\"></div>" +
                "    <p style=\"display:block; margin-top:0px; \">" +
                "        Cheque should be made payable to KARU'S INDIAN BANANA LEAF RESTAURANT" +
                "    </p>" +
                "    <div style=\"display:block; margin-top:80px;\">" +
                "        <div style=\"height:200px; width:200px; float:left;\">" +
                "            <div>___________________</div>" +
                "            <div>Authorised Signature</div>" +
                "        </div>" +
                "        <div style=\"height:200px; width:200px; float:right;\">" +
                "            <div>___________________</div>" +
                "            <div>Customer'sSignature</div>" +
                "        </div>" +
                "    </div>" +
                "</body>" +
                "</html>";

            StringReader sr = new StringReader(sb.ToString());

            try
            {
                var fileName = "Quatation_" + objItem.order.id + ".pdf";
                string fileNameWitPath = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/"), fileName);
                FileStream fs = new FileStream(fileNameWitPath, FileMode.OpenOrCreate);

                var htmlContent = String.Format(sb);
                var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
                var pdfBytes = htmlToPdf.GeneratePdf(htmlContent);
                fs.Write(pdfBytes, 0, pdfBytes.Length);
                fs.Close();
                try
                {
                    string subject = order["vcOrderId"] + " Quatation";
                    string body = "Hi, please see the Quatation <a href=" + (HttpContext.Current.Request.Url.Host + ("/Uploads/") + fileName) + ">Open Quatation</a> ";
                    string FromMail = "surendar28111989@gmail.com";
                    string emailTo = "surendar28ih@gmail.com";
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com", 587);
                    mail.From = new MailAddress(FromMail);
                    mail.To.Add(emailTo);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    SmtpServer.Port = 25;
                    SmtpServer.Credentials = new System.Net.NetworkCredential("surendar28111989@gmail.com", "12rnsB6090");
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Send(mail);
                }
                catch
                {
                }
                return Request.CreateResponse(HttpStatusCode.Created, fileName);
            }
            catch
            {
                throw;
            }
        }

        [HttpPost]
        [ActionName("placeorder")]
        public HttpResponseMessage insertMenuItem(PlaceOrder objItem)
        {
            try
            {
                List<Error> Error = new List<Error>();
                Error SingleError = new Error();

                if (!IsValidEmail(objItem.customer.vcEmailid))
                {
                    SingleError = new Error();
                    SingleError.code = "2005";
                    SingleError.message = "Email Invalid";
                    Error.Add(SingleError);
                }

                if (!IsPhoneNumber(objItem.customer.iPhonenumber.ToString()))
                {
                    SingleError = new Error();
                    SingleError.code = "2006";
                    SingleError.message = "Phone Invalid";
                    Error.Add(SingleError);
                }

                if (Error.Count == 0)
                {
                    var customers = DbAccess.DbASelect("select * from tbl_customer_details where iPhonenumber ='" + objItem.customer.iPhonenumber + "' or vcEmailid = '" + objItem.customer.vcEmailid + "'");

                    if (customers.Count > 0)
                    {
                        var customer = customers[0];
                        objItem.order.iCustomerId = Convert.ToInt32(customer["id"]);
                    }
                    else
                    {
                        var customer = DbAccess.DbAQueryInsert("insert into tbl_customer_details  (vcName, iPhonenumber, vcEmailid) values ('" + objItem.customer.vcName + "','" + objItem.customer.iPhonenumber + "','" + objItem.customer.vcEmailid + "')");
                        if (customer == "no added")
                        {
                            SingleError = new Error();
                            SingleError.code = "2001";
                            SingleError.message = "Customer can't added.";
                            Error.Add(SingleError);
                        }
                        else
                        {
                            objItem.order.iCustomerId = Convert.ToInt32(customer);
                        }
                    }
                }

                if (Error.Count == 0)
                {
                    string order = string.Empty;
                    if (objItem.order.id == null)
                    {
                        order = DbAccess.DbAQueryInsert("insert into tbl_order (vcOrderId, iPackageId, isQuatation, iCustomerId, iPack, iPackPerPrice, iOthers, iGST, vcFuncName, vcFuncAddress, vcFuncLocation, vcFunctionDate, vcNote, isTax, iConfirm) values ('O-" + RandomNumber()
                        + "','" + objItem.order.iPackageId
                        + "','" + objItem.order.isQuatation
                        + "','" + objItem.order.iCustomerId
                        + "','" + objItem.order.iPack
                        + "','" + objItem.order.iPackPerPrice
                        + "','" + objItem.order.iOthers
                        + "','" + objItem.order.iGST
                        + "','" + objItem.order.vcFuncName
                        + "','" + objItem.order.vcFuncAddress
                        + "','" + objItem.order.vcFuncLocation
                        + "','" + objItem.order.vcFunctionDate
                        + "','" + objItem.order.vcNote
                        + "','" + objItem.order.isTax
                        + "','" + objItem.order.iConfirm + "')");
                    }
                    else
                    {
                        order = DbAccess.DbAQueryInsert("update tbl_order set iPackageId='" + objItem.order.iPackageId
                        + "', iCustomerId='" + objItem.order.iCustomerId
                        + "', iPackPerPrice='" + objItem.order.iPackPerPrice
                        + "', iPack='" + objItem.order.iPack
                        + "', iOthers='" + objItem.order.iOthers
                        + "', iGST='" + objItem.order.iGST
                        + "', vcFuncName='" + objItem.order.vcFuncName
                        + "', vcFuncAddress='" + objItem.order.vcFuncAddress
                        + "', vcFuncLocation='" + objItem.order.vcFuncLocation
                        + "', vcFunctionDate='" + objItem.order.vcFunctionDate
                        + "', vcNote='" + objItem.order.vcNote
                        + "', isTax='" + objItem.order.isTax
                        + "', isQuatation='" + objItem.order.isQuatation
                        + "', iConfirm='" + objItem.order.iConfirm
                        + "' where id = '" + objItem.order.id + "'");
                    }

                    if (order == "no added")
                    {
                        SingleError = new Error();
                        SingleError.code = "2001";
                        SingleError.message = "order can't added.";
                        Error.Add(SingleError);
                    }
                    else
                    {
                        var orderId = Convert.ToInt32(order);
                        if (orderId == 0)
                        {
                            orderId = Convert.ToInt32(objItem.order.id);
                        }
                        DbAccess.DbAQueryInsert("delete from tbl_order_item_mapping  where iOrderId ='" + orderId + "'");
                        DbAccess.DbAQueryInsert("delete from tbl_order_logistic_mapping  where iOrderId ='" + orderId + "'");
                        foreach (OrderMenuItem objMenu in objItem.menuitems)
                        {
                            DbAccess.DbAQueryInsert("insert into tbl_order_item_mapping (iOrderId, iItemId, iQuantity, dPrice, itemName) values ('" + orderId + "','" + objMenu.iItemId + "','" + objMenu.iQuantity + "','" + objMenu.dPrice + "','" + objMenu.itemName + "')");
                        }

                        foreach (OrderLogisticItem objMenu in objItem.logisticitems)
                        {
                            DbAccess.DbAQueryInsert("insert into tbl_order_logistic_mapping (iOrderId, iLogisticId, iQuantity, dPrice, itemName) values ('" + orderId + "','" + objMenu.iLogisticId + "','" + objMenu.iQuantity + "','" + objMenu.dPrice + "','" + objMenu.itemName + "')");
                        }

                        return Request.CreateResponse(HttpStatusCode.OK, orderId);
                    }
                }

                if (Error.Count > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, Error);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "success");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string RandomNumber()
        {
            Random random = new Random();
            return random.Next(0, 9999).ToString("D4");
        }

        bool IsValidEmail(string strIn)
        {
            return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        bool IsPhoneNumber(string number)
        {
            return Regex.IsMatch(number, "[ ^ 0-9]");
        }
    }

    public class Error
    {
        public string code
        {
            get;
            set;
        }

        public string message
        {
            get;
            set;
        }
    }

    public class OrderDetail
    {
        public List<Dictionary<string, object>> order
        {
            get;
            set;
        }

        public List<Dictionary<string, object>> deposit
        {
            get;
            set;
        }

        public List<Dictionary<string, object>> menuitems
        {
            get;
            set;
        }

        public List<Dictionary<string, object>> logisticitems
        {
            get;
            set;
        }
    }

    public class Recipt
    {
        public Order order
        {
            get;
            set;
        }

        public Deposit deposit
        {
            get;
            set;
        }

        public ImageData customer
        {
            get;
            set;
        }

        public ImageData reciver
        {
            get;
            set;
        }


    }

    public class Deposit : General
    {
        public int iOrderId
        {
            get;
            set;
        }

        public decimal dAmt
        {
            get;
            set;
        }

        public string vcType
        {
            get;
            set;
        }
    }

    public class PlaceOrder
    {
        public Order order
        {
            get;
            set;
        }

        public CustomerDetails customer
        {
            get;
            set;
        }

        public List<OrderMenuItem> menuitems
        {
            get;
            set;
        }

        public List<OrderLogisticItem> logisticitems
        {
            get;
            set;
        }
    }

    public class OrderMenuItem : General
    {
        public int iOrderId
        {
            get;
            set;
        }

        public int iItemId
        {
            get;
            set;
        }

        public int iQuantity
        {
            get;
            set;
        }

        public decimal dPrice
        {
            get;
            set;
        }

        public string itemName
        {
            get;
            set;
        }
    }

    public class OrderLogisticItem : General
    {
        public int iOrderId
        {
            get;
            set;
        }

        public int iLogisticId
        {
            get;
            set;
        }

        public int iQuantity
        {
            get;
            set;
        }

        public decimal dPrice
        {
            get;
            set;
        }

        public string itemName
        {
            get;
            set;
        }
    }

    public class Order : General
    {
        public string searchkey
        {
            get;
            set;
        }
        public string vcOrderId
        {
            get;
            set;
        }

        public string vcInvoiceId
        {
            get;
            set;
        }

        public int iPackageId
        {
            get;
            set;
        }

        public int iCustomerId
        {
            get;
            set;
        }

        public Boolean isQuatation
        {
            get;
            set;
        }

        public int iPack
        {
            get;
            set;
        }

        public Decimal iPackPerPrice
        {
            get;
            set;
        }

        public Decimal iFoodCost
        {
            get;
            set;
        }

        public Decimal iOthers
        {
            get;
            set;
        }

        public int iGST
        {
            get;
            set;
        }

        public string vcFuncName
        {
            get;
            set;
        }

        public string vcFuncAddress
        {
            get;
            set;
        }

        public string vcFuncLocation
        {
            get;
            set;
        }

        public string vcFunctionDate
        {
            get;
            set;
        }

        public string vcFunctionTime
        {
            get;
            set;
        }

        public string vcNote
        {
            get;
            set;
        }

        public Boolean isTax
        {
            get;
            set;
        }

        public Boolean iConfirm
        {
            get;
            set;
        }

        public int limit
        {
            get;
            set;
        }

        public int offset
        {
            get;
            set;
        }


    }

    public enum Gender
    {
        M, F
    }

    public class CustomerDetails : General
    {
        public string vcName
        {
            get;
            set;
        }

        public string iPhonenumber
        {
            get;
            set;
        }

        public string vcEmailid
        {
            get;
            set;
        }

    }
}


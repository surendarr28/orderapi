using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
using iTextSharp.text;
using iTextSharp.text.pdf;
using orderapis.Core;

namespace orderapis.Controllers
{
    public class MenuItemController : ApiController
    {
        [HttpPost]
        [ActionName("add")]
        public HttpResponseMessage insertMenuItem(MenuItem objItem)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, DbAccess.DbAInsert("tbl_menu_item", objItem));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [ActionName("package")]
        public HttpResponseMessage packageMenuItem(MenuItem objItem)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, DbAccess.DbASelect("select u.vcPackagename as packagename,"
                                                            + " u.dprice as packageprice, u.id as packageid, "
                                                            + " mi.vcName as itemname,"
                                                            + " mi.dPrice as itemprice,"
                                                            + " mi.id as itemid,"
                                                            + " pm.iQuantity as packagequantity,"
                                                            + " pm.iPrice as packageitemprice"
                                                            + " from (SELECT * FROM tbl_package) as u"
                                                            + " left join tbl_package_mapping  pm on pm.iPackageId = u.id"
                                                            + " left join tbl_menu_item mi on mi.id = pm.iItemId"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [ActionName("menuitem")]
        public HttpResponseMessage menuitem()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, DbAccess.DbASelect("select * from tbl_menu_item where deletedAt IS NULL"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [ActionName("logisticitem")]
        public HttpResponseMessage logisticitem()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, DbAccess.DbASelect("select * from tbl_logistic_items where deletedAt IS NULL"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [ActionName("uploadimage")]
        public HttpResponseMessage UploadImage(ImageData imageData)
        {
            try
            {
                var fileName = imageData.iEntityID + "_" + imageData.vcFileName + ".png";
                string fileNameWitPath = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/"), fileName);
                using (FileStream fs = new FileStream(fileNameWitPath, FileMode.Create))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        byte[] data = Convert.FromBase64String(imageData.imageData);
                        bw.Write(data);
                        bw.Close();
                        return Request.CreateResponse(HttpStatusCode.Created, fileName);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        [HttpPost]
        [ActionName("pdfgenerate")]
        public HttpResponseMessage pdfgenerate(ImageData imageData)
        {
            Rectangle pageSize = new Rectangle(PageSize.A4);
            Document doc = new Document(pageSize);
            try
            {
                var fileName = imageData.vcFileName + DateTime.Now.ToFileTime() + ".pdf";
                string fileNameWitPath = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/"), fileName);
                FileStream fs = new FileStream(fileNameWitPath, FileMode.Create, FileAccess.Write, FileShare.None);
                PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                doc.Open();
                doc.Add(new Paragraph("Order"));
                Paragraph paragraph = new Paragraph("Customer");
                string imageURL = HttpContext.Current.Server.MapPath("~/Uploads/") + "/1_signature.png";
                iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageURL);
                jpg.ScaleToFit(140f, 120f);
                jpg.SpacingBefore = 10f;
                jpg.SpacingAfter = 1f;
                jpg.Alignment = Element.ALIGN_LEFT;
                doc.Add(paragraph);
                doc.Add(jpg);
                return Request.CreateResponse(HttpStatusCode.Created, fileName);
            }
            catch
            {
                throw;
            }
            finally
            {

                doc.Close();
            }
        }
    }

    public class ImageData
    {
        public string imageData
        {
            get;
            set;
        }
        public int iEntityID
        {
            get;
            set;
        }

        public string vcFileName
        {
            get;
            set;
        }
    }

    public class MenuItem : General
    {
        public string vcName
        {
            get;
            set;
        }

        public decimal dPrice
        {
            get;
            set;
        }

    }


}

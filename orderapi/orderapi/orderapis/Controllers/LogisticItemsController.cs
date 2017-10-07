using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using orderapis.Core;

namespace orderapis.Controllers
{
    public class LogisticItemsController : ApiController
    {

        [HttpPost]
        [ActionName("add")]
        public HttpResponseMessage insertMenuItem(LogisticItems objItem)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, DbAccess.DbAInsert("tbl_logistic_items", objItem));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }



    public class LogisticItems : General
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

        public decimal dTax
        {
            get;
            set;
        }
    }
}






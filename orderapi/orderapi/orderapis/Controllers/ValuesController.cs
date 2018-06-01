using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using orderapis.Core;

namespace orderapis.Controllers
{
    public class ValuesController : ApiController
    {
        [HttpPost]
        [ActionName("add")]
        public HttpResponseMessage insertMenuItem(Dictionary<string, object> objItem)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, DbAccess.DbAInsertGeneral(objItem["table"].ToString(), objItem));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [ActionName("login")]
        public HttpResponseMessage login(Dictionary<string, object> objItem)
        {
            try
            {
                var userData = DbAccess.DbASelect("select * from tbl_user where vcUsername = '" + objItem["username"].ToString() + "'");

                if (userData.Count > 0)
                {
                    if (userData[0]["vcPassword"].ToString() == DbAccess.Encrypt(objItem["password"].ToString()).ToString())
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, DbAccess.Encrypt(objItem["username"].ToString()).ToString());
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.Unauthorized, "Password is wrong!");
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, "Username is wrong!");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [ActionName("selectdata")]
        public HttpResponseMessage commonselect(Query query)
        {
            try
            {
                var data = DbAccess.DbASelect(query.query);

             return Request.CreateResponse(HttpStatusCode.Accepted, data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        [ActionName("updateinsertdata")]
        public HttpResponseMessage commonupdateinsertselect(Query query)
        {
            try
            {
                var data = DbAccess.DbAUpdatecommon(query.query);

                return Request.CreateResponse(HttpStatusCode.Accepted, data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        [HttpPost]
        [ActionName("chcektoken")]
        public HttpResponseMessage checkLogin(Dictionary<string, object> objItem)
        {
            try
            {
                if (DbAccess.Decrypt(objItem["token"].ToString()).ToString() != objItem["token"].ToString())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, true);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, false);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, false);
            }
        }
    }
}
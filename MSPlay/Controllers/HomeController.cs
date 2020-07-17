using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using MSPlay.Models;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Claims;
using QRCoder;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace MSPlay.Controllers
{
    public class HomeController : Controller
    {
        //[CustomAuthFilter]
        public ActionResult Index()
        {
            SignIn();
            //var userClaims = User.Identity as System.Security.Claims.ClaimsIdentity;

            ////You get the user’s first and last name below:
            //Session["Name"] = userClaims?.FindFirst("name")?.Value;
            return View();
        }
        /// <summary>
        /// Send an OpenID Connect sign-in request.
        /// Alternatively, you can just decorate the SignIn method with the [Authorize] attribute
        /// </summary>
        public void SignIn()
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties { RedirectUri = "/" },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
            //var userClaims = User.Identity as System.Security.Claims.ClaimsIdentity;

            ////You get the user’s first and last name below:
            //Session["Name"] = System.Security.Claims.ClaimsPrincipal.Current.FindFirst("name").Value;
            //return View("Index");
        }

        /// <summary>
        /// Send an OpenID Connect sign-out request.
        /// </summary>
        public void SignOut()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(
                    OpenIdConnectAuthenticationDefaults.AuthenticationType,
                    CookieAuthenticationDefaults.AuthenticationType);
        }
        [CustomAuthFilter]
        public ActionResult About()
        {
            ViewBag.Message = "MS Play description.";
            return View();
        }
        [CustomAuthFilter]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
                return View();
        }
        [CustomAuthFilter]
        public ActionResult MyBooking()
        {
            //GetQRCode("rfr");
            Booking objuser = new Booking();
            DataSet ds = new DataSet();
            string name = ClaimsPrincipal.Current.FindFirst("preferred_username").Value;
            name = name.Remove(name.Length - 14, 14);

            string connStr = ConfigurationManager.ConnectionStrings["myConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("getBookingList", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@gameID", "All");
                    cmd.Parameters.AddWithValue("@name", name);
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    List<Booking> userlist = new List<Booking>();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Booking uobj = new Booking();
                        uobj.SlNo = Convert.ToInt32(ds.Tables[0].Rows[i]["Sl No."].ToString());
                        uobj.VenueID = ds.Tables[0].Rows[i]["VenueID"].ToString();
                        uobj.InTime = Convert.ToDateTime(ds.Tables[0].Rows[i]["InTime"].ToString());
                        uobj.OutTime = Convert.ToDateTime(ds.Tables[0].Rows[i]["OutTime"].ToString());
                        uobj.EmployeeAlias = ds.Tables[0].Rows[i]["EmployeeAlias"].ToString();
                        uobj.EmployeeName = ds.Tables[0].Rows[i]["EmployeeName"].ToString();
                        userlist.Add(uobj);
                    }
                    objuser.GetBookingList = userlist;
                }
                    con.Close();
            }
            return View(objuser.GetBookingList);
        }

        public ActionResult GetQRCode(string encode)
        {
            byte[] imagebyte;
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(encode, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            using(MemoryStream stream = new MemoryStream())
            {
                qrCodeImage.Save(stream, ImageFormat.Png);
                imagebyte = stream.ToArray();
            }

            var baseimage = Convert.ToBase64String(imagebyte);
            ViewBag.image = baseimage;
            //< img src = "@String.Format("data: image / png; base64,{ 0}",ViewBag.Image)" />
            return PartialView();
        }
    }
}
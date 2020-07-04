using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MSPlay.Models;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Security.Claims;

namespace MSPlay.Controllers
{
    public class GamesController : Controller
    {
        [CustomAuthFilter]
        public ActionResult getData(string gameID)
        {
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
                    cmd.Parameters.AddWithValue("@gameID", gameID);
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
            return PartialView("~/Views/Games/_partialGames.cshtml", objuser.GetBookingList);
        }
        [CustomAuthFilter]
        public ActionResult GoToGame(string game)
        {
            ViewBag.Game = game;
            return View("~/Views/Games/GoToGame.cshtml");
        }
        public ActionResult GetModalData(string category)
        {
            VenueDetails objVenue = new VenueDetails();
            DataSet ds = new DataSet();
            string connStr = ConfigurationManager.ConnectionStrings["myConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("getModalData", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@category", category);
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    List<VenueDetails> userlist = new List<VenueDetails>();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        VenueDetails uobj = new VenueDetails();
                        uobj.ID = Convert.ToInt32(ds.Tables[0].Rows[i]["ID"].ToString());
                        uobj.VenueName = ds.Tables[0].Rows[i]["VenueDetails"].ToString();
                        uobj.VenueID = ds.Tables[0].Rows[i]["VenueID"].ToString();
                        uobj.Category = ds.Tables[0].Rows[i]["Category"].ToString();
                        userlist.Add(uobj);
                    }
                    objVenue.GetVenueList = userlist;
                }
                con.Close();
                SelectList Place = new SelectList(objVenue.GetVenueList, "ID", "VenueDetails");
                ViewBag.VenueList = Place;
            }
            var userClaims = User.Identity as System.Security.Claims.ClaimsIdentity;
            string alias;
            //You get the user’s first and last name below:
            ViewBag.Name = userClaims?.FindFirst("name")?.Value;

            // The 'preferred_username' claim can be used for showing the username
            //
            alias = userClaims?.FindFirst("preferred_username")?.Value;
            ViewBag.Username = alias.Remove(alias.Length - 14, 14);
            return PartialView("~/Views/Games/_getModalData.cshtml", objVenue.GetVenueList);
        }
        private bool CheckExistingBooking(string VenueID, DateTime inTime, DateTime outTime)
        {
            bool found = false;
            string connStr = ConfigurationManager.ConnectionStrings["myConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand("CheckExisting", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@VenueID", VenueID);
                    cmd.Parameters.AddWithValue("@inTime", inTime);
                    cmd.Parameters.AddWithValue("@outTime", outTime);
                    con.Open();
                    var reader = System.Convert.ToInt32(cmd.ExecuteScalar());
                    //check if the time slot already exists.
                    if (reader > 0)
                        found = true;
                    else
                        found = false;
                }
                con.Close();
            }
            return found;
        }
        public string AddBooking(string VenueID, string inTime, string outTime, string Alias, string ID)
        {

            DateTime inDateTime = DateTime.Parse(inTime);
            DateTime outDateTime = DateTime.Parse(outTime);
            if (inDateTime < DateTime.Now)
                inDateTime = inDateTime.AddDays(1);
            if (outDateTime < DateTime.Now)
                outDateTime = outDateTime.AddDays(1);
            var recordFound = CheckExistingBooking(VenueID, inDateTime, outDateTime);
            //TimeSpan inSpan = TimeSpan.ParseExact((inTime);
            if (recordFound == false)
            {
                string connStr = ConfigurationManager.ConnectionStrings["myConnection"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    using (SqlCommand cmd = new SqlCommand("AddBooking", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@VenueID", VenueID);
                        cmd.Parameters.AddWithValue("@inTime", inDateTime);
                        cmd.Parameters.AddWithValue("@outTime", outDateTime);
                        cmd.Parameters.AddWithValue("@Alias", Alias);
                        cmd.Parameters.AddWithValue("@ID", ID);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }
                return "entry added";
            }
            else
                return "restricted";
        }
        public ActionResult Delete(string ID, string game)
        {
            string connStr = ConfigurationManager.ConnectionStrings["myConnection"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                string query = "Delete from tbl_PlayBookings where [Sl No.]=@id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", ID);
                cmd.ExecuteNonQuery();
            }
            //ViewBag.Game = game;
            return getData(game);
        }
    }
}
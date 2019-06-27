using SHP.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.DirectoryServices;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SHP.Controllers
{
    public class HomeController : Controller
    {
        private SHPContext db = new SHPContext();

        public ActionResult Index()
        {
            string UserLog = User.Identity.Name;
            string[] words = UserLog.Split('\\');
            string UserId = words[1];

            //string adPath = "LDAP://atg.root"; // + Session["dominio"];
            //LDAPAutenticadorController aut = new LDAPAutenticadorController(adPath);
            LDAPAutenticadorController aut = new LDAPAutenticadorController();
            ArrayList propUsers = new ArrayList();
            var data = new User();

            propUsers = aut.GetInfoUser(words[1]);
            //ddlUsuarios.Items.Clear();
            //ddlUsuarios.Items.Add(new ListItem(propUsers[0] as string));

            var userTemp = db.User.Where(u => u.UserId == UserId).FirstOrDefault();

            if (propUsers.Count > 1)
            {
                data.UserId = UserId;
                data.UserName = propUsers[0] as string;
                data.UserOffice = propUsers[1] as string;
                data.UserMail = propUsers[2] as string;
                data.UserCountry = propUsers[3] as string;
                data.UserCity = propUsers[4] as string;
                data.UserTitle = propUsers[5] as string;
                data.AnswerList = db.Answer.Include(q => q.Question).Where(a => a.UserId == UserId).ToList();

                if (userTemp == null)
                {
                    createUser(data);
                }
            }
            else
            if (userTemp != null)
            {
                data.UserId = UserId;
                data.UserName = userTemp.UserName;
                data.UserOffice = userTemp.UserOffice;
                data.UserMail = userTemp.UserMail;
                data.UserCountry = userTemp.UserCountry;
                data.UserCity = userTemp.UserCity;
                data.UserTitle = userTemp.UserTitle;
                data.AnswerList = db.Answer.Include(q => q.Question).Where(a => a.UserId == UserId).ToList();
            }
            var status = propUsers[6] as string;
            var isornot = aut.isUserLocked;
            if (decimal.Parse(status) > 0 || isornot)
            {
                ViewBag.Status = true;
            }

            //bool FlagAccountLocked = aut.isUserLocked;

            //if (FlagAccountLocked)
            //{
            //    ViewBag.Status = true;
            //}

            return PartialView("index", data);
        }

        public ActionResult GetUser()
        {
            string UserLog = User.Identity.Name;
            string[] words = UserLog.Split('\\');
            string UserId = words[1];

            //string adPath = "LDAP://atg.root"; // + Session["dominio"];
            //LDAPAutenticadorController aut = new LDAPAutenticadorController(adPath);
            LDAPAutenticadorController aut = new LDAPAutenticadorController();
            ArrayList propUsers = new ArrayList();
            var data = new User();

            propUsers = aut.GetInfoUser(words[1]);
            //ddlUsuarios.Items.Clear();
            //ddlUsuarios.Items.Add(new ListItem(propUsers[0] as string));

            var userTemp = db.User.Where(u => u.UserId == UserId).FirstOrDefault();

            if (propUsers.Count > 1)
            {
                data.UserId = UserId;
                data.UserName = propUsers[0] as string;
                data.UserOffice = propUsers[1] as string;
                data.UserMail = propUsers[2] as string;
                data.UserCountry = propUsers[3] as string;
                data.UserCity = propUsers[4] as string;
                data.UserTitle = propUsers[5] as string;
                data.AnswerList = db.Answer.Include(q => q.Question).Where(a => a.UserId == UserId).ToList();

                if (userTemp == null)
                {
                    createUser(data);
                }
            }
            else
            if (userTemp != null)
            {
                data.UserId = UserId;
                data.UserName = userTemp.UserName;
                data.UserOffice = userTemp.UserOffice;
                data.UserMail = userTemp.UserMail;
                data.UserCountry = userTemp.UserCountry;
                data.UserCity = userTemp.UserCity;
                data.UserTitle = userTemp.UserTitle;
                data.AnswerList = db.Answer.Include(q => q.Question).Where(a => a.UserId == UserId).ToList();
            }
            var status = propUsers[6] as string;
            var isornot = aut.isUserLocked;
            if (decimal.Parse(status) > 0 || isornot)
            {
                ViewBag.Status = true;
            }

            //bool FlagAccountLocked = aut.isUserLocked;

            //if (FlagAccountLocked)
            //{
            //    ViewBag.Status = true;
            //}

            return PartialView("index", data);
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult login(string UserId, string UserPassword)
        {
            LDAPAutenticadorController aut = new LDAPAutenticadorController();
            ArrayList gruposDe = new ArrayList();
            ArrayList propUsers = new ArrayList();

            try
            {
                if (aut.autenticado((string)Session["dominio"], UserId, UserPassword) == true)
                {
                    // lblUsuario.Text = aut.getCN();

                    propUsers = aut.getListaPropiedades();
                    //ddlUsuarios.Items.Clear();
                    //ddlUsuarios.Items.Add(new ListItem(propUsers[0] as string));

                    if (propUsers.Count > 1)
                    {
                        ViewBag.UserName = propUsers[0] as string;
                        ViewBag.UserFirstName = propUsers[1] as string;
                        ViewBag.UserLastName = propUsers[2] as string;

                        return RedirectToAction("Index");

                    }

                    //llenarGrupos(lblUsuario.Text);

                    //gruposDe = aut.GetGroups();
                    //listaGrupos.Items.Clear();
                    //for (int i = 0; i < gruposDe.Count; i++)
                    //{
                    //    listaGrupos.Items.Add(new ListItem(gruposDe[i] as string));
                    //    if (gruposDe[i] as string == "Administrators")
                    //    {
                    //        Response.Write("Bienvenido administrador");
                    //    }
                    //}
                }
            }

            catch (Exception e)
            {
                @ViewBag.Error = "Error al ingresar, por favor comunicarse con el administrador " + e.ToString();
            }

            return View();

        }

        public ActionResult LogOut()
        {
            HttpCookie cookie = Request.Cookies["TSWA-Last-User"];

            if (User.Identity.IsAuthenticated == false || cookie == null || StringComparer.OrdinalIgnoreCase.Equals(User.Identity.Name, cookie.Value))
            {
                string name = string.Empty;

                if (Request.IsAuthenticated)
                {
                    name = User.Identity.Name;
                }

                cookie = new HttpCookie("TSWA-Last-User", name);
                Response.Cookies.Set(cookie);

                Response.AppendHeader("Connection", "close");
                Response.StatusCode = 0x191;
                Response.Clear();
                //should probably do a redirect here to the unauthorized/failed login page
                //if you know how to do this, please tap it on the comments below
                Response.Write("Unauthorized. Reload the page to try again...");
                Response.End();

                return RedirectToAction("Index");
            }

            cookie = new HttpCookie("TSWA-Last-User", string.Empty)
            {
                Expires = DateTime.Now.AddYears(-5)
            };

            Response.Cookies.Set(cookie);

            return RedirectToAction("Index");

        }

        private void createUser(User user)
        {
            db.User.Add(user);
            db.SaveChanges();
        }

    }
}
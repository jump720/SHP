using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Collections;
using System.Web.Security;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;
using System.DirectoryServices;
using System.Reflection;

namespace SHP.Controllers
{
    public class LDAPAutenticadorController : Controller
    {
        public string _path = "LDAP://atg.root";
        private string _filterAttribute;
        public string info;
        public const string DCUserName = "ATG\\ATG-Script"; //Admin Account for query
        public const string DCPassWord = "$cr1pt.@tg";
        public bool isUserLocked = false;
        private ArrayList listaPropiedades = new ArrayList();

        //public LDAPAutenticadorController(string path)
        //{
        //    _path = path;
        //}

        public ArrayList getListaPropiedades()
        {
            return listaPropiedades;
        }

        public string getCN()
        {
            return _filterAttribute;
        }

        public string getInfo()
        {
            return info;
        }
        public bool autenticado(string dominio, string usuario, string pass)
        {
            bool autenticado = true;

            string acceso = dominio + @"\" + usuario;
            DirectoryEntry entry = new DirectoryEntry(_path, usuario, pass);
            entry.AuthenticationType = AuthenticationTypes.Secure;

            try
            {
                object obj = entry.NativeObject;
                DirectorySearcher search = new DirectorySearcher(entry);

                search.Filter = "(SAMAccountName=" + usuario + ")";
                string[] requiredProperties = new string[] { "cn", "givenname", "sn" };
                foreach (String property in requiredProperties)
                    search.PropertiesToLoad.Add(property);

                SearchResult result = search.FindOne();

                if (null == result)
                {
                    autenticado = false;
                }
                else
                {

                    foreach (String property in requiredProperties)
                        foreach (Object myCollection in result.Properties[property])
                            listaPropiedades.Add(myCollection.ToString());
                }

                //Update the new path to the user in the directory.
                _path = result.Path;
                _filterAttribute = (string)result.Properties["cn"][0];
            }
            catch (Exception ex)
            {
                throw new Exception("Error de autenticación. " + ex.Message);
            }

            return autenticado;
        }

        public ArrayList GetInfoUser(string usuario)
        {
            ArrayList infoUser = new ArrayList();
            try
            {
                DirectoryEntry entry = new DirectoryEntry(_path, DCUserName, DCPassWord);
                //entry.Path = _path;
                DirectorySearcher search = new DirectorySearcher(entry);
                search.Filter = "(SAMAccountName=" + usuario + ")";
                //pwdAccountLockedTime
                //physicalDeliveryOfficeName
                //pwdFailureTime
                string[] requiredProperties = new string[] { "cn", "physicalDeliveryOfficeName", "mail", "co", "l", "title", "lockoutTime" };
                // string[] requiredProperties2 = new string[] { "lockoutTime" };
                foreach (String property in requiredProperties)
                    search.PropertiesToLoad.Add(property);
                SearchResult result = search.FindOne();
                if (result != null)
                {
                    foreach (String property in requiredProperties)
                        foreach (Object myCollection in result.Properties[property])
                            infoUser.Add(myCollection.ToString());
                    //infoUser.Add(result.Properties("lockoutTime").Value);
                }
                //Update the new path to the user in the directory.
                _path = result.Path;

                //isUserLocked = IsLocked(_path, DCUserName, DCPassWord);

                _filterAttribute = (string)result.Properties["cn"][0];
            }
            catch (Exception ex)
            {
                infoUser.Add("Usuario Bloqueado");
            }

            return infoUser;
        }

        //public static bool IsLocked(string userDn, string adminuser, string adminpass)
        //{
        //    try
        //    {
        //        DirectoryEntry user = new DirectoryEntry(userDn, adminuser, adminpass);
        //        string attribName = "msDS-User-Account-Control-Computed";
        //        user.RefreshCache(new string[] { attribName });
        //        const int UF_LOCKOUT = 0x0010;
        //        int userFlags = (int)user.Properties[attribName].Value;
        //        if ((userFlags & UF_LOCKOUT) == UF_LOCKOUT)
        //        {
        //            // if this is the case, the account is locked out
        //            return true;
        //        }
        //        return false;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //public bool IsLocked(string userDn, string adminuser, string adminpass)
        //{
        //    DirectoryEntry user = new DirectoryEntry(userDn, adminuser, adminpass);
        //    //if they have a lockoutTime
        //    //if (user.Properties.Contains("lockoutTime"))
        //    //{
        //    //    string attribName = "msDS-User-Account-Control-Computed";
        //    //    user.RefreshCache(new string[] { attribName });
        //    //    var fileTicks = user.Properties["lockoutTime"].Value;

        //    //    //check to see if it's not already unlocked
        //    //    //if (fileTicks != 0)
        //    //    //{
        //    //    //    //now check to see if it was automatically unlocked
        //    //    //    DateTime lockoutTime = DateTime.FromFileTime(fileTicks);

        //    //    //    DirectoryEntry parent = user.Parent;
        //    //    //    while (parent.SchemaClassName != "domainDNS")
        //    //    //        parent = parent.Parent;

        //    //    //    long durationTicks = LongFromLargeInteger(parent.Properties["lockoutDuration"].Value);

        //    //    //    return (DateTime.Now.CompareTo(lockoutTime.AddTicks(-durationTicks)) < 0);
        //    //    //}
        //    //}

        //    if (user.Properties.Contains("lockoutTime")) //could be locked out, check time 

        //    {
        //        string attribName = "lockoutTime";
        //        user.RefreshCache(new string[] { attribName });
        //        long ldate = LongFromLargeInteger(user.Properties["lockoutTime"][0]);
        //        if (ldate != 0)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        //decodes IADsLargeInteger objects into a FileTime format (long)
        //private long LongFromLargeInteger(object largeInteger)
        //{
        //    System.Type type = largeInteger.GetType();
        //    int highPart = (int)type.InvokeMember("HighPart", BindingFlags.GetProperty, null, largeInteger, null);
        //    int lowPart = (int)type.InvokeMember("LowPart", BindingFlags.GetProperty, null, largeInteger, null);

        //    return (long)highPart << 32 | (uint)lowPart;
        //}

        public ArrayList GetGroups(string cn)
        {
            DirectorySearcher search = new DirectorySearcher(_path);
            search.Filter = "(cn=" + cn + ")";
            search.PropertiesToLoad.Add("memberOf");
            ArrayList grupos = new ArrayList();

            try
            {
                SearchResult result = search.FindOne();
                int propertyCount = result.Properties["memberOf"].Count;
                string dn;
                int equalsIndex, commaIndex;

                for (int propertyCounter = 0; propertyCounter < propertyCount; propertyCounter++)
                {
                    dn = (string)result.Properties["memberOf"][propertyCounter];
                    equalsIndex = dn.IndexOf("=", 1);
                    commaIndex = dn.IndexOf(",", 1);
                    if (-1 == equalsIndex)
                    {
                        return null;
                    }
                    grupos.Add(dn.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error obtaining group names. " + ex.Message);
            }
            return grupos;
        }

        public ArrayList getTodosUsuarios()
        {
            ArrayList cnUsuarios = new ArrayList();
            try
            {
                DirectoryEntry entry = new DirectoryEntry();
                entry.Path = _path;
                DirectorySearcher search = new DirectorySearcher(entry);
                search.PropertiesToLoad.Add("cn");


                SearchResultCollection allUsers = search.FindAll();

                foreach (SearchResult result in allUsers)
                {
                    if (result.Properties["cn"].Count > 0)
                    {
                        cnUsuarios.Add(String.Format("{0,-20} : {1}", result.Properties["cn"][0].ToString()));
                    }
                }

            }
            catch (Exception exc)
            {
                cnUsuarios.Add("Error: " + exc.ToString());
            }

            return cnUsuarios;
        }
        //
        public JsonResult changePassword(string userDn, string oldPass, string newPass)
        {
            string result = "";
            try
            {
                DirectoryEntry entry = new DirectoryEntry();
                entry.Path = _path;
                DirectorySearcher search = new DirectorySearcher(entry);
                search.Filter = "(SAMAccountName=" + userDn + ")";
                search.PropertiesToLoad.Add("password");
                SearchResult resultS = search.FindOne();

                if (resultS != null)
                {
                    // create new object from search result  
                    DirectoryEntry entryToUpdate = resultS.GetDirectoryEntry();
                    entryToUpdate.Invoke("ChangePassword", new object[] { oldPass, newPass });
                    entryToUpdate.CommitChanges();
                    result = "The password was changed!";
                }
                else
                {
                    result = "Error changing password!";
                }
            }
            catch (Exception ex)
            {
                result = "Error changing password!";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UnlockUser(string userDn)
        {

            string result = "";
            try
            {
                DirectoryEntry uEntry = new DirectoryEntry(_path, DCUserName, DCPassWord);

                DirectorySearcher search = new DirectorySearcher(uEntry);
                search.Filter = "(SAMAccountName=" + userDn + ")";

                SearchResult resultS = search.FindOne();

                _path = resultS.Path;

                uEntry.Path = _path;               
                uEntry.Properties["LockOutTime"].Value = 0; //unlock account
                uEntry.CommitChanges(); //may not be needed but adding it anyways
                uEntry.Close();

                result = "The user was unlocked!";
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException ex)
            {
                result = "The user could not be unlocked!";

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult resetPassword(string userDn, string newPassword)
        {

            string result = "";
            try
            {
                DirectoryEntry uEntry = new DirectoryEntry(_path, DCUserName, DCPassWord);

                DirectorySearcher search = new DirectorySearcher(uEntry);
                search.Filter = "(SAMAccountName=" + userDn + ")";

                SearchResult resultS = search.FindOne();

                _path = resultS.Path;

                uEntry.Path = _path;
                //uEntry.UsePropertyCache = false;
                uEntry.Properties["LockOutTime"].Value = 0; //unlock account
                uEntry.Invoke("SetPassword", new object[] { newPassword }); //reset password
                uEntry.CommitChanges(); //may not be needed but adding it anyways
                uEntry.Close();

                result = "Your password was reset!";
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException ex)
            {
                result = "Your password could not be reset!";

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
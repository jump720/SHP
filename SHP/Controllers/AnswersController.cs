using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SHP.Models;
using System.Threading.Tasks;

namespace SHP.Controllers
{
    public class AnswersController : Controller
    {
        private SHPContext db = new SHPContext();

        // GET: Answers
        public ActionResult Index()
        {
            var answer = db.Answer.Include(a => a.Question).Include(a => a.User);
            return View(answer.ToList());
        }

        // GET: Answers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answer answer = db.Answer.Find(id);
            if (answer == null)
            {
                return HttpNotFound();
            }
            return View(answer);
        }

        // GET: Answers/Create
        public ActionResult Create()
        {
            string UserLog = User.Identity.Name;
            string[] words = UserLog.Split('\\');
            string UserId = words[1];
            ViewBag.QuestionId = new SelectList(db.Question, "QuestionId", "QuestionDesc");
            ViewBag.UserId = UserId;
            return View();
        }

        // POST: Answers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "QuestionId,UserId,AnswerDesc")] Answer answer)
        {
            if (ModelState.IsValid)
            {
                db.Answer.Add(answer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.QuestionId = new SelectList(db.Question, "QuestionId", "QuestionDesc", answer.QuestionId);
            ViewBag.UserId = new SelectList(db.User, "UserId", "UserName", answer.UserId);
            return View(answer);
        }

        // GET: Answers/Create
        public ActionResult SetUser()
        {
            return PartialView("_SetUser");
        }

        [HttpPost]
        public ActionResult SetUser(Answer answer)
        {
            try
            {
                var userTemp = db.Answer.Include(a => a.Question).Where(a => a.UserId == answer.UserId).ToList();

                if (userTemp.Count >= 1)
                {
                    return PartialView("_SecurityQuestion", userTemp);
                }
                else
                {
                    ViewBag.Javascript = "User Not Found!";
                    return RedirectToAction("../Home/Index");

                    //return JavaScript("<script language='javascript' type='text/javascript'>alert('User Not Found!');</script>");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error de Usuario. " + ex.Message);
            }
        }

        [HttpPost]
        public ActionResult SecurityQuestions(FormCollection form = null)
        {
            try
            {
                int ln_rows = int.Parse(form["UltimoItem"].ToString());
                var userIdTemp = form["UserId"].ToString();
                var anwersTemp = db.Answer.Where(a => a.UserId == userIdTemp).ToList();
                int validAnswers = 0;


                if (anwersTemp != null)
                {
                    for (int row = 1; row <= ln_rows; row++)
                    {
                        validAnswers = 0;
                        foreach (var answers in anwersTemp)
                        {
                            if (form["QuestionId_"+row].ToString() == answers.QuestionId.ToString() && form["AnswerDesc_" + row].ToString() == answers.AnswerDesc.ToString())
                            {
                                validAnswers = validAnswers + 1;
                                break;
                            }
                        }
                        if (validAnswers == 0)
                        {
                            string a = "alert('invalid answers!')";
                            //return JavaScript("<script language='javascript' type='text/javascript'>alert('invalid answers!');</script>");
                            return Content("<script language='javascript' type='text/javascript'>alert('invalid answers!');</script>");
                        }
                    }

                    return PartialView("_ResetPassword", anwersTemp);
                }
                else
                {
                    return View("Index");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error de Usuario. " + ex.Message);
            }
        }

        // GET: Answers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answer answer = db.Answer.Find(id);
            if (answer == null)
            {
                return HttpNotFound();
            }
            ViewBag.QuestionId = new SelectList(db.Question, "QuestionId", "QuestionDesc", answer.QuestionId);
            ViewBag.UserId = new SelectList(db.User, "UserId", "UserName", answer.UserId);
            return View(answer);
        }

        // POST: Answers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "QuestionId,UserId,AnswerDesc")] Answer answer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(answer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.QuestionId = new SelectList(db.Question, "QuestionId", "QuestionDesc", answer.QuestionId);
            ViewBag.UserId = new SelectList(db.User, "UserId", "UserName", answer.UserId);
            return View(answer);
        }

        // GET: Answers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answer answer = db.Answer.Find(id);
            if (answer == null)
            {
                return HttpNotFound();
            }
            return View(answer);
        }

        // POST: Answers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Answer answer = db.Answer.Find(id);
            db.Answer.Remove(answer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

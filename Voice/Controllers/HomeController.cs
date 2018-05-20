using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using System.Threading;
using Voice.Models;
using System.Web.Security;
using System.IO;
using System.Data.Entity;


namespace Voice.Controllers
{
    public static class FileHelp
    {
        public static byte[] GetBytes(string path)
        {
            return File.ReadAllBytes(path);
        }
    }
    public class HomeController : Controller
    {
        public ActionResult ShowVisits()
        {
            return View();
        }
        [Authorize]
        public ActionResult Index()
        {
            HttpCookie cookie = new HttpCookie("FileSave");
            if (User.Identity.IsAuthenticated == false)
            {
                cookie["Username"] = "Guest";
                Response.Cookies.Add(cookie);
            }
            else
            {
                cookie["Username"] = User.Identity.Name;
                Response.Cookies.Add(cookie);
            }
            return RedirectToAction("Information");
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
        public ActionResult Triangle()
        {
            return View();
        }
        public ActionResult Longer()
        {
            return View();
        }
        [HttpGet]
        public ActionResult HarmonicAnalysis()
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                if (db.Visits.FirstOrDefault(v => v.Patient.Login == User.Identity.Name) != null)
                {
                    ViewBag.RecordExist = "True";
                }
                else
                {
                    ViewBag.RecordExist = "False";
                }
            }

            return View();
        }

        [Authorize]
        [HttpGet]
        public void AddVisit()
        {
            List<WavFile> files = new List<WavFile>();
            var RequestQuery = Request.QueryString;
            for (int i = 0; i < RequestQuery[1].Split(',').Length; i++)
                files.Add(new WavFile
                {
                    Name = RequestQuery[1].Split(',')[i],
                    Jitter = double.Parse(RequestQuery[3].Split(',')[i].Replace('.', ',')),
                    Shimmer = double.Parse(RequestQuery[2].Split(',')[i].Replace('.', ',')),
                    HNR = double.Parse(RequestQuery[4].Split(',')[i].Replace('.', ',')),
                    Intensity = double.Parse(RequestQuery[5].Split(',')[i].Replace('.', ','))
                });

            Visit visit = new Visit();
            using (DatabaseContext db = new DatabaseContext())
            {
                var patient = db.Patients.FirstOrDefault(p => p.Login == User.Identity.Name);
                if (RequestQuery[0].Split(',')[1].Split('_')[0] != "undefined")
                    if (bool.Parse(RequestQuery[0].Split(',')[1].Split('_')[0]))
                    {
                        db.Visits.Add(new Visit { Patient = patient, DateTime = DateTime.Now });
                        db.WavFiles.AddRange(files);
                    }
                if (RequestQuery[0].Split(',')[1].Split('_')[1] != "undefined")
                    if (bool.Parse(RequestQuery[0].Split(',')[1].Split('_')[1]))
                    {
                        visit = db.Visits
                            .OrderByDescending(v => v.DateTime)
                            .FirstOrDefault();
                        foreach (var file in files)
                        {
                            file.VisitId = visit.Id;
                            visit.Files.Add(file);
                        }
                        db.WavFiles.AddRange(files);
                    }
                db.SaveChanges();
            }
        }

        [Authorize]
        [HttpGet]
        public void DeleteAudio()
        {
            int visitId = int.Parse(Request.QueryString[1]);
            string name = Request.QueryString[0];
            using (DatabaseContext db = new DatabaseContext())
            {
                var file = db.WavFiles.Where(f => f.Name == name && f.VisitId == visitId);
                db.WavFiles.RemoveRange(file);
                db.SaveChanges();
            }
        }

        public ActionResult Upl()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult Information()
        {
            Patient patient = null;
            using (DatabaseContext db = new DatabaseContext())
            {
                patient = db.Patients.FirstOrDefault(p => p.Login == User.Identity.Name);
            }
            if (patient != null)
            {
                ViewBag.Text = "Изменить информацию о себе";
                ViewBag.Button = "Сохранить изменения";
                ViewBag.Name = patient.Name;
                ViewBag.Surname = patient.Surname;
                ViewBag.Age = patient.Age;
                ViewBag.Sex = patient.Sex;
            }
            else
            {
                ViewBag.Text = "Добавить информацию о себе";
                ViewBag.Button = "Добавить информацию";
                ViewBag.Name = "";
                ViewBag.Surname = "";
                ViewBag.Age = "";
                ViewBag.Sex = "";
            }
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Information(InformationModel model)
        {
            if (ModelState.IsValid)
            {
                // поиск пользователя в бд
                Patient patient = null;
                User user = null;
                using (UserContext uc = new UserContext())
                {
                    user = uc.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
                }
                if (user != null)
                {
                    using (DatabaseContext db = new DatabaseContext())
                    {
                        patient = db.Patients.FirstOrDefault(p => p.Login == User.Identity.Name);
                        if (patient == null)
                        {
                            db.Patients.Add(new Patient { Login = user.Login, Name = model.Name, Age = model.Age, Sex = model.Sex, Surname = model.Surname });
                        }
                        else
                        {
                            //db.Patients.Remove(patient);
                            //db.Patients.Add(new Patient { Login = user.Login, Name = model.Name, Age = model.Age, Sex = model.Sex, Surname = model.Surname });
                            patient.Age = model.Age;
                            patient.Name = model.Name;
                            patient.Surname = model.Surname;
                            patient.Sex = model.Sex;
                        }
                        db.SaveChanges();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Пользователя с таким логином и паролем нет");
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult Upload()
        {
            string fileName = "";
            string PraatPath = Server.MapPath("~/Files");
            string ScriptJitter = "Jitter_Script";
            string ScriptSpectrum = "Get_Spectrum";
            string[] resultJit = new string[] { };
            string[] resultSpec = new string[] { };
            foreach (string file in Request.Files)
            {
                var upload = Request.Files[file];
                if (upload != null)
                {
                    // получаем имя файла
                    fileName = System.IO.Path.GetFileName(upload.FileName);
                    // сохраняем файл в папку Files в проекте
                    upload.SaveAs(Server.MapPath("~/Files/" + fileName));
                    ProcessStartInfo p1 = new ProcessStartInfo();
                    p1.FileName = "cmd";
                    p1.Arguments = @"/c cd " + PraatPath + " & praat.exe --run " + ScriptJitter + " " + fileName.Split('.')[0];
                    Process.Start(p1);
                    ProcessStartInfo p2 = new ProcessStartInfo();
                    p2.FileName = "cmd";
                    p2.Arguments = @"/c cd " + PraatPath + " & praat.exe --run " + ScriptSpectrum + " " + fileName.Split('.')[0];
                    Process.Start(p2);
                    Thread.Sleep(3000);
                    resultJit = System.IO.File.ReadAllLines(Server.MapPath("~/Files/praat_output.txt"));
                    resultSpec = System.IO.File.ReadAllLines(Server.MapPath("~/Files/Spectrum.txt"));
                }
            }
            return Json(resultJit/*.Concat(resultSpec)*/);
        }

        [HttpGet]
        public string GetAudio()
        {
            string FileName = Request.QueryString["file"];
            byte[] Filebytes = FileHelp.GetBytes(Server.MapPath("~/Files/" + FileName));
            string test = "[" + string.Join(",", Filebytes.Select(b => b.ToString())) + "]";
            return Convert.ToBase64String(Filebytes);
        }

        public ActionResult PlayTest()
        {
            return View();
        }
    }
}
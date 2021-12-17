using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using leet.Models;
using System.Web.Mvc;
using System.Net;
using System.IO;
using System.Net.Http;
using RestSharp;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace leet.Controllers
{
    public class HomeController : Controller
    {   
        TranslateEntities db = new TranslateEntities();

        // GET: Home
        public ActionResult Index()
        {
            Types types = new Types();
            types.TypeNames = (from k in db.Type
                               select new SelectListItem
                               {
                                   Text = k.TypeName,
                                   Value = k.ID.ToString()
                               }).ToList();
            return View(types);
        }
        [HttpPost]
        public ActionResult Index(string username, string text, Types selectedType)
        {
            Types types = new Types();

            types.TypeNames = (from k in db.Type
                               select new SelectListItem
                               {
                                   Text = k.TypeName,
                                   Value = k.ID.ToString()
                               }).ToList();

            User user = new User();
            user.UserName = username;
            var check = db.User.Where(x => x.UserName == username).FirstOrDefault();
            if (check == null)
            {
                db.User.Add(user);
                db.SaveChanges();

            }
            var userId = db.User.Where(x => x.UserName == username).FirstOrDefault();

            Text t = new Text();
            t.Text1 = text;
            t.TypeID = Convert.ToInt32(selectedType.SelectedItemId);
            t.UserID = userId.ID;
            if(t.TypeID == 1)
            {
                t.Translated = TranslateLeet(text);
            }else if(t.TypeID == 2)
            {
                t.Translated = TranslateYoda(text);
            }
            else
            {
                t.Translated = "Still in development";
            }
            
            db.Text.Add(t);
            db.SaveChanges();

            Response.Redirect("Home/Translated?ID=" + t.ID);
            return View(types);
        }


        public ActionResult Translated()
        {

            int id = Convert.ToInt32(Request.QueryString["ID"]);

            ViewBag.Translate = db.Text.Where(x => x.ID == id).FirstOrDefault().Translated;
            return View();
        }


        private string TranslateLeet(string text)
        {
            var client = new RestClient("https://api.funtranslations.com/translate/leetspeak.json");
            var request = new RestRequest();
            request.AddParameter("text", text);

            var response = client.Post(request);
            var content = response.Content;
            JsonObj obj = JsonConvert.DeserializeObject<JsonObj>(content);

            text = obj.contents.translated;
            return text;

        }


        private string TranslateYoda(string text)
        {
            var client = new RestClient("https://api.funtranslations.com/translate/yoda.json");
            var request = new RestRequest();
            request.AddParameter("text", text);

            var response = client.Post(request);
            var content = response.Content; 
            JsonObj obj = JsonConvert.DeserializeObject<JsonObj>(content);

            text = obj.contents.translated;
            return text;

        }

    }
}
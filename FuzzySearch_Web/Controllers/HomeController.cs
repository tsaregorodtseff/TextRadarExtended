using FuzzySearchDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FuzzySearchDemo.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {

            return View();

        }

        public ActionResult Details(int Index, string SearchString, double Relevance, string Version)
        {

            string[] WarAndPeace = HttpContext.Application["WarAndPeace"] as string[];

            var Search = new Models.Search()
            {
                DataString = WarAndPeace[Index - 1],
                SearchString = SearchString,
                Index = Index - 1,
                Mode = 2
            };

            Search.CalculateRelevance(Version);
            Search.Relevance = Relevance;

            return View(Search);
        }

        public ActionResult TestSearch(int Mode, string DataString, string SearchString)
        {

            var Search = new Models.Search()
            {
                DataString = DataString,
                SearchString = SearchString,
                Mode = Mode
            };

            Search.CalculateRelevance();

            return View(Search);

        }

        public ActionResult Pagination()
        {

            return View(HttpContext.Application["WarAndPeaceNumberOfPages"]);
        }

        public ActionResult Page(int PageIndex)
        {

            string[] WarAndPeace = HttpContext.Application["WarAndPeace"] as string[];

            var Page = new Models.Page()
            {
                Id = PageIndex - 1,
                Text = WarAndPeace[PageIndex - 1]
            };

            return View(Page);
        }

        [HttpPost]
        public ActionResult Search(Models.SearchSourceData Sourse)
        {

            var Search = new Models.Search()
            {
                DataString = Sourse.DataString,
                SearchString = Sourse.SearchString,
                Mode = 1
            };

            Search.CalculateRelevance();

            return PartialView(Search);
        }


        [HttpPost]
        public ActionResult SearchInBook(Models.SearchSourceData Sourse)
        {

            var SearchInBook = new Models.SearchInBook_WarAndPeace(HttpContext.Application["WarAndPeace"] as string[]);
            SearchInBook.SearchString = Sourse.SearchString;

            SearchInBook.Search();

            return PartialView(SearchInBook);
        }

        [HttpPost]
        public ActionResult MultiSearch(Models.MultiSearchSourceData Sourse)
        {

            if (Sourse.SearchLocation == "InDataString")
            {

                var Search = new Models.Search()
                {
                    DataString = Sourse.DataString,
                    SearchString = Sourse.SearchString,
                    Mode = 3
                };

                Search.CalculateRelevance("Base");

                return PartialView("Search", Search);

            }


            if (Sourse.SearchLocation == "InWarAndPeaceFull")
            {

                var SearchInBook = new Models.SearchInBook_WarAndPeace(
                    HttpContext.Application["WarAndPeace"] as string[])
                {
                    SearchString = Sourse.SearchString
                };
                SearchInBook.limit = 0.5;

                SearchInBook.Search("Full");

                return PartialView("SearchInBook", SearchInBook);
            }

            if (Sourse.SearchLocation == "InWarAndPeaceBase")
            {

                var SearchInBook = new Models.SearchInBook_WarAndPeace(
                    HttpContext.Application["WarAndPeace"] as string[])
                {
                    SearchString = Sourse.SearchString
                };
                SearchInBook.limit = 0.5;

                SearchInBook.Search("Base");

                return PartialView("SearchInBook", SearchInBook);
            }

            return PartialView();

        }

        public ActionResult About()
        {
            ViewBag.Message = "";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "";

            return View();
        }
    }
}
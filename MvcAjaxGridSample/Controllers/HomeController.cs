using System;
using System.Linq;
using System.Web.Mvc;
using MvcAjaxGridSample.Models;
using MvcAjaxGridSample.Types;
using MvcAjaxGridSample.Types.DataModel;
using MvcAjaxGridSample.Types.DataStorage;

namespace MvcAjaxGridSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<Book> _bookRepository;

        public HomeController(IRepository<Book> bookRep)
        {
            _bookRepository = bookRep;

            _bookRepository.Save(new Book {Title = "Book 1", IssueYear = 2001});
            _bookRepository.Save(new Book {Title = "Book 2", IssueYear = 2002});
            _bookRepository.Save(new Book {Title = "Book 3", IssueYear = 2003});
            _bookRepository.Save(new Book {Title = "Book 4", IssueYear = 2004});
            _bookRepository.Save(new Book {Title = "Book 5", IssueYear = 2005});
        }

        public ActionResult Index()
        {
            var books = _bookRepository.Get();
            var totalCount = books.Count();

            var pageSize = Configuration.Grid.PageSize;
            var model = new GridViewModel<Book>
            {
                Paging = new GridPagingViewModel
                {
                    PageSize = pageSize,
                    PageIndex = 1,
                    TotalItems = totalCount,
                    TotalPages = totalCount/pageSize + 1
                }
            };

            var data = books.Skip(pageSize*(model.Paging.PageIndex - 1)).Take(pageSize);
            model.Data = data.ToArray();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CommandDispatcher(GridViewModel<Book> model)
        {
            var books = _bookRepository.Get();

            // filtering if command = filter or clear
            if (model.Command == GridCommand.Filter || model.Command == GridCommand.FilterClear)
            {
                switch (model.Command)
                {
                    case GridCommand.Filter:
                        model.Filter.Current.Title = model.Filter.Title;
                        model.Filter.Current.IssueYear = model.Filter.IssueYear;
                        break;
                    case GridCommand.FilterClear:
                        model.Filter.Current.Title = model.Filter.Title = null;
                        model.Filter.Current.IssueYear = model.Filter.IssueYear = null;
                        break;
                }
                if (string.IsNullOrWhiteSpace(model.Filter.Current.Title) == false)
                    books = books.Where(b => b.Title.Contains(model.Filter.Current.Title));
                if (model.Filter.Current.IssueYear.HasValue)
                    books = books.Where(b => b.IssueYear == model.Filter.Current.IssueYear.Value);

                // update paging

                var totalCount = books.Count();
                model.Paging.PageIndex = 1;
                model.Paging.TotalItems = totalCount;
                model.Paging.TotalPages = totalCount/model.Paging.PageSize + 1;
            }
            else
            {
                // restore filter to the active one if Apply/Clear was not pressed

                model.Filter.Title = model.Filter.Current.Title;
                model.Filter.IssueYear = model.Filter.Current.IssueYear;
            }

            // changing a page
            switch (model.Command)
            {
                case GridCommand.PageLeft:
                    model.Paging.PageIndex = Math.Max(model.Paging.PageIndex - 1, 1);
                    break;
                case GridCommand.PageRight:
                    model.Paging.PageIndex = Math.Min(model.Paging.PageIndex + 1, model.Paging.TotalPages);
                    break;
                case GridCommand.GoTo:
                    model.Paging.PageIndex = Math.Max(Math.Min(model.Paging.PageIndex, model.Paging.TotalPages), 1);
                    break;
            }

            var data = books.Skip(Configuration.Grid.PageSize*(model.Paging.PageIndex - 1)).Take(Configuration.Grid.PageSize);
            model.Data = data.ToArray();
            return PartialView("_GridViewBooks", model);
        }
    }
}
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

            _bookRepository.Save(new Book { Title = "Book 1", IssueYear = 2001 });
            _bookRepository.Save(new Book { Title = "Book 2", IssueYear = 2002 });
            _bookRepository.Save(new Book { Title = "Book 3", IssueYear = 2003 });
            _bookRepository.Save(new Book { Title = "Book 4", IssueYear = 2004 });
            _bookRepository.Save(new Book { Title = "Book 5", IssueYear = 2005 });
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
                    TotalPages = 3
                }
            };

            var data = books.Skip(pageSize * (model.Paging.PageIndex - 1)).Take(pageSize);
            model.Data = data.ToArray();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CommandDispatcher(GridViewModel<Book> model)
        {
            var books = _bookRepository.Get();

            switch (model.Command)
            {
                case GridCommand.PageLeft:
                    model.Paging.PageIndex = Math.Max(model.Paging.PageIndex-1, 1);
                    break;
                case GridCommand.PageRight:
                    model.Paging.PageIndex = Math.Min(model.Paging.PageIndex+1, model.Paging.TotalPages);
                    break;
            }

            var data = books.Skip(Configuration.Grid.PageSize * (model.Paging.PageIndex - 1)).Take(Configuration.Grid.PageSize);
            model.Data = data.ToArray();

            var model1 = new GridViewModel<Book>
            {
                Data = model.Data,
                Filter = model.Filter,
                Paging = new GridPagingViewModel
                {
                    PageSize = 2,
                    PageIndex = model.Paging.PageIndex,
                    TotalItems = books.Count(),
                    TotalPages = 3
                }
            };
            return PartialView("_GridViewBooks", model1);
        }
    }
}

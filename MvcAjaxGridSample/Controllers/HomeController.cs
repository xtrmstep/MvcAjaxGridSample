﻿using System.Linq;
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
            var data = books.Take(Configuration.Grid.PageSize);

            return View(new GridViewModel<Book>
            {
                Data = data.ToArray(),
                Paging = new GridPagingViewModel(totalCount, Configuration.Grid.PageSize)
            });
        }

    }
}
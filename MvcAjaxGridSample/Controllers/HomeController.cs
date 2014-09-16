﻿using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Web.Mvc;
using MvcAjaxGridSample.Controllers.ActionResults;
using MvcAjaxGridSample.Models;
using MvcAjaxGridSample.Types;
using MvcAjaxGridSample.Types.DataModel;
using MvcAjaxGridSample.Types.DataStorage;

namespace MvcAjaxGridSample.Controllers
{
    public class HomeController : Controller
    {
        private static bool _repositoryIsInitialized;
        private static readonly object _lock = new object();
        private readonly IRepository<Book> _bookRepository;

        public HomeController(IRepository<Book> bookRep)
        {
            _bookRepository = bookRep;

            // initialize sample storage only once
            if (!_repositoryIsInitialized)
            {
                lock (_lock)
                    if (!_repositoryIsInitialized)
                    {
                        _bookRepository.Save(new Book {Title = "Book 1", IssueYear = 2001});
                        _bookRepository.Save(new Book {Title = "Book 2", IssueYear = 2002});
                        _bookRepository.Save(new Book {Title = "Book 3", IssueYear = 2003});
                        _bookRepository.Save(new Book {Title = "Book 4", IssueYear = 2004});
                        _bookRepository.Save(new Book {Title = "Book 5", IssueYear = 2005});
                        _repositoryIsInitialized = true;
                    }
            }
        }

        public ActionResult Index()
        {
            var books = _bookRepository.Get();
            var totalCount = books.Count();

            var pageSize = Configuration.Grid.PageSize;
            var model = new GridViewModel<BookViewModel>
            {
                Paging =
                {
                    PageSize = pageSize,
                    PageIndex = 1,
                    TotalItems = totalCount,
                    TotalPages = totalCount / pageSize + 1
                }
            };

            var data = books.Skip(pageSize * (model.Paging.PageIndex - 1)).Take(pageSize);
            model.Data = data.Select(d => new BookViewModel(d)).ToArray();
            return View(model);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult CommandDispatcher(GridViewModel<Book> model, string gridOptions)
        //{
        //    if (model.DeletedId.HasValue)
        //        _bookRepository.Delete(model.DeletedId.Value);

        //    var options = (GridViewModel<Book>.GridOptions) new MvcSerializer().Deserialize(gridOptions, SerializationMode.Signed);
        //    options.Command = model.Options.Command;
        //    var books = _bookRepository.Get();

        //    // filtering if command = filter or clear
        //    if (options.Command == GridCommand.Filter || options.Command == GridCommand.FilterClear)
        //    {
        //        switch (options.Command)
        //        {
        //            case GridCommand.Filter:
        //                options.Filter.Title = model.Options.Filter.Title;
        //                options.Filter.IssueYear = model.Options.Filter.IssueYear;
        //                break;
        //            case GridCommand.FilterClear:
        //                options.Filter.Title = null;
        //                options.Filter.IssueYear = null;
        //                break;
        //        }
        //        options.Paging.PageIndex = 1;
        //    }

        //    if (string.IsNullOrWhiteSpace(options.Filter.Title) == false)
        //        books = books.Where(b => b.Title.Contains(options.Filter.Title));
        //    if (options.Filter.IssueYear.HasValue)
        //        books = books.Where(b => b.IssueYear == options.Filter.IssueYear.Value);

        //    // update paging

        //    var totalCount = books.Count();
        //    options.Paging.TotalItems = totalCount;
        //    options.Paging.TotalPages = totalCount/options.Paging.PageSize;
        //    if (totalCount%options.Paging.PageSize > 0) options.Paging.TotalPages++;

        //    // changing a page
        //    switch (options.Command)
        //    {
        //        case GridCommand.PageLeft:
        //            options.Paging.PageIndex = Math.Max(options.Paging.PageIndex - 1, 1);
        //            break;
        //        case GridCommand.PageRight:
        //            options.Paging.PageIndex = Math.Min(options.Paging.PageIndex + 1, options.Paging.TotalPages);
        //            break;
        //        case GridCommand.GoTo:
        //            options.Paging.PageIndex = Math.Max(Math.Min(model.Options.Paging.PageIndex, options.Paging.TotalPages), 1);
        //            break;
        //    }

        //    var data = books.Skip(Configuration.Grid.PageSize*(options.Paging.PageIndex - 1)).Take(Configuration.Grid.PageSize);

        //    model.Data = data.ToArray();
        //    model.Options = options;
        //    return PartialView("_GridViewBooks", model);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id)
        {
            var book = _bookRepository.Get(id);
            var bookViewModel = new BookEditViewModel();
            ModelCopier.CopyModel(book, bookViewModel);
            return PartialView("_EditBook", bookViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(BookEditViewModel bookViewModel)
        {
            //if (ModelState.IsValid)
            //{
            //    var book = new Book();
            //    ModelCopier.CopyModel(bookViewModel, book);
            //    _bookRepository.Save(book);

            //    var gridOptions = new GridViewModel<Book>.GridOptions
            //    {
            //        Filter =
            //        {
            //            Title = book.Title,
            //            IssueYear = book.IssueYear
            //        },
            //        Paging =
            //        {
            //            PageIndex = 1,
            //            PageSize = Configuration.Grid.PageSize
            //        }
            //    };
            //    var options = new MvcSerializer().Serialize(gridOptions, SerializationMode.Signed);
            //    var model = new GridViewModel<Book> {Options = {Command = GridCommand.Filter}};
            //    return CommandDispatcher(model, options);
            //}
            return new ModelValidationActionResult(ModelState);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New()
        {
            var book = new Book();
            var bookViewModel = new BookEditViewModel();
            ModelCopier.CopyModel(book, bookViewModel);
            return PartialView("_EditBook", bookViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GoToPage(GridPaging model)
        {

            return new EmptyResult();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Filter(BookEditViewModel filter)
        {
            return new EmptyResult();
        }
    }
}
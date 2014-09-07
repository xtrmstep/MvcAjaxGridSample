using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Web.Helpers;
using System.Web.Mvc;
using Microsoft.Web.Mvc;
using MvcAjaxGridSample.Models;
using MvcAjaxGridSample.Types;
using MvcAjaxGridSample.Types.DataModel;
using MvcAjaxGridSample.Types.DataStorage;

namespace MvcAjaxGridSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<Book> _bookRepository;
        private static bool _repositoryIsInitialized = false;
        private static readonly object _lock = new object();

        public HomeController(IRepository<Book> bookRep)
        {
            _bookRepository = bookRep;

            // initialize sample storage only once
            if(!_repositoryIsInitialized)
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

        public ActionResult Index()
        {
            var books = _bookRepository.Get();
            var totalCount = books.Count();

            var pageSize = Configuration.Grid.PageSize;
            var model = new GridViewModel<Book>
            {
                Options =
                {
                    Paging = new GridPagingViewModel
                    {
                        PageSize = pageSize,
                        PageIndex = 1,
                        TotalItems = totalCount,
                        TotalPages = totalCount/pageSize + 1
                    }
                }
            };

            var data = books.Skip(pageSize*(model.Options.Paging.PageIndex - 1)).Take(pageSize);
            model.Data = data.ToArray();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CommandDispatcher(GridViewModel<Book> model, string gridOptions)
        {
            if (model.DeletedId.HasValue)
                _bookRepository.Delete(model.DeletedId.Value);


            var options = (GridViewModel<Book>.GridOptions)new MvcSerializer().Deserialize(gridOptions, SerializationMode.Signed);
            options.Command = model.Options.Command;
            var books = _bookRepository.Get();

            // filtering if command = filter or clear
            if (options.Command == GridCommand.Filter || options.Command == GridCommand.FilterClear)
            {
                switch (options.Command)
                {
                    case GridCommand.Filter:
                        options.Filter.Title = model.Options.Filter.Title;
                        options.Filter.IssueYear = model.Options.Filter.IssueYear;
                        break;
                    case GridCommand.FilterClear:
                        options.Filter.Title = null;
                        options.Filter.IssueYear = null;
                        break;
                }
                options.Paging.PageIndex = 1;
            }


            if (string.IsNullOrWhiteSpace(options.Filter.Title) == false)
                books = books.Where(b => b.Title.Contains(options.Filter.Title));
            if (options.Filter.IssueYear.HasValue)
                books = books.Where(b => b.IssueYear == options.Filter.IssueYear.Value);

            // update paging

            var totalCount = books.Count();
            options.Paging.TotalItems = totalCount;
            options.Paging.TotalPages = totalCount / options.Paging.PageSize;
            if (totalCount%options.Paging.PageSize > 0) options.Paging.TotalPages++;

            // changing a page
            switch (options.Command)
            {
                case GridCommand.PageLeft:
                    options.Paging.PageIndex = Math.Max(options.Paging.PageIndex - 1, 1);
                    break;
                case GridCommand.PageRight:
                    options.Paging.PageIndex = Math.Min(options.Paging.PageIndex + 1, options.Paging.TotalPages);
                    break;
                case GridCommand.GoTo:
                    options.Paging.PageIndex = Math.Max(Math.Min(model.Options.Paging.PageIndex, options.Paging.TotalPages), 1);
                    break;
            }

            var data = books.Skip(Configuration.Grid.PageSize * (options.Paging.PageIndex - 1)).Take(Configuration.Grid.PageSize);

            model.Data = data.ToArray();
            model.Options = options;
            return PartialView("_GridViewBooks", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id)
        {
            var book = _bookRepository.Get(id);
            return PartialView("_EditBook", book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Book book)
        {
            if (ModelState.IsValid)
            {
                _bookRepository.Save(book);

                var gridOptions = new GridViewModel<Book>.GridOptions
                {
                    Filter =
                    {
                        Title = book.Title,
                        IssueYear = book.IssueYear
                    },
                    Paging =
                    {
                        PageIndex = 1,
                        PageSize = Configuration.Grid.PageSize
                    }
                };
                var options = new MvcSerializer().Serialize(gridOptions, SerializationMode.Signed);
                var model = new GridViewModel<Book> {Options = {Command = GridCommand.Filter}};
                return CommandDispatcher(model, options);
            }
            return ModelStateResult(ModelState);
        }

        private ActionResult ModelStateResult(ModelStateDictionary modelState)
        {
            return new ModelValidationResult { IsValid = modelState.IsValid, ModelState = modelState };
        }

        class ModelValidationResult : ActionResult
        {
            public override void ExecuteResult(ControllerContext context)
            {
                var httpContext = context.RequestContext.HttpContext;
                httpContext.ClearError();

                var response = httpContext.Response;
                response.Clear();
                response.ContentEncoding = Encoding.UTF8;
                response.HeaderEncoding = Encoding.UTF8;
                response.TrySkipIisCustomErrors = true;
                response.StatusCode = (int) (IsValid ? HttpStatusCode.OK : HttpStatusCode.InternalServerError);

                response.ContentType = "application/json";

                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .Select(ms => new
                    {
                        Field = ms.Key,
                        ErrorMessage = string.Join(";", ms.Value.Errors.Select(e => e.ErrorMessage))
                    }).ToArray();

                var jsonResult = System.Web.Helpers.Json.Encode(errors);
                var bytes = Encoding.ASCII.GetBytes(jsonResult);
                response.BinaryWrite(bytes);
            }

            public bool IsValid { get; set; }
            public ModelStateDictionary ModelState { get; set; }
        }

        public ActionResult New()
        {
            var book = new Book();
            return PartialView("_EditBook", book);
        }
    }
}
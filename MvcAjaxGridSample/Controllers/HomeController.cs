using System;
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
            var model = GetGridModel();
            return View(model);
        }

        private GridViewModel<BookViewModel> GetGridModel(GridViewModel<BookViewModel>.GridOptions options = null)
        {
            var model = new GridViewModel<BookViewModel>();
            if (options != null) model.Options = options;
            var data = GetData(model.Options);
            model.Data = data;
            return model;
        }

        /// <summary>
        /// Returns data with respect to filetring and sorting, and update paging
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public BookViewModel[] GetData(GridViewModel<BookViewModel>.GridOptions options)
        {
            var books = _bookRepository.Get();

            #region filtering

            if (options.Filter != null)
            {
                foreach (var field in options.Filter.Fields)
                {
                    var name = field.Name;
                    var value = field.Value;
                    if (string.IsNullOrWhiteSpace(value)) continue;

                    switch (name)
                    {
                        case "Title":
                            books = books.Where(b => b.Title.Contains(value));
                            break;
                        case "IssueYear":
                            books = books.Where(b => b.IssueYear.ToString().StartsWith(value));
                            break;
                    }
                }
            }
            else
                options.Filter = new GridFilter<BookViewModel>();
            #endregion

            #region sorting

            if (options.Sorting != null)
            {
                IOrderedQueryable<Book> ordered = null;
                foreach (var field in options.Sorting.Fields)
                {
                    var name = field.Name;
                    var isAscending = field.Ascending;
                    if (!isAscending.HasValue) continue;

                    switch (name)
                    {
                        case "Title":
                            if (isAscending.Value)
                                ordered = ordered == null ? books.OrderBy(b => b.Title) : ordered.ThenBy(b => b.Title);
                            else
                                ordered = ordered == null ? books.OrderByDescending(b => b.Title) : ordered.ThenByDescending(b => b.Title);
                            break;
                        case "IssueYear":
                            if (isAscending.Value)
                                ordered = ordered == null ? books.OrderBy(b => b.IssueYear) : ordered.ThenBy(b => b.IssueYear);
                            else
                                ordered = ordered == null ? books.OrderByDescending(b => b.IssueYear) : ordered.ThenByDescending(b => b.IssueYear);
                            break;
                    }
                }
                if (ordered != null)
                    books = ordered;
            }
            else
                options.Sorting = new GridSorting<BookViewModel>();

            #endregion

            #region paging

            if (options.Paging == null) options.Paging = new GridPaging();

            var pageIndex = options.Paging.PageIndex; 
            var pageSize = Configuration.Grid.PageSize;
            var totalCount = books.Count();
            var totalPages = totalCount / pageSize;
            if (totalCount % pageSize > 0) totalPages++;
            pageIndex = Math.Max(Math.Min(pageIndex, totalPages), 1);

            options.Paging.TotalItems = totalCount;
            options.Paging.TotalPages = totalPages;
            options.Paging.PageIndex = pageIndex;
            options.Paging.PageSize = pageSize;

            books = books.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
            #endregion

            return books.Select(d => new BookViewModel(d)).ToArray();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sort(SortingField sorting, string options)
        {
            if (!sorting.Ascending.HasValue) 
                sorting.Ascending = true;
            else
                sorting.Ascending = !sorting.Ascending;

            var objOptions = System.Web.Helpers.Json.Decode<GridViewModel<BookViewModel>.GridOptions>(options);

            objOptions.Sorting.Set(sorting);
            var model = GetGridModel(objOptions);
            return PartialView("_Grid", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Page(string index, string options)
        {
            var objOptions = System.Web.Helpers.Json.Decode<GridViewModel<BookViewModel>.GridOptions>(options);
            // use index if specified, if not - do nothing
            // if index = up|down, then calculate the index
            // if index = number, then use it
            if (!string.IsNullOrWhiteSpace(index))
            {
                if (index == "up")
                {
                    objOptions.Paging.PageIndex += 1;
                }else if (index == "down")
                {
                    objOptions.Paging.PageIndex -= 1;
                }
                else
                {
                    int intIndex;
                    if (int.TryParse(index, out intIndex))
                        objOptions.Paging.PageIndex = intIndex;
                }
            }

            var model = GetGridModel(objOptions);
            return PartialView("_Grid", model);
        }
    }
}
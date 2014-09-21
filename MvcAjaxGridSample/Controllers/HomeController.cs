using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Web.Mvc;
using MvcAjaxGridSample.Controllers.ActionResults;
using MvcAjaxGridSample.Models;
using MvcAjaxGridSample.Types;
using MvcAjaxGridSample.Types.DataModel;
using MvcAjaxGridSample.Types.DataStorage;
using WebGrease.Css.Extensions;

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id)
        {
            var book = id.HasValue ? _bookRepository.Get(id.Value) : new Book();
            var bookViewModel = new BookEditViewModel();
            ModelCopier.CopyModel(book, bookViewModel);
            return PartialView("_EditBook", bookViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(BookEditViewModel bookEditViewModel, string options)
        {
            if (ModelState.IsValid)
            {
                var objOptions = System.Web.Helpers.Json.Decode<GridViewModel<BookViewModel>.GridOptions>(options);

                var book = new Book();
                ModelCopier.CopyModel(bookEditViewModel, book);
                _bookRepository.Save(book);

                var model = GetGridModel(objOptions);
                return PartialView("_Grid", model);
            }
            return new ModelValidationActionResult(ModelState);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Filter(FilterField[] filter, string options)
        {
            var objOptions = System.Web.Helpers.Json.Decode<GridViewModel<BookViewModel>.GridOptions>(options);
            // if filter Name is Empty, then clean the filter
            // if not empty, then filter
            if (filter != null && filter.Any(f => !string.IsNullOrWhiteSpace(f.Name)))
            {
                objOptions.Filter.Fields.ForEach(field => {
                    var filterField = filter.FirstOrDefault(userField => userField.Name == field.Name);
                    if (filterField != null)
                        field.Value = filterField.Value;
                });
            }
            else
            {
                objOptions.Filter.Clear();
            }
            var model = GetGridModel(objOptions);
            return PartialView("_Grid", model);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, string options)
        {
            _bookRepository.Delete(id);
            var objOptions = System.Web.Helpers.Json.Decode<GridViewModel<BookViewModel>.GridOptions>(options);
            var model = GetGridModel(objOptions);
            return PartialView("_Grid", model);
        }
    }
}
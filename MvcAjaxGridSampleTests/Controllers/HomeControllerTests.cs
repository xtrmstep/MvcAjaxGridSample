using System;
using System.Linq;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcAjaxGridSample.Models;
using MvcAjaxGridSample.Types;
using MvcAjaxGridSample.Types.DataModel;
using MvcAjaxGridSample.Types.DataStorage;
using MvcAjaxGridSampleTests;

namespace MvcAjaxGridSample.Controllers.Tests
{
    [TestClass]
    public class HomeControllerTests
    {
        private static bool _initialized;

        private readonly IRepository<Book> _bookRep;
        private readonly HomeController _homeController;
        private IContainer _resolver;

        public HomeControllerTests()
        {
            _resolver = MvcApplication.RegisterTypes(new TypeRegistrationModuleForTest());

            _bookRep = _resolver.Resolve<IRepository<Book>>();
            Configuration.Grid.PageSize = 10;
            if (!_initialized)
            {
                _bookRep.Delete(1);
                _bookRep.Delete(2);
                _bookRep.Delete(3);
                _bookRep.Delete(4);
                _bookRep.Delete(5);

                _initialized = true;
            }
            // Home adds data again
            _homeController = new HomeController(_bookRep);
        }

        [TestInitialize]
        public void Setup() {}

        [TestMethod]
        public void GetData_should_sort_data_ascending_by_title()
        {
            var book = new Book {Title = "Book 11", IssueYear = 2011};
            _bookRep.Save(book);

            var unsortedData = _bookRep.Get().ToArray();
            Assert.AreEqual("Book 11", unsortedData.Last().Title);

            var model = new GridViewModel<BookViewModel>();
            model.Options.Sorting.Set(new SortingField {Name = "Title", Ascending = true});
            var sortedData = _homeController.GetData(model.Options);

            Assert.AreEqual(6, sortedData.Length);
            Assert.AreEqual("Book 11", sortedData[1].Title);
            Assert.AreEqual("Book 5", sortedData.Last().Title);

            _bookRep.Delete(book.Id);
        }

        [TestMethod]
        public void GetData_should_sort_data_descending_by_title()
        {
            var book = new Book {Title = "Book 11", IssueYear = 2011};
            _bookRep.Save(book);

            var unsortedData = _bookRep.Get().ToArray();
            Assert.AreEqual("Book 11", unsortedData.Last().Title);

            var model = new GridViewModel<BookViewModel>();
            model.Options.Sorting.Set(new SortingField {Name = "Title", Ascending = false});
            var sortedData = _homeController.GetData(model.Options);

            Assert.AreEqual(6, sortedData.Length);
            Assert.AreEqual("Book 4", sortedData[1].Title);
            Assert.AreEqual("Book 1", sortedData.Last().Title);

            _bookRep.Delete(book.Id);
        }

        [TestMethod]
        public void GetData_should_sort_data_ascending_by_year()
        {
            var book = new Book { Title = "Book 11", IssueYear = 2011 };
            _bookRep.Save(book);

            var unsortedData = _bookRep.Get().ToArray();
            Assert.AreEqual("Book 11", unsortedData.Last().Title);

            var model = new GridViewModel<BookViewModel>();
            model.Options.Sorting.Set(new SortingField { Name = "IssueYear", Ascending = true });
            var sortedData = _homeController.GetData(model.Options);

            Assert.AreEqual(6, sortedData.Length);
            Assert.AreEqual("Book 2", sortedData[1].Title);
            Assert.AreEqual("Book 11", sortedData.Last().Title);

            _bookRep.Delete(book.Id);
        }

        [TestMethod]
        public void GetData_should_sort_data_descending_by_year()
        {
            var book = new Book { Title = "Book 11", IssueYear = 2011 };
            _bookRep.Save(book);

            var unsortedData = _bookRep.Get().ToArray();
            Assert.AreEqual("Book 11", unsortedData.Last().Title);

            var model = new GridViewModel<BookViewModel>();
            model.Options.Sorting.Set(new SortingField { Name = "IssueYear", Ascending = false });
            var sortedData = _homeController.GetData(model.Options);

            Assert.AreEqual(6, sortedData.Length);
            Assert.AreEqual("Book 5", sortedData[1].Title);
            Assert.AreEqual("Book 1", sortedData.Last().Title);

            _bookRep.Delete(book.Id);
        }

        [TestMethod]
        public void GetData_should_return_correct_dataPage()
        {
            var model = new GridViewModel<BookViewModel>();
            model.Options.Sorting.Set(new SortingField {Name = "Title", Ascending = true});
            model.Options.Paging.PageIndex = 3;
            Configuration.Grid.PageSize = 2; // set page size to 2 in order to have 3 pages. the latest one will have only one record

            var pageData = _homeController.GetData(model.Options);

            Assert.AreEqual(1, pageData.Length);
            Assert.AreEqual("Book 5", pageData[0].Title);
        }

        [TestMethod]
        public void GetData_should_correctly_filter_data()
        {
            var model = new GridViewModel<BookViewModel>();
            model.Options.Sorting.Set(new SortingField { Name = "Title", Ascending = true });
            model.Options.Filter.Fields[0].Value = "5"; // filter by Title contaning '5'

            var pageData = _homeController.GetData(model.Options);

            Assert.AreEqual(1, pageData.Length);
            Assert.AreEqual("Book 5", pageData[0].Title);
        }
    }
}
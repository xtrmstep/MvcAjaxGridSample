using System.Linq;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MvcAjaxGridSample;
using MvcAjaxGridSample.Types.DataModel;
using MvcAjaxGridSample.Types.DataStorage;

namespace MvcAjaxGridSampleTests.Types.DataStorage.Implementation
{
    [TestClass]
    public class RepositoryTests
    {
        private IRepository<Book> _bookRep;
        private IContainer _resolver;

        [TestInitialize]
        public void Setup()
        {
            _resolver = MvcApplication.RegisterTypes(new TypeRegistrationModuleForTest());

            _bookRep = _resolver.Resolve<IRepository<Book>>();
        }

        [TestMethod]
        public void Get_should_return_whole_list()
        {
            var actual = _bookRep.Get();
            Assert.AreEqual(5, actual.Count());
        }

        [TestMethod]
        public void Get_should_return_notNull()
        {
            _bookRep.Delete(1);
            _bookRep.Delete(2);
            _bookRep.Delete(3);
            _bookRep.Delete(4);
            _bookRep.Delete(5);

            var actual = _bookRep.Get();

            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Count());
        }

        [TestMethod]
        public void Get_should_return_item_by_id()
        {
            var item = _bookRep.Get(4);
            Assert.AreEqual(4, item.Id);
        }

        [TestMethod]
        public void Get_should_return_null_for_wrong_id()
        {
            var item = _bookRep.Get(7);
            Assert.IsNull(item);
        }

        [TestMethod]
        public void Delete_should_remove_item_by_id()
        {
            _bookRep.Delete(4);

            var all = _bookRep.Get();
            Assert.AreEqual(4, all.Count());

            var item = _bookRep.Get(4);
            Assert.IsNull(item);
        }

        [TestMethod]
        public void Delete_should_do_nothing_for_wrong_id()
        {
            _bookRep.Delete(7);

            var all = _bookRep.Get();
            Assert.AreEqual(5, all.Count());
        }

        [TestMethod]
        public void Save_should_add_new_item()
        {
            var newBook = new Book();
            _bookRep.Save(newBook);

            var all = _bookRep.Get();
            Assert.AreEqual(6, all.Count());
        }

        [TestMethod]
        public void Save_should_update_existing_item()
        {
            var item = _bookRep.Get(2);
            item.Title = "Updated";
            _bookRep.Save(item);

            var all = _bookRep.Get();
            Assert.AreEqual(5, all.Count());

            var updated = _bookRep.Get(2);
            Assert.AreEqual("Updated", updated.Title);
        }
    }
}
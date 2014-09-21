using System.Web.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MvcAjaxGridSample.Models.Tests
{
    [TestClass]
    public class GridViewModelTests
    {
        [TestMethod]
        public void JsonEncoder_should_generate_a_string_forOptions()
        {
            var model = new GridViewModel<BookViewModel>();
            var actual = Json.Encode(model.Options);
            const string EXPECTED = "{\"Filter\":{\"Fields\":[{\"Name\":\"Title\",\"Value\":null},{\"Name\":\"IssueYear\",\"Value\":null}]},\"Sorting\":{\"Fields\":[{\"Name\":\"Title\",\"Ascending\":null},{\"Name\":\"IssueYear\",\"Ascending\":null}]},\"Paging\":{\"TotalItems\":0,\"TotalPages\":0,\"PageSize\":0,\"PageIndex\":0}}";
            Assert.AreEqual(EXPECTED, actual);
        }
    }
}
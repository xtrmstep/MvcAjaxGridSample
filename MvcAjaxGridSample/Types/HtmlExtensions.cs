using System;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MvcAjaxGridSample.Models;

namespace MvcAjaxGridSample.Types
{
    public static class HtmlExtensions
    {
        public static IHtmlString Command<TModel>(this HtmlHelper<TModel> html, string text, Expression<Func<TModel, GridCommand?>> commandProperty, GridCommand command)
        {
            var btn = new TagBuilder("button");
            btn.Attributes["type"] = "submit";
            btn.AddCssClass("btn");
            btn.AddCssClass("btn-xs");
            btn.AddCssClass("btn-default");
            btn.Attributes["name"] = html.NameFor(commandProperty).ToString();
            btn.Attributes["value"] = command.ToString();
            btn.SetInnerText(text);
            return new MvcHtmlString(btn.ToString());
        }
    }
}
﻿using System;
using System.Collections.Generic;
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

        public static IHtmlString SimpleHidden<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> nameProperty, object value)
        {
            var inp = new TagBuilder("input");
            inp.Attributes["type"] = "hidden";
            inp.Attributes["name"] = html.NameFor(nameProperty).ToString();
            inp.Attributes["value"] = value.ToString();
            return new MvcHtmlString(inp.ToString());
        }

        public static IHtmlString SimpleEdit<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> nameProperty, object value, IDictionary<string, object> htmlAttributes = null)
        {
            var inp = new TagBuilder("input");
            inp.Attributes["type"] = "text";
            inp.Attributes["name"] = html.NameFor(nameProperty).ToString();
            inp.Attributes["value"] = value.ToString();
            if (htmlAttributes != null) inp.MergeAttributes(htmlAttributes, true);
            return new MvcHtmlString(inp.ToString());
        }
    }
}
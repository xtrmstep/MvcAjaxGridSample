using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcAjaxGridSample.Models
{
    public enum GridCommand
    {
        Delete,
        Edit,
        New,
        PageLeft,
        PageRight,
        GoTo,
        Filter,
        FilterClear
    }
}
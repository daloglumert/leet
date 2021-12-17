using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace leet.Models
{
    public class Types
    {

        public string SelectedItemId { get; set; }
        public IEnumerable<SelectListItem> TypeNames;
       
     

    }
}
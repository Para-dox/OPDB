using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPDB.Models
{
    public class UnitViewModel
    {
        public Unit Unit { get; set; }

        public List<UserInfoViewModel> Information { get; set; }
    }
}
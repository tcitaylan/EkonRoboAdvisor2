using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Models.ViewModels
{
    public class PortfolioItem
    {
        public string SymbolName { get; set; }
        public int SymbolID { get; set; }
        public double Lot { get; set; }
        public double TotalAmount { get; set; }
        public double Cost { get; set; }
        public double Perc { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Models.ViewModels
{
    public class BacktestResult
    {
        public string Name { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public List<double> Profits { get; set; }
        public List<double> Balance { get; set; }
        public List<DateTime> Dates { get; set; }
        public List<PortfolioItem> Portfolio { get; set; }
        public List<List<PortfolioItem>> PortfolioHistory { get; set; }
    }
}

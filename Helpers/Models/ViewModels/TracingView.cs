using Helpers.Models.Dtos;
using System.Collections.Generic;

namespace Helpers.Models.ViewModels
{
    public class TracingView
    {
        public string Balance { get; set; }
        public string RegisterDate { get; set; }
        public string SurveyDate { get; set; }
        public string FirstBalance { get; set; }
        public string LastBalance { get; set; }
        public string LoginCount { get; set; }
        public string SurveyRenewal { get; set; }
        public UserBasketsDto Basket { get; set; }
        public List<BasketDetailItem> Assets { get; set; }
        public List<BacktestModel> Backtest { get; set; }
        public List<int> ContractID { get; set; }
        public int SelectContract { get; set; }

        public WebSiteView UnselectedBaskets { get; set; }
    }
    
    public class BasketDetailItem
    {
        public string Lot { get; set; }
        public string AvgPrice { get; set; }
        public string TotalAmount { get; set; }
        public string Name { get; set; }
        public string Explanation { get; set; }
    }
}
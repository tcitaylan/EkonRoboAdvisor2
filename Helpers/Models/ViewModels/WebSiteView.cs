
using Helpers.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Helpers.Models.ViewModels
{
    public class WebSiteView
    {      
        public string NameSurname { get; set; }
        public UserCategoryHistoryDto UserResult { get; set; }        
        public List<TemplateBasketsDto> TemplateBaskets { get; set; }
        public IEnumerable<SymbolsDto> BasketSymbols { get; set; }
        public IEnumerable<TemplateBasketStocksDto> TemplateBasketStock { get; set; }
        public List<BacktestModel> BackTest { get; set; }
        public List<int> ContractID { get; set; }
        public ExistingPortfolioModel ExistingBasket { get; set; }
        public string CategoryName { get; set; }
    }

    public class BacktestModel
    {
        public string Basketname { get; set; }      
        public List<double> Data { get; set; }  
        public List<DateTime> Date { get; set; }
    }

    public class ExistingPortfolioModel
    {
        public List<int > SymbolId { get; set; }
        public List<string> SymbolName { get; set; }
        public List<double> SymPercentage { get; set; }
    }
}
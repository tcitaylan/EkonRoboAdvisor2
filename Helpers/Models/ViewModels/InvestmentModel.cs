using Helpers.Models.Dtos;
using System.Collections.Generic;

namespace Helpers.Models.ViewModels
{    
    public class InvestmentModel
    {
        public List<BasketSymbol> SymbolList { get; set; }
        public UserBasketsDto UserBasket { get; set; }
        public int ContractID { get; set; }
        public List<int> Contracts{ get; set; }
        public int BasketID { get; set; } 
        public string BasketName{ get; set; }         
        
    }
        
    public class BasketSymbol
    {
        public int SymbolID { get; set; }
        public string Name { get; set; }
        public string Explanation { get; set; }
        public int Lot { get; set; }
        public decimal Price { get; set; }
        public double AvgPrice { get; set; }
    }
}
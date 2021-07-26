using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Helpers.Models.Dtos;
using Helpers.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RoboAdvisorApi.Mapper;
using RoboAdvisorApi.Models;

namespace ERoboServices.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HisseController : ControllerBase
    {
        private readonly EkonRoboDBContext _context;

        public HisseController(EkonRoboDBContext context)
        {
            _context = context;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCategory(int userId)
        {
            var category = await _context.UserCategoryHistory.FirstOrDefaultAsync(x => x.UserId == userId);
            if (category != null)
                return Ok(category);
            else return Ok(null);
        }

        [HttpGet("{userid}")]
        public async Task<IActionResult> GetWebSiteModel(int userid)
        {
            try
            {
                var basketsreal = await _context.TemplateBaskets.OrderByDescending(z => z.RecordDate).Take(5)
                    .ToListAsync(); // Hisse için kullanılan

                var baskets = await _context.TemplateBaskets
                    .Where(x => x.RecordDate == _context.TemplateBaskets.Min(z => z.RecordDate))
                    .ToListAsync();
                baskets.AddRange(basketsreal);
                List<BacktestModel> backtestler = new List<BacktestModel>();

                foreach (var basket in baskets)
                {
                    var backtest = _context.TemplateBasketBackTests.FirstOrDefault(x => x.TemplateBasketId == basket.RecordId);
                    // backtest = null;
                    //OnError
                    if (backtest == null)
                    {
                        continue;
                    }
                    dynamic results = JsonConvert.DeserializeObject(backtest.Result);
                    List<double> data = new List<double>();
                    List<DateTime> date = new List<DateTime>();
                    var i = 0;
                    foreach (dynamic result in results)
                    {
                        if (DateTime.Now.AddYears(-1) <= Convert.ToDateTime(result.Date.Value))
                        {
                            i++;
                            if (i % 20 == 0)
                            {
                                data.Add(result.Balance.Value);
                                date.Add(Convert.ToDateTime(result.Date.Value));
                            }
                        }
                    }

                    data.Reverse();
                    date.Reverse();

                    backtestler.Add(new BacktestModel
                    {
                        Basketname = basket.Name,
                        Data = data,
                        Date = date
                    });

                }
                baskets.RemoveAt(0);

                /*USDTRY data*/
                try
                {
                    var data1 = "";
                    dynamic usdtry;
                    int j = 0;

                    using (var httpClient = new HttpClient())
                    {
                        var response = await httpClient.GetAsync(@"http://185.122.200.217:6778/Data/GetCandleData?symbol=USDTRY&period=1440&count=365");
                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {

                        }
                        data1 = await response.Content.ReadAsStringAsync();

                        usdtry = JsonConvert.DeserializeObject(data1);
                    }
                    List<double> datausd = new List<double>();
                    List<DateTime> dateusd = new List<DateTime>();
                    double base1 = 10000 / usdtry[0].close.Value;
                    foreach (var d in usdtry)
                    {
                        if (DateTime.Now.AddYears(-1) <= Convert.ToDateTime(DateTime.Now))
                        {
                            j++;
                            if (j % 5 == 0)
                            {
                                datausd.Add(base1 * d.close.Value);
                                Console.WriteLine(d.date.Value);
                                dateusd.Add(Convert.ToDateTime(d.date.Value));
                            }
                        }
                    }
                    datausd.Reverse();
                    dateusd.Reverse();
                    backtestler.Add(new BacktestModel
                    {
                        Basketname = "USDTRY",
                        Data = datausd,
                        Date = dateusd
                    });

                }
                catch (Exception)
                {

                }
                /*eof USDTRY data*/

                // ** DB data update !!!
                List<int> recIds = new List<int>();
                foreach (var bask in baskets)
                {
                    recIds.Add(bask.RecordId);
                }
                var basketsstocks =  // include existing
                (from a in _context.TemplateBasketStocks.ToList()
                 where recIds.Contains(Convert.ToInt32(a.TemplateBasketId))
                 select a
                ).ToList();
                List<TemplateBasketStocks> cash_list = new List<TemplateBasketStocks>();
                foreach (var b in baskets)
                {
                    double tot_sym_percentage = 0;
                    foreach (var perc in basketsstocks)
                    {
                        if (perc.TemplateBasketId == b.RecordId)
                        {
                            tot_sym_percentage += perc.Perc ?? default;
                        }
                    }
                    if (1 - tot_sym_percentage > 0)
                    {
                        Console.WriteLine(b.RecordId);
                        Console.WriteLine("CATCHED");
                        cash_list.Add(new TemplateBasketStocks
                        {
                            TemplateBasketId = b.RecordId,
                            SymbolId = 98000,
                            Perc = (1 - tot_sym_percentage)
                        });
                    }

                }
                basketsstocks.AddRange(cash_list);



                var symbol = new List<Symbols>();

                foreach (var item in baskets)
                {
                    var symIds = _context.TemplateBasketStocks
                        .Where(c => c.TemplateBasketId == item.RecordId)
                        .Select(c => c.SymbolId).ToList();

                    List<Symbols> symbols = _context.Symbols
                        .Where(c => symIds.Contains(c.RecordId)).ToList();

                    var symbolIds = symbols.Select(a => a.RecordId).ToList();
                    var symbolstock =
                    (from a in _context.TemplateBasketStocks.ToList()
                     where symbolIds.Contains(Convert.ToInt32(a.SymbolId))
                     && a.TemplateBasketId == item.RecordId
                     select a
                    );

                    symbol.AddRange(symbols);
                }
                ExistingPortfolioModel ex = new ExistingPortfolioModel();
                ex.SymbolId = new List<int>();
                ex.SymbolName = new List<string>();
                ex.SymPercentage = new List<double>();
                ex.SymbolId = new List<int> { 30, 34, 36, 40 };
                ex.SymbolName = new List<string> { "PETKM", "SISE", "SODA", "TKFEN" };
                ex.SymPercentage = new List<double> { };
                backtestler[0].Basketname = "Mevcut Porföyünüz";

                var contract = await _context.UserBaskets.Where(c => c.UserId == userid).ToListAsync();

                List<int> contractID = new List<int>();
                foreach (var item in contract.Select(x => x.ContractId).GroupBy(x => x.Value).ToList())
                {
                    contractID.Add(item.Key);
                }
                WebSiteView wModel = new WebSiteView
                {
                    NameSurname = _context.Users.SingleOrDefault(c => c.RecordId == userid).NameSurname,
                    //UserResult = AutoMapperBase._mapper.Map<UserCategoryHistory, UserCategoryHistoryDto>(cate),
                    TemplateBaskets = AutoMapperBase._mapper.Map<List<TemplateBaskets>, List<TemplateBasketsDto>>(baskets.ToList()),
                    BasketSymbols = AutoMapperBase._mapper.Map<List<Symbols>, List<SymbolsDto>>(symbol),
                    BackTest = backtestler,
                    TemplateBasketStock = AutoMapperBase._mapper.Map<List<TemplateBasketStocks>, List<TemplateBasketStocksDto>>(basketsstocks),
                    ContractID = contractID,
                    ExistingBasket = ex
                };
                return Ok(wModel);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }


        public BacktestModel BacktestSymbolsdata(IEnumerable<TemplateBasketStocks> model, int price, DateTime begDate, DateTime endDate)
        {
            BacktestModel mainres = new BacktestModel();

            try
            {
                List<BacktestModel> res = new List<BacktestModel>();
                List<SymbolData> data = new List<SymbolData>();
                double lot = 0;
                foreach (var items in model)
                {
                    BacktestModel item = new BacktestModel();

                    data = _context.SymbolData.Where(c => c.FundId == items.SymbolId).Where(s => s.Date >= begDate && s.Date <= endDate).ToList();
                    data.Reverse();

                    double prec = Convert.ToDouble(items.Perc);
                    double currentBalance = price * prec / 100;
                    if (data.Count() > 0)
                    {
                        lot = currentBalance / Convert.ToDouble(data[0].Value);
                        List<double> symdata = new List<double>();
                        List<DateTime> datelist = new List<DateTime>();
                        double alisfiyati = Convert.ToDouble(data[0].Value);
                        for (int i = 0; i < data.Count(); i++)
                        {
                            DateTime dt = Convert.ToDateTime(data[i].Date);
                            datelist.Add(dt);
                            double deger = Convert.ToDouble(data[i].Value);
                            double hesap = (deger - alisfiyati) * lot;
                            symdata.Add(currentBalance + hesap);
                        }
                        item.Data = symdata;
                        item.Date = datelist;
                        res.Add(item);
                    }
                }
                var ss = res.GroupBy(s => s.Date);
                mainres.Data = new List<double>();
                mainres.Date = res[0].Date;

                for (int a = 0; a < res[0].Data.Count; a++)
                {
                    double sum = 0;
                    for (int i = 0; i < res.Count; i++)
                    {
                        sum += res[i].Data[a];
                    }
                    sum = Math.Round(sum, 2);
                    mainres.Data.Add(sum);

                }
                mainres.Data.Reverse();
                mainres.Date.Reverse();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //BACKTESTİ AYLIK KESİTLER HALİNE GETİR
            List<double> filteredData = new List<double>();
            List<DateTime> filteredDates = new List<DateTime>();
            int lastmonth = 0;
            for (int i = 0; i < mainres.Data.Count; i++)
            {
                if (mainres.Date[i].Month != lastmonth)
                {
                    filteredData.Add(mainres.Data[i]);
                    filteredDates.Add(mainres.Date[i]);
                    lastmonth = mainres.Date[i].Month;
                }
            }
            mainres.Data = filteredData;
            mainres.Date = filteredDates;

            return mainres;
        }

        [HttpGet()]
        public async Task<IActionResult> Tracing(int userId, int? basketid, int? contractId)
        {
            TracingView res = null;
            List<int> contract = new List<int>();
            UserBaskets bsk = new UserBaskets();
            int templateBasketId = 0;
            try
            {
                if (basketid != null && basketid > 0)
                {
                    bsk = await _context.UserBaskets.FirstOrDefaultAsync(c => c.RecordId == Convert.ToInt32(basketid));
                    templateBasketId = bsk.TemplateBasketId ?? default(int);

                }
                else
                {
                    var baskets = await _context.UserBaskets.Where(c => c.UserId == userId).ToListAsync();

                    if (baskets.Count > 0)
                    {
                        if (contractId != null)
                        {
                            bsk = baskets.Where(x => x.ContractId == contractId).OrderByDescending(x => x.RecordDate).First();
                            templateBasketId = bsk.TemplateBasketId ?? default(int);
                        }
                        else
                        {
                            bsk = baskets.OrderByDescending(x => x.RecordDate).First();
                            templateBasketId = bsk.TemplateBasketId ?? default(int);
                        }

                        foreach (var item in baskets.Select(x => x.ContractId).GroupBy(c => c.Value).ToList())
                        {
                            if (item != null)
                            {
                                contract.Add(Convert.ToInt32(item.Key));
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                }

                if (bsk.RecordId > 0)
                {


                    res = new TracingView();

                    res.ContractID = contract;
                    res.SelectContract = Convert.ToInt32(bsk.ContractId);

                    var user = await _context.Users.Select(c => new { c.RecordId, c.RecordDate }).FirstOrDefaultAsync(a => a.RecordId == userId);

                    res.LoginCount = _context.Logins.Select(x => x.UserId == user.RecordId).ToList().Count().ToString();

                    List<UserBasketStocks> stocks = await _context.UserBasketStocks.Where(c => c.UserBasketId == bsk.RecordId).ToListAsync();

                    res.Assets = new List<BasketDetailItem>();
                    double total = 0;

                    foreach (var item in stocks)
                    {
                        var symbol = await _context.Symbols.FirstOrDefaultAsync(c => c.RecordId == item.SymbolId);
                        if (Math.Round((Convert.ToDouble(item.Lot) * Convert.ToDouble(item.AvgPrice))) > 0)
                            res.Assets.Add(new BasketDetailItem()
                            {
                                AvgPrice = item.AvgPrice.ToString(),
                                Lot = item.Lot.ToString(),
                                TotalAmount = Math.Round((Convert.ToDouble(item.Lot) * Convert.ToDouble(item.AvgPrice))).ToString(),
                                Name = symbol.Name,
                                Explanation = symbol.Explanation
                            });
                        total += (Convert.ToDouble(item.Lot) * Convert.ToDouble(item.AvgPrice));
                    }
                    var t_basket_id = bsk.TemplateBasketId;
                    double tot_perc = 0;
                    if (t_basket_id != null)
                    {
                        foreach (var t_symbol in _context.TemplateBasketStocks.Where(x => x.TemplateBasketId == t_basket_id).ToList())
                        {
                            tot_perc += t_symbol.Perc ?? default(int);
                        }
                        if (1 - tot_perc > 0)
                        {
                            var cash = Math.Round(Convert.ToDouble(bsk.ContractAmount) * Convert.ToDouble(1 - tot_perc));

                            res.Assets.Add(new BasketDetailItem
                            {
                                Name = "NAKİT",
                                Lot = (Convert.ToDouble(bsk.ContractAmount) * Convert.ToDouble(1 - tot_perc)).ToString(),
                                TotalAmount = Math.Round(Convert.ToDouble(bsk.ContractAmount) * Convert.ToDouble(1 - tot_perc)).ToString(),
                                Explanation = "NAKİT",
                                AvgPrice = 1.ToString()
                            });
                            total += cash;
                        }
                    }


                    res.Basket = AutoMapperBase._mapper.Map<UserBaskets, UserBasketsDto>(bsk);
                    res.RegisterDate = Convert.ToDateTime(user.RecordDate).ToString("dd MMMM yyyy");
                    res.SurveyDate = Math.Ceiling((Convert.ToDateTime(DateTime.Now) - Convert.ToDateTime(bsk.RecordDate)).TotalDays).ToString();
                    res.SurveyRenewal = "-";
                    res.FirstBalance = bsk.ContractAmount.ToString();
                    res.Balance = bsk.ContractAmount.ToString();

                    var basketId = _context.TemplateBaskets.OrderByDescending(a => a.RecordDate).FirstOrDefault(x => x.Name == bsk.Name).RecordId;

                    //BACKTEST DATASI HAZIRLA
                    BacktestModel prepareBacktest(int basketId2)
                    {
                        var backtest = _context.TemplateBasketBackTests.FirstOrDefault(x => x.TemplateBasketId == basketId2);
                        dynamic results = JsonConvert.DeserializeObject(backtest.Result);
                        List<double> data = new List<double>();
                        List<DateTime> date = new List<DateTime>();
                        var i = 0;
                        foreach (dynamic result in results)
                        {
                            if (DateTime.Now.AddYears(-1) <= Convert.ToDateTime(result.Date.Value))
                            {
                                i++;
                                if (i % 20 == 0)
                                {
                                    data.Add(result.Balance.Value);
                                    date.Add(Convert.ToDateTime(result.Date.Value));
                                }
                            }
                        }
                        data.Reverse();
                        date.Reverse();

                        return new BacktestModel
                        {
                            Basketname = bsk.Name,
                            Data = data,
                            Date = date
                        };

                    }
                    res.Backtest = new List<BacktestModel>();
                    res.Backtest.Add(prepareBacktest(basketId));
                    res.Backtest.Add(prepareBacktest(16429));

                    List<TemplateBasketStocks> stock = new List<TemplateBasketStocks>();

                    // https://localhost:44342/api/hisse/tracing?userid=3008&basketid=-1&contractid=2                    

                    res.LastBalance = Math.Round(total).ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
                // {
                //     new RoboLoggerService().LogError("RoboWebsiteService", userId, ex, "Tracing");
                // }));
            }

            // res.UnselectedBaskets = Website(userId);

            return Ok(res);
        }

        [HttpGet()]
        public IActionResult Investment(int BasketID, int ContractID, decimal Amount, int userId)
        {
            InvestmentModel iModel = new InvestmentModel();
            //http://localhost:5009/api/website/investment?basketId=30&contractId=1&amount=10000&userId=2007

            try
            {
                iModel.UserBasket = new UserBasketsDto();
                iModel.UserBasket.ContractAmount = Amount;
                iModel.UserBasket.ContractId = ContractID;
                iModel.UserBasket.PortfolioNotif = 0;
                iModel.SymbolList = new List<BasketSymbol>();
                iModel.ContractID = ContractID;

                iModel.BasketName = _context.TemplateBaskets.FirstOrDefault(c => c.RecordId == BasketID).Name;

                var stock = _context.TemplateBasketStocks.Where(c => c.TemplateBasketId == BasketID).ToList();

                var symbols = _context.Symbols.ToList();

                var sId = stock.Select(a => a.SymbolId).ToList();

                var symbolsdata = _context.SymbolData.Where(c => sId.Contains(c.FundId));

                double tot_percentage = 0;

                foreach (var item in stock)
                {
                    BasketSymbol model = new BasketSymbol();
                    model.Name = symbols.Where(s => s.RecordId == item.SymbolId).FirstOrDefault().Name;
                    model.Explanation = symbols.Where(s => s.RecordId == item.SymbolId).FirstOrDefault().Explanation;
                    model.Price = Amount * Convert.ToDecimal(item.Perc);
                    model.SymbolID = Convert.ToInt32(item.SymbolId);
                    DateTime dt = Convert.ToDateTime(DateTime.Now.ToString("dd.MM.yyyy"));

                    tot_percentage += item.Perc ?? default;

                    var data = "";

                    using (var httpClient = new HttpClient())
                    {
                        var name = _context.Symbols.FirstOrDefault(x => x.RecordId == item.SymbolId).Name;
                        var response = httpClient.GetAsync(@"http://185.122.200.217:6778/Data/GetCandleData?symbol=" + name + "&period=1&count=1").Result;
                        if (!response.IsSuccessStatusCode)
                        {
                            continue;
                        }
                        data = response.Content.ReadAsStringAsync().Result;
                        data = data.Replace("[", "").Replace("]", "");

                        Console.WriteLine(data);

                        var t = JsonConvert.DeserializeObject<TefasModel>(data);
                    }

                    //var data = symbolsdata.FirstOrDefault(s => s.FundId == item.SymbolId);//symbolsdata.Where(s => s.FundId == item.SymbolID && s.Date == dt).FirstOrDefault();
                    if (data != null)
                    {
                        TefasModel tmodel = JsonConvert.DeserializeObject<TefasModel>(data);
                        model.AvgPrice = Convert.ToDouble(tmodel.close);
                        model.Lot = Convert.ToInt32(Math.Floor(model.Price / Convert.ToDecimal(tmodel.close)));
                    }
                    else
                    {
                        model.Lot = 0;
                    }
                    iModel.SymbolList.Add(model);
                }
                if (1 - tot_percentage > 0)
                {
                    iModel.SymbolList.Add(new BasketSymbol
                    {
                        Name = "NAKİT",
                        SymbolID = 98000,
                        Explanation = "",
                        Price = Amount * Convert.ToDecimal(1 - tot_percentage),
                        AvgPrice = 1
                    });

                }

                List<int> contract = new List<int>();

                var baskets = _context.UserBaskets.Where(c => c.UserId == userId).ToList();

                foreach (var item in baskets.Select(x => x.ContractId).GroupBy(c => c.Value).ToList())
                {
                    if (item != null)
                    {
                        contract.Add(Convert.ToInt32(item.Key));
                    }
                }
                iModel.Contracts = new List<int>();
                iModel.Contracts = contract;

                iModel.BasketID = BasketID;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
                // {
                //     new RoboLoggerService().LogError("RoboWebsiteService", userId, ex, "Investment");
                // }));
            }

            return Ok(iModel);
        }

        [HttpPost()]
        public async Task<IActionResult> PostInvestment(InvestmentModel model)
        {
            try
            {
                model.UserBasket.RecordDate = DateTime.Now;
                model.UserBasket.Status = true;



                TemplateBaskets tmpbasket = await _context.TemplateBaskets.FirstAsync(c => c.RecordId == model.BasketID);   //getmodel.Type.GetI<TemplateBasketsDto>(model.BasketID);
                model.UserBasket.Name = tmpbasket.Name;
                model.UserBasket.Explanation = tmpbasket.Explanation;

                var usrBasket = AutoMapperBase._mapper.Map<UserBasketsDto, UserBaskets>(model.UserBasket);
                usrBasket.TemplateBasketId = tmpbasket.RecordId;

                _context.UserBaskets.Add(usrBasket);
                await _context.SaveChangesAsync();

                foreach (var item in model.SymbolList)
                {
                    UserBasketStocks items = new UserBasketStocks();
                    items.Lot = item.Lot;
                    items.RecordDate = DateTime.Now;
                    items.SymbolId = item.SymbolID;
                    items.AvgPrice = item.AvgPrice;
                    items.UserBasketId = usrBasket.RecordId;

                    _context.UserBasketStocks.Add(items);
                    await _context.SaveChangesAsync();
                }
                if (usrBasket.RecordId != 0)
                {
                    return Ok(usrBasket.RecordId);
                    //Mail Gönderim İşlemi
                    // SendMail("Deneme", "Başarılı bir şekilde işlem yapıldı.", "turkay@ekonteknoloji.com");
                    // res.message = "Başarılı";
                    // res.message2 = "Sözleşme başarılı bir şekilde kaydedildi.";
                }
                else
                {
                    return StatusCode(500);
                    // res.message = "Hata";
                    // res.message2 = "İşlemler sırasında bir hata meydana geldi.";
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500);
                // Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
                // {
                //     new RoboLoggerService().LogError("RoboWebsiteService", Convert.ToInt32(model.UserBasket.UserID), ex, "PostInvestment");
                // }));
                // res.message = "Hata";
                // res.message2 = "İşlemler sırasında bir hata meydana geldi.";
            }
            //return Ok();
        }


        [HttpGet()]
        public async Task<IActionResult> SurveyView()
        {
            List<SurveyQuestions> questions = new List<SurveyQuestions>();

            try
            {
                questions = await _context.SurveyQuestions.Where(c => c.RecordStatus == 1).OrderBy(c => c.ListOrder).ToListAsync();
            }
            catch (Exception)
            {

            }

            return Ok(questions);
        }


        [HttpGet()]
        public async Task<IActionResult> BacktestFromDB(bool createCsv = true)
        {
            var templates = _context.TemplateBaskets.ToList().OrderBy(x => x.RecordDate);
            var groups = templates.GroupBy(x => x.Name);

            List<BacktestResult> results = new List<BacktestResult>();

            foreach (var basketGroup in groups)
            {
                BacktestResult res = new BacktestResult();
                res.Name = basketGroup.Key;
                res.Start = basketGroup.First().RecordDate;
                res.End = basketGroup.Last().RecordDate;
                res.Balance = new List<double>();
                res.Profits = new List<double>();
                res.Dates = new List<DateTime>();
                res.Portfolio = new List<PortfolioItem>();
                res.PortfolioHistory = new List<List<PortfolioItem>>();

                res.Balance.Add(10000);
                res.Profits.Add(0);
                res.Dates.Add(Convert.ToDateTime(basketGroup.First().RecordDate));

                var baskets = basketGroup.ToList();
                List<int> dataSymbols = new List<int>();
                Dictionary<int, List<SymbolData>> datas = new Dictionary<int, List<SymbolData>>();
                List<Symbols> syms = new List<Symbols>();
                StringBuilder csv = new StringBuilder();

                for (int i = 0; i < baskets.Count; i++)
                {
                    if (i + 1 < baskets.Count && baskets[i + 1].RecordDate.Value.Date == baskets[i].RecordDate.Value.Date) continue;

                    var stocks = _context.TemplateBasketStocks.Where(x => x.TemplateBasketId == baskets[i].RecordId);
                    double lastestBalance = res.Balance.Last();
                    DateTime currentDate = Convert.ToDateTime(baskets[i].RecordDate).Date;
                    double newBalance = 0;

                    foreach (var stock in stocks)
                    {
                        if (dataSymbols.Count(x => x == stock.SymbolId) == 0)
                        {
                            dataSymbols.Add(Convert.ToInt32(stock.SymbolId));

                            using (var httpClient = new HttpClient())
                            {
                                try

                                {
                                    var sym = _context.Symbols.First(x => x.RecordId == stock.SymbolId);
                                    var response = await httpClient.GetAsync(@"http://185.122.200.217:6778/Data/GetCandleData?symbol=" + sym.Name + "&period=1440&count=500");
                                    var data1 = await response.Content.ReadAsStringAsync();
                                    var dynamicList = JsonConvert.DeserializeObject<List<dynamic>>(data1);

                                    List<SymbolData> data = new List<SymbolData>();

                                    foreach (var item in dynamicList)
                                    {
                                        SymbolData dt = new SymbolData() { FundId = stock.SymbolId, Date = Convert.ToDateTime(item.date), Value = Convert.ToDouble(item.close) };
                                        data.Add(dt);
                                    }

                                    datas.Add(Convert.ToInt32(stock.SymbolId), data);
                                    syms.Add(_context.Symbols.Find(Convert.ToInt32(stock.SymbolId)));
                                }
                                catch
                                {

                                }

                            }

                        }
                    }

                    if (res.Balance.Count == 1)
                    {
                        double totalPerc = 0;
                        foreach (var stock in stocks)
                        {
                            totalPerc += Convert.ToDouble(stock.Perc);

                            if (!datas.ContainsKey(Convert.ToInt32(stock.SymbolId)) || datas[Convert.ToInt32(stock.SymbolId)].Count(x => x.Date <= currentDate) == 0)
                            {
                                var totalLot = (res.Balance.Last() * stock.Perc) / 1;
                                var totalAmount = Convert.ToDouble(totalLot * 1);
                                res.Portfolio.Add(new PortfolioItem() { Cost = Convert.ToDouble(1), Lot = Convert.ToDouble(totalLot), SymbolID = Convert.ToInt32(stock.SymbolId), TotalAmount = totalAmount, SymbolName = "NAKİT", Perc = Convert.ToDouble(stock.Perc) });
                                newBalance += totalAmount;
                            }
                            else
                            {
                                var cost = datas[Convert.ToInt32(stock.SymbolId)].Where(x => x.Date <= currentDate).Last().Value;
                                var totalLot = (res.Balance.Last() * stock.Perc) / cost;
                                var totalAmount = Convert.ToDouble(totalLot * cost);
                                res.Portfolio.Add(new PortfolioItem() { Cost = Convert.ToDouble(cost), Lot = Convert.ToDouble(totalLot), SymbolID = Convert.ToInt32(stock.SymbolId), TotalAmount = totalAmount, SymbolName = syms.First(x => x.RecordId == stock.SymbolId).Sym, Perc = Convert.ToDouble(stock.Perc) });
                                newBalance += totalAmount;
                            }
                        }

                        if (totalPerc < 1)
                        {
                            var totalLot = (res.Balance.Last() * (1 - totalPerc)) / 1;
                            var totalAmount = Convert.ToDouble(totalLot * 1);
                            res.Portfolio.Add(new PortfolioItem() { Cost = Convert.ToDouble(1), Lot = Convert.ToDouble(totalLot), SymbolID = 0, TotalAmount = totalAmount, SymbolName = "NAKİT", Perc = Convert.ToDouble(1 - stocks.Sum(x => x.Perc)) });
                            newBalance += totalAmount;
                        }
                    }
                    else
                    {
                        foreach (var item in res.Portfolio)
                        {
                            if (!datas.ContainsKey(Convert.ToInt32(item.SymbolID)) || datas[Convert.ToInt32(item.SymbolID)].Count(x => x.Date <= currentDate) == 0)
                            {
                                var price = 1;
                                var newTotal = item.Lot * price;
                                newBalance += newTotal;
                            }
                            else
                            {
                                var price = datas[Convert.ToInt32(item.SymbolID)].Where(x => x.Date <= currentDate).Last().Value;
                                var newTotal = item.Lot * price;
                                newBalance += Convert.ToDouble(newTotal);
                                var profit = newTotal - item.TotalAmount;
                            }
                        }

                        res.Portfolio.Clear();

                        double totalPerc = 0;
                        foreach (var stock in stocks)
                        {
                            totalPerc += Convert.ToDouble(stock.Perc);

                            if (!datas.ContainsKey(Convert.ToInt32(stock.SymbolId)) || datas[Convert.ToInt32(stock.SymbolId)].Count(x => x.Date <= currentDate) == 0)
                            {
                                var cost = 1;
                                var totalLot = (newBalance * stock.Perc) / cost;
                                var totalAmount = Convert.ToDouble(totalLot * cost);
                                res.Portfolio.Add(new PortfolioItem() { Cost = Convert.ToDouble(cost), Lot = Convert.ToDouble(totalLot), SymbolID = Convert.ToInt32(stock.SymbolId), TotalAmount = totalAmount, SymbolName = "NAKİT", Perc = Convert.ToDouble(stock.Perc) });
                            }
                            else
                            {
                                var cost = datas[Convert.ToInt32(stock.SymbolId)].Where(x => x.Date <= currentDate).Last().Value;
                                var totalLot = (newBalance * stock.Perc) / cost;
                                var totalAmount = Convert.ToDouble(totalLot * cost);
                                res.Portfolio.Add(new PortfolioItem() { Cost = Convert.ToDouble(cost), Lot = Convert.ToDouble(totalLot), SymbolID = Convert.ToInt32(stock.SymbolId), TotalAmount = totalAmount, SymbolName = syms.First(x => x.RecordId == stock.SymbolId).Sym, Perc = Convert.ToDouble(stock.Perc) });
                            }
                        }

                        if (totalPerc < 1)
                        {
                            var totalLot = (newBalance * (1 - totalPerc)) / 1;
                            var totalAmount = Convert.ToDouble(totalLot * 1);
                            res.Portfolio.Add(new PortfolioItem() { Cost = Convert.ToDouble(1), Lot = Convert.ToDouble(totalLot), SymbolID = 0, TotalAmount = totalAmount, SymbolName = "NAKİT", Perc = Convert.ToDouble(1 - stocks.Sum(x => x.Perc)) });
                        }
                    }

                    res.Balance.Add(newBalance);
                    res.Profits.Add(newBalance - lastestBalance);
                    res.Dates.Add(currentDate);
                    string portstring = "";
                    foreach (var item in res.Portfolio)
                    {
                        portstring += item.SymbolName + "(Lot:" + Math.Round(item.Lot, 2) + " Maliyet:" + item.Cost + "), ";
                    }

                    List<PortfolioItem> items = new List<PortfolioItem>();
                    foreach (var item in res.Portfolio)
                    {
                        items.Add(new PortfolioItem() { Cost = item.Cost, Lot = item.Lot, SymbolID = item.SymbolID, SymbolName = item.SymbolName, TotalAmount = item.TotalAmount, Perc = item.Perc });
                    }
                    res.PortfolioHistory.Add(items);

                    csv.Append(currentDate.ToString() + ";" + portstring + ";" + Math.Round(res.Profits.Last(), 2) + ";" + Math.Round(res.Balance.Last(), 2) + Environment.NewLine);
                }

                if (createCsv)
                    System.IO.File.WriteAllText(res.Name + " Backtest.csv", csv.ToString(), Encoding.UTF8);
                results.Add(res);
            }
            dynamic results1 = JsonConvert.SerializeObject(results);

            System.IO.File.WriteAllText("Backtest.json", results1, Encoding.UTF8);


            return Ok(results);
        }


    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Helpers.Models.Dtos;
using Helpers.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RoboAdvisorApi.Mapper;
using RoboAdvisorApi.Models;

namespace RoboAdvisorApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FonController : ControllerBase
    {
        private readonly EkonRoboDBContext _context;

        public FonController(EkonRoboDBContext context)
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
                var usercategoryid = await _context.UserCategoryHistory.FirstOrDefaultAsync(x => x.UserId == userid);
                var categoryname = _context.RiskCategories.FirstOrDefault(a => a.RecordId == usercategoryid.CategoryId).CategoryName;

                // if (usercategoryid == null){
                //     usercategoryid.CategoryId = 1000;
                // }
                IEnumerable<int> returncategories =
                   (from bas in await _context.BasketCategory.ToListAsync()
                    where usercategoryid.CategoryId == bas.RiskCategoryId
                    select bas.TempBasketId.GetValueOrDefault());


                var baskets = await _context.TemplateBaskets
                    .Where(x => x.RecordDate == _context.TemplateBaskets.Min(z => z.RecordDate))
                    .ToListAsync();

                var basketsforreal = await _context.TemplateBaskets.Where(c => returncategories.Contains(c.RecordId)).Where(x => x.RecordDate > DateTime.Today).ToListAsync();
                if(basketsforreal.Count == 0)
                    basketsforreal = await _context.TemplateBaskets.Where(c => returncategories.Contains(c.RecordId)).Where(x => x.RecordDate > DateTime.Today.AddDays(-1)).ToListAsync();


                baskets.AddRange(basketsforreal);


                List<BacktestModel> backtestler = new List<BacktestModel>();

                foreach (var basket in baskets)
                {
                    try
                    {
                        var backtest = _context.TemplateBasketBackTests.FirstOrDefault(x => x.TemplateBasketId == basket.RecordId);
                        if (backtest == null)
                        {
                            continue;
                        }
                        dynamic results = JsonConvert.DeserializeObject(backtest.Result);
                        var taq = results[0].Balance;
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
                        if (data[0] > 10000)
                        {
                            var diff = data[0] - 10000;
                            for (
                                int ij = 0; ij < data.Count; ij++)
                            {
                                data[ij] = data[ij] - diff;
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
                    catch (Exception exx)
                    {
                        Console.WriteLine(exx);
                    }
                }
                baskets.RemoveAt(0);

                /*USDTRY data*/
                var data1 = "";
                dynamic usdtry;
                int j = 0;

                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(@"http://185.122.200.217:6778/Data/GetCandleData?symbol=USDTRY&period=1440&count=365");
                    data1 = await response.Content.ReadAsStringAsync();

                    usdtry = JsonConvert.DeserializeObject(data1);
                }
                List<double> datausd = new List<double>();
                List<DateTime> dateusd = new List<DateTime>();
                double base1 = 10000 / usdtry[0].close.Value;
                //var usdiff = base1 - 10000;
                foreach (var d in usdtry)
                {
                    if (DateTime.Now.AddYears(-1) <= Convert.ToDateTime(DateTime.Now))
                    {
                        Console.WriteLine(d);
                        j++;
                        if (j % 5 == 0) //5 idi
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
                /*eofBIST100 ve USDTRY data*/

                // ** DB data update !!!
                List<int> recIds = new List<int>();
                foreach (var bask in baskets)
                {
                    recIds.Add(bask.RecordId);
                }
                var basketsstocks =
                (from a in _context.TemplateBasketStocks.ToList()
                 where recIds.Contains(Convert.ToInt32(a.TemplateBasketId))
                 select a
                ).ToList();

                //****CASH
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
                //**** eof CASH

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

                /*user existing basket*/

                List<TemplateBasketStocks> existing = new List<TemplateBasketStocks>();
                ExistingPortfolioModel ex = new ExistingPortfolioModel();

                ex.SymbolId = new List<int>();
                ex.SymbolName = new List<string>();
                ex.SymPercentage = new List<double>();

                ex.SymbolId = new List<int> { 100, 121, 162, 133 };
                ex.SymbolName = new List<string> { "HSBC PORTFÖY ÇOKLU VARLIK İKINCI DEĞIŞKEN FON", "KT PORTFÖY BİRİNCİ KATILIM FONU", "İŞ PORTFÖY EMTIA YABANCI BYF FON SEPETI FONU", "OYAK PORTFÖY İKİNCİ DEĞİŞKEN FON" };
                ex.SymPercentage = new List<double> { };
                //backtestler[0].Basketname = "Mevcut Porföyünüz";

                var contract = await _context.UserBaskets.Where(c => c.UserId == userid).ToListAsync();

                List<int> contractID = new List<int>();
                foreach (var item in contract.Select(x => x.ContractId).GroupBy(x => x.Value).ToList())
                {
                    contractID.Add(item.Key);
                }

                var cate = await _context.UserCategoryHistory.Include(c => c.Category).FirstOrDefaultAsync(c => c.UserId == userid);

                WebSiteView wModel = new WebSiteView
                {
                    NameSurname = _context.Users.SingleOrDefault(c => c.RecordId == userid).NameSurname,
                    UserResult = AutoMapperBase._mapper.Map<UserCategoryHistory, UserCategoryHistoryDto>(cate),
                    TemplateBaskets = AutoMapperBase._mapper.Map<List<TemplateBaskets>, List<TemplateBasketsDto>>(baskets.ToList()),
                    BasketSymbols = AutoMapperBase._mapper.Map<List<Symbols>, List<SymbolsDto>>(symbol),
                    BackTest = backtestler,
                    TemplateBasketStock = AutoMapperBase._mapper.Map<List<TemplateBasketStocks>, List<TemplateBasketStocksDto>>(basketsstocks),
                    ContractID = contractID,
                    ExistingBasket = ex,
                    CategoryName = categoryname
                };
                return Ok(wModel);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        public class BacktestDatPoint
        {
            public DateTime date { get; set; }
            public double value { get; set; }

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

                    data = _context.SymbolData.Where(c => c.FundId == items.SymbolId).Where(s => s.Date >= begDate && s.Date <= endDate).OrderBy(x => x.Date).ToList();
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
                //mainres.Data.Reverse();
                //mainres.Date.Reverse();
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
                            var cash = Math.Round(Convert.ToDouble(bsk.ContractAmount) * Convert.ToDouble(1 - tot_perc) / 100);

                            res.Assets.Add(new BasketDetailItem
                            {
                                Name = "NAKİT",
                                Lot = (Convert.ToDouble(bsk.ContractAmount) * Convert.ToDouble(1 - tot_perc)).ToString(),
                                TotalAmount = Math.Round(Convert.ToDouble(bsk.ContractAmount) * Convert.ToDouble(1 - tot_perc) / 100).ToString(),
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
                                    if (basketId2 < 270)
                                    {
                                        data.Add(result.Balance.Value * 10);
                                    }
                                    else
                                        data.Add(result.Balance.Value / 10);
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
                    res.Backtest.Add(prepareBacktest(260));


                    //res.Backtest = new List<BacktestModel>();

                    //List<TemplateBasketStocks> stock = new List<TemplateBasketStocks>();

                    //foreach (var item in stocks)
                    //{
                    //    stock.Add(new TemplateBasketStocks
                    //    {
                    //        SymbolId = item.SymbolId,
                    //        Perc = Convert.ToDouble(Math.Round((Convert.ToDecimal(item.Lot * item.AvgPrice) / Convert.ToDecimal(bsk.ContractAmount)) * 100)),
                    //    });
                    //}

                    //var backtest = BacktestSymbolsdata(stock, Convert.ToInt32(bsk.ContractAmount), DateTime.Now.AddYears(-1), DateTime.Now);

                    //res.Backtest.Add(backtest);
                    // res.Backtest.Add(dnm);

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

            //res.UnselectedBaskets = Website(userId);

            return Ok(res);
        }

        [HttpGet()]
        public async Task<IActionResult> Investment(int BasketID, int ContractID, decimal Amount, int userId)
        {
            InvestmentModel iModel = new InvestmentModel();
            //http://localhost:5009/api/website/investment?basketId=1048&contractId=2&amount=7000&userId=4

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
                    model.Price = Amount * Convert.ToDecimal(item.Perc) / 100;
                    model.SymbolID = Convert.ToInt32(item.SymbolId);
                    DateTime dt = Convert.ToDateTime(DateTime.Now.ToString("dd.MM.yyyy"));
                    tot_percentage += item.Perc ?? default;
                    var data = symbolsdata.FirstOrDefault(s => s.FundId == item.SymbolId);//symbolsdata.Where(s => s.FundId == item.SymbolID && s.Date == dt).FirstOrDefault();
                    if (data != null)
                    {
                        model.AvgPrice = Convert.ToDouble(data.Value);
                        model.Lot = Convert.ToInt32(Math.Floor(model.Price / Convert.ToDecimal(data.Value)));
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
                        Price = Math.Round(Amount * Convert.ToDecimal(1 - tot_percentage) / 100),
                        AvgPrice = 1
                    });

                }

                List<int> contract = new List<int>();

                var baskets = await _context.UserBaskets.Where(c => c.UserId == userId).ToListAsync();

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
                Console.WriteLine(ex);
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

        [HttpPost()]
        public async Task<IActionResult> PostAnswers(List<Answers> answers)
        {
            //List<Answers>answers = (List<Answers>)lanswers;
            //MessageResult res = new MessageResult();

            int srvid = 0;
            await _context.BasketCategory.ToListAsync();
            MessageResult res = new MessageResult();

            try
            {
                string category = "";

                List<SurveyQuestions> allQuestions = _context.SurveyQuestions.Where(c => c.RecordStatus == 1).ToList(); //  getmodel.Type.Get <SurveyQuestions>();  //CrudGetOperations.GetSurveyQuestions().ToList();

                #region Soru Cevap Kontrol
                List<Answers> newanswers = new List<Answers>();
                foreach (var item in allQuestions.Where(x => x.RecordStatus == 1))
                {
                    Answers anw = new Answers();
                    anw = answers.FirstOrDefault(x => x.QuestionId == item.RecordId && x.Answer != "");
                    newanswers.Add(anw);
                }
                answers.Clear();
                answers = newanswers;

                #endregion                
                List<RiskCategories> riskCategories = _context.RiskCategories.ToList();
                UserCategoryHistory srv = new UserCategoryHistory();
                srv.RecordDate = DateTime.Now;
                srv.UserId = Convert.ToInt32(answers[0].UserId);

                _context.UserCategoryHistory.Add(srv);
                _context.SaveChanges();
                srvid = srv.RecordId;

                foreach (var item in newanswers)
                {
                    item.RecordDate = DateTime.Now;
                    item.SurveyId = srvid;
                    _context.Answers.Add(item);
                    _context.SaveChanges();
                }

                int totalScore = 0;
                int totalScoreQuestion = 0;

                foreach (var item in allQuestions.Where(x => x.RecordStatus == 1))
                {
                    if (!string.IsNullOrEmpty(item.ScoreProps))
                    {
                        List<string> scoreProps = item.ScoreProps.Split(',').ToList();
                        List<int> scores = new List<int>();
                        foreach (var score in scoreProps)
                        {
                            if (score.Contains(':'))
                            {
                                int a = Convert.ToInt32(score.Split(':')[1]);
                                scores.Add(a);
                            }
                            else
                            {
                                scores.Add(0);
                            }
                        }
                        int dnm = scores.OrderByDescending(x => x).ToList().First();
                        totalScoreQuestion += dnm;
                    }
                }

                foreach (var item in newanswers)
                {
                    try
                    {
                        var question = allQuestions.Find(x => x.RecordId == item.QuestionId);

                        if (!string.IsNullOrEmpty(question.ScoreProps))
                        {
                            List<string> scoreProps = question.ScoreProps.Split(',').ToList();

                            if (question.AnswerType == "choice")
                            {
                                if (scoreProps.Count(x => x.Contains(item.Answer)) > 0)
                                {
                                    totalScore += Convert.ToInt32(scoreProps.First(x => x.Contains(item.Answer)).Split(':')[1]);
                                }
                            }
                            else if (question.AnswerType == "number")
                            {
                                foreach (var prop in scoreProps)
                                {
                                    var operatorchar = prop.Substring(0, 1);
                                    var withoutOperator = prop.Substring(1, prop.Length - 1);

                                    if (operatorchar == "<" && Convert.ToSingle(item.Answer) < Convert.ToSingle(withoutOperator.Split(':')[0]))
                                    {
                                        totalScore += Convert.ToInt32(withoutOperator.Split(':')[1]);
                                        break;
                                    }
                                    else if (operatorchar == ">" && Convert.ToSingle(item.Answer) > Convert.ToSingle(withoutOperator.Split(':')[0]))
                                    {
                                        totalScore += Convert.ToInt32(withoutOperator.Split(':')[1]);
                                        break;
                                    }
                                    else if (operatorchar == "=" && Convert.ToSingle(item.Answer) == Convert.ToSingle(withoutOperator.Split(':')[0]))
                                    {
                                        totalScore += Convert.ToInt32(withoutOperator.Split(':')[1]);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        res.message = "Hata";
                        res.message2 = "İşlemler sırasında bir hata meydana geldi. " + e.Message;
                    }
                }

                var ab = (Convert.ToDecimal(totalScore) / Convert.ToDecimal(totalScoreQuestion)) * 100;
                totalScore = Convert.ToInt32(ab);

                foreach (var item in riskCategories)
                {
                    if (totalScore >= item.MinValue && totalScore <= item.MaxValue)
                    {
                        category = item.CategoryName;
                        UserCategoryHistory model = _context.UserCategoryHistory.FirstOrDefault(c => c.RecordId == srvid);
                        model.RecordDate = DateTime.Now;
                        model.CategoryId = item.RecordId;
                        model.UserId = answers[0].UserId;
                        model.UserScore = totalScore;
                        model.SurveyId = srvid;
                        _context.UserCategoryHistory.Update(model);
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                res.message = "Hata";
                res.message2 = "İşlemler sırasında bir hata meydana geldi. " + ex.Message;
            }

            if (srvid > 0)
            {
                res.message = "Başarılı";
                res.message2 = "Anket başarılı bir şekilde kaydedildi.";
            }

            return Ok(res);
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
                            List<SymbolData> data = _context.SymbolData.Where(x => x.FundId == stock.SymbolId).OrderBy(x => x.Date).ToList();
                            datas.Add(Convert.ToInt32(stock.SymbolId), data);
                            syms.Add(_context.Symbols.Find(Convert.ToInt32(stock.SymbolId)));
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
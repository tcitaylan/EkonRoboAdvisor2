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
using Newtonsoft.Json;

namespace RoboAdvisorHisse.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WebsiteController : ControllerBase
    {
        private readonly Urls _urls;
        public WebsiteController(Urls urls)
        {
            _urls = urls;
            Console.WriteLine(urls.Safeserver);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCategory(int userId)
        {
            var apiResponse = "TT";

            using (var httpClient = new HttpClient())
            {


                var response = await httpClient.GetAsync(_urls.Safeserver + "api/Hisse/getCategory/" + userId);

                apiResponse = await response.Content.ReadAsStringAsync();
                return Ok(apiResponse);
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetWebSiteModel(int userid)
        {
            var apiResponse = "";

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(_urls.Safeserver + "api/Hisse/GetWebSiteModel/" + userid);

                apiResponse = await response.Content.ReadAsStringAsync();
                return Ok(apiResponse);
            }
        }

        [HttpGet()]
        public async Task<IActionResult> Tracing(int userId, int? basketid, int? contractId)
        {
            var apiResponse = "";

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(_urls.Safeserver + "api/Hisse/Tracing?userId="
                    + userId + "&basketid=" + basketid + "&contractId=" + contractId);

                apiResponse = await response.Content.ReadAsStringAsync();
                return Ok(apiResponse);
            }
        }

        [HttpGet()]
        public async Task<IActionResult> Investment(int BasketID, int ContractID, decimal Amount, int userId)
        {
            var apiResponse = "";

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(_urls.Safeserver + "api/Hisse/Investment?BasketID="
                    + BasketID + "&ContractID=" + ContractID + "&Amount=" + Amount + "&userId=" + userId);

                apiResponse = await response.Content.ReadAsStringAsync();
                return Ok(apiResponse);
            }

        }

        [HttpGet()]
        public async Task<IActionResult> SurveyView()
        {
            var apiResponse = "";

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(_urls.Safeserver + "api/Hisse/SurveyView");

                apiResponse = await response.Content.ReadAsStringAsync();
                return Ok(apiResponse);
            }
        }

        [HttpPost()]
        public async Task<IActionResult> PostInvestment(InvestmentModel model)
        {
            var apiResponse = "";
            var postdata = JsonConvert.SerializeObject(model);

            HttpContent c = new StringContent(postdata, Encoding.UTF8, "application/json");

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(_urls.Safeserver + "api/Hisse/PostInvestment", c);

                apiResponse = await response.Content.ReadAsStringAsync();
                return Ok(apiResponse);
            }
        }

        [HttpPost()]
        public async Task<IActionResult> PostAnswers(List<AnswersDto> answers)
        {
            var apiResponse = "";
            var postdata = JsonConvert.SerializeObject(answers);

            HttpContent c = new StringContent(postdata, Encoding.UTF8, "application/json");

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(_urls.Safeserver + "api/Hisse/PostAnswers", c);

                apiResponse = await response.Content.ReadAsStringAsync();

                return Ok(apiResponse);
            }
        }


        public async Task<IActionResult> Register(UsersDto usersDto)
        {
            var apiResponse = "";
            var postdata = JsonConvert.SerializeObject(usersDto);

            HttpContent c = new StringContent(postdata, Encoding.UTF8, "application/json");

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(_urls.Safeserver + "api/Auth/Register", c);

                apiResponse = await response.Content.ReadAsStringAsync();
                return Ok(apiResponse);
            }

        }

        public async Task<IActionResult> Login(UsersDto usersDto)
        {
            var apiResponse = "";
            var postdata = JsonConvert.SerializeObject(usersDto);

            HttpContent c = new StringContent(postdata, Encoding.UTF8, "application/json");

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(_urls.Safeserver + "api/Auth/Login", c);

                apiResponse = await response.Content.ReadAsStringAsync();
                return Ok(apiResponse);
            }

        }
    }
}
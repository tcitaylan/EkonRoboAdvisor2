using AutoMapper;
using Helpers.Models.Dtos;
using RoboAdvisorApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoboAdvisorApi.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Answers, AnswersDto>();
            CreateMap<AnswersDto, Answers>();

            CreateMap<BasketCategory, BasketCategoryDto>();
            CreateMap<BasketCategoryDto, BasketCategory>();

            CreateMap<UserBasketStocks, UserBasketStocksDto>();
            CreateMap<UserBasketStocksDto, UserBasketStocks>();

            CreateMap<RiskCategories, RiskCategoriesDto>();
            CreateMap<RiskCategoriesDto, RiskCategories>();

            CreateMap<UserCategoryHistory, UserCategoryHistoryDto>();
            CreateMap<UserCategoryHistoryDto, UserCategoryHistory>();

            CreateMap<UserBaskets, UserBasketsDto>();
            CreateMap<UserBasketsDto, UserBaskets>();

            CreateMap<UserBaskets, UserBasketsDto>();
            CreateMap<UserBasketsDto, UserBaskets>();

            CreateMap<Users, UsersDto>();
            CreateMap<UsersDto, Users>();

            CreateMap<RecordLogs, RecordLogsDto>();
            CreateMap<RecordLogsDto, RecordLogs>();

            CreateMap<Exceptions, ExceptionsDto>();
            CreateMap<ExceptionsDto, Exceptions>();

            CreateMap<SurveyQuestions, SurveyQuestionsDto>();
            CreateMap<SurveyQuestionsDto, SurveyQuestions>();

            CreateMap<Symbols, SymbolsDto>();
            CreateMap<SymbolsDto, Symbols>();

            CreateMap<SymbolData, SymbolDataDto>();
            CreateMap<SymbolDataDto, SymbolData>();

            CreateMap<TemplateBaskets, TemplateBasketsDto>();
            CreateMap<TemplateBasketsDto, TemplateBaskets>();

            CreateMap<TemplateBasketStocks, TemplateBasketStocksDto>();
            CreateMap<TemplateBasketStocksDto, TemplateBasketStocks>();

            CreateMap<Logs, LogsDto>();
            CreateMap<LogsDto, Logs>();

            CreateMap<SpecialBaskets, SpecialBasketsDto>();
            CreateMap<SpecialBasketsDto, SpecialBaskets>();

            CreateMap<SpecialBasketStocks, SpecialBasketStocksDto>();
            CreateMap<SpecialBasketStocksDto, SpecialBasketStocks>();

            CreateMap<TemplateBasketBackTests, TemplateBasketBackTestsDto>();
            CreateMap<TemplateBasketBackTestsDto, TemplateBasketBackTests>();
        }
    }
}

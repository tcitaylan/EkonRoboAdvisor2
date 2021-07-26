using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RoboAdvisorApi.Mapper
{
    public static class AutoMapperBase
    {
        public static readonly IMapper _mapper;

        static AutoMapperBase()
        {
            var config = new MapperConfiguration(c => c.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();
        }

    }


}
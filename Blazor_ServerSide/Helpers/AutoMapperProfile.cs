// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 03.12.2020 12:10
// Developer      Benjamin Moser
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using AutoMapper;
using SIKOSI.Exchange.Model;
using SIKOSI.SampleDatabase02.Entities;

namespace SIKOSI.Sample01_WebApp.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AuthUserModel, TblUser>();
            CreateMap<TblUser, AuthUserModel>();
        }
    }
}
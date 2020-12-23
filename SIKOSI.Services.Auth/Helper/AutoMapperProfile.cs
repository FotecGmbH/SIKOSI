// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 07.12.2020 21:20
// Developer       Manuel Fasching
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using AutoMapper;
using SIKOSI.Exchange.Interfaces;
using SIKOSI.Exchange.Model;

namespace SIKOSI.Services.DB.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<IFile, ExFile>().ReverseMap();
        }
    }
}
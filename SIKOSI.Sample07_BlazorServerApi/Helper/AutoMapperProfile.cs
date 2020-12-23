// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 03.12.2020 22:14
// Developer      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using AutoMapper;
using SIKOSI.Exchange.Interfaces;
using SIKOSI.Exchange.Model;
using SIKOSI.SampleDatabase02.Entities;

namespace SIKOSI.Sample07_EncryptedChat.Helper
{
    /// <summary>
    /// The AutoMapperProfile class.
    /// </summary>
    public class AutoMapperProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapperProfile"/> class.
        /// </summary>
        public AutoMapperProfile()
        {
            CreateMap<AuthUserModel, TblUser>();
            CreateMap<TblUser, AuthUserModel>();
            CreateMap<User, TblUser>();
            CreateMap<IUser, TblUser>();
        }
    }
}
// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 04.12.2020 12:59
// Developer      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SIKOSI.Exchange.Interfaces;
using SIKOSI.Exchange.Model;
using SIKOSI.Services.Auth.Interfaces;
using SIKOSI.Services.DB.Interfaces;

namespace SIKOSI.Services.Auth
{
    public class UserFileService<TTblUserFiles, TDataContext> : UserService<TTblUserFiles, TDataContext>, IUserFileService
        where TTblUserFiles : class, ITblUserFiles
        where TDataContext : DbContext, IUserFilesDataContext<TTblUserFiles>
    {
        private readonly TDataContext _context;
        private readonly IMapper _mapper;

        public UserFileService(TDataContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region Interface Implementations

        public IEnumerable<IFile> GetAllUserFiles(int userId)
        {
            var userFiles = _context.UserFiles.Where(y => y.Id == userId).Include(x => x.Files);
            return userFiles?.FirstOrDefault()?.Files;
        }

        public IFile GetUserFile(int userId, int fileId)
        {
            return GetAllUserFiles(userId).FirstOrDefault(x => x.Id == fileId);
        }

        public void SaveUserFile(int userId, IFile file)
        {
            var exFile = _mapper.Map<ExFile>(file);

            var user = _context.UserFiles.Find(userId);

            user.Files.Add(exFile);

            _context.SaveChanges();
        }

        #endregion
    }
}
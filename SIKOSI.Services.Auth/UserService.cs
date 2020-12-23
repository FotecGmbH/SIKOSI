// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 24.11.2020 09:43
// Developer      Manuel Fasching
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
//using Sample0_Basic.Entities;
using SIKOSI.Exceptions;
using SIKOSI.Exchange.Interfaces;
//using SIKOSI.SampleDatabase01.Context;
using SIKOSI.Services.Auth.Interfaces;
using SIKOSI.Services.DB.Interfaces;

namespace SIKOSI.Services.Auth
{
    public class UserService<TTblUser, TDataContext> : IUserService 
        where TTblUser : class, ITblUser 
        where TDataContext : DbContext, IUserDataContext<TTblUser>
    {
        private readonly TDataContext _context;
        private readonly IMapper _mapper;

        public UserService(TDataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        ///     Creates a password hash for a given password
        /// </summary>
        /// <param name="password">password plain</param>
        /// <param name="passwordHash">password hash</param>
        /// <param name="passwordSalt">password salt</param>
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                    if (computedHash[i] != storedHash[i])
                        return false;
            }

            return true;
        }

        #region Interface Implementations

        public IUser Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.Users.SingleOrDefault(x => x.Username == username);

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // authentication successful
            return user;
        }

        public IEnumerable<IUser> GetAll()
        {
            return _context.Users;
        }

        /// <summary>
        ///     Gets a user by his ID
        /// </summary>
        /// <param name="id">ID of user</param>
        /// <returns>User</returns>
        public IUser GetById(int id)
        {
            var user = _context.Users.Find(id);

            if (user == null)
                return null;

            //Never show the password hash or salt
            user.PasswordHash = null;
            user.PasswordSalt = null;

            return user;
        }

        /// <summary>
        ///     Create a new user
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="password">Password</param>
        /// <returns>User</returns>
        public IUser Create(IUser user, string password)
        {
            //mapper must have a mapping for conversion of "IUser" to "TTblUser" must exist
            var u = _mapper.Map<TTblUser>(user);

            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (_context.Users.Any(x => x.Username == u.Username))
                throw new AppException("Username \"" + u.Username + "\" is already taken");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            u.PasswordHash = passwordHash;
            u.PasswordSalt = passwordSalt;

            _context.Users.Add(u);
            _context.SaveChanges();

            return user;
        }

        /// <summary>
        ///     Update User Information
        /// </summary>
        /// <param name="userParam">User</param>
        /// <param name="password">Password</param>
        public void Update(IUser userParam, string password = null)
        {
            var user = _context.Users.Find(userParam.Id);

            if (user is null)
                throw new AppException("User not found");

            // update username if it has changed
            if (!string.IsNullOrWhiteSpace(userParam.Username) && userParam.Username != user.Username)
            {
                // throw error if the new username is already taken
                if (_context.Users.Any(x => x.Username == userParam.Username))
                    throw new AppException("Username " + userParam.Username + " is already taken");

                user.Username = userParam.Username;
            }

            // update user properties if provided
            if (!string.IsNullOrWhiteSpace(userParam.FirstName))
                user.FirstName = userParam.FirstName;

            if (!string.IsNullOrWhiteSpace(userParam.LastName))
                user.LastName = userParam.LastName;

            // update password if provided
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        /// <summary>
        ///     Removes a user
        /// </summary>
        /// <param name="id">ID of User</param>
        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        #endregion
    }
}
// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 09.06.2020 11:27
// Developer      Manuel Fasching
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sample0_Basic.Entities;
using SIKOSI.SampleDatabase01.Context;

namespace SIKOSI.Sample01_Auth.Controllers
{

    /// <summary>
    /// User Controller for User Managmenet
    /// </summary>
    [ApiController]
    [Authorize(Roles = "Admin")] //Only Admin Members cann access this api
    public class UsersController : ControllerBase
    {
        //DB Context
        private readonly DataContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">DB Context</param>
        public UsersController(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get Users
        /// </summary>
        /// <returns>User</returns>
        [HttpGet]
        [Produces(typeof(IEnumerable<TblUser>))]
        [Route("/api/getusers/")]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _context.Users.ToListAsync());
        }

        /// <summary>
        /// Get a specific user
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/getuser/{id}")]
        public async Task<ActionResult<TblUser>> GetTblUser(int id)
        {
            var tblUser = await _context.Users.FindAsync(id);

            if (tblUser == null) return NotFound();

            return tblUser;
        }

        
        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="tblUser">user</param>
        /// <returns></returns>
        [HttpPut]
        [Route("/api/updateuser/{id}")]
        public async Task<IActionResult> PutTblUser(int id, TblUser tblUser)
        {
            if (id != tblUser.Id) return BadRequest();

            _context.Entry(tblUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TblUserExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Success status</returns>
        [HttpPost]
        [Route("/api/adduser")]
        public async Task<ActionResult<TblUser>> PostTblUser(TblUser user)
        {
            _ = await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTblUser",
                                   new { id = user.Id },
                                   user);
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("/api/deleteuser/{id}")]
        public async Task<ActionResult<TblUser>> DeleteTblUser(int id)
        {
            var tblUser = await _context.Users.FindAsync(id);
            if (tblUser == null) return NotFound();

            _context.Users.Remove(tblUser);
            await _context.SaveChangesAsync();

            return tblUser;
        }

        /// <summary>
        /// Check if a user exists
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        private bool TblUserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
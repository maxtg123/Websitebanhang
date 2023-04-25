using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Areas.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ShopContext _context;

        public UserController(ShopContext context)
        {
            _context = context;
        }

        // GET: api/User
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<UserModel>>> GetUserModel()
        //{
        //    //return await _context.UserModel.ToListAsync();
        //    return await _context.UserModel.ToListAsync();
        //}


        //[Authorize]
        //public IActionResult AdminEndPoints()
        //{
        //    var currentUser = GetCurrentUser();

        //    return Ok($"Hi {currentUser.UserName}, Chào mừng đến với...");
        //}

        //private UserModel GetCurrentUser()
        //{
        //    var identity = HttpContext.User.Identity as ClaimsIdentity;

        //    if (identity != null)
        //    {
        //        var userClaims = identity.Claims;

        //        return new UserModel
        //        {
        //            UserName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value,
        //            EmailAddress = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value,
        //            GiveName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.GivenName)?.Value,
        //            SurName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Surname)?.Value,
        //            Role = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value
        //        };
        //    }
        //    return null;
        //}

        //// GET: api/User/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<UserModel>> GetUserModel(int id)
        //{
        //    var userModel = await _context.UserModel.FindAsync(id);

        //    if (userModel == null)
        //    {
        //        return NotFound();
        //    }

        //    return userModel;
        //}

        //// GET: api/User/5
        //[HttpPut]
        //public async Task<ActionResult<UserModel>> Duyet(int id)
        //{
        //    var userModel = await _context.UserModel.FindAsync(id);

        //    if (userModel == null)
        //    {
        //        return NotFound();
        //    }

        //    return userModel;
        //}

        //// PUT: api/User/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutUserModel(int id, UserModel userModel)
        //{
        //    if (id != userModel.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(userModel).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!UserModelExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/User
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<UserModel>> PostUserModel(UserModel userModel)
        //{
        //    _context.UserModel.Add(userModel);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetUserModel", new { id = userModel.Id }, userModel);
        //}

        //// DELETE: api/User/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteUserModel(int id)
        //{
        //    var userModel = await _context.UserModel.FindAsync(id);
        //    if (userModel == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.UserModel.Remove(userModel);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        //private bool UserModelExists(int id)
        //{
        //    return _context.UserModel.Any(e => e.Id == id);
        //}
    }
}

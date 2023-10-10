using Employee.Contants;
using Employee.Dtos;
using Employee.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Employee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly EmployeesContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(EmployeesContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> CreateUser([FromBody] UserDto user)
        {
            var userExists = await _context.UserApp.Where(a => a.UserName == user.Gmail).FirstOrDefaultAsync();
            if (userExists != null)
            {
                return BadRequest("User exits");
            }
            var data = new UserApp()
            {
                UserName = user.Gmail,
                Email = user.Gmail,
                SecurityStamp = Guid.NewGuid().ToString(),

            };
            var result = await _userManager.CreateAsync(data, user.Password);

            if (result.Succeeded)
            {
                if (await _roleManager.RoleExistsAsync(UserRole.Admin))
                {
                    await _userManager.AddToRoleAsync(data, UserRole.User);
                }
                return Ok("Add User Success");
            }
            else
            {
                return BadRequest("Add User Fail");
            }
        }
    }
}

using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RealTimeCharts.Server.HubConfig;
using RealTimeCharts.Server.Models;
using RealTimeCharts.Server.Services;


namespace RealTimeCharts.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public IUserService usersService { get; set; }
        public IAuthService authService { get; }
        public IHubContext<ChartHub> _hub { get; }

        public AuthController(IUserService users, IAuthService authService, IHubContext<ChartHub> hub)
        {
            usersService = users;
            this.authService = authService;
            _hub = hub;
        }
        [HttpPost("register")]
        public ActionResult<RegResponce> Register(UserDto request)
        {
            if (!usersService.CheckIfAvailable(request.UserName)) return Ok(new RegResponce(false));
            authService.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            User user = new User {
                UserName = request.UserName,
                LastRequest = DateTime.Now.ToUniversalTime(),
                PersonalCode = Guid.NewGuid().ToString(),
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };            

            usersService.AddNew(user);
            _hub.Clients.All.SendAsync("activeUsers", usersService.Get());

            return Ok(new RegResponce(true));
        }

        [HttpPost("login")]

        public ActionResult<LoginReturnModel> Login(UserDto request)
        {
            var user = usersService.GetByUserName(request.UserName);
            if (user == null) return Ok(null);
            if(!authService.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return Ok(null);
            }
            var responce = new LoginReturnModel();
            user.PasswordSalt = new byte[1];
            user.PasswordHash = new byte[1];
            user.LastRequest = DateTime.Now.ToUniversalTime();
            responce.user = user;
            responce.Token = authService.CreateToken(user);
            return Ok(responce);
        }


        

    }
}

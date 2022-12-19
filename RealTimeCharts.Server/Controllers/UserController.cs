using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RealTimeCharts.Server.HubConfig;
using RealTimeCharts.Server.Models;
using RealTimeCharts.Server.Services;

namespace RealTimeCharts.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IHubContext<ChartHub> _hub;
        private readonly IUserService userservice;
        public UserController(IHubContext<ChartHub> hub, IUserService userservice)
        {
            _hub = hub;
            this.userservice = userservice;
        }
        [HttpPost]
        [Route("reg")]
        public ActionResult<User> Post([FromBody] string userName)
        {

            if (!userservice.CheckIfAvailable(userName))
            {
                return Ok("UserName Taken");
            }
            var newUser = new User()
            {
                UserName = userName,
                LastRequest = DateTime.Now.ToUniversalTime(),
                PersonalCode = Guid.NewGuid().ToString(),
            };
            userservice.AddNew(newUser);
            _hub.Clients.All.SendAsync("activeUsers", userservice.Get());
            return Ok(newUser);
        }
        [HttpGet, Authorize]
        public ActionResult<List<User>> Get()
        {
            var allUsers = userservice.Get();
            DateTime tmpdt = DateTime.Now.ToUniversalTime();
            return Ok(allUsers);
        }
        [HttpPost]
        [Route("logout")]
        public async void Logout([FromBody] string userName)
        {
            var tmp = userservice.GetByUserName(userName);
            userservice.DeleteUser(tmp.Id);
            await _hub.Clients.All.SendAsync("activeUsers", userservice.Get());
        }
        [HttpPost]
        [Route("upd")]
        public void Update([FromBody] string userName)
        {
            var tmp = userservice.GetByUserName(userName);
            if (tmp == null) return;
            userservice.UpdateUser(userName, tmp);
        }
        [HttpGet("{code}")]
        public ActionResult<User> UserByCode(string code)
        {
            return userservice.GetUserByCode(code);
        }
    }
}

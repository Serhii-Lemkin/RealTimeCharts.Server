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
        public IActionResult Post([FromBody] string userName)
        {

            if (!userservice.CheckIfAvailable(userName))
            {
                return Ok(new { Message = "UserName Taken" });
            }
            userservice.AddNew(new User()
            {
                UserName = userName,
                LastRequest = DateTime.Now.ToUniversalTime(),
                PersonalCode = Guid.NewGuid().ToString(),
            });
            var newUser = userservice.GetByUserName(userName);
            _hub.Clients.All.SendAsync(userName, newUser);
            _hub.Clients.All.SendAsync("activeUsers", userservice.Get());
            return Ok(new { Message = "Request Completed" });
        }
        [HttpGet]
        public ActionResult<List<User>> Get()
        {
            var allUsers = userservice.Get();
            foreach (var u in allUsers)
            {
                DateTime tmpdt = DateTime.Now.ToUniversalTime();
                if ((tmpdt - u.LastRequest).TotalMinutes > 15)
                {
                    userservice.DeleteUser(u.Id);
                }

            }
            return Ok(allUsers);
        }
        [HttpPost]
        [Route("logout")]
        public void Logout([FromBody] string userName)
        {
            var tmp = userservice.GetByUserName(userName);
            userservice.DeleteUser(tmp.Id);
            _hub.Clients.All.SendAsync("activeUsers", userservice.Get());
        }
    }
}

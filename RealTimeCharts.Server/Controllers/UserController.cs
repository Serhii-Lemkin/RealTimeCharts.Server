using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RealTimeCharts.Server.HubConfig;
using RealTimeCharts.Server.Models;
using RealTimeCharts.Server.Services;
using System.Security.Claims;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace RealTimeCharts.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize]
    public class UserController : Controller
    {
        private readonly IHubContext<ChartHub> _hub;
        private readonly IUserService userservice;
        public UserController(IHubContext<ChartHub> hub, IUserService userservice)
        {
            _hub = hub;
            this.userservice = userservice;


        }

        [HttpGet("/me")]
        public ActionResult<User> GetMe()
        {
            string userName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
            if (userName == null) { return BadRequest(); }
            var me = userservice.GetByUserName(userName);
            if (me == null) return Ok();
            me.PasswordHash = new byte[1];
            me.PasswordSalt = new byte[1];
            return Ok(me);
        }

        [HttpGet]
        public ActionResult<List<User>> Get()
        {

            string userName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)!.Value;
            if (userName != null) { Update(userName); }

            var allUsers = userservice.GetActive();

            _hub.Clients.All.SendAsync("activeUsers", allUsers);

            //allUsers = allUsers.Where(x => (DateTime.Now.ToUniversalTime() - x.LastRequest).TotalMinutes < 5).ToList();
            //_hub.Clients.All.SendAsync("activeUsers", allUsers);
            return Ok(allUsers);
        }
        [HttpPost]
        [Route("logout")]
        public async void Logout([FromBody] string userName)
        {
            var tmp = userservice.GetByUserName(userName);
            tmp.LastRequest = DateTime.Now.AddDays(-1);
            userservice.UpdateUser(tmp.Id, tmp);
            var allUsers = userservice.GetActive();

            await _hub.Clients.All.SendAsync("activeUsers", allUsers);
        }

        [HttpPost]
        [Route("upd")]
        public void Update([FromBody] string userName)
        {
            var tmp = userservice.GetByUserName(userName);
            if (tmp == null) return;
            tmp.LastRequest = DateTime.Now.ToUniversalTime();
            userservice.UpdateUser(tmp.Id, tmp);
        }
        [HttpGet("{code}")]
        public ActionResult<User> UserByCode(string code)
        {
            return userservice.GetUserByCode(code);
        }
    }
}

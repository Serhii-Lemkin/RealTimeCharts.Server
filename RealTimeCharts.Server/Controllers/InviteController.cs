﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RealTimeCharts.Server.HubConfig;
using RealTimeCharts.Server.Models;
using RealTimeCharts.Server.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RealTimeCharts.Server.Controllers
{
    [Route("api/[controller]"), Authorize]
    [ApiController]
    public class InviteController : ControllerBase
    {
        private readonly IHubContext<InviteHub> _hub;
        private readonly IUserService userservice;
        public InviteController(IHubContext<InviteHub> hub, IUserService userservice)
        {
            _hub = hub;
            this.userservice = userservice;
        }

        // POST api/<InviteController>
        [HttpPost("{username}")] 
        public async Task<ActionResult<User>> Post(string username, [FromBody] string code)
        {
            User user = userservice.GetUserByCode(code);
            await _hub.Clients.All.SendAsync(username, user.UserName);
            return Ok(user);
        }
        [HttpPost("react/{username}")]
        public async Task<ActionResult<User>> ReactToInvite(string username, [FromBody] string responce)
        {
            User u = userservice.GetByUserName(username);
            await _hub.Clients.All.SendAsync(u.PersonalCode, responce);
            return Ok(u);
        }

    }
}

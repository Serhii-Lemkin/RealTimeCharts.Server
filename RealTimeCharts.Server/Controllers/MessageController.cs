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
    public class MessageController : ControllerBase
    {
        private IHubContext<MessageHub> _hub;
        private IUserService userservice;

        public MessageController(IHubContext<MessageHub> hub, IUserService userservice)
        {
            _hub = hub;
            this.userservice = userservice;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Message message)
        {
            if (message == null) return BadRequest(string.Empty);
            await _hub.Clients.All.SendAsync(message.Code, message);
            return Ok();
        }
        [HttpPost("ttt")]
        public async Task<IActionResult> StartTTT([FromBody] GameInitModel action)
        {
            if (action.Action == "start")
            {
                await _hub.Clients.All.SendAsync(action.Code, action.Action);
                return Ok();
            }
            return BadRequest(string.Empty);
        }
    }
}

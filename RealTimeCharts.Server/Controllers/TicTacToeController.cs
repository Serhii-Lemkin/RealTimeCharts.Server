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
    [ApiController, Authorize]
    public class TicTacToeController : ControllerBase
    {
        private readonly IHubContext<TicTacToeHub> hub;
        private readonly IUserService userservice;

        public TicTacToeController(IHubContext<TicTacToeHub> hub, IUserService userservice)
        {
            this.hub = hub;
            this.userservice = userservice;
        }

        [HttpPost]
        public IActionResult NextMove([FromBody] TTTMove move)
        {
            hub.Clients.All.SendAsync(move.RoomId, move);
            return Ok();
        }
        
        [HttpPost("restart/{roomId}")]
        public IActionResult RestartGame([FromBody] string res, string roomId)
        {
            hub.Clients.All.SendAsync(roomId, res);
            return Ok();
        }

    }
}

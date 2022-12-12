using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RealTimeCharts.Server.HubConfig;
using RealTimeCharts.Server.Services;

namespace RealTimeCharts.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicTacToeController : ControllerBase
    {
        private readonly IHubContext<TicTacToeHub> hub;
        private readonly IUserService userservice;

        public TicTacToeController(IHubContext<TicTacToeHub> hub, IUserService userservice)
        {
            this.hub = hub;
            this.userservice = userservice;
        }
        
    }
}

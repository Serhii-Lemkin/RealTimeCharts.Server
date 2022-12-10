using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using RealTimeCharts.Server.HubConfig;
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
    }
}

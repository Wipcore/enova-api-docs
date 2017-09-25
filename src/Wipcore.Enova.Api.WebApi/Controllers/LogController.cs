using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wipcore.eNova.Api.WebApi.Services;
using Wipcore.Enova.Api.OAuth;
using Wipcore.Enova.Api.WebApi.Services;

namespace Wipcore.Enova.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class LogController : ApiController
    {
        [HttpGet("ApiLogs")]
        [Authorize(Roles = AuthService.AdminRole)]
        public JsonResult ApiLogs(int level = 0)
        {
            var logs = ArrayNlogTarget.GetLog().Where(x => level == 0 || x.Level.Ordinal >= level).Reverse().Select(x => x.Message);
            return Json(logs);
        }
    }
}

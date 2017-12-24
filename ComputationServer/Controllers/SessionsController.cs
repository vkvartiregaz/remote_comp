using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ComputationServer.WebService.Entities;
using ComputationServer.Data.Entities;
using ComputationServer.Execution.Control;

namespace ComputationServer.WebService.Controllers
{
    public class SessionsController : Controller
    {
        private ExecutionManager _executionManager = new ExecutionManager();
        ILogger _logger;

        public SessionsController(ILogger logger)
        {
            _logger = logger;
        }

        [HttpPost("session")]
        public async Task<IActionResult> StartSession([FromBody]Session session)
        {
            //TODO: record into db

            if (!checkStartParameters(session))
            {
                _logger.LogTrace(Resource.LogBadParams);
                return BadRequest(Resource.ResponseBadParams);
            }
            
            bool started;

            try
            {
                started = _executionManager.StartSession(session);
            }
            catch(Exception ex)
            {
                _logger.LogError(Resource.LogException);
                return BadRequest(Resource.ResponseRequestFailed);
            }

            if (started)
            {
                _logger.LogTrace(Resource.LogRequestFailed);
                return BadRequest(Resource.ResponseRequestFailed);                
            }

            _logger.LogTrace(Resource.LogSuccess);
            return Ok(new SessionStarted { SessionId = session.Id });
        }

        [HttpGet("/session/{sessionId}")]
        public async Task<IActionResult> CheckSession(int sessionId)
        {
            Session result;

            try
            {
                result = _executionManager.CheckSession(sessionId);
            }
            catch
            {
                _logger.LogError(Resource.LogException);
                return BadRequest(Resource.ResponseRequestFailed);
            }

            _logger.LogTrace(Resource.LogSuccess);
            return Ok(result);
        }

        [HttpDelete("/session/{sessionId}")]
        public async Task<IActionResult> StopSession(int sessionId)
        {
            //TODO: record into db

            bool result;
            
            try
            {
                result = _executionManager.StopSession(sessionId);
            }
            catch
            {
                _logger.LogError(Resource.LogException);
                return BadRequest(Resource.ResponseRequestFailed);
            }
            
            if (!result)
            {
                _logger.LogTrace(Resource.LogRequestFailed);
                return BadRequest(Resource.ResponseRequestFailed);
            }

            _logger.LogTrace(Resource.LogSuccess);
            return Ok();
        }

        [HttpPut("session/{sessionId}")]
        public async Task<IActionResult> ModifySession([FromRoute] int sessionId, 
            [FromBody]ModifySessionParameters parameters)
        {
            //TODO: record into db

            if (!checkModifyParameters(parameters))
            {
                _logger.LogTrace(Resource.LogBadParams);
                return BadRequest(Resource.ResponseBadParams);
            }

            var session = new Session
            {
                Id = sessionId,
                Budget = parameters.Budget,
                Deadline = parameters.Deadline
            };

            bool result;

            try
            {
                result = _executionManager
                    .ModifySession(session);
            }
            catch(Exception ex)
            {
                _logger.LogError(Resource.LogException);
                return BadRequest(Resource.ResponseRequestFailed);
            }

            if(!result)
            {
                _logger.LogTrace(Resource.LogRequestFailed);
                return BadRequest(Resource.ResponseRequestFailed);
            }

            _logger.LogTrace(Resource.LogSuccess);
            return Ok();
        }

        private bool checkStartParameters(Session parameters)
        {
            if (parameters == null)
                return false;

            if (parameters.Deadline <= DateTime.Now)
                return false;

            if (parameters.Budget < 0)
                return false;

            if (parameters.CompGraph == null)
                return false;

            if (!parameters.CompGraph.IsValid())
                return false;

            return true;
        }

        private bool checkModifyParameters(ModifySessionParameters parameters)
        {
            if (parameters == null)
                return false;

            if (parameters.Deadline <= DateTime.Now)
                return false;

            if (parameters.Budget < 0)
                return false;

            return true;
        }
    }
}

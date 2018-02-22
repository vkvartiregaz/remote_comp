﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ComputationServer.Data.Entities;

namespace ComputationServer.WebService.Controllers
{
    [Route("api/sessions")]
    public class SessionsController : Controller
    {
        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            return Ok($"get works on id = {id}!");
        }

        // POST api/<controller>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Session session)
        {
            return Ok($"post works for sessionId = {session.Id}");
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody]Session session)
        {
            return BadRequest("put does not work and is not yet intended to");
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            return Ok($"delete works for id = {id}");
        }
    }
}

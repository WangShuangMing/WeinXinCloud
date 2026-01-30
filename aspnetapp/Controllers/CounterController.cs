#nullable disable
using aspnetapp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;

public class CounterResponse
{
    public string Data { get; set; }
}

namespace aspnetapp.Controllers
{
    [Route("api")]
    [ApiController]
    public class CounterController : ControllerBase
    {
        private readonly CounterContext _context;

        public CounterController(CounterContext context)
        {
            _context = context;
        }
        private async Task<Counter> getCounterWithInit(int userId)
        {
            var counters = await _context.Counters.ToListAsync();
            if (counters.Count() > 0)
            {
                return counters[0];
            }
            else
            {
                var counter = new Counter { createdAt = DateTime.Now, updatedAt = DateTime.Now };
                _context.Counters.Add(counter);
                await _context.SaveChangesAsync();
                return counter;
            }
        }
        // GET: api/count
        [HttpGet("userlist")]
        public async Task<ActionResult<CounterResponse>> GetUserList()
        {
            var counters = await _context.Counters.ToListAsync();
            return new CounterResponse { Data = JsonConvert.SerializeObject(counters) };
        }
        [HttpGet("user")]
        public async Task<ActionResult<CounterResponse>> GetUser(int userId)
        {
            var counter = await getCounterWithInit(userId);
            return new CounterResponse { Data = counter.data };
        }

        // POST: api/Counter
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("userdata")]
        public async Task<ActionResult<CounterResponse>> PostUserData(int userId, string data)
        {
            try
            {
                var counters = await _context.Counters.ToListAsync();
                var userData = counters.Find(user => userId == user.id);
                if (userData == null)
                {
                    userData = new Counter()
                    {
                        userId = userId,
                        data = data,
                        createdAt = DateTime.Now,
                        updatedAt = DateTime.Now
                    };
                    await _context.AddAsync(userData);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    userData.data = data;
                }
                return new CounterResponse { Data = data };
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

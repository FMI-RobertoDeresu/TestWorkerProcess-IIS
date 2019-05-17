using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TestWorkerProcess.Ws.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet("get/{id}")]
        public async Task<ActionResult<string>> Get(int id)
        {
            var sw = Stopwatch.StartNew();
            await BusyWait(10000);

            var elapsed = sw.ElapsedMilliseconds;
            return Ok(new { id, elapsed });
        }

        [HttpGet("get2/{id}")]
        public async Task<ActionResult<string>> Get2(int id)
        {
            var sw = Stopwatch.StartNew();
            await BusyWait2(10000);

            var elapsed = sw.ElapsedMilliseconds;
            return Ok(new { id, elapsed });
        }

        public async Task BusyWait(int milliseconds)
        {
            await Task.Run(() =>
            {
                var sw = Stopwatch.StartNew();
                while (sw.ElapsedMilliseconds < milliseconds)
                {
                    Thread.SpinWait(1000);
                }
            });
        }

        public async Task BusyWait2(int milliseconds)
        {
            await Task.Run(() =>
            {
                Thread.Sleep(milliseconds);
            });
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OnlineExamAPI.BL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OnlineExamAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowMyOrigin")]
    public class studentController : ControllerBase
    {
        ExamBL em = new ExamBL();
        // GET: api/<studentController>
        [AllowAnonymous]
        [HttpPost]
        [Route("StudentLogin")]
        public IActionResult StudentLogin([FromHeader] string userName, [FromHeader] string password)
        {
            var result1 = em.StudentLogin(userName);
            return Ok(result1);
        }
    }
}

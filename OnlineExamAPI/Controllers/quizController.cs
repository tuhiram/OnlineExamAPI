using Microsoft.AspNetCore.Authorization;
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
    public class quizController : ControllerBase
    {
        ExamBL qz = new ExamBL();
       
        [AllowAnonymous]
        [HttpPost]
        [Route("getquizquestion")]
        public IActionResult GetAllQuestion([FromHeader] string examid)
        {
            var result1 = qz.quizquestions(examid);
            return Ok(result1);
        }
        [HttpGet]
        [Route("GetQuestionById/{id}")]
        public IActionResult GetQuestionById(string id)
        {
            try
            {
                var result1 = qz.GetQuestionById(id);
                return Ok(result1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("getsectionlist")]
        public IActionResult GetSectionList()
        {
            try
            {
                var result1 = qz.GetSectionList();
                return Ok(result1);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

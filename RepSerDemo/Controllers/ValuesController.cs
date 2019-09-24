using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Models;
using RepSerDemo.Auth;
//using Service;

namespace RepSerDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {


        readonly ITestService _testService;

        public ValuesController(ITestService testService)//构造方法注入
        {
            _testService = testService;
        }
         
        /// <summary>
        /// 数据库查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "Get")]
        [Authorize]
        public async Task<List<Notes>> Get(int id)
        {
            //ITestService testService = new TestService(); 
            //return await testService.Query(d => d.Id == id);


            return await _testService.Query(d=>d.Id==id);
        }


        /// <summary>
        /// 计算
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        [HttpGet]
        public int Get(int i, int j)
        {
            //var tservice = new TestService();
            //return tservice.Sum(i, j);

            return _testService.Sum(i, j);
        }


        /// <summary>
        /// 获取JWT字符串
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetJWTStr")]
        public string GetJWTStr()
        {
            return new JwtHelper().GetJwtStr();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Models;
using Newtonsoft.Json;
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
        [HttpGet]
        [Route("GetData")]
        [Authorize]
        public async Task<object> GetData(int id)
        {
            //ITestService testService = new TestService(); 
            //return await testService.Query(d => d.Id == id);


            var val = await _testService.Query(d => d.Id == id);
            var models = JsonConvert.SerializeObject(val);
            var data = new { success = true, data = models };
            return data;
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
        public object GetJWTStr(string name, string pass)
        {
            if (name == "admins" && pass == "admins")
            {
                var jwt = new JwtHelper().BuildJwtToken();
                return jwt;
            }
            else
            {
                string message = "login fail!!!";
                bool suc = false;
                return new { success = suc, message = message };
            }
        }

        /// <summary>
        /// 请求刷新Token（以旧换新）
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("RefreshToken")]
        public object RefreshToken(string token = "")
        {
            string jwtStr = string.Empty;

            if (string.IsNullOrEmpty(token))
            {
                return new { success = false, message = "token无效，请重新登录！" };
            }
            return new JwtHelper().BuildJwtToken();
        }

    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using IService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
//using Repository.sugar;
using RepSerDemo.Auth;
//using Service;
using Swashbuckle.AspNetCore.Swagger;

namespace RepSerDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //BaseDBConfig.ConnectionString = Configuration.GetSection("AppSettings:SqlServerConnection").Value;
            JwtHelper.Configuration = Configuration;

            //services.AddCors();
            //services.AddCors(c=> {
            //    c.AddPolicy("AllRequests", policy =>
            //    {
            //        policy.AllowAnyOrigin()
            //        .AllowAnyMethod()
            //        .AllowAnyHeader()
            //        .AllowCredentials();
            //    });
            //});

            #region SwaggerService
            services.AddSwaggerGen(o =>
            {
                #region SwaggerDoc基本配置
                o.SwaggerDoc("v1", new Info
                {
                    Version = "1.0",
                    Description = "异步泛型仓储+服务+抽象接口",
                    Title = "异步泛型仓储+服务+抽象接口 API",
                    Contact = new Contact { Email = "xx.com", Name = "qishuai", Url = "ff.com" }
                });


                var bathPath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;

                var filePath = Path.Combine(bathPath, "RepSerDemo.xml");
                o.IncludeXmlComments(filePath, true);

                var modelFilePath = Path.Combine(bathPath, "Model.xml");
                o.IncludeXmlComments(modelFilePath);
                #endregion

                #region Token绑定到ConfigureServices
                //添加header验证信息
                var security = new Dictionary<string, IEnumerable<string>> { { "api", new string[] { } } };

                o.AddSecurityRequirement(security);

                o.AddSecurityDefinition("api", new ApiKeyScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
                    Name = "Authorization",//jwt默认的参数名称
                    In = "header",//jwt默认存放Authorization信息的位置(请求头中)
                    Type = "apiKey"
                });
                #endregion
            });
            #endregion

            #region jwt


            //读取配置文件
            var jwtConfig = Configuration.GetSection("JWT");
            var symmetricKeyAsBase64 = jwtConfig["Secret"];
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);


            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);



            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtConfig["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtConfig["Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true
            };

            services.AddAuthentication(a =>
            {
                a.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                a.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(j =>
              {
                  j.TokenValidationParameters = tokenValidationParameters;
              });
            #endregion

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //NetCore 自带的注入
            //services.AddSingleton<ITestService, TestService>();

            #region Autofac 要将方法返回对象改为 IServiceProvider
            //实例化Autofac容器
            var builder = new ContainerBuilder();

            #region 1.单个组件注入
            // 注册要通过反射创建的组件
            //builder.RegisterType<TestService>().As<ITestService>();
            #endregion

            #region 2.Load 模式,注入程序集
            //// 要记得!!!这个注入的是实现类层，不是接口层！不是 IServices
            //var assemblysServices = Assembly.Load("Service");
            ////指定已扫描程序集中的类型注册为提供所有其实现的接口。
            //builder.RegisterAssemblyTypes(assemblysServices).AsImplementedInterfaces();

            //var assemblysRepositorys = Assembly.Load("Repository");
            //builder.RegisterAssemblyTypes(assemblysRepositorys).AsImplementedInterfaces();
            #endregion

            #region 3. Assembly.LoadFile和 Assembly.LoadFrom 
            /* Assembly.LoadFile只载入相应的dll文件， 比如Assembly.LoadFile("a.dll")，则载入a.dll，
             *      假如a.dll中引用了b.dll的话，b.dll并不会被载入。
             * Assembly.LoadFrom则不一样，它会载入dll文件及其引用的其他dll，比如上面的例子，b.dll也会被载入。
             */
            var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;
            var serviceDllFile = Path.Combine(basePath, "Service.dll");
            var assemblyService = Assembly.LoadFrom(serviceDllFile);
            builder.RegisterAssemblyTypes(assemblyService).AsImplementedInterfaces();

            var repositoryDllFile = Path.Combine(basePath, "Repository.dll");
            var assemblysRepositorys = Assembly.LoadFrom(repositoryDllFile);
            builder.RegisterAssemblyTypes(assemblysRepositorys).AsImplementedInterfaces();
            #endregion

            //将services添加到Autofac容器生成器
            builder.Populate(services);

            //使用已进行的组件登记创建新容器
            var applicationContainer = builder.Build();

            //第三方IOC接管 core内置DI容器
            return new AutofacServiceProvider(applicationContainer);
            #endregion 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }
            //app.UseCors("AllRequests");

            //app.UseCors(c=> {
            //    c.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials();
            //});

            app.UseSwagger();
            app.UseSwaggerUI(u =>
            {
                u.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                u.RoutePrefix = ""; 
            });


            app.UseAuthentication();
            app.UseMvc();
        }
    }
}

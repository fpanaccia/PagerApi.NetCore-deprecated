# PagerApi.NetCore

This project implements a pager in net core, whitout having the need to touch the parameters of your methods.

# How to implement it in your project

You need to add to your "Configure" method in startup this System.Web.HttpContext.Configure(app.ApplicationServices.GetRequiredService<Microsoft.AspNetCore.Http.IHttpContextAccessor>());

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            System.Web.HttpContext.Configure(app.ApplicationServices.GetRequiredService<Microsoft.AspNetCore.Http.IHttpContextAccessor>());
        }

It HAS to be below "app.UseMvc();", some times its necesary to add "services.AddHttpContextAccessor();" in "ConfigureServices",

For more information, follow this example -> https://github.com/fpanaccia/PagerApi.NetCore.Example

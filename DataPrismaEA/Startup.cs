using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MQTTnet.AspNetCore;
using DataPrismaEA.Services;
using Demo.AspNetCore.PushNotifications.Services;
using DataPrismaEA.Formatters;
using Newtonsoft.Json.Converters;
using MQTTnet.Server;
using System;

namespace DataPrismaEA
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            //this adds a hosted mqtt server to the services
            //services.AddHostedMqttServer(builder => builder.WithDefaultEndpointPort(1883));

            //this adds tcp server support based on Microsoft.AspNetCore.Connections.Abstractions
            services.AddMqttConnectionHandler();

            //this adds websocket support
            services.AddMqttWebSocketServerAdapter();

            services.AddPushSubscriptionStore(Configuration)
                .AddPushNotificationService(Configuration)
                .AddPushNotificationsQueue();

            services.AddScoped<IMQTTClientService, MQTTClientService>();

            services.AddControllersWithViews(options =>
            {
                options.InputFormatters.Add(new TextPlainInputFormatter());
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });


            MQTTnet.AspNetCore.ServiceCollectionExtensions.AddHostedMqttServer(services, new MqttServerOptions { });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Console.WriteLine("Configuring backend.");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            defaultFilesOptions.DefaultFileNames.Clear();
            defaultFilesOptions.DefaultFileNames.Add("push-notifications.html");

            app.UseDefaultFiles(defaultFilesOptions)
                .UseStaticFiles()
                .UsePushSubscriptionStore()
                .UseRouting()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            app.UseMqttEndpoint();
            app.UseWebSockets();
            app.UseHttpsRedirection();
            //app.UseMvc();
        }
    }
}

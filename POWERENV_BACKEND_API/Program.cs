using Microsoft.AspNetCore.Authentication.Cookies;
using POWER_ENV.GLOBAL.NETWORK;
using POWERENV_BACKEND_API.Redis;
using POWERENV_BACKEND_API.SignalR;
using StackExchange.Redis;

namespace POWERENV_BACKEND_API
{
    public class Program
    {
        public struct STRUCT_REQUEST_DATA
        {
            /// <summary> true: ok; false: problems occured. </summary>
            public bool operationStatus { get; set; }
            /// <summary> Detailed message about the operation status. </summary>
            public string statusMessage { get; set; }
            public object packetData { get; set; }
        }

        public struct STRUCT_NETWORK_INTERFACE_CONFIG_PACKED_DATA
        {
            public int eth_index { get; set; }
            public List<ENUM_STATIC_NETWORK_PROPERTIES> changedProperties { get; set; }
            public List<string> newValues { get; set; }
            public string IPAddressType { get; set; }
        }

        public struct STRUCT_IP_ADDRESSES_PERMITIONS_DATA
        {
            public List<int> indexes { get; set; }
            public List<string> IPAddresses { get; set; }
        }

        public static void Main(string[] args)
        {
            string? hostIPAddress = Environment.GetEnvironmentVariable("POWERENV_HOST_IPADDRESS");
            Console.WriteLine($"POWERENV_HOST_IPADDRESS: {hostIPAddress}");

            if (hostIPAddress == null) {
                Console.WriteLine("POWERENV_HOST_IPADDRESS environment variable is not set.");
                Console.WriteLine("Please set the environment variable and restart the application.");
                return;
            }

            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel(serverOptions =>
            {
                serverOptions.ListenAnyIP(5000); // Listens on all interfaces (equivalent to 0.0.0.0 and [::])
            });

            // Add services to the container.
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://powerenv.hyperfield.com", $"https://{hostIPAddress}")
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            builder.Services.AddControllers();

            /*builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                return ConnectionMultiplexer.Connect(config.GetConnectionString("RedisCache"));
            });*/

            builder.AddRedisDistributedCache("RedisCache");

            // 3. Configure Security perimeter policies
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = ".PowerEnvAuthToken";
                    options.Cookie.HttpOnly = true;               // Blocks XSS read exploits
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Demands HTTPS
                    options.Cookie.SameSite = SameSiteMode.Strict;   // Defends against CSRF forged state attacks
                    options.ExpireTimeSpan = TimeSpan.FromHours(2);
                    options.SessionStore = new RedisAuthCookieTicketStore(builder.Services.BuildServiceProvider());
                });

            builder.Services.AddSignalR();

            builder.Services.AddHostedService<TaskGatewayBKGService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.MapHub<OS_TERMINAL_WSS_HUB>("/osTerminal");

            app.UseCors("AllowFrontend");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
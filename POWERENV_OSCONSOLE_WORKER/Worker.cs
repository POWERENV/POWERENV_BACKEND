using POWERENV_PGSQL_DB_HANDLER;
using StackExchange.Redis;

namespace POWERENV_OSCONSOLE_WORKER
{
    public class Worker : BackgroundService
    {
        // ===========================================FIELDS===========================================
        // Logging
        private readonly ILogger<Worker> _logger;

        //Data Source Handling
        private POWERDB_PGSQL_DATA_HANDLING DB_HANDLER;
        private IConnectionMultiplexer redisCache;

        private OSSESSION_HANDLING osSessionHandler;

        // ===========================================CONSTRUCTOR===========================================

        /// <summary>
        /// Worker's main Class Constructor.
        /// </summary>
        /// <param name="logger">Logger Object.</param>
        /// <param name="redis">Redis Object.</param>
        public Worker(ILogger<Worker> logger, IConnectionMultiplexer redis)
        {
            _logger = logger;
            redisCache = redis;
            DB_HANDLER = new POWERDB_PGSQL_DATA_HANDLING("C:\\DATA\\DATA\\ficheiros\\documentos\\Informática\\programação\\C#\\IBM_PSERIES_HARDWARE_MANAGEMENT_SERVICE\\POWERENV_APP\\POWERENV_BACKEND_API\\bin\\Debug\\net8.0\\");

            osSessionHandler = new OSSESSION_HANDLING(_logger, redisCache, DB_HANDLER);
        }

        // ===========================================METHODS===========================================

        /// <summary>
        /// This method executes along the lifetime of the worker service, continuously checking when it needs to trigger a certain action.
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await osSessionHandler.checkForNewQueuedOSSessions();
                await osSessionHandler.checkForCloseSessions();
                await osSessionHandler.checkForSessionCommands();
                await Task.Delay(200, stoppingToken);
            }
        }
    }
}
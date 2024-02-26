namespace viki_01.Services
{
    public class TokenCleanerService : IHostedService, IDisposable
    {
        private readonly IServiceProvider services;
        private readonly ILogger<TokenCleanerService> logger;
        private Timer timer;

        public TokenCleanerService(IServiceProvider services, ILogger<TokenCleanerService> logger)
        {
            this.services = services;
            this.logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Token cleaner service is up");
            timer = new Timer(DeleteExpiredTokens, null, TimeSpan.Zero, TimeSpan.FromHours(1));

            return Task.CompletedTask;
        }

        private void DeleteExpiredTokens(object state)
        {
            logger.LogInformation("Scan and delete expired tokens");
            using (var scope = services.CreateScope())
            {
                var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
                authService.DeleteExpiredTokens();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
            timer = null;
        }
    }

}

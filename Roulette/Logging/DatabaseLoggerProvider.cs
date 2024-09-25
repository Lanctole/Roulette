using Roulette.Data;

namespace Roulette.Logging;

public class DatabaseLoggerProvider : ILoggerProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ApplicationDbContext _context;

    public DatabaseLoggerProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ILogger CreateLogger(string categoryName)
    {
        var context = _serviceProvider.GetRequiredService<ApplicationDbContext>();
        return new DatabaseLogger(context, categoryName);
    }

    public void Dispose()
    {

    }
}
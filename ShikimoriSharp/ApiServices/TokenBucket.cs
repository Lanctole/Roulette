using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace ShikimoriSharp.ApiServices;

public class TokenBucket : IDisposable
{
    private readonly SemaphoreSlim _sem;
    private readonly Timer _timer;
    private readonly object _lock = new();

    public TokenBucket(string name, int maxTokens, double refreshTime)
    {
        Name = name;
        MaxTokens = maxTokens;
        RefreshTime = refreshTime;

        _sem = new SemaphoreSlim(maxTokens, maxTokens);
        _timer = new Timer(refreshTime)
        {
            AutoReset = true
        };
        _timer.Elapsed += Refresh;
    }

    public string Name { get; }
    public int MaxTokens { get; }
    public double RefreshTime { get; }

    private void Refresh(object sender, ElapsedEventArgs args)
    {
        lock (_lock)
        {
            if (_sem.CurrentCount < MaxTokens)
            {
                _sem.Release();
            }
        }
    }

    public async Task TokenRequest()
    {
        if (!_timer.Enabled)
        {
            _timer.Start();
        }
        await _sem.WaitAsync();
    }

    public void Dispose()
    {
        _timer?.Dispose();
        _sem?.Dispose();
    }
}
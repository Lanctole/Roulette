using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace ShikimoriSharp.ApiServices;

/// <summary>
///     Класс для управления токенами с использованием алгоритма "Token Bucket".
///     Этот класс помогает регулировать частоту запросов, обеспечивая ограничение на количество токенов, доступных за определённое время.
/// </summary>
public class TokenBucket : IDisposable
{
    private readonly SemaphoreSlim _sem;
    private readonly Timer _timer;
    private readonly object _lock = new();

    /// <summary>
    ///     Инициализирует новый экземпляр класса <see cref="TokenBucket"/>.
    /// </summary>
    /// <param name="name">Имя бакета для идентификации.</param>
    /// <param name="maxTokens">Максимальное количество токенов, которые бакет может содержать.</param>
    /// <param name="refreshTime">Время в миллисекундах, через которое бакет будет обновляться.</param>
    /// <param name="logger">Логгер для записи информации о состоянии и действиях бакета.</param>
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

    /// <summary>
    ///     Метод, вызываемый для обновления количества токенов в бакете.
    ///     Если количество доступных токенов меньше максимального, добавляется один токен.
    /// </summary>
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

    /// <summary>
    ///     Запрашивает токен из бакета. Ожидает, пока токен станет доступен, если бакет пуст.
    ///     Если таймер ещё не запущен, он будет запущен.
    /// </summary>
    /// <returns>Задача, представляющая асинхронную операцию.</returns>
    public async Task TokenRequest()
    {
        if (!_timer.Enabled)
        {
            _timer.Start();
        }
        await _sem.WaitAsync();
    }

    /// <summary>
    ///     Освобождает ресурсы, используемые бакетом, и останавливает таймер.
    /// </summary>
    public void Dispose()
    {
        _timer?.Dispose();
        _sem?.Dispose();
    }
}
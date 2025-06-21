﻿using CleverDocs.Core.Abstractions.Documents;
using Microsoft.Extensions.Hosting;

namespace CleverDocs.Infrastructure.Documents;

public class QueuedHostedService : BackgroundService
{
    private readonly  IBackgroundTaskQueue _taskQueue;
    private readonly Serilog.ILogger _logger;
    
    public QueuedHostedService(
        IBackgroundTaskQueue taskQueue,
        Serilog.ILogger logger)
    {
        _taskQueue = taskQueue;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var workItem = await _taskQueue.DequeueAsync(stoppingToken);
            try
            {
                await workItem(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred executing background task.");
            }
        }
    }
}
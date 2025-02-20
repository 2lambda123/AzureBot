﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using System.Threading;

namespace AzureBot
{
    [Serializable]
    public class AzureBotLuisDialog<R> : LuisDialog<R>
    {
        public AzureBotLuisDialog(params ILuisService[] services) : base(services)
        {
        }

        public async Task<bool> CanHandle(string query, CancellationToken cancellationToken)
        {
            var tasks = services.Select(async(s) => await s.QueryAsync(query, cancellationToken)).ToArray();
            var results = await Task.WhenAll(tasks);

            var winners = from result in results.Select((value, index) => new { value, index })
                          let resultWinner = BestIntentFrom(result.value)
                          where resultWinner != null
                          select new LuisServiceResult(result.value, resultWinner, this.services[result.index]);

            var winner = this.BestResultFrom(winners);
            return winner != null && winner.BestIntent.Intent != "None";
        }
    }
}
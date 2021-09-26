using System;
using System.Collections.Generic;
using System.Linq;

namespace Refactoring.FirstExampleTests
{
    public partial class Theater
    {
        internal class StatementData
        {
            public string Customer { get; set; }
            public IEnumerable<Performance> Performances { get; set; }
            
            public int TotalAmount { get; set; }
            public int TotalVolumeCredits { get; set; }
        }
        
        private Performance EnrichPerformance(Performance performance)
        {
            var p = performance.Clone();
            p.Play = PlayFor(p);
            p.Amount = AmountFor(p);
            p.VolumeCredits = VolumeCreditsFor(p);
            return p;
        }
        
        private int TotalAmount(StatementData data)
        {
            return data.Performances.Sum(perf => perf.Amount);
        }

        private int TotalVolumeCredits(StatementData data)
        {
            return data.Performances.Sum(perf => perf.VolumeCredits);
        }
        
        private int VolumeCreditsFor(Performance perf)
        {
            int result = 0;
            // add volume credits
            result += Math.Max(perf.Audience - 30, 0);
            // add extra credit for every ten comedy attendees
            if ("comedy" == perf.Play.Type) result += (int) Math.Floor((double) perf.Audience / 5);
            return result;
        }

        private Play PlayFor( Performance perf)
        {
            return _plays[perf.PlayId];
        }

        private int AmountFor(Performance aPerformance)
        {
            int result;
            switch (aPerformance.Play.Type)
            {
                case "tragedy":
                    result = 40000;
                    if (aPerformance.Audience > 30)
                    {
                        result += 1000 * (aPerformance.Audience - 30);
                    }

                    break;
                case "comedy":
                    result = 30000;
                    if (aPerformance.Audience > 20)
                    {
                        result += 10000 + 500 * (aPerformance.Audience - 20);
                    }

                    result += 300 * aPerformance.Audience;
                    break;
                default:
                    throw new Exception($"unknown type: {aPerformance.Play.Type}");
            }

            return result;
        }
    }
}
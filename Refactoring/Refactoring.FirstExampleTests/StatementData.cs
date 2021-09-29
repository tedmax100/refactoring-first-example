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
        
        private StatementData CreateStatementData(Invoice invoice)
        {
            var statementData = new StatementData
            {
                Customer = invoice.Customer,
                Performances = invoice.Performances.Select(p => EnrichPerformance(p)).ToList()
            };
            statementData.TotalAmount = TotalAmount(statementData);
            statementData.TotalVolumeCredits = TotalVolumeCredits(statementData);
            return statementData;
        }
        
        private Performance EnrichPerformance(Performance performance)
        {
            var calculator = new PerformanceCalculator(performance, PlayFor(performance));
            var p = performance.Clone();
            p.Play = calculator.Play;
            p.Amount = calculator.Amount();
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
    }
}
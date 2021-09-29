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
            p.VolumeCredits = calculator.VolumeCredits();//VolumeCreditsFor(p);
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

        private Play PlayFor( Performance perf)
        {
            return _plays[perf.PlayId];
        }

        private int AmountFor(Performance aPerformance)
        {
           return new PerformanceCalculator(aPerformance, PlayFor(aPerformance)).Amount();
        }
        
        public class PerformanceCalculator
        {
            public Performance Performance;
            public Play Play { get; }
            public PerformanceCalculator(Performance perf, Play play)
            {
                this.Performance = perf;
                this.Play = play;
            }
            
            public int Amount()
            {
                int result;
                switch (this.Play.Type)
                {
                    case "tragedy":
                        result = 40000;
                        if (this.Performance.Audience > 30)
                        {
                            result += 1000 * (Performance.Audience - 30);
                        }

                        break;
                    case "comedy":
                        result = 30000;
                        if (Performance.Audience > 20)
                        {
                            result += 10000 + 500 * (Performance.Audience - 20);
                        }

                        result += 300 * Performance.Audience;
                        break;
                    default:
                        throw new Exception($"unknown type: {Performance.Play.Type}");
                }

                return result;
            }
            
            public int VolumeCredits( )
            {
                int result = 0;
                // add volume credits
                result += Math.Max(Performance.Audience - 30, 0);
                // add extra credit for every ten comedy attendees
                if ("comedy" == Play.Type) result += (int) Math.Floor((double) Performance.Audience / 5);
                return result;
            }
        }
    }
    
}
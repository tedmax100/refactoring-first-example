using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Refactoring.FirstExampleTests
{
    public class Theater
    {
        internal class StatementData
        {
            public string Customer { get; set; }
            public IEnumerable<Performance> Performances { get; set; }
            
            public int TotalAmount { get; set; }
            public int TotalVolumeCredits { get; set; }
        }
        
        private Dictionary<string, Play> _plays;
        public string Statement(Invoice invoice, Dictionary<string, Play> plays)
        {
            _plays = plays;
            return RenderPlainText(CreateStatementData(invoice), plays);
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
            var p = performance.Clone();
            p.Play = PlayFor(p);
            p.Amount = AmountFor(p);
            p.VolumeCredits = VolumeCreditsFor(p);
            return p;
        }

        private string RenderPlainText(StatementData data, Dictionary<string, Play> plays)
        {
            _plays = plays;

            var result = $"Statement for {data.Customer}\r\n";

            foreach (var perf in data.Performances)
            {
                // print line for this order
                result += $" {perf.Play.Name}: {USD(perf.Amount)} ({perf.Audience} seats)\r\n";
            }

            result += $"Amount owed is {USD(data.TotalAmount)}\r\n";
            result += $"You earned {data.TotalVolumeCredits} credits";
            return result;
        }

        private int TotalAmount(StatementData data)
        {
            return data.Performances.Sum(perf => perf.Amount);
        }

        private int TotalVolumeCredits(StatementData data)
        {
            return data.Performances.Sum(perf => perf.VolumeCredits);
        }

        private string USD(int thisAmount)
        {
            return IntlNumberFormat("en-US", "C", "$", 2)(thisAmount / 100);
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

        /// <summary>
        /// In Fowler's Refactoring book the example was written in Javascript.
        /// The format function was implemented as follows:
        /// <code>const format = new Intl.NumberFormat(...)</code>
        /// To keep the example more or less the same I used the Format method to format the amounts.
        /// </summary>
        /// <param name="locale"></param>
        /// <param name="style"></param>
        /// <param name="currency"></param>
        /// <param name="minimalFractionDigits"></param>
        /// <returns></returns>
        private Func<int, string> IntlNumberFormat(string locale, string style, string currency, int minimalFractionDigits)
        {
            var format = new CultureInfo(locale, false).NumberFormat;
            format.CurrencyDecimalDigits = minimalFractionDigits;
            format.CurrencySymbol = currency;
            format.CurrencyDecimalSeparator = ".";
            format.CurrencyGroupSeparator = ",";

            return s => s.ToString(style, format);
        }
    }
}
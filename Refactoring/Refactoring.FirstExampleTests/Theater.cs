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
        }
        
        private Dictionary<string, Play> _plays;
        public string Statement(Invoice invoice, Dictionary<string, Play> plays)
        {
            var statementData = new StatementData() {Customer = invoice.Customer, Performances = EnrichPerformance(invoice)};
            return RenderPlainText(statementData, plays);
        }

        private IEnumerable<Performance> EnrichPerformance(Invoice invoice)
        {
            return invoice.Performances.Select(p => p);
        }

        private string RenderPlainText(StatementData data, Dictionary<string, Play> plays)
        {
            _plays = plays;

            var result = $"Statement for {data.Customer}\r\n";

            foreach (var perf in data.Performances)
            {
                // print line for this order
                result += $" {PlayFor(perf).Name}: {USD(AmountFor(perf))} ({perf.Audience} seats)\r\n";
            }

            result += $"Amount owed is {USD(TotalAmount(data))}\r\n";
            result += $"You earned {TotalVolumeCredits(data)} credits";
            return result;
        }

        private int TotalAmount(StatementData data)
        {
            var result = 0;
            foreach (var perf in data.Performances)
            {
                result += AmountFor(perf);
            }

            return result;
        }

        private int TotalVolumeCredits(StatementData data)
        {
            var volumeCredits = 0;
            foreach (var perf in data.Performances)
            {
                volumeCredits += VolumeCreditsFor(perf);
            }

            return volumeCredits;
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
            if ("comedy" == PlayFor(perf).Type) result += (int) Math.Floor((double) perf.Audience / 5);
            return result;
        }

        private Play PlayFor( Performance perf)
        {
            return _plays[perf.PlayId];
        }

        private int AmountFor(Performance aPerformance)
        {
            int result;
            switch (PlayFor(aPerformance).Type)
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
                    throw new Exception($"unknown type: {PlayFor(aPerformance).Type}");
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
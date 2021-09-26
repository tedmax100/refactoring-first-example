using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Refactoring.FirstExampleTests
{
    public partial class Theater
    {
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
        
        private string USD(int thisAmount)
        {
            return IntlNumberFormat("en-US", "C", "$", 2)(thisAmount / 100);
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
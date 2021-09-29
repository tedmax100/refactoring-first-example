using System;

namespace Refactoring.FirstExampleTests
{
    public class PerformanceCalculator
    {
        public Performance Performance { get; set; }

        public  Play Play { get; set; }
        public PerformanceCalculator(Performance perf, Play play)
        {
            this.Performance = perf;
            this.Play = play;
        }
        
        public int Amount()
        {
            int result;
            switch (Play.Type)
            {
                case "tragedy":
                    result = 40000;
                    if (Performance.Audience > 30)
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
                    throw new Exception($"unknown type: {Play.Type}");
            }

            return result;
        }
        
        public int VolumeCredits()
        {
            int result = 0;
            // add volume credits
            result += Math.Max(this.Performance.Audience - 30, 0);
            // add extra credit for every ten comedy attendees
            if ("comedy" == Play.Type) result += (int) Math.Floor((double) Performance.Audience / 5);
            return result;
        }
    }
}
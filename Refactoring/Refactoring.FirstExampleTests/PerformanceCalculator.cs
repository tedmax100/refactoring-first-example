using System;
using System.Diagnostics;

namespace Refactoring.FirstExampleTests
{
    public abstract class PerformanceCalculator
    {
        public Performance Performance { get; set; }

        public  Play Play { get; set; }
        public PerformanceCalculator(Performance perf, Play play)
        {
            this.Performance = perf;
            this.Play = play;
        }
        
        public static PerformanceCalculator CreatePerformanceCalculator(Performance perf, Play play)
        {
            switch (play.Type)
            {
                case "tragedy":
                    return new TragedyCalculator(perf, play);
                case "comedy":
                    return new ComedyCalculator(perf, play);
                default:
                    throw new Exception($"unknown type: {play.Type}");
            }
        }
        
        public virtual int Amount()
        {
            throw new Exception("subclass responsibility");
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

    public class TragedyCalculator : PerformanceCalculator
    {
        public TragedyCalculator(Performance perf, Play play) : base(perf, play)
        { }
        
        public override int Amount()
        {
            var result = 40000;
            if (Performance.Audience > 30)
            {
                result += 1000 * (Performance.Audience - 30);
            }

            return result;
        }
    }
    
    public class ComedyCalculator : PerformanceCalculator
    {
        public ComedyCalculator(Performance perf, Play play) : base(perf, play)
        { }
        
        public override int Amount()
        {
            var result = 30000;
            if (Performance.Audience > 20)
            {
                result += 10000 + 500 * (Performance.Audience - 20);
            }

            result += 300 * Performance.Audience;

            return result;
        }
    }
}
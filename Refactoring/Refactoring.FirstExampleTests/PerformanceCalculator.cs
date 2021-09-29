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
    }
}
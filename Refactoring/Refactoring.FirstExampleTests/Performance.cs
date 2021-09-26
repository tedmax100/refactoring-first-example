namespace Refactoring.FirstExampleTests
{
    public class Performance
    {
        public string PlayId { get; }
        public Play Play { get; set; }
        public int Audience { get; }

        public int Amount { get; set; }
        
        public int VolumeCredits { get; set; }
        public Performance(string playId, int audience)
        {
            PlayId = playId;
            Audience = audience;
        }

        public Performance Clone()
        {
            return (Performance) this.MemberwiseClone();
        }
    }
}
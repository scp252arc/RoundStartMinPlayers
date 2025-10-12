using System.ComponentModel;

namespace RoundStartMinPlayers.Configs
{
    public class Config
    {
        [Description("Minimum players needed to start the round.")]
        public int MinPlayers { get; set; } = 2;
    }
}
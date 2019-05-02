namespace SS.Reward.Core
{
    public class ConfigInfo
    {
        public bool IsEnabled { get; set; } = false;
        public decimal DefaultAmount { get; set; } = 2;
        public string Description { get; set; } = "如果觉得我们的文章对您有用，请随意打赏。您的支持将鼓励我们继续创作！";
    }
}
using System;
using Datory;

namespace SS.Reward.Core
{
    [Table("ss_reward_record")]
	public class RecordInfo : Entity
	{
        [TableColumn]
        public int SiteId { get; set; }

        [TableColumn]
        public int ChannelId { get; set; }

        [TableColumn]
        public int ContentId { get; set; }

        [TableColumn]
        public string Message { get; set; }

        [TableColumn]
        public decimal Amount { get; set; }

        [TableColumn]
        public string OrderNo { get; set; }

        [TableColumn]
        public bool IsPayed { get; set; }

        [TableColumn]
        public DateTime? AddDate { get; set; }
    }
}

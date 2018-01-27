using System;

namespace SS.Reward.Model
{
	public class RecordInfo
	{
        public int Id { get; set; }

        public int PublishmentSystemId { get; set; }

        public int ChannelId { get; set; }

        public int ContentId { get; set; }

        public string Message { get; set; }

        public decimal Amount { get; set; }

        public string OrderNo { get; set; }

        public bool IsPaied { get; set; }

        public DateTime AddDate { get; set; }
    }
}

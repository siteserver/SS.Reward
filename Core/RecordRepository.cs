using System.Collections.Generic;
using Datory;
using SiteServer.Plugin;

namespace SS.Reward.Core
{
    public class RecordRepository
    {
        private readonly Repository<RecordInfo> _repository;
        public RecordRepository()
        {
            _repository = new Repository<RecordInfo>(Context.Environment.DatabaseType, Context.Environment.ConnectionString);
        }

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(RecordInfo.Id);
            public const string SiteId = nameof(RecordInfo.SiteId);
            public const string ChannelId = nameof(RecordInfo.ChannelId);
            public const string ContentId = nameof(RecordInfo.ContentId);
            public const string Message = nameof(RecordInfo.Message);
            public const string Amount = nameof(RecordInfo.Amount);
            public const string OrderNo = nameof(RecordInfo.OrderNo);
            public const string IsPayed = nameof(RecordInfo.IsPayed);
            public const string AddDate = nameof(RecordInfo.AddDate);
        }

        public int Insert(RecordInfo recordInfo)
        {
            return _repository.Insert(recordInfo);
        }

        public void UpdateIsPayed(string orderNo)
        {
            _repository.Update(Q
                .Set(Attr.IsPayed, true)
                .Where(Attr.OrderNo, orderNo)
            );
        }

        public bool IsPayed(string orderNo)
        {
            return _repository.Get<bool>(Q
                .Select(Attr.IsPayed)
                .Where(Attr.OrderNo, orderNo)
            );
        }

        public void Delete(int recordId)
        {
            _repository.Delete(recordId);
        }

        public int GetPayedCount(int siteId)
        {
            return _repository.Count(Q
                .Where(nameof(RecordInfo.SiteId), siteId)
                .Where(nameof(RecordInfo.IsPayed), true)
            );
        }

        public IList<RecordInfo> GetPayedRecordInfoList(int siteId, int page, int perPage)
        {
            return _repository.GetAll(Q
                .Where(nameof(RecordInfo.SiteId), siteId)
                .Where(nameof(RecordInfo.IsPayed), true)
                .OrderByDesc(nameof(RecordInfo.Id))
                .ForPage(page, perPage)
                );
        }
    }
}

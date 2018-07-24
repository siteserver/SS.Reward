using System.Collections.Generic;
using System.Data;
using SiteServer.Plugin;
using SS.Reward.Model;

namespace SS.Reward.Provider
{
    public class RecordDao
    {
        public const string TableName = "ss_reward_record";

        public static List<TableColumn> Columns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(RecordInfo.Id),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(RecordInfo.PublishmentSystemId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(RecordInfo.ChannelId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(RecordInfo.ContentId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(RecordInfo.Message),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(RecordInfo.Amount),
                DataType = DataType.Decimal
            },
            new TableColumn
            {
                AttributeName = nameof(RecordInfo.OrderNo),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(RecordInfo.IsPaied),
                DataType = DataType.Boolean
            },
            new TableColumn
            {
                AttributeName = nameof(RecordInfo.AddDate),
                DataType = DataType.DateTime
            }
        };

        private readonly string _connectionString;
        private readonly IDatabaseApi _helper;

        public RecordDao()
        {
            _connectionString = Main.Instance.ConnectionString;
            _helper = Main.Instance.DatabaseApi;
        }

        public int Insert(RecordInfo contentInfo)
        {
            string sqlString = $@"INSERT INTO {TableName}
(
    {nameof(RecordInfo.PublishmentSystemId)}, 
    {nameof(RecordInfo.ChannelId)}, 
    {nameof(RecordInfo.ContentId)}, 
    {nameof(RecordInfo.Message)},
    {nameof(RecordInfo.Amount)},
    {nameof(RecordInfo.OrderNo)},
    {nameof(RecordInfo.IsPaied)},
    {nameof(RecordInfo.AddDate)}
) VALUES (
    @{nameof(RecordInfo.PublishmentSystemId)}, 
    @{nameof(RecordInfo.ChannelId)}, 
    @{nameof(RecordInfo.ContentId)}, 
    @{nameof(RecordInfo.Message)},
    @{nameof(RecordInfo.Amount)},
    @{nameof(RecordInfo.OrderNo)},
    @{nameof(RecordInfo.IsPaied)},
    @{nameof(RecordInfo.AddDate)}
)";

            var parameters = new[]
            {
                _helper.GetParameter(nameof(contentInfo.PublishmentSystemId), contentInfo.PublishmentSystemId),
                _helper.GetParameter(nameof(contentInfo.ChannelId), contentInfo.ChannelId),
                _helper.GetParameter(nameof(contentInfo.ContentId), contentInfo.ContentId),
                _helper.GetParameter(nameof(contentInfo.Message), contentInfo.Message),
                _helper.GetParameter(nameof(contentInfo.Amount), contentInfo.Amount),
                _helper.GetParameter(nameof(contentInfo.OrderNo), contentInfo.OrderNo),
                _helper.GetParameter(nameof(contentInfo.IsPaied), contentInfo.IsPaied),
                _helper.GetParameter(nameof(contentInfo.AddDate), contentInfo.AddDate)
            };

            return _helper.ExecuteNonQueryAndReturnId(TableName, nameof(RecordInfo.Id), _connectionString, sqlString, parameters);
        }

        public void UpdateIsPaied(string orderNo)
        {
            string sqlString = $@"UPDATE {TableName} SET
                {nameof(RecordInfo.IsPaied)} = @{nameof(RecordInfo.IsPaied)} WHERE
                {nameof(RecordInfo.OrderNo)} = @{nameof(RecordInfo.OrderNo)}";

            var parameters = new List<IDataParameter>
            {
                _helper.GetParameter(nameof(RecordInfo.IsPaied), true),
                _helper.GetParameter(nameof(RecordInfo.OrderNo), orderNo)
            };

            _helper.ExecuteNonQuery(_connectionString, sqlString, parameters.ToArray());
        }

        public bool IsPaied(string orderNo)
        {
            var isPaied = false;

            string sqlString = $@"SELECT {nameof(RecordInfo.IsPaied)} FROM {TableName} WHERE {nameof(RecordInfo.OrderNo)} = @{nameof(RecordInfo.OrderNo)}";

            var parameters = new List<IDataParameter>
            {
                _helper.GetParameter(nameof(RecordInfo.OrderNo), orderNo)
            };

            using (var rdr = _helper.ExecuteReader(_connectionString, sqlString, parameters.ToArray()))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    isPaied = rdr.GetBoolean(0);
                }
                rdr.Close();
            }

            return isPaied;
        }

        public string GetSelectString(int siteId)
        {
            return $@"SELECT {nameof(RecordInfo.Id)}, 
            {nameof(RecordInfo.PublishmentSystemId)}, 
            {nameof(RecordInfo.ChannelId)}, 
            {nameof(RecordInfo.ContentId)}, 
            {nameof(RecordInfo.Message)},
            {nameof(RecordInfo.Amount)},
            {nameof(RecordInfo.OrderNo)},
            {nameof(RecordInfo.IsPaied)},
            {nameof(RecordInfo.AddDate)}
            FROM {TableName} WHERE {nameof(RecordInfo.PublishmentSystemId)} = {siteId} ORDER BY Id DESC";
        }

        public void Delete(List<int> deleteIdList)
        {
            string sqlString =
                $"DELETE FROM {TableName} WHERE Id IN ({string.Join(",", deleteIdList)})";
            _helper.ExecuteNonQuery(_connectionString, sqlString);
        }
    }
}

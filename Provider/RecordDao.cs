using System.Collections.Generic;
using System.Data;
using SiteServer.Plugin;
using SS.Reward.Model;

namespace SS.Reward.Provider
{
    public static class RecordDao
    {
        public const string TableName = "ss_reward_record";

        public static List<TableColumn> Columns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(RecordInfo.Id),
                DataType = DataType.Integer,
                IsPrimaryKey = true,
                IsIdentity = true
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

        public static int Insert(RecordInfo contentInfo)
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
                Context.DatabaseApi.GetParameter(nameof(contentInfo.PublishmentSystemId), contentInfo.PublishmentSystemId),
                Context.DatabaseApi.GetParameter(nameof(contentInfo.ChannelId), contentInfo.ChannelId),
                Context.DatabaseApi.GetParameter(nameof(contentInfo.ContentId), contentInfo.ContentId),
                Context.DatabaseApi.GetParameter(nameof(contentInfo.Message), contentInfo.Message),
                Context.DatabaseApi.GetParameter(nameof(contentInfo.Amount), contentInfo.Amount),
                Context.DatabaseApi.GetParameter(nameof(contentInfo.OrderNo), contentInfo.OrderNo),
                Context.DatabaseApi.GetParameter(nameof(contentInfo.IsPaied), contentInfo.IsPaied),
                Context.DatabaseApi.GetParameter(nameof(contentInfo.AddDate), contentInfo.AddDate)
            };

            return Context.DatabaseApi.ExecuteNonQueryAndReturnId(TableName, nameof(RecordInfo.Id), Context.ConnectionString, sqlString, parameters);
        }

        public static void UpdateIsPaied(string orderNo)
        {
            string sqlString = $@"UPDATE {TableName} SET
                {nameof(RecordInfo.IsPaied)} = @{nameof(RecordInfo.IsPaied)} WHERE
                {nameof(RecordInfo.OrderNo)} = @{nameof(RecordInfo.OrderNo)}";

            var parameters = new List<IDataParameter>
            {
                Context.DatabaseApi.GetParameter(nameof(RecordInfo.IsPaied), true),
                Context.DatabaseApi.GetParameter(nameof(RecordInfo.OrderNo), orderNo)
            };

            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString, parameters.ToArray());
        }

        public static bool IsPaied(string orderNo)
        {
            var isPaied = false;

            string sqlString = $@"SELECT {nameof(RecordInfo.IsPaied)} FROM {TableName} WHERE {nameof(RecordInfo.OrderNo)} = @{nameof(RecordInfo.OrderNo)}";

            var parameters = new List<IDataParameter>
            {
                Context.DatabaseApi.GetParameter(nameof(RecordInfo.OrderNo), orderNo)
            };

            using (var rdr = Context.DatabaseApi.ExecuteReader(Context.ConnectionString, sqlString, parameters.ToArray()))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    isPaied = rdr.GetBoolean(0);
                }
                rdr.Close();
            }

            return isPaied;
        }

        public static string GetSelectString(int siteId)
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

        public static void Delete(List<int> deleteIdList)
        {
            string sqlString =
                $"DELETE FROM {TableName} WHERE Id IN ({string.Join(",", deleteIdList)})";
            Context.DatabaseApi.ExecuteNonQuery(Context.ConnectionString, sqlString);
        }
    }
}

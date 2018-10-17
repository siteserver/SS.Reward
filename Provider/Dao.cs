using SiteServer.Plugin;

namespace SS.Reward.Provider
{
    public class Dao
    {
        private readonly string _connectionString;
        private readonly IDatabaseApi _helper;

        public Dao()
        {
            _connectionString = Context.ConnectionString;
            _helper = Context.DatabaseApi;
        }

        public int GetIntResult(string sqlString)
        {
            var count = 0;

            using (var conn = _helper.GetConnection(_connectionString))
            {
                conn.Open();
                using (var rdr = _helper.ExecuteReader(conn, sqlString))
                {
                    if (rdr.Read() && !rdr.IsDBNull(0))
                    {
                        count = rdr.GetInt32(0);
                    }
                    rdr.Close();
                }
            }
            return count;
        }
    }
}
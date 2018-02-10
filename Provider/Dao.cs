using SiteServer.Plugin;

namespace SS.Reward.Provider
{
    public class Dao
    {
        private readonly string _connectionString;
        private readonly IDataApi _helper;

        public Dao()
        {
            _connectionString = Main.Instance.ConnectionString;
            _helper = Main.Instance.DataApi;
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
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineExamAPI.dbconfig
{
    public sealed class SqlServerHelper : IDisposable
    {
        private const int COMMAND_TIMEOUT = 1200;
        private string mstrCN;
        private SqlConnection mCn;
        private SqlCommand mCmd;
        private SqlTransaction mTrans;
        private bool UseTransaction = false;

        public SqlServerHelper()
        {
            mstrCN = Config.SqlServerConnectionString();
        }

        public SqlServerHelper(bool useTransaction)
        {
            UseTransaction = useTransaction;
            mstrCN = Config.SqlServerConnectionString();
            if (useTransaction)
            {
                mCn = new SqlConnection(mstrCN);
                mCn.Open();
                mTrans = mCn.BeginTransaction();
            }
        }

        public SqlServerHelper(string connStr)
        {
            mstrCN = connStr;
        }

        public string ConnectionString
        {
            get { return mstrCN; }
            set { mstrCN = value; }
        }

        public SqlConnection SQLConnection
        {
            get { return mCn; }
            set { mCn = value; }
        }

        public SqlCommand SQLCommand
        {
            get { return mCmd; }
            set { mCmd = value; }
        }

        public SqlTransaction SQLTransaction
        {
            get { return mTrans; }
            set { mTrans = value; }
        }

        #region Private Functions

        private void InitCommandForSP(string spName)
        {
            if (UseTransaction)
                OpenTransConnection();
            else
                OpenConnection();
            mCmd = mCn.CreateCommand();
            mCmd.CommandTimeout = COMMAND_TIMEOUT;
            mCmd.CommandType = CommandType.StoredProcedure;
            mCmd.CommandText = spName;
        }

        private void InitCommandForFun(string spName)
        {
            OpenConnection();
            mCmd = mCn.CreateCommand();
            mCmd.CommandTimeout = COMMAND_TIMEOUT;
            mCmd.CommandType = CommandType.StoredProcedure;
            mCmd.CommandText = spName;
        }

        private void InitCommandForSQL(string sqlText)
        {
            OpenConnection();
            mCmd = mCn.CreateCommand();
            mCmd.CommandTimeout = COMMAND_TIMEOUT;
            mCmd.CommandType = CommandType.Text;
            mCmd.CommandText = sqlText;
        }

        #endregion Private Functions

        public void OpenTransConnection()
        {
            if (mCn == null || mCn.State != ConnectionState.Open)
            {
                mCn = new SqlConnection(mstrCN);
                mCn.Open();
                mTrans = SQLConnection.BeginTransaction();
            }
        }

        private void OpenConnection()
        {
            CloseConnection();
            mCn = new SqlConnection(mstrCN);
            mCn.Open();
        }

        private void CloseConnection()
        {
            if (mCn != null) // If the connection has already been set to a value
            {
                if (mCn.State != ConnectionState.Closed) // If the connection is not closed.
                {
                    mCn.Close();
                }
            }
        }

        public void PrepareTransCommand(string spName, SqlParameter[] parms)
        {
            mCmd = mCn.CreateCommand();
            mCmd.Transaction = mTrans;
            mCmd.CommandTimeout = COMMAND_TIMEOUT;
            mCmd.CommandType = CommandType.StoredProcedure;
            mCmd.CommandText = spName;
            mCmd.Prepare();
            foreach (SqlParameter pr in parms)
                if (pr != null)
                    mCmd.Parameters.Add(pr);
        }

        public void PrepareCommand(string spName, SqlParameter[] parms)
        {
            OpenConnection();
            mCmd = mCn.CreateCommand();
            mCmd.CommandTimeout = COMMAND_TIMEOUT;
            mCmd.CommandType = CommandType.StoredProcedure;
            mCmd.CommandText = spName;
            mCmd.Prepare();
            foreach (SqlParameter pr in parms)
                if (pr != null)
                    mCmd.Parameters.Add(pr);
        }

        public void ExecutePrepared()
        {
            mCmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Returns a SqlDataReader object with the results of the specified stored procedure
        /// </summary>
        /// <param name="spName">The name of the stored procedure to execute</param>
        /// <returns>MySqlDataReader</returns>
        public SqlDataReader RunSpReturnDr(string spName)
        {
            //SqlDataReader dr = null;
            InitCommandForSP(spName);

            using (SqlDataReader dr = mCmd.ExecuteReader(CommandBehavior.CloseConnection))
            {
                //dr = mCmd.ExecuteReader(CommandBehavior.CloseConnection);
                mCmd.Dispose();
                return dr;
            }

            //return dr;
        }

        /// <summary>
        /// Returns a SqlDataReader object with the results of the specified stored procedure
        /// </summary>
        /// <param name="spName">The name of the stored procedure to execute</param>
        /// <param name="parms">MySql Parameter array</param>
        /// <returns>MySqlDataReader</returns>
        public SqlDataReader RunSpReturnDr(string spName, SqlParameter[] parms)
        {
            //SqlDataReader DR = null;
            InitCommandForSP(spName);

            foreach (SqlParameter pr in parms)
            {
                if (pr != null)
                    mCmd.Parameters.Add(pr);
            }

            using (SqlDataReader DR = mCmd.ExecuteReader(CommandBehavior.CloseConnection))
            {
                //DR = mCmd.ExecuteReader(CommandBehavior.CloseConnection);
                mCmd.Dispose();
                return DR;
            }

            //return DR;
        }

        /// <summary>
        /// Returns a dataset representing the query result set
        /// </summary>
        /// <param name="spName">The name of the stored procedure to execute</param>
        /// <returns>DataSet</returns>
        public DataSet RunSpReturnDs(string spName)
        {
            DataSet ds = new DataSet();
            InitCommandForSP(spName);
            using (mCmd)
            {
                SqlDataAdapter da = new SqlDataAdapter(mCmd);
                da.Fill(ds);
                if (!UseTransaction)
                    mCmd.Connection.Close();
                da.Dispose();
            }
            return ds;
        }

        /// <summary>
        /// Returns a datatable representing the query result set add by sky
        /// </summary>
        /// <param name="spName">The name of the stored procedure to execute</param>
        /// <returns>DataTable</returns>
        public DataTable RunSpReturnDt(string spName)
        {
            DataTable dt = new DataTable();
            InitCommandForSP(spName);

            using (mCmd)
            {
                SqlDataAdapter da = new SqlDataAdapter(mCmd);
                da.Fill(dt);
                if (!UseTransaction)
                    mCmd.Connection.Close();
                da.Dispose();
            }
            return dt;
        }

        /// <summary>
        /// Returns a dataset representing the query result set
        /// </summary>
        /// <param name="spName">The name of the stored procedure to execute</param>
        /// <param name="parms">Sql Parameter array</param>
        /// <returns>DataSet</returns>
        public DataSet RunSpReturnDs(string spName, SqlParameter[] parms)
        {
            DataSet ds = new DataSet();
            InitCommandForSP(spName);

            using (mCmd)
            {
                SqlDataAdapter da = new SqlDataAdapter(mCmd);
                foreach (SqlParameter pr in parms)
                    if (pr != null)
                        mCmd.Parameters.Add(pr);

                da.Fill(ds);
                if (!UseTransaction)
                    mCmd.Connection.Close();
                da.Dispose();
            }
            return ds;
        }

        /// <summary>
        /// Returns a dataset representing the query result set
        /// </summary>
        /// <param name="spName">The name of the stored procedure to execute</param>
        /// <param name="parms">Sql Parameter array</param>
        /// <returns>DataSet</returns>
        public DataSet RunSpReturnDs(string spName, List<SqlParameter> parms)
        {
            DataSet ds = new DataSet();
            InitCommandForSP(spName);
            using (mCmd)
            {
                SqlDataAdapter da = new SqlDataAdapter(mCmd);
                foreach (SqlParameter pr in parms)
                    if (pr != null)
                        mCmd.Parameters.Add(pr);

                da.Fill(ds);
                if (!UseTransaction)
                    mCmd.Connection.Close();
                da.Dispose();
            }
            return ds;
        }

        /// <summary>
        /// Returns a datatable representing the query result set add by sky
        /// </summary>
        /// <param name="spName">The name of the stored procedure to execute</param>
        /// <param name="parms">MySql Parameter array</param>
        /// <returns>DataTable</returns>
        public DataTable RunSpReturnDt(string spName, SqlParameter[] parms)
        {
            DataTable dt = new DataTable();
            InitCommandForSP(spName);

            using (mCmd)
            {
                SqlDataAdapter da = new SqlDataAdapter(mCmd);
                foreach (SqlParameter pr in parms)
                    if (pr != null)
                        mCmd.Parameters.Add(pr);

                da.Fill(dt);
                if (!UseTransaction)
                    mCmd.Connection.Close();
                da.Dispose();
            }
            return dt;
        }

        public DataTable RunSpReturnDt(string spName, List<SqlParameter> parms)
        {
            DataTable dt = new DataTable();
            InitCommandForSP(spName);

            using (mCmd)
            {
                SqlDataAdapter da = new SqlDataAdapter(mCmd);
                foreach (SqlParameter pr in parms)
                    if (pr != null)
                        mCmd.Parameters.Add(pr);

                da.Fill(dt);
                if (!UseTransaction)
                    mCmd.Connection.Close();
                da.Dispose();
            }
            return dt;
        }

        /// <summary>
        /// Returns a dataset representing the query result set (Allows for custom paging)
        /// </summary>
        /// <param name="spName">The SQL string to execute</param>
        /// <param name="CurrentIndex">int Beginning record</param>
        /// <param name="PageSize">int Records allowed per page</param>
        /// <param name="SourceTable">string Source table name</param>
        /// <returns>DataSet</returns>
        public DataSet RunSqlReturnDs(string Sql, int CurrentIndex, int PageSize, string SourceTable)
        {
            DataSet ds = new DataSet();
            InitCommandForSQL(Sql);

            using (mCmd)
            {
                SqlDataAdapter da = new SqlDataAdapter(mCmd);
                da.Fill(ds, CurrentIndex, PageSize, SourceTable);
                if (!UseTransaction)
                    mCmd.Connection.Close();
                da.Dispose();
            }

            return ds;
        }

        /// <summary>
        /// Returns a dataset representing the query result set
        /// </summary>
        /// <param name="Sql">The Sql query to execute</param>
        /// <param name="parms">Sql Parameter array</param>
        /// <returns>Object</returns>
        public DataSet RunSqlReturnDs(string Sql, SqlParameter[] parms)
        {
            DataSet ds = new DataSet();
            InitCommandForSQL(Sql);

            using (mCmd)
            {
                foreach (SqlParameter pr in parms)
                    if (pr != null)
                        mCmd.Parameters.Add(pr);

                SqlDataAdapter da = new SqlDataAdapter(mCmd);
                da.Fill(ds);
                if (!UseTransaction)
                    mCmd.Connection.Close();
                da.Dispose();
            }
            return ds;
        }

        /// <summary>
        /// Returns a datatable representing the query result set add by sky
        /// </summary>
        /// <param name="Sql">The Sql query to execute</param>
        /// <param name="parms">Sql Parameter array</param>
        /// <returns>DataTable</returns>
        public DataTable RunSqlReturnDt(string Sql, SqlParameter[] parms)
        {
            DataTable dt = new DataTable();
            InitCommandForSQL(Sql);

            using (mCmd)
            {
                foreach (SqlParameter pr in parms)
                    if (pr != null)
                        mCmd.Parameters.Add(pr);

                SqlDataAdapter da = new SqlDataAdapter(mCmd);
                da.Fill(dt);
                if (!UseTransaction)
                    mCmd.Connection.Close();
                da.Dispose();
            }
            return dt;
        }

        /// <summary>
        /// Returns a datatable representing the query result set add by sky
        /// </summary>
        /// <param name="Sql">The Sql query to execute</param>      
        /// <returns>DataTable</returns>
        public DataTable RunSqlReturnDt(string Sql)
        {
            DataTable dt = new DataTable();
            InitCommandForSQL(Sql);

            using (mCmd)
            {
                SqlDataAdapter da = new SqlDataAdapter(mCmd);
                da.Fill(dt);
                if (!UseTransaction)
                    mCmd.Connection.Close();
                da.Dispose();
            }
            return dt;
        }

        public DataSet RunFunReturnDs(string spName, SqlParameter[] parms)
        {
            DataSet ds = new DataSet();
            InitCommandForSP(spName);

            using (mCmd)
            {
                SqlDataAdapter da = new SqlDataAdapter(mCmd);
                foreach (SqlParameter pr in parms)
                    if (pr != null)
                        mCmd.Parameters.Add(pr);

                da.Fill(ds);
                if (!UseTransaction)
                    mCmd.Connection.Close();
                da.Dispose();
            }

            return ds;
        }

        /// <summary>
        /// Returns a MySqlDataReader object with the results of the specified SQL statement
        /// </summary>
        /// <param name="MySql">SQL statement</param>
        /// <returns>MySqlDataReader</returns>
        public SqlDataReader RunSqlReturnDr(string Sql)
        {
            SqlDataReader DR = null;
            InitCommandForSQL(Sql);

            using (mCmd)
            {
                DR = mCmd.ExecuteReader(CommandBehavior.CloseConnection);
                mCmd.Dispose();
            }
            return DR;
        }

        /// <summary>
        /// Returns a MySqlDataReader object with the results of the specified SQL statement add by sky
        /// </summary>
        /// <param name="MySql">SQL statement</param>
        /// <returns>MySqlDataReader</returns>
        public SqlDataReader RunSqlReturnDr(string Sql, SqlParameter[] parms)
        {
            SqlDataReader DR = null;
            InitCommandForSQL(Sql);

            using (mCmd)
            {
                foreach (SqlParameter pr in parms)
                    if (pr != null)
                        mCmd.Parameters.Add(pr);
                DR = mCmd.ExecuteReader(CommandBehavior.CloseConnection);
                mCmd.Dispose();
                return DR;
            }
        }

        /// <summary>
        /// Executes the SQL query string 
        /// </summary>
        /// <param name="Sql">The SQL query string to execute</param>
        /// <returns>Object</returns>
        public object RunSqlReturnScalar(string Sql)
        {
            InitCommandForSQL(Sql);
            object obj = null;

            using (mCmd)
            {
                obj = mCmd.ExecuteScalar();
                if (!UseTransaction)
                    mCmd.Connection.Close();
                mCmd.Dispose();
            }

            return obj;
        }

        /// <summary>
        /// Executes the SQL query string
        /// </summary>
        /// <param name="Sql">The Sql query to execute</param>
        /// <param name="parms">Sql Parameter array</param>
        /// <returns>Object</returns>
        public object RunSqlReturnScalar(string Sql, SqlParameter[] parms)
        {
            InitCommandForSQL(Sql);
            object obj = null;

            using (mCmd)
            {
                foreach (SqlParameter pr in parms)
                    if (pr != null)
                        mCmd.Parameters.Add(pr);

                obj = mCmd.ExecuteScalar();
                if (!UseTransaction)
                    mCmd.Connection.Close();
            }
            return obj;
        }

        /// <summary>
        /// Executes the SQL query string 
        /// </summary>
        /// <param name="Sql">The SQL query string to execute</param>
        /// <returns>Number of rows affected as an Int values </returns>
        public int RunSql(string Sql)
        {
            InitCommandForSQL(Sql);
            int iRtnVal = -1;

            using (mCmd)
            {
                iRtnVal = mCmd.ExecuteNonQuery();
                if (!UseTransaction)
                    mCmd.Connection.Close();
                mCmd.Dispose();
            }

            return iRtnVal;
        }

        /// <summary>
        /// Returns a dataset representing the query result set
        /// </summary>
        /// <param name="spName">The SQL string to execute</param>
        /// <returns>DataSet</returns>
        public DataSet RunSqlReturnDs(string Sql)
        {
            DataSet ds = new DataSet();
            InitCommandForSQL(Sql);

            using (mCmd)
            {
                SqlDataAdapter da = new SqlDataAdapter(mCmd);
                da.Fill(ds);
                if (!UseTransaction)
                    mCmd.Connection.Close();
                da.Dispose();
            }

            return ds;
        }

        /// <summary>
        /// Returns the number of records affected by the specified stored procedure
        /// </summary>
        /// <param name="sSpName">The name of the stored procedure to execute</param>
        /// <param name="parms">Sql Parameter array</param>
        /// <returns>int</returns>
        public int RunSp(string spName, SqlParameter[] parms)
        {
            InitCommandForSP(spName);
            int rtnVal = 0;

            using (mCmd)
            {
                foreach (SqlParameter pr in parms)
                    if (pr != null)
                        mCmd.Parameters.Add(pr);

                rtnVal = mCmd.ExecuteNonQuery();
                if (!UseTransaction)
                    mCmd.Connection.Close();
            }
            return rtnVal;
        }

        public int RunSp(string spName, List<SqlParameter> parms)
        {
            InitCommandForSP(spName);
            int rtnVal = 0;

            using (mCmd)
            {
                foreach (SqlParameter pr in parms)
                    if (pr != null)
                        mCmd.Parameters.Add(pr);

                rtnVal = mCmd.ExecuteNonQuery();
                if (!UseTransaction)
                    mCmd.Connection.Close();
            }
            return rtnVal;
        }
        /// <summary>
        /// Returns the number of records affected by the specified stored procedure
        /// </summary>
        /// <param name="spName"></param>
        /// <returns></returns>
        public int RunSp(string spName)
        {
            InitCommandForSP(spName);
            int rtnVal = 0;

            using (mCmd)
            {
                rtnVal = mCmd.ExecuteNonQuery();
                if (!UseTransaction)
                    mCmd.Connection.Close();
            }

            return rtnVal;
        }

        public object RunSpReturnScalar(string spName, SqlParameter[] parms)
        {
            DataSet ds = new DataSet();
            InitCommandForSP(spName);

            using (mCmd)
            {
                SqlDataAdapter da = new SqlDataAdapter(mCmd);
                foreach (SqlParameter pr in parms)
                    if (pr != null)
                        mCmd.Parameters.Add(pr);

                da.Fill(ds);
                if (!UseTransaction)
                    mCmd.Connection.Close();
                da.Dispose();
            }
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                    return ds.Tables[0].Rows[0][0].ToString().Trim();
                else
                    return string.Empty;
            }
            else
                return string.Empty;
        }

        public object RunSpReturnScalar(string spName, List<SqlParameter> parms)
        {
            DataSet ds = new DataSet();
            InitCommandForSP(spName);

            using (mCmd)
            {
                SqlDataAdapter da = new SqlDataAdapter(mCmd);
                foreach (SqlParameter pr in parms)
                    if (pr != null)
                        mCmd.Parameters.Add(pr);

                da.Fill(ds);
                if (!UseTransaction)
                    mCmd.Connection.Close();
                da.Dispose();
            }

            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                    return ds.Tables[0].Rows[0][0].ToString().Trim();
                else
                    return string.Empty;
            }
            else
                return string.Empty;
        }

        public int RunSql(string Sql, SqlParameter[] parms)
        {
            InitCommandForSQL(Sql);
            int rtnVal = 0;

            using (mCmd)
            {
                foreach (SqlParameter pr in parms)
                    if (pr != null)
                        mCmd.Parameters.Add(pr);

                rtnVal = mCmd.ExecuteNonQuery();
                if (!UseTransaction)
                    mCmd.Connection.Close();
            }
            return rtnVal;
        }

        public int RunSql(string Sql, List<SqlParameter> parms)
        {
            InitCommandForSQL(Sql);
            int rtnVal = 0;

            using (mCmd)
            {
                foreach (SqlParameter pr in parms)
                    if (pr != null)
                        mCmd.Parameters.Add(pr);

                rtnVal = mCmd.ExecuteNonQuery();
                mCmd.Connection.Close();
            }
            return rtnVal;
        }

        public int RunSqlBulk(string spName, List<SqlParameter> parms)
        {
            int rtnVal = 0;

            mCmd = mCn.CreateCommand();
            mCmd.CommandTimeout = COMMAND_TIMEOUT;
            mCmd.CommandType = CommandType.StoredProcedure;
            mCmd.CommandText = spName;

            foreach (SqlParameter pr in parms)
            {
                if (pr != null)
                    mCmd.Parameters.Add(pr);
            }

            rtnVal = mCmd.ExecuteNonQuery();
            return rtnVal;
        }

        public void BeginTransaction()
        {
            if (mCn != null && mCn.State == ConnectionState.Open)
            {
                mTrans = SQLConnection.BeginTransaction();
            }
        }

        public void Commit()
        {
            mTrans.Commit();
        }
        public void Rollback()
        {
            mTrans.Rollback();
        }

        public void Dispose()
        {
            if (mCmd != null)
                mCmd.Dispose();
            CloseConnection();
        }
    }
}

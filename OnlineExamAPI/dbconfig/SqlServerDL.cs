using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineExamAPI.dbconfig
{
    public class SqlServerDL
    {
       SqlServerHelper objSqlHelper = null;
        public SqlServerDL()
        {
            objSqlHelper = new SqlServerHelper();
        }
        public SqlServerDL(string connectionString)
        {
            objSqlHelper = new SqlServerHelper(connectionString);
        }
        public void Dispose()
        {
            if (objSqlHelper != null)
            {
                objSqlHelper.Dispose();
            }
        }
        public DataSet StudentLogin( string STUDENT_CODE)
        {
            List<SqlParameter> objParam = new List<SqlParameter>()
                {
                   new SqlParameter("@STUDENT_CODE", STUDENT_CODE)
                };
            DataSet ds = objSqlHelper.RunSpReturnDs("UDSP_STUDENTLOGIN", objParam);
            return ds;
        }
        public DataSet quizquestions(string examid)
        {
            List<SqlParameter> objParam = new List<SqlParameter>()
                {
                   new SqlParameter("@EXAMID", examid)
                };
            DataSet ds = objSqlHelper.RunSpReturnDs("UDSP_QUIZQUESTION", objParam);
            return ds;
        }
        public DataSet GetSectionList()
        {
          
            DataSet ds = objSqlHelper.RunSpReturnDs("UDSP_SECTIONLIST");
            return ds;
        }
    }
}

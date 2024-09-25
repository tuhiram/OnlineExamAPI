using OnlineExamAPI.dbconfig;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineExamAPI.BL
{
    public class ExamBL
    {
        SqlServerDL objdl = new SqlServerDL();
        public DataSet StudentLogin(string STUDENT_CODE)
        {
           return objdl.StudentLogin(STUDENT_CODE);
        }
        public DataSet quizquestions(string examid)
        {
            return objdl.quizquestions(examid);


        }
        public object GetQuestionById(string examid)
        {
            DataSet ds=objdl.quizquestions(examid);
            if (ds.Tables[0].Rows.Count > 0)
            {
                var result = (from row in ds.Tables[0].AsEnumerable()
                              select new
                              {
                                  QUESTIONID = row["QUESTIONID"].ToString(),
                                  QUESTION = row["QUESTION"].ToString(),
                                  OPTION1 = row["OPTION1"].ToString(),
                                  OPTION2 = row["OPTION2"].ToString(),
                                  OPTION3 = row["OPTION3"].ToString(),
                                  OPTION4 = row["OPTION4"].ToString(),
                                  ANSWER = row["ANSWER"].ToString()

                              }).ToList();
                return result;
            }
            return null;

        }
        public object GetSectionList()
        {
            DataSet ds = objdl.GetSectionList();
            if (ds.Tables[0].Rows.Count > 0)
            {
                var result = (from row in ds.Tables[0].AsEnumerable()
                              select new
                              {
                                  SECTIONID = row["SECTIONID"].ToString(),
                                  SECTION_NAME = row["SECTION_NAME"].ToString(),
                                  ID = row["ID"].ToString()

                              }).ToList();
                return result;
            }
            return null;

        }
    }
}

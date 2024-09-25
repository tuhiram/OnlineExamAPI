using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineExamAPI.BE
{
   
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Option
    {
        public int id { get; set; }
        public int questionId { get; set; }
        public string name { get; set; }
        public bool isAnswer { get; set; }
    }

    public class Question
    {
        public int QuestionId { get; set; }
        public string QuestionName { get; set; }
       // public int questionTypeId { get; set; }
      //  public int questionStatus { get; set; }
        public List<Option> options { get; set; }
      //  public QuestionType questionType { get; set; }
    }

    public class QuestionType
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool isActive { get; set; }
    }

    public class Root
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public List<Question> questions { get; set; }
    }



}

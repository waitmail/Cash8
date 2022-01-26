using System;
using System.Collections.Generic;
using System.Text;

namespace Cash8
{
    class AnswerCheckImcWorkState
    {
        public List<Result> results { get; set; }

        public class Ecr
        {
            public string stage { get; set; }
            public string status { get; set; }
            public string type { get; set; }
        }

        public class Fm
        {
            public int checkingCount { get; set; }
            public string noticeFreeMemory { get; set; }
            public bool noticeIsBegin { get; set; }
            public int noticeUnsentCount { get; set; }
            public int soldImcCount { get; set; }
            public string status { get; set; }
        }

        public class Result2
        {
            public Ecr ecr { get; set; }
            public Fm fm { get; set; }
        }

        public class Result
        {
            public string status { get; set; }
            public Result2 result { get; set; }
            public int errorCode { get; set; }
            public string errorDescription { get; set; }
        }
    }
}

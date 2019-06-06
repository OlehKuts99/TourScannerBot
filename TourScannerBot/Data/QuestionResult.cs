using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourScannerBot.Data
{
    public class QuestionResult
    {
        public QuestionResult()
        { }

        public string WriteResult()
        {
            string resultString = string.Format("Tap on the url : {0}",
                 string.Format("https://tourscanner.co/s/{0}/?d={1}", string.Join("-", Question.TravelTo.Split(" ")), 
                 Question.TravelWhen.ToString("dd-MM-yyyy")));

            new Question().ClearAll();

            return resultString;
        }
    }
}

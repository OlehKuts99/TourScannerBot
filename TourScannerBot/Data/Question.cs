using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourScannerBot.Data
{
    delegate string AskSome();

    public class Question : IQuestion
    {
        private static bool isStarted;
        
        public Question()
        {

        }

        public Question(ITurnContext<IMessageActivity> context)
        {
            _context = context;
        }

        public static QuestionsEnum CurrentQuestion { get; set; } = QuestionsEnum.StartDialoge;

        public static string TravelTo { get; set; }

        public static DateTime TravelWhen { get; set; }

        private ITurnContext<IMessageActivity> _context { get; set; }

        public void ClearAll()
        {
            TravelTo = null;
            TravelWhen = DateTime.MinValue;
            CurrentQuestion = QuestionsEnum.StartDialoge;
        }

        public string AskQuestion()
        {
            if (_context.Activity.Text.ToLower() == "reset")
            {
                ClearAll();
            }

            if (_context.Activity.Text.ToLower() == "start" || isStarted)
            {
                isStarted = true;

                switch (CurrentQuestion)
                {
                    case QuestionsEnum.StartDialoge:
                        CurrentQuestion = QuestionsEnum.TravelTo;
                        return this.ReflectQuestion();
                    case QuestionsEnum.TravelTo:
                        return AskAboutEndLocation();
                    case QuestionsEnum.TravelWhen:
                        return AskAboutDate();
                    case QuestionsEnum.EndDialoge:
                        TravelWhen = ParseTextToDate(_context.Activity.Text);
                        if (TravelWhen == DateTime.MinValue)
                        {
                            ClearAll();

                            return "Something went wrong!";
                        }
                        isStarted = false;
                        return new QuestionResult().WriteResult();
                }
            }

            return "To start searching send message 'start'";
        }

        private string AskAboutEndLocation()
        {
            TravelTo = _context.Activity.Text;

            CurrentQuestion = QuestionsEnum.TravelWhen;

            return this.ReflectQuestion();
        }

        private string AskAboutDate()
        {
            CurrentQuestion = QuestionsEnum.EndDialoge;

            return this.AskQuestion();
        }


        private DateTime ParseTextToDate(string text)
        {
            DateTime date;

            if (text.ToLower() == "today")
            {
                date = DateTime.Today;

                return date;
            }

            if (text.ToLower() == "tomorrow")
            {
                date = DateTime.Now.AddDays(1);

                return date;
            }

            var isCorrect = DateTime.TryParse(text, out date);

            if (!isCorrect)
            {
                return DateTime.MinValue;
            }

            if (date < DateTime.Now)
            {
                return DateTime.MinValue;
            }

            return date;
        }

        private string ReflectQuestion()
        {
            switch (CurrentQuestion)
            {
                case QuestionsEnum.TravelTo:
                    return "Where do you want to travel(City)?";
                case QuestionsEnum.TravelWhen:
                    return "When do you want to travel?";
            }

            return string.Empty;
        }
    }
}

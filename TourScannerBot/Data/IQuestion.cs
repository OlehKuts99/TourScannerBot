using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourScannerBot.Data
{
    interface IQuestion
    {
        void ClearAll();

        string AskQuestion();
    }
}

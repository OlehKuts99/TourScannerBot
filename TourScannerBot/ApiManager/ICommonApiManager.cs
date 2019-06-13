using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourScannerBot.ApiManager
{
    interface ICommonApiManager
    {
        Task<string> MainFlow();
    }
}

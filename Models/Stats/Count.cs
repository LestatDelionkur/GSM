using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMSService.Models.Stats
{
    public class Count
    {
        public DateTime Date { get; set; }
        public int Income { get; set; }
        public int Outcome { get; set; }

        public Count(DateTime date)
        {
            Date = date;
            Income = 0;
            Outcome = 0;
        }
    }
}
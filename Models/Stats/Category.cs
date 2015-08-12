using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMSService.Models.Stats
{
    public class Category
    {
        public DateTime Date { get; set; }
        public int Good { get; set; }
        public int Bad { get; set; }
        public int Neutral { get; set; }

        public Category(DateTime date)
        {
            Date = date;
            Good = 0;
            Bad = 0;
            Neutral = 0;
        }
    }
}
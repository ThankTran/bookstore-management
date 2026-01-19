using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bookstore_Management.Core.Enums
{
    public enum TimeRange
    {
        All,
        Today,
        ThisWeek,
        ThisMonth,
        ThisQuarter,
        ThisYear,
        ThePast30Days
    }
}

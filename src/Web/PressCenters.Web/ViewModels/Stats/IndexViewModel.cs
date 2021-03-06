﻿namespace PressCenters.Web.ViewModels.Stats
{
    using System;
    using System.Collections.Generic;

    public class IndexViewModel
    {
        public IEnumerable<GroupByViewModel<DayOfWeek>> NewsByDayOfWeek { get; set; }

        public IEnumerable<GroupByViewModel<int>> NewsByMonth { get; set; }

        public IEnumerable<GroupByViewModel<int>> NewsByYear { get; set; }

        public int NewsCount { get; set; }

        public int NewsToday { get; set; }

        public int NewsYesterday { get; set; }

        public int NewsTheDayBeforeYesterday { get; set; }

        public int SourcesCount { get; set; }
    }
}

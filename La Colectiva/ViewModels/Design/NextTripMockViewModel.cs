using La_Colectiva.Models.Classic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace La_Colectiva.ViewModels.Design
{
    public class NextTripList : List<NextTripMock> { }

    public class NextTripMock
    {
        public PointMock Start { get; set; } = new PointMock();
        public PointMock End { get; set; } = new PointMock();
        public BranchMock Branch { get; set; } = new BranchMock();
        public RouteStatus Status { get; set; }
        public OperationMock Operation { get; set; }
        public ScheduleMock OriginalSchedule { get; set; }

        public bool HasOriginalSchedule => OriginalSchedule != null;
        public bool HasOperation => Operation != null;
    }

    public class PointMock
    {
        public TimeSpan Time { get; set; }
    }

    public class BranchMock
    {
        public string Name { get; set; }
        public CompanyMock Company { get; set; } = new CompanyMock();
    }

    public class CompanyMock
    {
        public string Name { get; set; }
    }

    public class OperationMock
    {
        public OperationStatus ForcedStatus { get; set; }
        public string ManualMessage { get; set; }
    }

    public class ScheduleMock
    {
        public PointMock Start { get; set; } = new PointMock();
        public PointMock End { get; set; } = new PointMock();
    }
}

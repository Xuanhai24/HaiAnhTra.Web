using HaiAnhTra.Web.Models;

namespace HaiAnhTra.Web.ViewModels
{
    public class AdminDashboardVM
    {
        // KPIs
        public int TotalProducts { get; set; }
        public int TotalTea { get; set; }
        public int TotalTools { get; set; }
        public int TotalGuides { get; set; }
        public int PublishedGuides { get; set; }
        public int NewLeads24h { get; set; }
        public int LeadsThisWeek { get; set; }
        public int LeadsThisMonth { get; set; }
        public int ContactedLeads { get; set; }
        public int ClosedLeads { get; set; }

        // Charts & lists
        public List<string> LeadDays { get; set; } = new();
        public List<int> LeadCounts { get; set; } = new();

        public List<TopProductRow> TopProducts30d { get; set; } = new();
        public List<RecentLeadRow> RecentLeads { get; set; } = new();

        public class TopProductRow { public string Name { get; set; } = ""; public int Leads { get; set; } }
        public class RecentLeadRow
        {
            public int Id { get; set; }
            public DateTime CreatedAt { get; set; }
            public string Name { get; set; } = "";
            public string Phone { get; set; } = "";
            public string? Email { get; set; }
            public string? ProductName { get; set; }
            public LeadStatus Status { get; set; }
        }
    }
}

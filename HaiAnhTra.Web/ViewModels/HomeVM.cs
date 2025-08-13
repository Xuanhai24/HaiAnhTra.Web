using HaiAnhTra.Web.Models;
using System.Collections.Generic;

namespace HaiAnhTra.Web.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Product> FeaturedTeas { get; set; } = new List<Product>();
        public IEnumerable<Product> FeaturedTools { get; set; } = new List<Product>();
        public IEnumerable<Guide> LatestGuides { get; set; } = new List<Guide>();
    }
}

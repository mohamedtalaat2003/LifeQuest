using LifeQuest.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeQuest.BLL.DTOs
{
    public class BadgeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Point { get; set; }
        public string image { get; set; } = string.Empty;

        public int? RequiredLevelId { get; set; }
        public string RequiredLevelName { get; set; } = string.Empty;
        public string CriteriaType { get; set; } = "Level";
        public int CriteriaValue { get; set; }
    }
}

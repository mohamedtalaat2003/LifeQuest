using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeQuest.BLL.DTOs
{
    public class UserBadgeDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public int BadgeId { get; set; }
        public string BadgeName { get; set; } = string.Empty;

        public DateTime AwardedDate { get; set; }

    }
}

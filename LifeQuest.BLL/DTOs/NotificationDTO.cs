using System;

namespace LifeQuest.BLL.DTOs
{
    public class NotificationDTO
    {
        public string Message { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string ColorClass { get; set; } = string.Empty;
        public string Link { get; set; } = "#";
    }
}

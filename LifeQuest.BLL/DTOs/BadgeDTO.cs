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
        public int Id {  get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Point {  get; set; }
        public string image {  get; set; }

        public IEnumerable<Badges> Badges { get; set; }

    }
}

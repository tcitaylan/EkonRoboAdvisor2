using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Models.Dtos
{
    public class TemplateBasketBackTestsDto
    {
        public int BacktestId { get; set; }
        public int? TemplateBasketId { get; set; }
        public DateTime? RecordDate { get; set; }
        public string Result { get; set; }
    }
}

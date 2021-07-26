using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helpers.Models.Dtos
{
    public partial class RiskCategoriesDto
    {
        public RiskCategoriesDto()
        {
            UserCategoryHistory = new HashSet<UserCategoryHistoryDto>();
        }

        [Key]
        public int RecordId { get; set; }
        public string CategoryName { get; set; }
        public string Explanation { get; set; }
        public DateTime? RecordDate { get; set; }
        public int? MaxValue { get; set; }
        public int? MinValue { get; set; }
        public int? RecordStatus { get; set; }
        public int? CrId { get; set; }
        public int? UpId { get; set; }
        public DateTime? UpDate { get; set; }
        public byte[] Img { get; set; }
        public string Imgext { get; set; }

        [ForeignKey("CategoryId")]  
        public virtual ICollection<UserCategoryHistoryDto> UserCategoryHistory { get; set; }
    }
}

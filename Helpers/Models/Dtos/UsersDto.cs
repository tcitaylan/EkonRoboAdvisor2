
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helpers.Models.Dtos
{
    public partial class UsersDto
    {

        [Key]
        public int RecordId { get; set; }
        public DateTime? RecordDate { get; set; }
        [Required]
        [EmailAddress(ErrorMessage ="Geçerli bir e-posta adresi gerekmektedir.")]
        public string Email { get; set; }
        public string Hash { get; set; }
        public bool? ActiveStatus { get; set; }
        public string Type { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string Gsm { get; set; }
        public string Address { get; set; }
        public string NameSurname { get; set; }
        public string CustomerNo { get; set; }
        public string Tckn { get; set; }
        public string Ip { get; set; }
        public bool? Newsletter { get; set; }
        public string Authorization { get; set; }
        public bool? IsBlocked { get; set; }
        public int? FailedLoginCount { get; set; }
        public bool? AlgoTradingPermission { get; set; }
        public bool? LiveDataPermission { get; set; }
        public int? CrId { get; set; }
        public string CrDate { get; set; }
        public int? UpId { get; set; }
        public DateTime? UpDate { get; set; }

        [Required]
        [StringLength(8, MinimumLength=4,ErrorMessage="Şifre en az 4 en fazla 8 karekter olmalıdır.")]
        public string password { get; set; } //Only in Dto
    }
}

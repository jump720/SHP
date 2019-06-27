using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SHP.Models
{
    [Serializable]
    [Table("User")]
    public class User
    {
        [Key]
        [Required]
        [MaxLength(5)]
        [Display(Name = "Code")]
        public string UserId { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "UserMail")]
        public string UserMail { get; set; }

        [Required]
        [MaxLength(20)]
        [Display(Name = "UserCountry")]
        public string UserCountry { get; set; }

        [Required]
        [MaxLength(20)]
        [Display(Name = "UserCity")]
        public string UserCity { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "UserTitle")]
        public string UserTitle { get; set; }

        [Required]
        [MaxLength(4)]
        [Display(Name = "UserOffice")]
        public string UserOffice { get; set; }

        public virtual ICollection<Answer> AnswerList { get; set; }

    }

    [Serializable]
    [Table("Question")]
    public class Question
    {
        [Key]
        [Required]
        [Display(Name = "Question Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QuestionId { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Question")]
        public string QuestionDesc { get; set; }

        public virtual ICollection<Answer> AnswerList { get; set; }
    }

    [Serializable]
    [Table("Answer")]
    public class Answer
    {
        [Required]
        [Display(Name = "Question Id")]
        public int QuestionId { get; set; }

        [Required]
        [MaxLength(5)]
        [Display(Name = "Code")]
        public string UserId { get; set; }

        [Required]
        [MaxLength(100)]
        [DataType(DataType.Password)]
        [Display(Name = "Answer")]
        public string AnswerDesc { get; set; }

        public Question Question { get; set; }
        public User User { get; set; }

    }

}
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace EmailSender.Models
{
    /// <summary>
    /// The Message Table in the Database has the following columns :
    /// 
    /// <list type="bullet">
    /// 
    /// <listheader> Columns : </listheader>
    /// 
    /// <item>
    /// <term>Id</term>
    /// <description> This column is the primary key and auto-increment  </description>
    /// </item>
    /// <item>
    /// <term>Subject</term>
    /// <description> This field is not null with text length 150 , used to store the mail subjects </description>
    /// </item>
    /// <item>
    /// <term>MessageBody</term>
    /// <description> This field is not null with text length 2500 , used to store the mail body </description>
    /// </item>
    /// 
    /// </list>
    /// </summary>
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "The subject can't be empty"),MinLength(10, ErrorMessage = "Minimum Length is 10"), MaxLength(100, ErrorMessage = "Maximum Length is 2500")]
        [Display(Name = "Subject")]
        [Column(TypeName = "nvarchar(150)")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "The body of the mail can't be empty"),MinLength(10,ErrorMessage ="Minimum Length is 10"),MaxLength(2500, ErrorMessage = "Maximum Length is 2500")]
        [Display(Name = "Email Body")]
        [Column(TypeName = "nvarchar(2500)")]
        public string MessageBody { get; set; }
    }
}

using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Text.RegularExpressions;

namespace EmailSender.Models
{
    /// <summary>
    /// This Model is created for Message Send Form
    /// <list type="bullet">
    /// 
    /// <listheader> Available Properties [ Members ] : </listheader>
    ///
    /// <item>
    /// <term>Subject</term>
    /// <description> This field is disabled in Send Email form , just you can read it </description>
    /// </item>
    /// 
    /// <item>
    /// <term>MessageBody</term>
    /// <description> This field is disabled in Send Email form , just you can read it </description>
    /// </item>
    /// <item>
    /// <term>ToEmailAddresses</term>
    /// <description> Email Addresses you whish to send the mail to them </description>
    /// </item>
    /// </list>
    /// </summary>
    public class MailToBeSent: IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The subject can't be empty"), MinLength(10, ErrorMessage = "Minimum Length is 10"), MaxLength(100, ErrorMessage = "Maximum Length is 2500")]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "The body of the mail can't be empty"), MinLength(10, ErrorMessage = "Minimum Length is 10"), MaxLength(2500, ErrorMessage = "Maximum Length is 2500")]
        [Display(Name = "Email Body")]
        public string MessageBody { get; set; }
        //[Required(ErrorMessage = "The email addresses can't be empty")]
        [Display(Name = "Email Addresses")]
        public string ToEmailAddresses { get; set; }


        /// <summary>
        /// 
        /// This method created to validate the  Emaill Address before submitting it 
        /// 
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var memeberTobeValidated = new []{"ToEmailAddresses"};

            string emailPattern = @"^(([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)(\s*,\s*|\s*$))*$";
            Regex emailRegex = new Regex(emailPattern);
            
           
            if (this.ToEmailAddresses == null)
            {
                yield return new ValidationResult("You should enter atleast one email address", memeberTobeValidated);
            }
            else if(!this.ToEmailAddresses.Contains("@")) {

                yield return new ValidationResult("The email address should contains @", memeberTobeValidated);
            }
            else
            {
                // Check Addresses Pattern
        
                this.ToEmailAddresses = this.ToEmailAddresses.Trim(); // remove white spaces in the start and in the end [to make sure there are no additional whitespaces]
                MatchCollection matchedEmails = emailRegex.Matches(this.ToEmailAddresses);
                
                if (matchedEmails.Count == 0)
                {
                    // if there are one or more address not matched the pattern
                    yield return new ValidationResult("There are one or more wrong email addresses", memeberTobeValidated);
                }

            }
        }

    }
}

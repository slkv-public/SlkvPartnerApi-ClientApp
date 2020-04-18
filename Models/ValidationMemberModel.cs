using System.Collections.Generic;

namespace SwissLife.Slkv.Partner.ClientAppSample.Models
{
    public class ValidationMemberModel
    {
        public string Member { get; set; }
        public IReadOnlyCollection<ValidationMemberErrorModel> Errors { get; set; }
    }
}
using System.Collections.Generic;

namespace SwissLife.Slkv.Partner.ClientAppSample.Models
{
    public class ValidationResultModel
    {
        public bool IsValid { get; set; }

        public IReadOnlyCollection<ValidationMemberModel> Members { get; set; }
    }
}

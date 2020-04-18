using System.Collections.Generic;

namespace SwissLife.Slkv.Partner.ClientAppSample.Models
{
    public class ValidationMemberErrorModel
    {
        public string Code { get; set; }

        public IReadOnlyDictionary<string, object> Parameters { get; set; }
    }
}
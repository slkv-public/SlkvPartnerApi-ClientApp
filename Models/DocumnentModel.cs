using System;

namespace SwissLife.Slkv.Partner.ClientAppSample.Models
{
    public class DocumnentModel
    {
        public string DocumentId { get; set; }

        public string DocumentName { get; set; }

        public DateTime? DocumentDate { get; set; }

        public string ContractId { get; set; }

        public string InsuredPersonId { get; set; }
    }
}
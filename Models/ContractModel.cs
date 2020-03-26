using System;

namespace SwissLife.Slkv.Partner.ClientAppSample.Models
{
    public class ContractModel
    {
        public string ContractId { get; set; }

        public string ContractNumber { get; set; }

        public string CompanyName { get; set; }

        public string AddressLine01 { get; set; }

        public string AddressLine02 { get; set; }

        public string CareOfLine01 { get; set; }

        public string CareOfLine02 { get; set; }

        public string Zip { get; set; }

        public string City { get; set; }

        public string Language { get; set; }

        public string TypeOfPensionPlan { get; set; }

        public string Uid { get; set; }

        public DateTime? ContractStartDate { get; set; }
    }
}
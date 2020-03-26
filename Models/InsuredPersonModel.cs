using System;

namespace SwissLife.Slkv.Partner.ClientAppSample.Models
{
    public class InsuredPersonModel
    {
        public DateTime DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string CorrespondenceLanguage { get; set; }

        public decimal EmploymentRate { get; set; }

        public decimal AnnualSalary { get; set; }

        public bool IsFullyAbleToWork { get; set; }

        public string InsuredPersonId { get; set; }

        public long Svn { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public DateTime InsuranceEntryDate { get; set; }

        public DateTime CompanyEntryDate { get; set; }

        public bool? IsSelfEmployed { get; set; }

        public bool? IsCrossBorderCommuter { get; set; }

        public DateTime? DateOfMarriage { get; set; }
    }
}
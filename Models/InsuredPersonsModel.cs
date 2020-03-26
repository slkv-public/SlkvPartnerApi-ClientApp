namespace SwissLife.Slkv.Partner.ClientAppSample.Models
{
    public class InsuredPersonsModel
    {
        public InsuredPersonModel[] InsuredPersons { get; set; }

        public string Svn { get; set; }

        public int? PageSize { get; set; }

        public int? PageIndex { get; set; }

        public int TotalItems { get; set; }
    }
}
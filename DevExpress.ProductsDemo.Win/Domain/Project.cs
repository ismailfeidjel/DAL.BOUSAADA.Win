using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.ProductsDemo.Win
{
    public class AProject
    {
        public int Id { get; set; }

        // Daira / Commune
        public string Daira { get; set; }
        public string Commune { get; set; }

        // AdsecT
        public string IntutulePri { get; set; }
        public string ProgrammeYe { get; set; }
        public string Field { get; set; }
        public string Sector { get; set; }

        public decimal RegistrationMont { get; set; }
        public decimal FinancialConsumption { get; set; }
        public decimal FinancialProgress { get; set; }

        public string Status { get; set; }


        public List<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();




    }
}

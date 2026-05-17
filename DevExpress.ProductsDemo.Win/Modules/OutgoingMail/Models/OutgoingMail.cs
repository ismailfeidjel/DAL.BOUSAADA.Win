using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.ProductsDemo.Win.Modules.OutgoingMail
{



    public class OutgoingMail
    {
        public int Id { get; set; }

        public string MailNumber { get; set; }

        public DateTime MailDate { get; set; }

        public string Destination { get; set; }

        public string Subject { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.ProductsDemo.Win.Modules.OutgoingMail
{
    public interface IOutgoingMailRepository
    {
        List<OutgoingMail> GetAll();

        void Add(OutgoingMail mail);
    }
}

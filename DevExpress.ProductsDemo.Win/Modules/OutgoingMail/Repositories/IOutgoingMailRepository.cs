using System.Collections.Generic;

namespace DevExpress.ProductsDemo.Win.Modules.OutgoingMail
{
    public interface IOutgoingMailRepository
    {
        List<OutgoingMail> GetAll();

        void Add(OutgoingMail mail);
    }
}

using DevExpress.DataProcessing.InMemoryDataProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ProductsDemo.Win;

namespace DevExpress.ProductsDemo.Win
{
    public interface IProjectRepository
    {
        List<AProject> GetAllProjects();
    }
}

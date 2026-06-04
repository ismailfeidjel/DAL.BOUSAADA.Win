using DevExpress.ProductsDemo.Win.Domain;
using DevExpress.ProductsDemo.Win.Repositories;
using System.Collections.Generic;

namespace DevExpress.ProductsDemo.Win.Service
{
    public class ProjectService
    {
        private readonly ProjectRepository _repo =
            new ProjectRepository();

        public List<Project> GetProjects()
        {
            return _repo.GetAll();
        }

        public int Save(Project project)
        {
            return _repo.Insert(project);
        }
    }
}
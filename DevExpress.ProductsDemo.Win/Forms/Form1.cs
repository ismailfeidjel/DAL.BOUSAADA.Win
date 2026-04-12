using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.ProductsDemo.Win.Repositories;




namespace DevExpress.ProductsDemo.Win.Forms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            IProjectRepository repo = new ProjectRepository();
            //var data = repo.GetAll();

           // gridControl1.DataSource = data;

        }
    }
}

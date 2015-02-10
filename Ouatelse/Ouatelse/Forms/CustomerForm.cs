﻿using Ouatelse.Managers;
using Ouatelse.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ouatelse.Forms
{
    public partial class CustomerForm : Form
    {
        Customer obj = null;
        
        public CustomerForm(Customer obj)
        {
            InitializeComponent();
            this.obj = obj;
        }


        private void validateButton_Click(object sender, EventArgs e)
        {
            
        }

        private void CustomerForm_Load(object sender, EventArgs e)
        {
            b.Bind(this.FirstName, "Text", obj, "LastName");
            b.Bind(this.LastName, "Text", obj, "FirstName");
            //b.Bind();
            // TODO: FINISH TO POPULATE...

            b.Populate();
        }

        private void validateButton_Click_1(object sender, EventArgs e)
        {
            b.Hydrate();
            CustomerManager.Instance.Save(obj);
        }
    }
}

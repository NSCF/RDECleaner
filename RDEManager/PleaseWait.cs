using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RDEManager
{
    public partial class PleaseWait : Form
    {
        public PleaseWait(Form caller)
        {
            InitializeComponent();
            this.mainform = caller as Main;
            this.lblMessage.Text = mainform.modalMessage;
        }

        private Main mainform {get; set;}
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Room_creator
{
    public partial class MainWindow : Form
    {
        public MainWindow(string? jsonFilePath = null)
        {
            InitializeComponent();
            _jsonFilePath = jsonFilePath;
        }

        readonly string? _jsonFilePath;

        private void MainWindow_Load(object sender, EventArgs e)
        {
            if (_jsonFilePath != null)
            {

            }
        }
    }
}

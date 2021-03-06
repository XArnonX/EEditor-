﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EEditor
{
    public partial class SprayCan : Form
    {
        public SprayCan()
        {
            InitializeComponent();
        }

        private void SprayCan_Load(object sender, EventArgs e)
        {
            ToolTip tp = new ToolTip();
            tp.SetToolTip(numericUpDown1, "Size of the spraying area diameter, in blocks.");
            tp.SetToolTip(numericUpDown2, "Blocks per spray, in percentage.");
            numericUpDown1.Value = MainForm.userdata.sprayr;
            numericUpDown2.Value = MainForm.userdata.sprayp;
        }

        private void SprayCan_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            MainForm.userdata.sprayr = (int)numericUpDown1.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            MainForm.userdata.sprayp = (int)numericUpDown2.Value;
        }
    }
}

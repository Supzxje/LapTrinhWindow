﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_Game_LatThe
{
    public partial class Win_Hard : Form
    {
        int capDo;
        int diemSo;
        int timee;

        public Win_Hard(int cap, int diem, int time)
        {
            InitializeComponent();
            capDo = cap;
            diemSo = diem;
            timee = time;
            label4.Text = $"{capDo}";
            label5.Text = $"{diemSo}";
        }

        private void btnChoiTiep_Click(object sender, EventArgs e)
        {
            this.Hide();
            Hard tmp = new Hard(capDo + 1, timee + 6);
            tmp.ShowDialog();
            this.Close();
        }

        private void btnTroLai_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 tmp = new Form1();
            tmp.ShowDialog();
            this.Close();
        }
    }
}
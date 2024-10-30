using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace Project_Game_LatThe
{
    public partial class Normal : Form
    {
        SoundPlayer player = new SoundPlayer(Properties.Resources.SundialDreams_KevinKern_2637732);

        private int sltool1 = 2, sltool2 = 2;
        private int time;
        private int soHang;
        private int soCot;
        private PictureBox[,] theBai;
        private int capDo;
        private int diem = 0;
        private Timer dongHoTroChoi;
        private ProgressBar thanhThoiGian;
        private Image[,] anhMatTruoc; // ảnh của thẻ hàng i cột j
        private int[] visited; // xem số lần sử dụng của 1 hình ảnh trong resources
        private bool[,] check; // ktr xem hàng i cột j đã có hình chưa
        private int[] arrAnh; // mảng chứa các ảnh được random
        private int theDuocChon_1 = -1, theDuocChon_2 = -1; // lưu vị trí 2 thẻ được lật
        private int[,] arrViTriAnh; // lưu lại các ô sử dụng hình nào
        private int i_last = -1, j_last = -1;
        private int soTheDaXong = 0; // lưu lại số thẻ đã được lật xong trong ma trận


        private Button btnTool1, btnTool2;
        private FlowLayoutPanel toolPanel;
        private TableLayoutPanel mainLayout;
        private FlowLayoutPanel topPanel;
        private Label lblCapDo;
        private Label lblDiem;
        private Button btnTamDung;
        private Button btnAmThanh;
        private Panel khuVucLuoiGame;
        private Button btnTroLai;

        public Normal(int lv, int timemax)
        {
            capDo = lv;
            time = timemax;

            player.Play();
            InitializeComponent();
            this.Text = "Memory of Champions";
            this.ClientSize = new Size(600, 700);
            KhoiTaoGiaoDien();
            BatDauCapDoMoi();
        }

        private void KhoiTaoGiaoDien()
        {
            // Tạo mainLayout với 1 cột - 4 hàng cho việc chứa các panel
            mainLayout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 4, ColumnCount = 1, BackColor = Color.Transparent };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));

            // Xây dựng panel với các control cho vị trí cột 0 - hàng 0
            topPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight};
            lblCapDo = new Label { Text = $"Cấp độ: {capDo}", Font = new Font("Times New Roman", 18, FontStyle.Bold), ForeColor = Color.Gold, Width = 120, Height = 30, Margin = new Padding(0, 10, 0, 0)};
            
            lblDiem = new Label { Text = $"Điểm: {diem}", Font = new Font("Times New Roman", 18, FontStyle.Bold), ForeColor = Color.Gold, Width = 120, Height = 30, Margin = new Padding(0, 10, 0, 0) };

            btnTamDung = new Button { Text = "Tạm Dừng", Font = new Font("Times New Roman", 15, FontStyle.Bold), ForeColor = Color.White, BackColor = Color.Khaki, Width = 110, Height = 35, Margin = new Padding(10, 8, 0, 0) };
            btnTamDung.Click += BtnTamDung_Click;

            btnAmThanh = new Button { Text = "Tắt Nhạc", Font = new Font("Times New Roman", 15, FontStyle.Bold), ForeColor = Color.White, BackColor = Color.Khaki, Width = 110, Height = 35, Margin = new Padding(5, 8, 0, 0) };
            btnAmThanh.Click += BtnAmThanh_Click;

            btnTroLai = new Button { Text = "Trở Lại", Font = new Font("Times New Roman", 15, FontStyle.Bold), ForeColor = Color.White, BackColor = Color.Khaki, Width = 110, Height = 35, Margin = new Padding(5, 8, 0, 0) };
            btnTroLai.Click += btnTroLai_Click;


            topPanel.Controls.Add(lblCapDo);
            topPanel.Controls.Add(lblDiem);
            topPanel.Controls.Add(btnTamDung);
            topPanel.Controls.Add(btnAmThanh);
            topPanel.Controls.Add(btnTroLai);

            // Xây dựng panel cho vị trí cột 0 - hàng 1
            khuVucLuoiGame = new Panel { Dock = DockStyle.Fill };

            // Xây dựng panel với các control cho vị trí cột 0 - hàng 2
            toolPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
            btnTool1 = new Button { Text = "Lật Toàn Bộ", Font = new Font("Times New Roman", 13, FontStyle.Bold), ForeColor = Color.White, BackColor = Color.Khaki, Width = 135, Height = 35, Margin = new Padding(150, 4, 0, 0) };
            btnTool2 = new Button { Text = "Ngừng Thời Gian", Font = new Font("Times New Roman", 13, FontStyle.Bold), ForeColor = Color.White, BackColor = Color.Khaki, Width = 160, Height = 35, Margin = new Padding(8, 4, 0, 0) };
            btnTool1.Click += btnTool1_Click;
            btnTool2.Click += btnTool2_Click;
            toolPanel.Controls.Add(btnTool1);
            toolPanel.Controls.Add(btnTool2);

            // Xây dựng panel với control cho vị trí cột 0 - hàng 3
            thanhThoiGian = new ProgressBar { Dock = DockStyle.Fill, Maximum = time };

            // Đưa các panel vào bảng panel tương ứng và hiện ra màn hình
            mainLayout.Controls.Add(topPanel, 0, 0);
            mainLayout.Controls.Add(khuVucLuoiGame, 0, 1);
            mainLayout.Controls.Add(toolPanel, 0, 2);
            mainLayout.Controls.Add(thanhThoiGian, 0, 3);
            this.Controls.Add(mainLayout);

            // Thiết lập bước nhảy thời gian là 1s
            dongHoTroChoi = new Timer { Interval = 1000 };
            dongHoTroChoi.Tick += DongHoTroChoi_Tick;
        }

        // Cài đặt lại màn chơi
        private void BatDauCapDoMoi()
        {
            lblCapDo.Text = $"Cấp độ: {capDo}";
            lblDiem.Text = $"Điểm: {diem}";
            thanhThoiGian.Value = 0;
            dongHoTroChoi.Start();
            TaoLuoi();
        }

        // Cài đặt ma trận 
        private void TaoLuoi()
        {
            // Chỉnh số lượng hàng với cột cho các vòng
            soHang = 2 + capDo / 2;
            soCot = 2 + capDo / 2;
            if (soHang % 2 != 0)
                soHang -= 1;
            if (soCot % 2 != 0)
                soCot -= 1;
            theBai = new PictureBox[soHang, soCot];
            anhMatTruoc = new Image[soHang, soCot];
            visited = new int[76];
            check = new bool[soHang, soCot];

            // Tạo table panel để quản lí mảng theBai[, ]
            TableLayoutPanel luoiLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = soHang,
                ColumnCount = soCot
            };

            // Khởi tạo
            arrViTriAnh = new int[soHang, soCot];

            // Khởi tạo các hình ảnh được sử dụng 0 lần 
            for (int k = 0; k < 76; k++)
            {
                visited[k] = 0;
            }
            // Tạo 1 list các ảnh đã được random
            Random rand = new Random();
            arrAnh = new int[soHang * soCot / 2 + 1];
            bool[] ktra = new bool[soHang * soCot / 2 + 1]; // ktra xem vị trí h trong mảng đã có hình chưa
            for (int h = 1; h <= soHang * soCot / 2; h++)
            {
                ktra[h] = false;
                while (!ktra[h])
                {
                    int number_1 = rand.Next(0, 76);
                    if (visited[number_1] == 0)
                    {
                        arrAnh[h] = number_1;
                        visited[number_1] = 1;
                        ktra[h] = true;
                    }
                }
            }

            // Khởi tạo lại các hình ảnh được sử dụng 0 lần 
            for (int k = 0; k < 76; k++)
            {
                visited[k] = 0;
            }

            for (int i = 0; i < soHang; i++)
            {
                luoiLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / soHang)); // cài đặt kthuoc chiếm bao nhiêu % trong trong table
                for (int j = 0; j < soCot; j++)
                {

                    luoiLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / soCot)); // cài đặt kthuoc chiếm bao nhiêu % trong table

                    PictureBox the = new PictureBox
                    {
                        Dock = DockStyle.Fill,
                        Tag = $"{i}-{j}"
                    };
                    the.SizeMode = PictureBoxSizeMode.StretchImage;
                    the.Image = Image.FromFile("Resources/mat_sau.jpg");
                    the.Click += The_Click;
                    luoiLayout.Controls.Add(the, j, i);
                    theBai[i, j] = the;

                    check[i, j] = false;
                    while (!check[i, j])
                    {
                        int number_2 = rand.Next(1, soHang * soCot / 2 + 1);
                        if (visited[number_2] != 2)
                        {
                            anhMatTruoc[i, j] = Image.FromFile($"Resources/{arrAnh[number_2]}.jpg");
                            arrViTriAnh[i, j] = number_2;
                            check[i, j] = true;
                            visited[number_2] += 1;
                        }
                    }

                }
            }

            khuVucLuoiGame.Controls.Add(luoiLayout);
        }

        // Tương tác với chuột
        private async void The_Click(object sender, EventArgs e)
        {
            // Xử lý thẻ được chọn
            PictureBox the = sender as PictureBox;
            string strTag = the.Tag as string;
            int i = (int)strTag[0];
            int j = (int)strTag[2];
            theBai[i - 48, j - 48].Image = anhMatTruoc[i - 48, j - 48];
            theBai[i - 48, j - 48].Click -= The_Click; // xóa đi event click tạm thời để không bug

            await Task.Delay(500);

            if (theDuocChon_1 == -1 || theDuocChon_2 == -1)
            {
                if (theDuocChon_1 == -1)
                    theDuocChon_1 = arrViTriAnh[i - 48, j - 48];
                else theDuocChon_2 = arrViTriAnh[i - 48, j - 48];
            }

            if (theDuocChon_1 != -1 && theDuocChon_2 != -1)
            {
                if (theDuocChon_1 == theDuocChon_2)
                {
                    theBai[i - 48, j - 48].Image = Image.FromFile("Resources/background_trong_suot.png");
                    theBai[i - 48, j - 48].Click -= The_Click;
                    arrViTriAnh[i - 48, j - 48] = -1;
                    theBai[i_last - 48, j_last - 48].Image = Image.FromFile("Resources/background_trong_suot.png");
                    theBai[i_last - 48, j_last - 48].Click -= The_Click;
                    arrViTriAnh[i_last - 48, j_last - 48] = -1;
                    soTheDaXong += 2;
                    diem += 100;
                    lblDiem.Text = $"Điểm: {diem}";


                    theDuocChon_1 = -1;
                    theDuocChon_2 = -1;
                }
                else if (theDuocChon_1 != theDuocChon_2)
                {
                    theBai[i - 48, j - 48].Image = Image.FromFile("Resources/mat_sau.jpg");
                    theBai[i_last - 48, j_last - 48].Image = Image.FromFile("Resources/mat_sau.jpg");
                    theDuocChon_1 = -1;
                    theDuocChon_2 = -1;

                    // Thêm lại event click
                    theBai[i - 48, j - 48].Click += The_Click;
                    theBai[i_last - 48, j_last - 48].Click += The_Click;
                }
            }

            if (soTheDaXong == soHang * soCot)
            {
                dongHoTroChoi.Tick -= DongHoTroChoi_Tick;
                this.Hide();
                Win_Normal w = new Win_Normal(capDo, diem, time);
                w.ShowDialog();
                this.Close();
            }

            // Lưu lại vị trí thẻ
            i_last = i;
            j_last = j;
        }

        // Button tạm dừng
        private void BtnTamDung_Click(object sender, EventArgs e)
        {
            if (dongHoTroChoi.Enabled)
            {
                dongHoTroChoi.Stop();
                btnTamDung.Text = "Tiếp Tục";

                // Xóa tạm thời event click cho tất cả thẻ còn lại
                for (int i = 0; i < soHang; i++)
                {
                    for (int j = 0; j < soCot; j++)
                    {
                        if (arrViTriAnh[i, j] != -1)
                        {
                            theBai[i, j].Click -= The_Click;
                        }
                    }
                }

                // Vô hiệu hóa button
                btnTool1.Enabled = false;
                btnTool2.Enabled = false;
                btnAmThanh.Enabled = false;
            }
            else
            {
                dongHoTroChoi.Start();
                btnTamDung.Text = "Tạm Dừng";

                // Bật lại event click cho tất cả thẻ còn lại
                for (int i = 0; i < soHang; i++)
                {
                    for (int j = 0; j < soCot; j++)
                    {
                        if (arrViTriAnh[i, j] != -1)
                        {
                            theBai[i, j].Click += The_Click;
                        }
                    }
                }

                // Bật lại các button
                btnTool1.Enabled = true;
                btnTool2.Enabled = true;
                btnAmThanh.Enabled = true;
            }
        }

        private void Normal_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        // Button âm thanh
        private void BtnAmThanh_Click(object sender, EventArgs e)
        {
            /*amThanhBat = !amThanhBat;
            btnAmThanh.Text = amThanhBat ? "Âm Thanh Bật" : "Âm Thanh Tắt";*/
            if (btnAmThanh.Text == "Tắt Nhạc")
            {
                player.Stop();
                btnAmThanh.Text = "Mở Nhạc";
            }
            else if (btnAmThanh.Text == "Mở Nhạc")
            {
                player.Play();
                btnAmThanh.Text = "Tắt Nhạc";
            }    
        }

        // Button trở lại trang chủ
        private void btnTroLai_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn muốn trở lại trang chủ của trò chơi ?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                dongHoTroChoi.Tick -= DongHoTroChoi_Tick;
                this.Hide();
                Form1 tmp = new Form1();
                tmp.ShowDialog();
                this.Close();
            }    
        }

        // Progressbar thời gian
        private void DongHoTroChoi_Tick(object sender, EventArgs e)
        {
            if (thanhThoiGian.Value < thanhThoiGian.Maximum)
            {
                thanhThoiGian.Value += 1;
            }
            else
            {
                dongHoTroChoi.Stop();
                this.Hide();
                Lose_Normal l = new Lose_Normal(capDo, diem, time);
                l.ShowDialog();
                this.Close();
            }
        }

        // Tool lật toàn bộ thẻ
        private async void btnTool1_Click(object sender, EventArgs e)
        {
            if (sltool1 > 0)
            {
                for (int i = 0; i < soHang; i++)
                {
                    for (int j = 0; j < soCot; j++)
                    {
                        if (arrViTriAnh[i, j] != -1)
                        {
                            theBai[i, j].Image = anhMatTruoc[i, j];
                        }
                    }
                }

                await Task.Delay(800);

                for (int k = 0; k < soHang; k++)
                {
                    for (int h = 0; h < soCot; h++)
                    {
                        if (arrViTriAnh[k, h] != -1)
                        {
                            theBai[k, h].Image = Image.FromFile("Resources/mat_sau.jpg");
                        }
                    }
                }

                sltool1 -= 1;
            }
        }

        // Tool ngừng thời gian
        private async void btnTool2_Click(object sender, EventArgs e)
        {
            if (sltool2 > 0)
            {
                dongHoTroChoi.Tick -= DongHoTroChoi_Tick;

                await Task.Delay(4000);

                dongHoTroChoi.Tick += DongHoTroChoi_Tick;

                sltool2 -= 1;
            }
        }
    }
}

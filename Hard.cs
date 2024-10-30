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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using System.Media;

namespace Project_Game_LatThe
{
    public partial class Hard : Form
    {
        SoundPlayer player = new SoundPlayer(Properties.Resources.SundialDreams_KevinKern_2637732);

        Random rand = new Random();

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
        private int[,] arrViTriAnh; // lưu lại các ô sử dụng hình nào
        private int viTriAnhFind = -1;
        private int i_find = -1, j_find = -1;
        private int soTheDaXong = 0; // lưu lại số thẻ đã được lật xong trong ma trận
        private bool coTheDangLat = false; // để ktr xem có thể nào đang lật không, tránh việc người chơi bug

        private PictureBox anhCanTim = new PictureBox {BackColor = Color.Blue };
        private Panel imagePanel;
        private TableLayoutPanel mainLayout;
        private FlowLayoutPanel topPanel;
        private Label lblCapDo;
        private Label lblDiem;
        private Button btnTamDung;
        private Button btnAmThanh;
        private Panel khuVucLuoiGame;
        private Button btnTroLai;

        public Hard(int lv, int timemax)
        {
            capDo = lv;
            time = timemax;

            player.Play();
            InitializeComponent();
            this.Text = "Memory of Champions";
            this.ClientSize = new Size(600, 700);
            KhoiTaoGiaoDien();
            BatDauCapDoMoi();
            latToanBoThe();
        }

        private void KhoiTaoGiaoDien()
        {
            // Tạo mainLayout với 1 cột - 4 hàng cho việc chứa các panel
            mainLayout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 4, ColumnCount = 1, BackColor = Color.Transparent };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));

            // Xây dựng panel với các control cho vị trí cột 0 - hàng 0
            topPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
            lblCapDo = new Label { Text = $"Cấp độ: {capDo}", Font = new Font("Times New Roman", 18, FontStyle.Bold), ForeColor = Color.Gold, Width = 120, Height = 30, Margin = new Padding(0, 10, 0, 0) };

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

            // Xây dựng panel với các control cho vị trí cột 0 - hàng 1
            imagePanel = new Panel { Dock = DockStyle.Fill};
            anhCanTim.Image = Image.FromFile("Resources/mat_sau.jpg");
            anhCanTim.SizeMode = PictureBoxSizeMode.StretchImage;
            anhCanTim.Width = 200;
            anhCanTim.Height = 135;
            anhCanTim.Location = new Point(200, 2);
            imagePanel.Controls.Add(anhCanTim);
            

            // Xây dựng panel cho vị trí cột 0 - hàng 2
            khuVucLuoiGame = new Panel { Dock = DockStyle.Fill};

            // Xây dựng panel với control cho vị trí cột 0 - hàng 3
            thanhThoiGian = new ProgressBar { Dock = DockStyle.Fill, Maximum = time };

            // Đưa các panel vào bảng panel tương ứng và hiện ra màn hình
            mainLayout.Controls.Add(topPanel, 0, 0);
            mainLayout.Controls.Add(imagePanel, 0, 1);
            mainLayout.Controls.Add(khuVucLuoiGame, 0, 2);
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
                        int number = rand.Next(0, 76);
                        if (visited[number] == 0)
                        {
                            anhMatTruoc[i, j] = Image.FromFile($"Resources/{number}.jpg");
                            arrViTriAnh[i, j] = number;
                            check[i, j] = true;
                            visited[number] += 1;
                        }
                    }

                }
            }

            khuVucLuoiGame.Controls.Add(luoiLayout);
        }

        // Tương tác với chuột
        private async void The_Click(object sender, EventArgs e)
        {
            if (!coTheDangLat)
            {
                coTheDangLat = true;

                // Xử lý thẻ được chọn
                PictureBox the = sender as PictureBox;
                string strTag = the.Tag as string;
                int i = (int)strTag[0];
                int j = (int)strTag[2];
                theBai[i - 48, j - 48].Image = anhMatTruoc[i - 48, j - 48];
                theBai[i - 48, j - 48].Click -= The_Click; // xóa đi event click tạm thời để không bug

                await Task.Delay(300);

                if (i_find == -1 && j_find == -1)
                {
                    i_find = rand.Next(0, soHang);
                    j_find = rand.Next(0, soCot);
                    viTriAnhFind = arrViTriAnh[i_find, j_find];
                    anhCanTim.Image = anhMatTruoc[i_find, j_find];

                    theBai[i - 48, j - 48].Image = Image.FromFile("Resources/mat_sau.jpg");
                    theBai[i - 48, j - 48].Click += The_Click;
                }
                else if (arrViTriAnh[i - 48, j - 48] == viTriAnhFind)
                {
                    theBai[i - 48, j - 48].Image = Image.FromFile("Resources/background_trong_suot.png");
                    arrViTriAnh[i - 48, j - 48] = -1;

                    if (soTheDaXong != soHang * soCot - 1)
                    {
                        while (arrViTriAnh[i_find, j_find] == -1)
                        {
                            i_find = rand.Next(0, soHang);
                            j_find = rand.Next(0, soCot);
                        }
                        viTriAnhFind = arrViTriAnh[i_find, j_find];
                        anhCanTim.Image = anhMatTruoc[i_find, j_find];
                    }

                    diem += 100;
                    lblDiem.Text = $"Điểm: {diem}";

                    soTheDaXong += 1;
                }
                else
                {
                    theBai[i - 48, j - 48].Image = Image.FromFile("Resources/mat_sau.jpg");
                    theBai[i - 48, j - 48].Click += The_Click;
                }

                if (soTheDaXong == soHang * soCot)
                {
                    dongHoTroChoi.Tick -= DongHoTroChoi_Tick;
                    this.Hide();
                    Win_Hard w = new Win_Hard(capDo, diem, time);
                    w.ShowDialog();
                    this.Close();
                }

                coTheDangLat = false;
            }    
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
                btnAmThanh.Enabled = true;
            }
        }

        // Button âm thanh
        private void BtnAmThanh_Click(object sender, EventArgs e)
        {
            if (btnAmThanh.Text == "Tắt Nhạc")
            {
                player.Stop();
                btnAmThanh.Text = "Mở Nhạc";
            }
            else
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
                //MessageBox.Show("Hết giờ!");
                this.Hide();
                Lose_Hard l = new Lose_Hard(capDo, diem, time);
                l.ShowDialog();
                this.Close();
            }
        }

        // Lật toàn bộ thẻ khi vào cấp độ mới
        private async void latToanBoThe()
        {
            for (int i = 0; i < soHang; i++)
            {
                for (int j = 0; j < soCot; j++)
                {
                    theBai[i, j].Image = anhMatTruoc[i, j];
                }    
            }

            await Task.Delay(2000);

            for (int k = 0 ; k < soHang; k++)
            {
                for (int h = 0 ; h < soCot; h++)
                {
                    theBai[k, h].Image = Image.FromFile("Resources/mat_sau.jpg");
                }    
            }    
        }
    }
}

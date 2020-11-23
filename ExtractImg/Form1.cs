using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExtractImg
{
    public partial class Form1 : Form
    {
        private Helper Helper=new Helper();
        public Form1()
        {
            InitializeComponent();
           
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> cornerCoor = new List<string>();

            string leftTop = (textBox1.Text);
            string rightTop = (textBox2.Text);
            string leftBottom = (textBox3.Text);
            string rightBottom = (textBox4.Text);
            int zoom = int.Parse(textBox5.Text);

            //string leftTop = "117.199115,33.18152,11";
            //string rightTop = "120.495013,33.071111,11";
            //string leftBottom = "121.022357,29.857611,11";
            //string rightBottom = "115.573138,29.360927,11";
            //int zoom = 13;
            cornerCoor.Add(leftTop);
            cornerCoor.Add(rightTop);
            cornerCoor.Add(leftBottom);
            cornerCoor.Add(rightBottom);

            
            for(int i = 11; i <= zoom; i++)
            {
                List<Tile> tiles = new List<Tile>();
                string path = textBox6.Text+ '\\' + i.ToString();
                string dest = textBox7.Text;
                //string path = @"G:\DB\DATA\Img" + '\\' + i.ToString();
                //string dest = @"G:\DB\copyImag\";
                foreach (var item in cornerCoor)
                {
                    Coordnate coordnate = Helper.ConvertStringToCoord(item);
                    Tile tile = Helper.deg2Num(coordnate.Lon, coordnate.Lat, i);//传入坐标和zoom获取瓦片
                    tiles.Add(tile);
                }
                RectangleTile rectangleTile = new RectangleTile(tiles);
                int xmin = rectangleTile.xmin;
                int xmax = rectangleTile.xmax;
                int ymin = rectangleTile.ymin;
                int ymax = rectangleTile.ymax;
                for(int j = xmin; j < xmax; j++)
                {
                    for(int q = ymin; q < ymax; q++)
                    {
                        if (j == 1696&&q==824)
                        {
                        };
                      string  copypath = path + '\\' + j + '\\' + q +".png";
                        string destPath = dest + i + '\\' + j + '\\' + q + ".png";
                        string destDir = dest + i + '\\' + j;
                        if (File.Exists(copypath))
                        {
                            DirectoryHelper.MakeDirectory(destDir);
                            File.Copy(copypath, destPath, true);
                        }
                            
                        
                    }
                   
                }

                MessageBox.Show($"{i}级完成");
            }
            MessageBox.Show("复制完成");

        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox6.Text = dialog.SelectedPath;
                 
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox7.Text = dialog.SelectedPath;

            }
        }
    }
}

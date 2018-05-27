using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sudokuSolver10 {
    public partial class Form1 : Form {
        private SudokuList sudokuList;
        private string _ps;
        private int imgPointX, imgPointY, imgMaxWidth, imgMaxHeight;
        private double imgMaxRes;

        public Form1() {
            InitializeComponent();
            _ps = "";
            imgPointX = 20;
            imgPointY = 20;
            imgMaxWidth = Width - panel1.Width - 50;
            imgMaxHeight = Height - 50;
            imgMaxRes = ((double)imgMaxWidth) / imgMaxHeight;
        }

        private void button1_Click(object sender, EventArgs e) {
            var fbd = new FolderBrowserDialog {SelectedPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName) + @"\Image"};
            if (fbd.ShowDialog() != DialogResult.OK) return;
            var paths = Directory.GetFiles(fbd.SelectedPath, "*.jpg");
            sudokuList = new SudokuList(paths);
        }


        private void Form1_Paint(object sender, PaintEventArgs e) {
            if (sudokuList == null)
                return;
            lock (sudokuList.LockOn) {
                if (!sudokuList.Ready) return;
                var img = sudokuList.GetCurrentImg(_ps);
                var res = ((double)img.Width)/(img.Height);
                int w, h;
                if (imgMaxRes < res) {
                    w = imgMaxWidth;
                    h = (int) (w/res);
                }
                else {
                    h = imgMaxHeight;
                    w = (int) (h*res);
                }
                if (RotateCheck.Checked) using (var g = Graphics.FromImage(img)) {
                        
                        g.TranslateTransform((float)img.Width / 2, (float)img.Height / 2);
                        g.RotateTransform(sudokuList.GetRotate(sudokuList.currentImg));
                        g.TranslateTransform(-(float)img.Width / 2, -(float)img.Height / 2);
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.DrawImage(img, new Point(0, 0));
                    }
                e.Graphics.DrawImage(img, new Rectangle(imgPointX,imgPointY,w,h));
            }

        }



        /*private void FormGraphics(Graphics g) {
            if (!sudokuList.Ready)
                return;
            MessageBox.Show(3 + "");
            g.DrawImage(sudokuList.GetCurrentImg("bw"), new Point(0,0));
            //const int lineX = 210;
            //g.DrawLine(new Pen(Color.Black, 1), lineX, 0, lineX, Size.Height);
        }*/

        private void startButton_Click(object sender, EventArgs e) {
            if (sudokuList == null) {
                MessageBox.Show("Не выбрана папка");
                return;
            }
            progressBar1.Visible = true;
            progressBar1.Value = 0;
            backgroundWorker1.CancelAsync();
            backgroundWorker1.Dispose();
            backgroundWorker1 = new BackgroundWorker {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            backgroundWorker1.RunWorkerAsync();
        }

        private void button2_Click(object sender, EventArgs e) {
            if (sudokuList == null)
                return;
            lock (sudokuList.LockOn) {
                if (!sudokuList.Ready) {
                    return;
                }
            }
            sudokuList.mm();
            Invalidate();
        }

        private void button3_Click(object sender, EventArgs e) {
            if (sudokuList == null)
                return;
            lock (sudokuList.LockOn) {
                if (!sudokuList.Ready) {
                    return;
                }
            }
            sudokuList.pp();
            Invalidate();

        }

        private void ChangePropertiesString() {
            _ps = "";
            _ps += blackCheck.Checked ? "b" : "";
            _ps += whiteCheck.Checked ? "w" : "";
            _ps += whiteRectCheck.Checked ? "r" : "";
            _ps += sudokuLineCheck.Checked ? "s" : "";
            _ps += rotateLineCheck.Checked ? "l" : "";
            _ps += checkBox1.Checked ? "c" : "";
            Invalidate();
        }

        private void whiteCheck_CheckedChanged(object sender, EventArgs e) {
            ChangePropertiesString();
        }

        private void blackCheck_CheckedChanged(object sender, EventArgs e) {
            ChangePropertiesString();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            BackgroundWorker worker = sender as BackgroundWorker;
            sudokuList.SolveAll(worker);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            progressBar1.Visible = false;
            Invalidate();
        }

        private void RotateCheck_CheckedChanged(object sender, EventArgs e) {
            Invalidate();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) {
            ChangePropertiesString();
        }

        private void whiteRectCheck_CheckedChanged(object sender, EventArgs e) {
            ChangePropertiesString();
        }

        private void sudokuLineCheck_CheckedChanged(object sender, EventArgs e) {
            ChangePropertiesString();
        }

        private void rotateLineCheck_CheckedChanged(object sender, EventArgs e) {
            ChangePropertiesString();
        }
    }
}

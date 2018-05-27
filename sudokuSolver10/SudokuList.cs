using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sudokuSolver10 {
    class SudokuList {
        private List<sudokuImg> _sudokuImgs;

        public int GetRotate(int i) {
            try {
                return _sudokuImgs[i].RotateAngle;
            }
            catch {
                return 0;
            }
        }

        static private Pen _rotateLinePen = new Pen(Color.Red, 2);
        public int currentImg { get; private set; }

        public void pp() {
            currentImg = (currentImg + 1) % _sudokuImgs.Count;
        }
        public void mm() {
            currentImg = (currentImg + _sudokuImgs.Count - 1) % _sudokuImgs.Count;
        }

        public bool Ready { get; private set; }
        public readonly object LockOn = new object();
        private readonly Thread _thrd;

        public SudokuList(IEnumerable<string> paths) {
            _sudokuImgs = new List<sudokuImg>();
            foreach (var t in paths) {
                _sudokuImgs.Add(new sudokuImg(t));
            }
            currentImg = 0;
            //_thrd = new Thread(Run) {Name = "Solver"};
            lock (LockOn) {
                Ready = false;
            }
        }

        public void SolveAll(BackgroundWorker worker) {
            //_thrd.Start();
            var d = 100/_sudokuImgs.Count;
            worker.ReportProgress(0);
            for (var i = 0; i < _sudokuImgs.Count; i++) {
                _sudokuImgs[i].Solve();
                worker.ReportProgress((i + 1) * d);
            }
            worker.ReportProgress(100);
            lock (LockOn) {
                Ready = true;
            }
        }




        private void Run() {
            foreach (var t in _sudokuImgs) {
                t.Solve();
            }
            lock (LockOn) {
                Ready = true;
            }
            MessageBox.Show("333");
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties">Строка настроек. Любой порядок символов: b - black pixels, w - white pixels</param>
        /// <returns></returns>
        unsafe public Bitmap GetCurrentImg(string properties) {
            var ret = (Bitmap) Image.FromFile(_sudokuImgs[currentImg].Path);
            var bmpData = ret.LockBits(new Rectangle(0, 0, ret.Width, ret.Height), ImageLockMode.ReadWrite, ret.PixelFormat);
            var bytes = bmpData.Height*bmpData.Stride;
            var bytesArray = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, bytesArray, 0, bytes);
            foreach (var t in properties) {
                switch (t) {
                    case 'b':
                        if(_sudokuImgs[currentImg].Monochromical)
                            GetBitmap_Black(ref bytesArray, _sudokuImgs[currentImg]);
                        break;
                    case 'w':
                        if (_sudokuImgs[currentImg].Monochromical)
                            GetBitmap_White(ref bytesArray, _sudokuImgs[currentImg]);
                        break;
                    default:
                        continue;
                }
            }
            using (var ws = new UnmanagedMemoryStream((byte*)bmpData.Scan0.ToPointer(), bytes, bytes, FileAccess.Write)) {
                ws.Write(bytesArray, 0, bytes);
            }
            ret.UnlockBits(bmpData);

            foreach (var t in properties) {
                switch (t) {
                    case 'l':
                        if (_sudokuImgs[currentImg].Hough)
                            GetBitmap_RotateLine(ret, _sudokuImgs[currentImg]);
                        break;
                    case 'r':
                        GetBitmap_WhitePoly(ret, _sudokuImgs[currentImg]);
                        break;
                    case 's':
                        GetBitmap_SudokuLines(ret, _sudokuImgs[currentImg]);
                        break;
                    case 'c':
                        GetBitmap_SudokuCif(ret, _sudokuImgs[currentImg]);
                        break;
                    default:
                        continue;
                }
            }

            return ret;
        }

        unsafe private void GetBitmap_Black(ref byte[] imgBytes, sudokuImg si) {
            var mbm = si.MBM;
            fixed (byte* firstImgByte = &imgBytes[0]) fixed (bool* firstMBMRef = &mbm[0, 0])
            {
                var fib = firstImgByte;
                var fmr = firstMBMRef;
                for (var i = 0; i < imgBytes.Length / 3; i++) {
                    if (!*fmr++) {
                        fib += 3;
                    }
                    else {
                        *fib++ = 0x0;
                        *fib++ = 0x0;
                        *fib++ = 0x0;
                    }
                }
            }
        }
        unsafe private void GetBitmap_White(ref byte[] imgBytes, sudokuImg si) {
            var mbm = si.MBM;
            fixed (byte* firstImgByte = &imgBytes[0]) fixed (bool* firstMBMRef = &mbm[0, 0])
            {
                var fib = firstImgByte;
                var fmr = firstMBMRef;
                for (var i = 0; i < imgBytes.Length / 3; i++) {
                    if (*fmr++) {
                        fib += 3;
                    }
                    else {
                        *fib++ = 0xFF;
                        *fib++ = 0xFF;
                        *fib++ = 0xFF;
                    }
                }
            }
        }

        private void GetBitmap_RotateLine(Image bm, sudokuImg si) {
            using (var g = Graphics.FromImage(bm)) {
                g.DrawLine(new Pen(Color.Red, 2), 5, 0, 0, 5);
                int x0 = bm.Width / 3, x1=2 * (bm.Width / 3), y0=bm.Height/3, y1=2 * (bm.Height / 3);
                g.DrawRectangle(new Pen(Color.DeepSkyBlue, 1), x0, y0, x0, y0);
                if (si.MaxLine.Th == 0)
                    g.DrawLine(_rotateLinePen, si.MaxLine.Rh, y0, si.MaxLine.Rh, y1);
                else
                    g.DrawLine(_rotateLinePen, 0, si.MaxLine.Rh / Acc.Sins[si.MaxLine.Th], bm.Width, (si.MaxLine.Rh - bm.Width * Acc.Coss[si.MaxLine.Th]) / Acc.Sins[si.MaxLine.Th]);
            }

        }

        private void GetBitmap_WhitePoly(Image bm, sudokuImg si) {
            using (var g = Graphics.FromImage(bm)) {
                g.DrawPolygon(new Pen(Color.Red, 2), si.whitePolygon);
                /*g.DrawLine(new Pen(Color.Blue, 2), 0, si.topWhiteLine.y(0), bm.Width, si.topWhiteLine.y(bm.Width));
                g.DrawLine(new Pen(Color.Blue, 2), 0, si.bottomWhiteLine.y(0), bm.Width, si.bottomWhiteLine.y(bm.Width));
                g.DrawLine(new Pen(Color.Blue, 2), si.leftWhiteLine.x(0), 0, si.leftWhiteLine.x(bm.Height), bm.Height);
                g.DrawLine(new Pen(Color.Blue, 2), si.rightWhiteLine.x(0), 0, si.rightWhiteLine.x(bm.Height), bm.Height);*/
            }
        }
        private void GetBitmap_SudokuLines(Image bm, sudokuImg si) {
            using (var g = Graphics.FromImage(bm)) {
                if (si.horLines[0] == null) return;
                try {
                    //g.DrawRectangle(new Pen(Color.Brown, 2), si.rectL);
                    //g.DrawLine(new Pen(Color.Blue, 2), 0, si.horLines[0].y(0), bm.Width, si.horLines[0].y(bm.Width));
                    //MessageBox.Show(si.verLines[0].x(0) + "  " + si.verLines[0].y(bm.Height) + "   " + bm.Height);
                    /*for (int i = 0; i < 4; i++) {
                        g.DrawLine(new Pen(Color.Blue, 2), si.verLines[i * 3].x(0), 0, si.verLines[i * 3].x(bm.Height), bm.Height);
                        g.DrawLine(new Pen(Color.Blue, 2), 0, si.horLines[i * 3].y(0), bm.Width, si.horLines[i * 3].y(bm.Width));
                    }*/
                    for (int i = 0; i < 10; i++) {
                        g.DrawLine(new Pen(Color.Blue, 2), si.dots[0, i].X, si.dots[0, i].Y, si.dots[9, i].X, si.dots[9, i].Y);
                        g.DrawLine(new Pen(Color.Blue, 2), si.dots[i, 0].X, si.dots[i, 0].Y, si.dots[i, 9].X, si.dots[i, 9].Y);
                    }
                    //for (int i = 0; i < 10; i++) {
                    /*int j = 2;
                        for (int i = 0; i < 10; i++) {
                            g.DrawLine(new Pen(Color.Blue, 2), si.dots[i, j].X-3, si.dots[i, j].Y, si.dots[i, j].X+3, si.dots[i, j].Y);
                        }*/
                    //}
                }
                catch (Exception e) { }
                /*g.DrawLine(new Pen(Color.Blue, 2), 0, si.topWhiteLine.y(0), bm.Width, si.topWhiteLine.y(bm.Width));
                g.DrawLine(new Pen(Color.Blue, 2), 0, si.bottomWhiteLine.y(0), bm.Width, si.bottomWhiteLine.y(bm.Width));
                g.DrawLine(new Pen(Color.Blue, 2), si.leftWhiteLine.x(0), 0, si.leftWhiteLine.x(bm.Height), bm.Height);
                g.DrawLine(new Pen(Color.Blue, 2), si.rightWhiteLine.x(0), 0, si.rightWhiteLine.x(bm.Height), bm.Height);*/
            }
        }
        private void GetBitmap_SudokuCif(Image bm, sudokuImg si) {
            using (var g = Graphics.FromImage(bm)) {
                if (si.horLines[0] == null) return;
                try {
                    //g.DrawRectangle(new Pen(Color.Brown, 2), si.rectL);
                    //g.DrawLine(new Pen(Color.Blue, 2), 0, si.horLines[0].y(0), bm.Width, si.horLines[0].y(bm.Width));
                    //MessageBox.Show(si.verLines[0].x(0) + "  " + si.verLines[0].y(bm.Height) + "   " + bm.Height);
                    /*for (int i = 0; i < 4; i++) {
                        g.DrawLine(new Pen(Color.Blue, 2), si.verLines[i * 3].x(0), 0, si.verLines[i * 3].x(bm.Height), bm.Height);
                        g.DrawLine(new Pen(Color.Blue, 2), 0, si.horLines[i * 3].y(0), bm.Width, si.horLines[i * 3].y(bm.Width));
                    }*/
                    for (int i = 0; i < 9; i++) {
                        for (int j = 0; j < 9; j++) {
                            if (si.chi[i,j] <= 0) continue;
                            g.DrawString(si.chi[i, j] + "", new Font(FontFamily.GenericSansSerif, 20), new SolidBrush(Color.Red), si.dots[i,j].X, si.dots[i, j].Y-30);
                        }
                    }
                    
                }
                catch (Exception e) { }
                /*g.DrawLine(new Pen(Color.Blue, 2), 0, si.topWhiteLine.y(0), bm.Width, si.topWhiteLine.y(bm.Width));
                g.DrawLine(new Pen(Color.Blue, 2), 0, si.bottomWhiteLine.y(0), bm.Width, si.bottomWhiteLine.y(bm.Width));
                g.DrawLine(new Pen(Color.Blue, 2), si.leftWhiteLine.x(0), 0, si.leftWhiteLine.x(bm.Height), bm.Height);
                g.DrawLine(new Pen(Color.Blue, 2), si.rightWhiteLine.x(0), 0, si.rightWhiteLine.x(bm.Height), bm.Height);*/
            }
        }
    }
}

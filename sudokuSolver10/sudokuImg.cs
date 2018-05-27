using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace sudokuSolver10 {
    class sudokuImg {
        public static int iterator = 0;
        private readonly Bitmap _img;
        //private bool[,] MBM;

        /// <summary>
        /// first - height, second - width, \n
        /// true - white, false - black
        /// </summary>
        public bool[,] MBM { get; private set; }

        public string Path { get; }

        private const int _rect = 5;
        public int RotateAngle { get; private set; }

        public Line MaxLine { get; private set; }

        public bool Monochromical { get; private set; }
        public bool Hough { get; private set; }

        public Line leftWhiteLine { get; private set; }
        public Line rightWhiteLine { get; private set; }
        public Line topWhiteLine { get; private set; }
        public Line bottomWhiteLine { get; private set; }

        public Line[] horLines { get; private set; }
        public Line[] verLines { get; private set; }

        public Point[] whitePolygon { get; private set; }

        public Rectangle WhiteRectangle { get; private set; }

        private Point dlt, drt, dlb, drb;

        public Point[,] dots { get; private set; }
        public int[,] chi { get; private set; }

        public sudokuImg(string imagePath) {
            _img = (Bitmap)Image.FromFile(imagePath);
            Path = imagePath;
            Monochromical = false;
            Hough = false;
        }

        public void Solve() {
            MBM = GetMonochrimical(2);
            if (!HoughTransformThird()) return;
            if (!WhiteRect()) return;
            sudokuLines();

            MBM = GetMonochrimical(5);
            chi = new int[9, 9];
            for (int i = 0; i < 9; i++) {
                for (int j = 0; j < 9; j++) {
                    chi[i, j] = cif(j, i);
                }
            }
            return;
        }

        private unsafe bool[,] GetMonochrimical(int U) {
            byte[] rgbValues;
            {
                var bmpData = _img.LockBits(new Rectangle(0, 0, _img.Width, _img.Height), ImageLockMode.ReadOnly,
                    _img.PixelFormat);
                var bytes = bmpData.Stride*bmpData.Height;
                rgbValues = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, rgbValues, 0, bytes);
                _img.UnlockBits(bmpData);
            }
            var sumRect = new uint[_img.Height,_img.Width];

            // Построение сумирующей матрицы
            fixed (byte* rgbRef = &rgbValues[0])
            fixed (uint* sumRef = &sumRect[0, 0])
            {
                var rgbR = rgbRef;
                var sumR = sumRef;
                for (var i = 0; i < _img.Width * _img.Height; i++) {
                    *(sumR++) = (uint)((((*(rgbR++) * 28) + (*(rgbR++) * 150) + (*(rgbR++) * 77)) >> 8) & 255);
                }
                sumR = sumRef;
                Sum(sumR, _img.Height, _img.Width);
            }

            uint mean, sum;
            int Byte;
            var mbm = new bool[_img.Height,_img.Width];
            fixed (byte* rgbRef = &rgbValues[0])
            {
                var rgbR = rgbRef + 18 * _img.Width + 18;
                for (var i = 6; i < _img.Height - 5; i++) {
                    for (var j = 6; j < _img.Width - 5; j++) {
                        sum = sumRect[i - 6, j - 6] + sumRect[i + 5, j + 5] - sumRect[i - 6, j + 5] - sumRect[i + 5, j - 6];
                        mean = (sum / 121);
                        Byte = (((*rgbR++ * 28 + *rgbR++ * 150 + *rgbR++ * 77) >> 8) & 255);
                        mbm[i, j] = Byte < mean - U;
                    }
                    rgbR += 33;
                }
            }
            Monochromical = true;
            return mbm;
        }

        private unsafe void Sum(uint* p, int height, int width) {
            int x, y;
            uint* pA, pB, pC, pD;
            pA = p;

            pB = p + width;
            for (x = 1; x < height; x++) {
                *pB += *pA;
                pA += width;
                pB += width;
            }
            pA = p;
            pB = p + 1;
            for (y = 1; y < width; y++) {
                *pB += *pA;
                pA++;
                pB++;
            }
            pA = p + width + 1;
            pB = p + 1;
            pC = p + width;
            pD = p;
            for (x = 1; x < height; x++) {
                for (y = 1; y < width; y++) {
                    *pA += *pB + *pC - *pD;
                    pA++;
                    pB++;
                    pC++;
                    pD++;
                }
                pA++;
                pB++;
                pC++;
                pD++;
            }
        }

        private bool HoughTransformThird() {
            if (MBM == null) return false;
            var acc = new Acc(_img.Width, _img.Height);
            for (var i = _img.Width / 3; i < 2 * (_img.Width / 3); i++) {
                for (var j = _img.Height / 3; j < 2 * (_img.Height / 3); j++) {
                    if (!MBM[j, i])
                        continue;
                    acc.Add(i, j);
                }
            }
            RotateAngle = acc.RotateTheta;
            MaxLine = acc.Max;
            Hough = true;
            return true;
        }
        private Line HoughTransformThird(int x0, int y0, int x1, int y1) {
            if (MBM == null) return null;
            x0 = Math.Max(x0, 0);
            y0 = Math.Max(y0, 0);
            x1 = Math.Min(x1, _img.Width - 1);
            y1 = Math.Min(y1, _img.Height - 1);
            var acc = new Acc(_img.Width, _img.Height);
            for (var i = x0; i < x1; i++) {
                for (var j = y0; j < y1; j++) {
                    if (!MBM[j, i])
                        continue;
                    acc.Add(i, j);
                }
            }
            return acc.Max;
        }

        private Line HoughTransformThird(Line input, int offset, int loLimit, int hiLimit, int thetaOffset, bool isVertical) {
            int rho, x, y, yOffset, start, end, vote;
            Acc acc = new Acc(_img.Width, _img.Height);

            if (offset > 0) {
                start = input.Rh;
                end = start + offset;
            }
            else {
                end = input.Rh;
                start = end + offset;
            }
            double m_rhos =  Math.Sqrt(_img.Width*_img.Width+_img.Height*_img.Height);
            float sina = Acc.Sins[input.Th];
            float cosa = Acc.Coss[input.Th];
            //input.Th = 180 - input.Th;       

            if (isVertical) {
                for (rho = start; rho < end; rho += 1)  
                {
                    for (y = loLimit; y < hiLimit; y += 2) 
                    {
                        x = (int)((-y * sina + rho) / cosa);
                        if (x > 0 && x < _img.Width && y > 0 && y < _img.Height) {
                            if (MBM[y, x])
                                acc.Add(x, y, input.Th - thetaOffset, input.Th + thetaOffset);
                            //accumulator.Add(y, x, input.Th - thetaOffset, input.Th + thetaOffset);
                        }
                    }
                }
            }
            else {
                for (rho = start; rho < end; rho += 1)
                {
                    for (x = loLimit; x < hiLimit; x += 2)
                    {
                        y = (int)((-x * cosa + rho) / sina);
                        if (x > 0 && x < _img.Width && y > 0 && y < _img.Height) {

                            if (MBM[y, x])
                            {
                                acc.Add(x, y, input.Th - thetaOffset, input.Th + thetaOffset);
                                //accumulator.Add(x, y, inputLine.theta - thetaOffset, inputLine.theta + thetaOffset);    
                            }
                        }
                    }
                }
            }
            return acc.Max;
        }

        private bool WhiteRect() {
            //Горизонтальные линии
            var thH = MaxLine.Th > 45 && MaxLine.Th < 135 ? MaxLine.Th : (MaxLine.Th + 90)%180;
            const int RECT = 1;
            const int count = (RECT*2 + 1)*(RECT*2 + 1)/3;
            int minHRh, maxHRh, meanHRh;

            int xMin = _rect + 1 + RECT,
                xMax = _img.Width - 1 - _rect - RECT,
                yMin = xMin,
                yMax = _img.Height - 1 - _rect - RECT;
            {
                int rh00 = (int) (yMin*Acc.Sins[thH] + xMin*Acc.Coss[thH]),
                    rh01 = (int) (yMax*Acc.Sins[thH] + xMin*Acc.Coss[thH]),
                    rh10 = (int) (yMin*Acc.Sins[thH] + xMax*Acc.Coss[thH]),
                    rh11 = (int) (yMax*Acc.Sins[thH] + xMax*Acc.Coss[thH]);
                minHRh = Math.Min(rh00, Math.Min(rh10, Math.Min(rh01, rh11)));
                maxHRh = Math.Max(rh00, Math.Max(rh10, Math.Max(rh01, rh11)));
                meanHRh = (maxHRh + minHRh) / 2;
            }


            var topHWhiteLine = MaxLine;
            var maxWhiteLength = 0;
            for (var i = minHRh; i < meanHRh; i++) {
                var whiteLen = 0;
                var maxLength = 0;
                for (var x = xMin; x <= xMax; x++) {
                    var y = (int) ((i - x*Acc.Coss[thH])/Acc.Sins[thH]);
                    if (y >= yMin && y <= yMax) {
                        var sum = 0;
                        for (int yi = y - RECT; yi <= y + RECT; yi++) {
                            for (int xi = x - RECT; xi < x + RECT; xi++) {
                                if (MBM[yi, xi]) sum++;
                            }
                        }
                        if (count > sum)
                            whiteLen++;
                        else {
                            if (whiteLen > maxLength) {
                                maxLength = whiteLen;
                            }
                            whiteLen = 0;
                        }
                    }
                }
                if (maxLength <= maxWhiteLength) continue;
                maxWhiteLength = maxLength;
                topHWhiteLine = new Line { Rh = i, Th = thH };
            }


            var bottomHWhiteLine = MaxLine;
            maxWhiteLength = 0;
            for (var i = meanHRh + 1; i <= maxHRh; i++) {
                var whiteLen = 0;
                var maxLength = 0;
                for (var x = xMin; x <= xMax; x++) {
                    var y = (int) ((i - x*Acc.Coss[thH])/Acc.Sins[thH]);
                    if (y >= yMin && y <= yMax) {
                        var sum = 0;
                        for (int yi = y - RECT; yi <= y + RECT; yi++) {
                            for (int xi = x - RECT; xi < x + RECT; xi++) {
                                if (MBM[yi, xi]) sum++;
                            }
                        }
                        if (count > sum)
                            whiteLen++;
                        else {
                            if (whiteLen > maxLength) {
                                maxLength = whiteLen;
                            }
                            whiteLen = 0;
                        }
                    }
                }
                if (maxLength <= maxWhiteLength) continue;
                maxWhiteLength = maxLength;
                bottomHWhiteLine = new Line { Rh = i, Th = thH };
            }

            var thV = (thH + 90)%180;
            int minVRh, maxVRh, meanVRh;

            {
                int rh00 = (int) (yMin*Acc.Sins[thV] + xMin*Acc.Coss[thV]),
                    rh01 = (int) (yMax*Acc.Sins[thV] + xMin*Acc.Coss[thV]),
                    rh10 = (int) (yMin*Acc.Sins[thV] + xMax*Acc.Coss[thV]),
                    rh11 = (int) (yMax*Acc.Sins[thV] + xMax*Acc.Coss[thV]);
                minVRh = Math.Min(rh00, Math.Min(rh10, Math.Min(rh01, rh11)));
                maxVRh = Math.Max(rh00, Math.Max(rh10, Math.Max(rh01, rh11)));
                meanVRh = (minVRh + maxVRh) / 2;
            }

            var leftVWhiteLine = MaxLine;
            maxWhiteLength = 0;
            for (var i = minVRh; i < meanVRh; i++) {
                var whiteLen = 0;
                var maxLength = 0;
                for (var y = yMin; y <= yMax; y++) {
                    var x = (int) ((i - y*Acc.Sins[thV])/Acc.Coss[thV]);
                    if (x >= xMin && x <= xMax) {
                        var sum = 0;
                        for (int yi = y - RECT; yi <= y + RECT; yi++) {
                            for (int xi = x - RECT; xi < x + RECT; xi++) {
                                if (MBM[yi, xi]) sum++;
                            }
                        }
                        if (count > sum)
                            whiteLen++;
                        else {
                            if (whiteLen > maxLength) {
                                maxLength = whiteLen;
                            }
                            whiteLen = 0;
                        }
                    }
                }
                if (maxLength <= maxWhiteLength) continue;
                maxWhiteLength = maxLength;
                leftVWhiteLine = new Line { Rh = i, Th = thV };
            }
            var rightVWhiteLine = MaxLine;
            maxWhiteLength = 0;
            for (var i = meanVRh + 1; i <= maxVRh; i++) {
                var whiteLen = 0;
                var maxLength = 0;
                for (var y = yMin; y <= yMax; y++) {
                    var x = (int) ((i - y*Acc.Sins[thV])/Acc.Coss[thV]);
                    if (x >= xMin && x <= xMax) {
                        var sum = 0;
                        for (int yi = y - RECT; yi <= y + RECT; yi++) {
                            for (int xi = x - RECT; xi < x + RECT; xi++) {
                                if (MBM[yi, xi]) sum++;
                            }
                        }
                        if (count > sum)
                            whiteLen++;
                        else {
                            if (whiteLen > maxLength) {
                                maxLength = whiteLen;
                            }
                            whiteLen = 0;
                        }
                    }
                }
                if (maxLength <= maxWhiteLength) continue;
                maxWhiteLength = maxLength;
                rightVWhiteLine = new Line { Rh = i, Th = thV };
            }
            dlt = getCrossPoint(topHWhiteLine, leftVWhiteLine);
            drt = getCrossPoint(topHWhiteLine, rightVWhiteLine);
            dlb = getCrossPoint(bottomHWhiteLine, leftVWhiteLine);
            drb = getCrossPoint(bottomHWhiteLine, rightVWhiteLine);
            whitePolygon = new[] { dlt, dlb, drb, drt };
            if (RotateAngle > 0) {
                leftWhiteLine = rightVWhiteLine;
                rightWhiteLine = leftVWhiteLine;
            }
            else {
                leftWhiteLine = leftVWhiteLine;
                rightWhiteLine = rightVWhiteLine;
            }
            topWhiteLine = topHWhiteLine;
            bottomWhiteLine = bottomHWhiteLine;
            var drH = Math.Abs(bottomHWhiteLine.Rh - topHWhiteLine.Rh);
            var drV = Math.Abs(rightVWhiteLine.Rh - leftVWhiteLine.Rh);
            //if (drH/drV < 0.3333333 || drH/drV > 3)
            //    return false;
            return true;
        }

        public Point getCrossPoint(Line l1, Line l2) {
            var s = l1.Th - l2.Th > 0 ? Acc.Sins[l1.Th - l2.Th] : -Acc.Sins[-l1.Th + l2.Th];
            var y = ((l1.Rh*Acc.Coss[l2.Th] - l2.Rh*Acc.Coss[l1.Th] )/s);
            var x = (l1.Rh - y*Acc.Sins[l1.Th])/Acc.Coss[l1.Th];
            return new Point((int)x, (int)y);
        }

        public Rectangle rectL { get; private set; }
        private bool sudokuLines() {
            horLines = new Line[10];
            verLines = new Line[10];
            Line temp;
            int offt = dlb.Y + (dlt.Y-dlb.Y)/9;
            int offb = dlt.Y - (dlt.Y-dlb.Y)/9;

            // leftmost line
            verLines[0] = HoughTransformThird(leftWhiteLine, (rightWhiteLine.Rh - leftWhiteLine.Rh) / 9, offb, offt, 4, true);
            // rightmost line
            offt = drb.Y + (drt.Y - drb.Y) / 9;
            offb = drt.Y - (drt.Y - drb.Y) / 9;
            verLines[9] = HoughTransformThird(rightWhiteLine, (leftWhiteLine.Rh - rightWhiteLine.Rh) / 9, offb, offt, 4, true);
            // third line
            temp = new Line {
                Th = verLines[0].Th + (int)((verLines[9].Th - verLines[0].Th) * 3 / 9.0),
                Rh = verLines[0].Rh + (int)((verLines[9].Rh - verLines[0].Rh) * 3 / 9.0 - (verLines[9].Rh - verLines[0].Rh) / 26.0)
            };
            offt = dlb.Y + (drb.Y - dlb.Y) / 3 + (dlt.Y - dlb.Y) / 7;
            offb = dlt.Y + (drt.Y - dlt.Y) / 3 - (dlt.Y - dlb.Y) / 7;
            verLines[3] = HoughTransformThird(temp, (int)((verLines[9].Rh - verLines[0].Rh) / 9), offb, offt, 4, true);
            // sixth line
            temp.Th = verLines[0].Th + (int)((verLines[9].Th - verLines[0].Th) * 6 / 9.0);
            temp.Rh = verLines[0].Rh + (int)((verLines[9].Rh - verLines[0].Rh) * 6 / 9.0 - (verLines[9].Rh - verLines[0].Rh) / 26.0);
            offt = dlb.Y + 2 * (drb.Y - dlb.Y) / 3 + (dlt.Y - dlb.Y) / 7;
            offb = dlt.Y + 2 * (drt.Y - dlt.Y) / 3 - (dlt.Y - dlb.Y) / 7;
            verLines[6] = HoughTransformThird(temp, (int)((verLines[9].Rh - verLines[0].Rh) / 9), offb, offt, 4, true);

            // topmost line
            int offl, offr;
            if (RotateAngle < 0) {
                offl = dlt.X + (drt.X - dlt.X) / 9;
                offr = drt.X - (drt.X - dlt.X) / 9;
            }
            else {
                offr = dlt.X + (drt.X - dlt.X) / 9;
                offl = drt.X - (drt.X - dlt.X) / 9;
            }
            horLines[9] = HoughTransformThird(topWhiteLine, (bottomWhiteLine.Rh - topWhiteLine.Rh) / 9, offl, offr, 4, false);
            // bottom line

            if (RotateAngle < 0) {
                offl = dlb.X + (drb.X - dlb.X) / 9;
                offr = drb.X - (drb.X - dlb.X) / 9;
            }
            else {
                offr = dlb.X + (drb.X - dlb.X) / 9;
                offl = drb.X - (drb.X - dlb.X) / 9;
            }
            horLines[0] = HoughTransformThird(bottomWhiteLine, (topWhiteLine.Rh - bottomWhiteLine.Rh) / 9, offl, offr, 4, false);
            // third line
            temp.Th = horLines[0].Th + (int)((horLines[9].Th - horLines[0].Th) * 3 / 9.0);
            temp.Rh = horLines[0].Rh + (int)((horLines[9].Rh - horLines[0].Rh) * 3 / 9.0 - (horLines[9].Rh - horLines[0].Rh) / 25.0);
            if (RotateAngle < 0) {
                offl = dlb.X + (dlt.X - dlb.X) / 3 + (drb.X - dlb.X) / 7;
                offr = drb.X + (drt.X - drb.X) / 3 - (drb.X - dlb.X) / 7;
            }
            else {
                offr = dlb.X + (dlt.X - dlb.X) / 3 + (drb.X - dlb.X) / 7;
                offl = drb.X + (drt.X - drb.X) / 3 - (drb.X - dlb.X) / 7;
            }
            horLines[3] = HoughTransformThird(temp, (int)((horLines[9].Rh - horLines[0].Rh) / 9), offl, offr, 4, false);
            // sixth line
            temp.Th = horLines[0].Th + (int)((horLines[9].Th - horLines[0].Th) * 6 / 9.0);
            temp.Rh = horLines[0].Rh + (int)((horLines[9].Rh - horLines[0].Rh) * 6 / 9.0 - (horLines[9].Rh - horLines[0].Rh) / 25.0);
            if (RotateAngle < 0) {
                offl = dlb.X + 2 * (dlt.X - dlb.X) / 3 + (drb.X - dlb.X) / 7;
                offr = drb.X + 3 * (drt.X - drb.X) / 3 - (drb.X - dlb.X) / 7;
            }
            else {
                offr = dlb.X + 2 * (dlt.X - dlb.X) / 3 + (drb.X - dlb.X) / 7;
                offl = drb.X + 3 * (drt.X - drb.X) / 3 - (drb.X - dlb.X) / 7;
            }
            horLines[6] = HoughTransformThird(temp, (int)((horLines[9].Rh - horLines[0].Rh) / 9), offl, offr, 4, false);
            dots = new Point[10, 10];
            for (int y = 0; y < 10; y += 3) {
                for (int x = 0; x < 10; x += 3) {
                    dots[x, y] = getCrossPoint(horLines[x], verLines[y]);
                    //dots[x,y].X = (int)((c2 * t1 + c1) / (1 - t1 * t2));
                    //dots[x,y].Y = (int)(dots[x,y].X * t2 + c2);
                }
            }

            for (int y = 0; y < 10; y += 3) {
                dots[y, 1].X = dots[y, 0].X + (dots[y, 3].X - dots[y, 0].X) / 3;
                dots[y, 2].X = dots[y, 3].X - (dots[y, 3].X - dots[y, 0].X) / 3;
                dots[y, 4].X = dots[y, 3].X + (dots[y, 6].X - dots[y, 3].X) / 3;
                dots[y, 5].X = dots[y, 6].X - (dots[y, 6].X - dots[y, 3].X) / 3;
                dots[y, 7].X = dots[y, 6].X + (dots[y, 9].X - dots[y, 6].X) / 3;
                dots[y, 8].X = dots[y, 9].X - (dots[y, 9].X - dots[y, 6].X) / 3;
                dots[y, 1].Y = dots[y, 0].Y + (dots[y, 3].Y - dots[y, 0].Y) / 3;
                dots[y, 2].Y = dots[y, 3].Y - (dots[y, 3].Y - dots[y, 0].Y) / 3;
                dots[y, 4].Y = dots[y, 3].Y + (dots[y, 6].Y - dots[y, 3].Y) / 3;
                dots[y, 5].Y = dots[y, 6].Y - (dots[y, 6].Y - dots[y, 3].Y) / 3;
                dots[y, 7].Y = dots[y, 6].Y + (dots[y, 9].Y - dots[y, 6].Y) / 3;
                dots[y, 8].Y = dots[y, 9].Y - (dots[y, 9].Y - dots[y, 6].Y) / 3;
            }
            for (int x = 0; x < 10; x += 3) {
                dots[1, x].X = dots[0, x].X + (dots[3, x].X - dots[0, x].X) / 3;
                dots[2, x].X = dots[3, x].X - (dots[3, x].X - dots[0, x].X) / 3;
                dots[4, x].X = dots[3, x].X + (dots[6, x].X - dots[3, x].X) / 3;
                dots[5, x].X = dots[6, x].X - (dots[6, x].X - dots[3, x].X) / 3;
                dots[7, x].X = dots[6, x].X + (dots[9, x].X - dots[6, x].X) / 3;
                dots[8, x].X = dots[9, x].X - (dots[9, x].X - dots[6, x].X) / 3;
                dots[1, x].Y = dots[0, x].Y + (dots[3, x].Y - dots[0, x].Y) / 3;
                dots[2, x].Y = dots[3, x].Y - (dots[3, x].Y - dots[0, x].Y) / 3;
                dots[4, x].Y = dots[3, x].Y + (dots[6, x].Y - dots[3, x].Y) / 3;
                dots[5, x].Y = dots[6, x].Y - (dots[6, x].Y - dots[3, x].Y) / 3;
                dots[7, x].Y = dots[6, x].Y + (dots[9, x].Y - dots[6, x].Y) / 3;
                dots[8, x].Y = dots[9, x].Y - (dots[9, x].Y - dots[6, x].Y) / 3;
            }

            for (int a = 1; a < 9; a++) {
                if (a != 3 && a != 6) {
                    for (int b = 1; b < 9; b++) {
                        if (b == 1) {
                            dots[a, b].X = dots[a, 0].X + (dots[a, 3].X - dots[a, 0].X) / 3;
                            dots[b, a].Y = dots[0, a].Y + (dots[3, a].Y - dots[0, a].Y) / 3;
                        }
                        else if (b == 2) {
                            dots[a, b].X = dots[a, 3].X - (dots[a, 3].X - dots[a, 0].X) / 3;
                            dots[b, a].Y = dots[3, a].Y - (dots[3, a].Y - dots[0, a].Y) / 3;
                        }
                        else if (b == 4) {
                            dots[a, b].X = dots[a, 3].X + (dots[a, 6].X - dots[a, 3].X) / 3;
                            dots[b, a].Y = dots[3, a].Y + (dots[6, a].Y - dots[3, a].Y) / 3;
                        }
                        else if (b == 5) {
                            dots[a, b].X = dots[a, 6].X - (dots[a, 6].X - dots[a, 3].X) / 3;
                            dots[b, a].Y = dots[6, a].Y - (dots[6, a].Y - dots[3, a].Y) / 3;
                        }
                        else if (b == 7) {
                            dots[a, b].X = dots[a, 6].X + (dots[a, 9].X - dots[a, 6].X) / 3;
                            dots[b, a].Y = dots[6, a].Y + (dots[9, a].Y - dots[6, a].Y) / 3;
                        }
                        else if (b == 8) {
                            dots[a, b].X = dots[a, 9].X - (dots[a, 9].X - dots[a, 6].X) / 3;
                            dots[b, a].Y = dots[9, a].Y - (dots[9, a].Y - dots[6, a].Y) / 3;
                        }
                    }
                }
            }
            return true;
        }

        private int cif(int yBottom, int xLeft) {
            int sum, yindex, xindex;
            bool bIsWhite, bIsFirstWhite;
            Point lt = dots[xLeft,yBottom];
            Point rt = dots[xLeft,yBottom + 1];
            Point lb = dots[xLeft+1,yBottom];
            Point rb = dots[xLeft + 1,yBottom+1];
            int cellWidth  = (rt.X-1) - (lt.X+2);
            int cellHeight = (lt.Y-1) - (lb.Y+2);
            Point reslb, resrt;     
            Byte[,] blob = new Byte[100,100];	
            for (int i = 0; i < 100; i++) {
                for (int j = 0; j < 100; j++) {
                    blob[i, j] = 255;
                }
            }

            if (cellWidth < 5 || cellHeight < 9)
                return -1;
            
            sum = 0;
            for (int X = lt.X + 2 + cellWidth / 3; X < rt.X - 1 - cellWidth / 3; X++) {
                int Y = lb.Y + ((X - lt.X) * (rt.Y - lt.Y)) / (rt.X - lt.X);
                for (int y = Y + 2 + cellHeight / 3; y < Y + (lt.Y - lb.Y) - 1 - cellHeight / 4; y++) {
                    int x = X - (lt.X-lb.X) - ((y - Y) * (lb.X - lt.X)) / (lt.Y - lb.Y);
                    if (x < 0 || x > 639 || y < 0 || y > 479)
                        continue;
                    if (MBM[y,x])
                        sum++;
                }
            }

            if (sum < (cellWidth * cellHeight) / 31)            
            {
               
                return -2;       
            }


            yindex = 0;
            for (int Y = lb.Y + 2; Y < lt.Y - 1; Y++) {
                xindex = 0;
                int X = lb.X + ((Y - lb.Y) * (lt.X - lb.X)) / (lt.Y - lb.Y);
                for (int x = X + 2; x < X + (rb.X - lb.X) - 1; x++) {
                    int y = Y + ((x - X) * (rb.Y - lb.Y)) / (rb.X - lb.X);
                    if (yindex > 99 || xindex > 99) continue;
                    if (MBM[y,x])
                        blob[yindex,xindex] = 0;
                    xindex++;
                }
                yindex++;
            }

            


            reslb = new Point();

            bIsFirstWhite = false;
            reslb.Y = cellHeight / 3;
            for (int y = cellHeight / 3; y >= 0; y--) {
                bIsWhite = true;
                for (int x = cellWidth / 3; x < cellWidth - cellWidth / 3; x++) 
                {if(y>99 || x > 99) continue;
                    if (blob[y, x] == 0)    
                    {
                        reslb.Y = y;
                        bIsFirstWhite = false;
                        bIsWhite = false;
                        break;
                    }
                }

                if (bIsWhite == true) {
                    if (bIsFirstWhite == false)
                        bIsFirstWhite = true;
                    else
                        break;  
                }
            }
            resrt = new Point();
            bIsFirstWhite = false;
            resrt.Y = (2 * cellHeight) / 3;
            for (int y = (2 * cellHeight) / 3; y < cellHeight; y++) {
                bIsWhite = true;
                for (int x = cellWidth / 3; x < cellWidth - cellWidth / 3; x++) {
                    if (y > 99 || x > 99) continue;
                    if (blob[y, x] == 0)   
                    {
                        resrt.Y = y;
                        bIsFirstWhite = false;
                        bIsWhite = false;
                        break;
                    }
                }

                if (bIsWhite == true) {
                    if (bIsFirstWhite == false)
                        bIsFirstWhite = true;
                    else
                        break;  
                }
            }

            
            resrt.X = cellWidth / 2 + 1;
            for (int x = cellWidth / 2 + 1; x < cellWidth; x++) {
                bIsWhite = true;
                for (int y = reslb.Y + 2; y < resrt.Y - 1; y++) 
                {
                    if (x > 99 || y > 99) continue;
                    if (blob[y, x] == 0)   
                    {
                        resrt.X = x;
                        bIsWhite = false;
                        break;
                    }
                }

                if (bIsWhite == true) {
                    break; 
                }
            }
            
            reslb.X = cellWidth / 2;
            for (int x = cellWidth / 2; x >= 0; x--) {
                bIsWhite = true;
                for (int y = reslb.Y + 2; y < resrt.Y - 1; y++)
                {
                    if (blob[y, x] == 0)  
                    {
                        reslb.X = x;
                        bIsWhite = false;
                        break;
                    }
                }

                if (bIsWhite == true) {
                    break; 
                }
            }

            int width  = resrt.X - reslb.X;
            int height = resrt.Y - reslb.Y;
            /*Bitmap iimg = new Bitmap(width,height);*/

            blob = tr(blob);

            /*iimg = new Bitmap(width,height);
            for (int i = 0; i < width ; i++) {
                for (int j = 0; j < height; j++) {
                    iimg.SetPixel(i,j, Color.FromArgb(blob[i+reslb.X, j+reslb.Y], blob[i + reslb.X, j + reslb.Y], blob[i + reslb.X, j + reslb.Y]));
                }
            }*/


            OCRDigit ocr = new OCRDigit();
            int rett= ocr.Classify(blob, reslb, resrt);
            //iimg.Save("kkk" + iterator++ + "_"+rett+".jpg", ImageFormat.Jpeg);

            return rett;
        }

        private byte[,] tr(byte[,] qq) {
            byte[,] ee = new byte[100,100];
            for (int i = 0; i < 100; i++) {
                for (int j = 0; j < 100; j++) {
                    ee[i, j] = qq[99 - j, 99-i];
                }
            }
            byte[,] eee = new byte[100,100];
            for (int i = 0; i < 100; i++) {
                for (int j = 0; j < 100; j++) {
                    eee[i, j] = ee[99-i,99-j];
                }
            }
            return eee;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace sudokuSolver10 {
    class Line {
        public int Rh;
        public int Th;

        public int x(int y) {
            return (int)((Rh - y * Acc.Sins[Th]) / Acc.Coss[Th]);
        }
        public int y(int x) {
            return (int)((Rh - x * Acc.Coss[Th]) / Acc.Sins[Th]);
        }

        public override string ToString() {
            return "Rh: " + Rh +  ", Th:" + Th;
        }
    }

    class Acc {
        private int _width, _height;
        public readonly int RhoOffset;
        public static readonly float[] Sins = { (float)0.0, (float)0.017452, (float)0.034899, (float)0.052336, (float)0.069756, (float)0.087156, (float)0.104528, (float)0.121869, (float)0.139173, (float)0.156434, (float)0.173648, (float)0.190809, (float)0.207912, (float)0.224951, (float)0.241922, (float)0.258819, (float)0.275637, (float)0.292372, (float)0.309017, (float)0.325568, (float)0.342020, (float)0.358368, (float)0.374607, (float)0.390731, (float)0.406737, (float)0.422618, (float)0.438371, (float)0.453990, (float)0.469472, (float)0.484810, (float)0.500000, (float)0.515038, (float)0.529919, (float)0.544639, (float)0.559193, (float)0.573576, (float)0.587785, (float)0.601815, (float)0.615662, (float)0.629320, (float)0.642788, (float)0.656059, (float)0.669131, (float)0.681998, (float)0.694658, (float)0.707107, (float)0.719340, (float)0.731354, (float)0.743145, (float)0.754710, (float)0.766044, (float)0.777146, (float)0.788011, (float)0.798636, (float)0.809017, (float)0.819152, (float)0.829038, (float)0.838671, (float)0.848048, (float)0.857167, (float)0.866025, (float)0.874620, (float)0.882948, (float)0.891007, (float)0.898794, (float)0.906308, (float)0.913545, (float)0.920505, (float)0.927184, (float)0.933580, (float)0.939693, (float)0.945519, (float)0.951057, (float)0.956305, (float)0.961262, (float)0.965926, (float)0.970296, (float)0.974370, (float)0.978148, (float)0.981627, (float)0.984808, (float)0.987688, (float)0.990268, (float)0.992546, (float)0.994522, (float)0.996195, (float)0.997564, (float)0.998630, (float)0.999391, (float)0.999848, (float)1.0, (float)0.999848, (float)0.999391, (float)0.998630, (float)0.997564, (float)0.996195, (float)0.994522, (float)0.992546, (float)0.990268, (float)0.987688, (float)0.984808, (float)0.981627, (float)0.978148, (float)0.974370, (float)0.970296, (float)0.965926, (float)0.961262, (float)0.956305, (float)0.951057, (float)0.945519, (float)0.939693, (float)0.933580, (float)0.927184, (float)0.920505, (float)0.913545, (float)0.906308, (float)0.898794, (float)0.891007, (float)0.882948, (float)0.874620, (float)0.866025, (float)0.857167, (float)0.848048, (float)0.838671, (float)0.829038, (float)0.819152, (float)0.809017, (float)0.798635, (float)0.788011, (float)0.777146, (float)0.766044, (float)0.754710, (float)0.743145, (float)0.731354, (float)0.719340, (float)0.707107, (float)0.694658, (float)0.681998, (float)0.669131, (float)0.656059, (float)0.642788, (float)0.629321, (float)0.615661, (float)0.601815, (float)0.587785, (float)0.573576, (float)0.559193, (float)0.544639, (float)0.529919, (float)0.515038, (float)0.500000, (float)0.484810, (float)0.469472, (float)0.453991, (float)0.438371, (float)0.422618, (float)0.406737, (float)0.390731, (float)0.374607, (float)0.358368, (float)0.342020, (float)0.325568, (float)0.309017, (float)0.292372, (float)0.275637, (float)0.258819, (float)0.241922, (float)0.224951, (float)0.207912, (float)0.190809, (float)0.173648, (float)0.156434, (float)0.139173, (float)0.121869, (float)0.104528, (float)0.087156, (float)0.069756, (float)0.052336, (float)0.034899, (float)0.017452 };
        public static readonly float[] Coss = { (float)1.0, (float)0.999848, (float)0.999391, (float)0.998630, (float)0.997564, (float)0.996195, (float)0.994522, (float)0.992546, (float)0.990268, (float)0.987688, (float)0.984808, (float)0.981627, (float)0.978148, (float)0.974370, (float)0.970296, (float)0.965926, (float)0.961262, (float)0.956305, (float)0.951057, (float)0.945519, (float)0.939693, (float)0.933580, (float)0.927184, (float)0.920505, (float)0.913545, (float)0.906308, (float)0.898794, (float)0.891007, (float)0.882948, (float)0.874620, (float)0.866025, (float)0.857167, (float)0.848048, (float)0.838671, (float)0.829038, (float)0.819152, (float)0.809017, (float)0.798636, (float)0.788011, (float)0.777146, (float)0.766044, (float)0.754710, (float)0.743145, (float)0.731354, (float)0.719340, (float)0.707107, (float)0.694658, (float)0.681998, (float)0.669131, (float)0.656059, (float)0.642788, (float)0.629320, (float)0.615662, (float)0.601815, (float)0.587785, (float)0.573576, (float)0.559193, (float)0.544639, (float)0.529919, (float)0.515038, (float)0.500000, (float)0.484810, (float)0.469472, (float)0.453991, (float)0.438371, (float)0.422618, (float)0.406737, (float)0.390731, (float)0.374607, (float)0.358368, (float)0.342020, (float)0.325568, (float)0.309017, (float)0.292372, (float)0.275637, (float)0.258819, (float)0.241922, (float)0.224951, (float)0.207912, (float)0.190809, (float)0.173648, (float)0.156434, (float)0.139173, (float)0.121869, (float)0.104528, (float)0.087156, (float)0.069757, (float)0.052336, (float)0.034899, (float)0.017452, (float)0.0, (float)-0.017452, (float)-0.034899, (float)-0.052336, (float)-0.069756, (float)-0.087156, (float)-0.104529, (float)-0.121869, (float)-0.139173, (float)-0.156434, (float)-0.173648, (float)-0.190809, (float)-0.207912, (float)-0.224951, (float)-0.241922, (float)-0.258819, (float)-0.275637, (float)-0.292372, (float)-0.309017, (float)-0.325568, (float)-0.342020, (float)-0.358368, (float)-0.374607, (float)-0.390731, (float)-0.406737, (float)-0.422618, (float)-0.438371, (float)-0.453990, (float)-0.469472, (float)-0.484810, (float)-0.500000, (float)-0.515038, (float)-0.529919, (float)-0.544639, (float)-0.559193, (float)-0.573576, (float)-0.587785, (float)-0.601815, (float)-0.615661, (float)-0.629320, (float)-0.642788, (float)-0.656059, (float)-0.669131, (float)-0.681998, (float)-0.694658, (float)-0.707107, (float)-0.719340, (float)-0.731354, (float)-0.743145, (float)-0.754710, (float)-0.766044, (float)-0.777146, (float)-0.788011, (float)-0.798635, (float)-0.809017, (float)-0.819152, (float)-0.829037, (float)-0.838671, (float)-0.848048, (float)-0.857167, (float)-0.866025, (float)-0.874620, (float)-0.882948, (float)-0.891006, (float)-0.898794, (float)-0.906308, (float)-0.913545, (float)-0.920505, (float)-0.927184, (float)-0.933580, (float)-0.939693, (float)-0.945519, (float)-0.951056, (float)-0.956305, (float)-0.961262, (float)-0.965926, (float)-0.970296, (float)-0.974370, (float)-0.978148, (float)-0.981627, (float)-0.984808, (float)-0.987688, (float)-0.990268, (float)-0.992546, (float)-0.994522, (float)-0.996195, (float)-0.997564, (float)-0.998630, (float)-0.999391, (float)-0.999848 };
        public Line Max { get; private set; }
        private const int Theta = 180;
        public int RotateTheta => (Max.Th % 90) > 45 ? 90-(Max.Th % 90) : -Max.Th % 90;
        private readonly int[,] _bins;
        public int this[int i, int j] => _bins[i, j];
        private int _maxb;

        public Acc(int width, int height) {
            _width = width;
            _height = height;
            RhoOffset = (int)Math.Sqrt(width * width + height * height);
            Max = new Line {
                Rh = 0,
                Th = 0
            };
            _bins = new int[RhoOffset*2, Theta];
            _maxb = 0;
        }
        public void Add(int x, int y) {
            for (var theta = 0; theta < Theta; theta++) {
                var rho = (int)(x * Coss[theta]) + (int)(y * Sins[theta]) + RhoOffset;
                if (++_bins[rho, theta] <= _maxb) continue;
                _maxb = _bins[rho, theta];
                Max = new Line {
                    Rh = rho - RhoOffset,
                    Th = theta
                };
            }
        }
        public void Add(int x, int y, int thStart, int thEnd) {

            for (var thet = thStart; thet <= thEnd; thet++) {
                int theta = (thet + 180)%180;
                var rho = (int)(x * Coss[theta]) + (int)(y * Sins[theta%180]) + RhoOffset;
                if (++_bins[rho, theta] <= _maxb) continue;
                _maxb = _bins[rho, theta];
                Max = new Line {
                    Rh = rho - RhoOffset,
                    Th = theta
                };
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
namespace VISAP商科应用
{
    public class Stat
    {
        //下面是分位数计算
        //分位数计算统一用double
        public static double BetaCDF(double x, double a, double b)
        {
            //Beta的累积密度函数，a，b为自由度
            //x在0～1之间
            int m, n;
            double I = 0, U = 0;
            double ta = 0, tb = 0;
            m = (int)(2 * a);
            n = (int)(2 * b);
            //下面分四种情况
            if (m % 2 == 1 && n % 2 == 1)
            {
                ta = 0.5;
                tb = 0.5;
                U = Math.Sqrt(x * (1.0 - x)) / Math.PI;
                I = 1.0 - 2.0 / Math.PI * Math.Atan(Math.Sqrt((1.0 - x) / x));
            }
            else if (m % 2 == 1 && n % 2 == 0)
            {
                ta = 0.5;
                tb = 0.1;
                U = 0.5 * Math.Sqrt(x) * (1.0 - x);
                I = Math.Sqrt(x);
            }
            else if (m % 2 == 0 && n % 2 == 1)
            {
                ta = 1;
                tb = 0.5;
                U = 0.5 * x * Math.Sqrt(1.0 - x);
                I = 1.0 - Math.Sqrt(1.0 - x);
            }
            else if (m % 2 == 0 && n % 2 == 0)
            {
                ta = 1;
                tb = 1;
                U = x * (1.0 - x);
                I = x;
            }
            while (ta < a)
            {
                I = I - U / ta;
                U = (ta + tb) / ta * x * U;
                ta++;
            }
            while (tb < b)
            {
                I = I + U / tb;
                U = (ta + tb) / tb * (1.0 - x) * U;
                tb++;
            }
            return I;
        }
        
        public static double TDIST(double x, int v,double Tail)
        {
            //计算t分布的累积密度函数
            //v为自由度
            //Tail为单/双尾
            //1为单尾，2为双尾
            double t, prob;
            t = v / (v + x * x);
            if (x > 0)
            {
                prob = 1 - 0.5 * BetaCDF(t, v / 2.0, 0.5);
            }
            else
            {
                prob = 0.5 * BetaCDF(t, v / 2.0, 0.5);
            }
            //对结果进行调整
            return (1-prob)*Tail ;
        }
        public static double FDIST(double x, int m, int n)
        {
            //计算F分布的累积密度函数
            //m，n为两个自由度
            double y, prob;
            if (x <= 0)
            {
                return 0;
            }
            y = m * x / (n + m * x);
            prob = BetaCDF(y, m / 2.0, n / 2.0);
            return 1 - prob;
        }
        public static double BinomialCDF(double x, double p, int n)
        {
            //二项分布的累积密度函数
            //事件发生的概率为p
            double prob = 0.0;
            if (x < 0)
            {
                prob = 0.0;
                return prob;
            }
            else if (x >= n)
            {
                prob = 1.0;
                return prob;
            }
            else
            {
                prob = BetaCDF(1.0 - p, n - (int)x, (int)x + 1);
                return prob;
            }

        }
       
        public static double BetaUa(double af, double a, double b)
        {
            //Beta函数的分位数
            //af为概率
            //a，b为自由度
            //返回分位数
            int MaxTime = 500;
            int times = 0;
            double x1, x2, xn = 0.0;
            double f1, f2, fn, ua;
            double eps = 1.0e-10;
            x1 = 0.0;
            x2 = 1.0;
            f1 = BetaCDF(x1, a, b) - (1.0 - af);
            f2 = BetaCDF(x2, a, b) - (1.0 - af);
            while (Math.Abs((x2 - x1) / 2.0) > eps)
            {
                xn = (x1 + x2) / 2.0;
                fn = BetaCDF(xn, a, b) - (1.0 - af);
                if (f1 * fn < 0)
                {
                    x2 = xn;
                }
                else if (fn * f2 < 0)
                {
                    x1 = xn;
                }
                f1 = BetaCDF(x1, a, b) - (1.0 - af);
                f2 = BetaCDF(x2, a, b) - (1.0 - af);
                times++;
                if (times > MaxTime)
                {
                    break;
                }
            }
            ua = xn;
            return ua;
        }
       
        public static double TINV(double af, int v)
        {
            //T分布的分位数
            //af为概率
            double ua = 0.0, tbp, bf;
            bf = 1 - af;
            if (af <= 0.5)
            {
                tbp = BetaUa(1 - 2 * af, v / 2.0, 0.5);
                ua = Math.Sqrt(v / tbp - v);
            }
            else if (af > 0.5)
            {
                tbp = BetaUa(1 - 2 * (1.0 - af), v / 2.0, 0.5);
                ua = -Math.Sqrt(v / tbp - v);
            }
            return ua;
        }
        
        public static double FINV(double af, int m, int n)
        {
            //F分布的分位数
            //上侧概率分位数
            double ua, tbp, bf;
            bf = 1 - af;
            tbp = BetaUa(af, m / 2.0, n / 2.0);
            ua = n * tbp / (m * (1.0 - tbp));
            return ua;
        }
        
        public static double chi2(double x, int Freedom)
        {
            //计算卡方分布累积密度函数
            int k, n;
            double f, h, prob;
            k = Freedom % 2;
            if (k == 1)
            {
                f = Math.Exp(-x / 2.0) / Math.Sqrt(2 * Math.PI * x);
                h = 2.0 * GaossFx1(Math.Sqrt(x)) - 1.0;
                n = 1;
                while (n < Freedom)
                {
                    n = n + 2;
                    f = x / (n - 2.0) * f;
                    h = h - 2.0 * f;
                }
            }
            else
            {
                f = Math.Exp(-x / 2.0) / 2.0;
                h = 1.0 - Math.Exp(-x / 2.0);
                n = 2;
                while (n < Freedom)
                {
                    n = n + 2;
                    f = x / (n - 2.0) * f;
                    h = h - 2.0 * f;
                }
            }
            prob = h;
            return prob;
        }
        
        private static double chi21(double x, int Freedom)
        {
            //这个函数一般无需调用
            int k, n;
            double f, h, prob;
            k = Freedom % 2;
            if (k == 1)
            {
                f = Math.Exp(-x / 2.0) / Math.Sqrt(2 * Math.PI * x);
                h = 2.0 * GaossFx1(Math.Sqrt(x)) - 1.0;
                n = 1;
                while (n < Freedom)
                {
                    n = n + 2;
                    f = x / (n - 2.0) * f;
                    h = h - 2.0 * f;
                }
            }
            else
            {
                f = Math.Exp(-x / 2.0) / 2.0;
                h = 1.0 - Math.Exp(-x / 2.0);
                n = 2;
                while (n < Freedom)
                {
                    n = n + 2;
                    f = x / (n - 2.0) * f;
                    h = h - 2.0 * f;
                }
            }
            prob = h;
            return prob;
        }
        
        public static double PossionCDF(double x, double p)
        {
            //Possion分布的累积密度函数
            double prob = 0.0;
            prob = 1.0 - chi21(2 * p, 2 * ((int)x) + 1);
            return prob;
        }
        
        public static double chi2Ua(double af, int Freedom)
        {
            //卡方分布的上侧分位数的计算  
            int times;
            //int MaxTime = 500;
            int MaxTime = 5000;
            double eps = 1.0e-10;
            double ua, x0, xn, bf;
            bf = 1 - af;
            x0 = chi2Ua0(af, Freedom);
            if (Freedom == 1 || Freedom == 2)
            {
                ua = x0;
            }
            else
            {
                times = 1;
                int Try = 1;
                //在这里我用了一个不是办法的办法，就是尝试着找到最合适的初值。

                xn = x0 - (chi21(x0, Freedom) - 1 + af) / chi2Px(x0, Freedom);
                while (Math.Abs(xn - x0) > eps)
                {
                    x0 = xn;
                    if (chi2Px(x0, Freedom) > 0.00000001)
                    {
                        //确保初值合适
                        xn = x0 - (chi21(x0, Freedom) - 1 + af) / chi2Px(x0, Freedom);
                    }
                    else
                    {
                        //如果初值不合适，造成剧烈波动，则尝试其他的初值
                        xn = Try;
                        Try++;
                    }
                    times++;
                    if (times > MaxTime) break;
                }
                ua = xn;
            }
            return ua;
        }
        
        private static double chi2Ua0(double af, int Freedom)
        {
            //这个函数一般无需调用
            double ua, p, temp;
            if (Freedom == 1)
            {
                p = 1.0 - (1.0 - af + 1.0) / 2.0;
                temp = NORMSINV(p);
                ua = temp * temp;
            }
            else if (Freedom == 2)
            {
                ua = -2.0 * Math.Log(af);
            }
            else
            {
                temp = 1.0 - 2.0 / (9.0 * Freedom) + Math.Sqrt(2.0 / (9.0 * Freedom)) * NORMSINV(af);
                ua = Freedom * (temp * temp * temp);
            }
            return ua;
        }
       
        public static double chi2Px(double x, int Freedom)
        {
            //卡方分布的密度函数  
            double p, g;
            if (x <= 0) return 0.0;
            g = Gama(Freedom);
            p = 1.0 / Math.Pow(2.0, Freedom / 2.0) / g * Math.Exp(-x / 2.0) * Math.Pow(x, Freedom / 2.0 - 1.0);
            return p;
        }
        public static double Gama(int n)//伽马分布函数Gama(n/2)  
        {
            double g;
            int i, k;
            k = n / 2; if (n % 2 == 1)
            {
                g = Math.Sqrt(Math.PI) * 0.5;
                for (i = 1; i < k; i++)
                    g *= (i + 0.5);
            }
            else
            {
                g = 1.0;
                for (i = 1; i < k; i++)
                    g *= i;
            }
            return g;
        }
        
        private  static double GaossFx1(double x)
        {
            //高斯函数
            //一般无需调用
            double prob = 0, t, temp;
            int i, n, symbol;
            temp = x;
            if (x < 0)
                x = -x;
            n = 28;
            if (x >= 0 && x <= 3.0)
            {
                t = 0.0;
                for (i = n; i >= 1; i--)
                {
                    if (i % 2 == 1) symbol = -1;
                    else symbol = 1;
                    t = symbol * i * x * x / (2.0 * i + 1.0 + t);
                }
                prob = 0.5 + GaossPx(x) * x / (1.0 + t);
            }
            else if (x > 3.0)
            {
                t = 0.0;
                for (i = n; i >= 1; i--)
                    t = 1.0 * i / (x + t);
                prob = 1 - GaossPx(x) / (x + t);
            }
            x = temp;
            if (x < 0)
                prob = 1.0 - prob; return prob;
        }
        public static double NORMDIST(double x)//正态分布的分布函数的计算  
        {
            double prob = 0, t, temp;
            int i, n, symbol;
            temp = x;
            if (x < 0)
                x = -x;
            n = 28;//连分式展开的阶数  
            if (x >= 0 && x <= 3.0)//连分式展开方法  
            {
                t = 0.0;
                for (i = n; i >= 1; i--)
                {
                    if (i % 2 == 1) symbol = -1;
                    else symbol = 1;
                    t = symbol * i * x * x / (2.0 * i + 1.0 + t);
                }
                prob = 0.5 + GaossPx(x) * x / (1.0 + t);
            }
            else if (x > 3.0)
            {
                t = 0.0;
                for (i = n; i >= 1; i--)
                    t = 1.0 * i / (x + t);
                prob = 1 - GaossPx(x) / (x + t);
            }
            x = temp;
            if (x < 0)
                prob = 1.0 - prob;
            return prob;
        }
        private static double GaossPx(double x)//正态分布的密度函数  
        {
            //用于计算密度函数
            double f;
            f = 1.0 / Math.Sqrt(2.0 * Math.PI) * Math.Exp(-x * x / 2.0);
            return f;
        }
        //计算正态分布的分位数
        public static double NORMSINV(double alpha)
        {
            if (0.5 < alpha && alpha < 1)
            {
                alpha = 1 - alpha;
            }
            double[] b = new double[11];
            b[0] = 0.1570796288 * 10;
            b[1] = 0.3706987906 * 0.1;
            b[2] = -0.8364353589 * 0.001;
            b[3] = -0.2250947176 * 0.001;
            b[4] = 0.6841218299 * 0.00001;
            b[5] = 0.5824238515 * 0.00001;
            b[6] = -0.1045274970 * 0.00001;
            b[7] = 0.8360937017 * 0.0000001;
            b[8] = -0.3231081277 * 0.00000001;
            b[9] = 0.3657763036 * 0.0000000001;
            b[10] = 0.6657763036 * 0.000000000001;
            double sum = 0;
            double y = -Math.Log(4 * alpha * (1 - alpha));
            for (int i = 0; i < 11; i++)
            {
                sum += b[i] * Math.Pow(y, i);
            }
            return Math.Pow(sum * y, 0.5);
        }
        public static string CI (string VarName,string [] Numbers, string Tail, string Statistics,double  Significance){
            //Confidence Interval 置信区间
            //Tail为单双尾,Tail = "单尾"，Tail = "双尾"
            //Statistics为统计量  "均值" "比例"
            int count = 0;
            //count用于统计样本个数
            StringBuilder result = new StringBuilder();
            //result记录反馈结果
            BigDecimal Sd = 0;
            //Sd为标准差
            double Temp = 0;
            //Temp用于记录临时数据
            BigDecimal BigTemp = 0;
            //BigTemp用于记录BigDecimal类型的临时数据
            if (Tail == "双尾"){
                Significance =1 - (1 - Significance) /2;
                // 1 - (1 - 0.95)/2
            }
            if (Statistics == "均值" || Statistics == "方差")
            {
                BigDecimal Mean = 0;
                BigDecimal Sum = 0;
                BigDecimal Sum2 = 0;
                foreach (string Num in Numbers)
                {
                    if (double.TryParse(Num,out Temp)){
                        Sum += Temp;
                        Sum2 += (BigDecimal)Temp * (BigDecimal)Temp;
                        count++;
                    }
                }
                if (count <= 1)
                {
                    return "样本数量过少，无法进行参数估计";
                }
                else
                {
                    Mean = Sum / count;
                    BigDecimal Variance = (Sum2 / count - Mean * Mean) * count / (count - 1);
                    Sd = MathV.Sqrt(Variance.ToString()).ToString();
                    result.Append(StrManipulation.PadLeftX(StrManipulation.VariableNamePolish(VarName), ' ', 12));
                    result.Append("\t");
                    result.Append(StrManipulation.PadLeftX(count.ToString(), ' ', 12));
                    result.Append("\t");
                    result.Append(StrManipulation.PadLeftX(MathV.NumberPolish(Mean.ToString()), ' ', 12));
                    result.Append("\t");
                    result.Append(StrManipulation.PadLeftX(MathV.NumberPolish(Sd.ToString()), ' ', 12));
                    result.Append("\t");
                    if (Statistics == "均值")
                    {
                        if (count >= 30)
                        {
                            //大样本
                            BigTemp = NORMSINV(Significance) * Sd / MathV.Sqrt(count).ToString();
                        }
                        else
                        {
                            //小样本
                            BigTemp = TINV(1 - Significance, (count - 1)) * Sd / MathV.Sqrt(count).ToString();
                        }
                        result.Append(StrManipulation.PadLeftX(MathV.NumberPolish((Mean - BigTemp).ToString()), ' ', 12));
                        result.Append("\t");
                        result.Append(StrManipulation.PadLeftX(MathV.NumberPolish((Mean + BigTemp).ToString()), ' ', 12));
                        result.Append("\r\n");
                    }
                    else
                    {
                        //Statistics == "方差"
                        BigTemp = (count - 1) * Variance;
                        result.Append(StrManipulation.PadLeftX(MathV.NumberPolish((BigTemp/Stat.chi2Ua(1-Significance,count -1)).ToString()), ' ', 12));
                        result.Append("\t");
                        MessageBox.Show(Significance.ToString());
                        MessageBox.Show(Stat.chi2Ua(Significance, count - 1).ToString());
                        result.Append(StrManipulation.PadLeftX(MathV.NumberPolish((BigTemp / Stat.chi2Ua(Significance, count - 1)).ToString()), ' ', 12));
                        result.Append("\r\n");
                    }
                    return result.ToString();
                }
            }
            else if (Statistics == "比例")
            {
                int OneCounts = 0;
                int ZeroCounts = 0;
                foreach (string Num in Numbers)
                {
                    if (Num.Trim() == "1"){
                        OneCounts++;
                    }
                    else if (Num.Trim() == "0"){
                        ZeroCounts++;
                    }
                }
                if (ZeroCounts+OneCounts  == 0)
                {
                    return "样本不存在，无法进行参数估计\r\n";
                }
                else
                {
                    if (ZeroCounts + OneCounts < 30)
                    {
                        result.Append("样本数量较少，进行比例的参数估计可能会不稳定\r\n");
                    }
                    BigDecimal Proportion = (BigDecimal)OneCounts / ((BigDecimal)ZeroCounts + (BigDecimal)OneCounts);
                    count = ZeroCounts + OneCounts;
                    result.Append(StrManipulation.PadLeftX(StrManipulation.VariableNamePolish(VarName), ' ', 12));
                    result.Append("\t");
                    result.Append(StrManipulation.PadLeftX((ZeroCounts + OneCounts).ToString(), ' ', 12));
                    result.Append("\t");
                    result.Append(StrManipulation.PadLeftX(MathV.NumberPolish(Proportion.ToString()), ' ', 12));
                    result.Append("\t");
                    BigTemp = Proportion * (1 - Proportion);
                    result.Append(StrManipulation.PadLeftX(MathV.NumberPolish(MathV.Sqrt(BigTemp.ToString()).ToString()), ' ', 12));
                    result.Append("\t");
                    BigTemp = NORMSINV(Significance) * ((BigDecimal)MathV.Sqrt(BigTemp.ToString()).ToString() / (BigDecimal)MathV.Sqrt(count).ToString());
                    result.Append(StrManipulation.PadLeftX(MathV.NumberPolish((Proportion - BigTemp).ToString()).ToString(), ' ', 12));
                    result.Append("\t");
                    result.Append(StrManipulation.PadLeftX(MathV.NumberPolish((Proportion + BigTemp).ToString()).ToString(), ' ', 12));
                    result.Append("\r\n");
                    return result.ToString();
                }
            }
            else
            {
                return "未知统计量\r\n";
            }
        }
        private const string Header = "      变量名\t      样本数\t        均值\t      标准差\t    置信下限\t    置信上限\r\n";
        public static void LoopCI(DataGridView dataGridView1,RichTextBox richTextBox1, string Cols, string Tail, string Statistics, double Significance)
        {
            //这个函数的目的是循环对所要求的变量进行参数估计
            char[] separator = { ',' };
            string[] AllCols = Cols.Split(separator);
            string result = "";
            richTextBox1.AppendText("参数估计: ");
            richTextBox1.AppendText(Statistics);
            richTextBox1.AppendText("\r\n");
            richTextBox1.AppendText("置信度: ");
            richTextBox1.AppendText(Significance.ToString());
            richTextBox1.AppendText("\t");
            richTextBox1.AppendText(Tail);
            richTextBox1.AppendText("\r\n");
            richTextBox1.AppendText(Header);
            int ColNum = 0;
            foreach (string EachCol in AllCols)
            {
                if (Int32.TryParse(EachCol,out ColNum))
                {
                    string[] Numbers = Tabulation.ReadVector(MainForm.MainDT, ColNum - 1).ToArray();
                    result = CI(dataGridView1.Columns[ColNum - 1].Name, Numbers, Tail, Statistics, Significance);
                   richTextBox1.AppendText(result);
                }
            }
        }

        public static string QuickSummary(DataTable MainDT,int ColNum)
        {
            string ColName = MainDT.Columns[ColNum].ColumnName;
            List <string> Numbers = Tabulation.ReadVector(MainDT, ColNum);
            StringBuilder Result = new StringBuilder();
                BigDecimal sum = 0;
                BigDecimal mean = 0;
                BigDecimal Variance = 0;
                int  count = 0;
                BigDecimal max = 0;
                BigDecimal min = 0;
                BigDecimal sum2 = 0;
                Double TempNum = 0;
                //sum2用于计算数字的平方，方便计算方差
                foreach (string Num in Numbers)
                {
                    if (Double.TryParse(Num, out TempNum))
                    {
                        sum+= TempNum;
                        sum2 += (BigDecimal)TempNum * (BigDecimal)TempNum;
                        count++;
                        if (count == 1)
                        {
                            max = TempNum;
                            min = TempNum;
                        }
                        else{
                            if (max < TempNum)
                            {
                                max = TempNum;
                            }
                            if (min > TempNum)
                            {
                                min = TempNum;
                            }
                        }
                    }
                }
                if (count == 0)
                {
                    //如果没有获取任何数字
                    return "";
                }
                else if (count == 1)
                {
                    //如果只有一个数字，可以计算均值，但是不可以计算标准差
                    mean = sum / count;
                    Result.Append(StrManipulation.PadLeftX(StrManipulation.VariableNamePolish(ColName), ' ', 12));
                    Result.Append("\t");
                    Result.Append(StrManipulation.PadLeftX(count.ToString(), ' ', 12));
                    Result.Append("\t");
                    Result.Append(StrManipulation.PadLeftX(MathV.NumberPolish(mean.ToString()), ' ', 12));
                    Result.Append("\t");
                    Result.Append(StrManipulation.PadLeftX("NA", ' ', 12));
                    Result.Append("\t");
                    Result.Append(StrManipulation.PadLeftX(MathV.NumberPolish(min.ToString()), ' ', 12));
                    Result.Append("\t");
                    Result.Append( StrManipulation.PadLeftX(MathV.NumberPolish(max.ToString()), ' ', 12));
                    Result.Append("\r\n");
                    //Temp += Result;
                    return Result.ToString();
                }
                else
                {
                    //开始计算均值和方差
                    //方差要乘以调整系数
                    mean = sum / count;
                    Variance = (sum2 / count - mean * mean) * count / (count - 1);
                    Result.Append(StrManipulation.PadLeftX(StrManipulation.VariableNamePolish(ColName), ' ', 12));
                    Result.Append("\t");
                    Result.Append(StrManipulation.PadLeftX(count.ToString(), ' ', 12));
                    Result.Append("\t");
                    Result.Append(StrManipulation.PadLeftX(MathV.NumberPolish(mean.ToString()), ' ', 12));
                    Result.Append("\t");
                    Result.Append(StrManipulation.PadLeftX(MathV.NumberPolish(MathV.Sqrt(Variance.ToString()).ToString()), ' ', 12));
                    Result.Append("\t");
                    Result.Append(StrManipulation.PadLeftX(MathV.NumberPolish(min.ToString()), ' ', 12));
                    Result.Append("\t");
                    Result.Append( StrManipulation.PadLeftX(MathV.NumberPolish(max.ToString()), ' ', 12));
                    Result.Append("\r\n");
                    return Result.ToString();
                }
            }
        }
    }


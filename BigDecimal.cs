using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Windows.Forms;
namespace 箱线图
{
    public class BigDecimal
    {
        private const int Base = 10;
        string IntPart;
        int Rank;
        public BigDecimal(string Str)
        {
            int DecimalPoint = Str.IndexOf('.');
            if (DecimalPoint == -1)
            {
                Rank = 0;
                IntPart = Str;
            }
            else
            {
                Rank = -(Str.Length - DecimalPoint - 1);
                IntPart = Str.Remove(DecimalPoint, 1);
            }
        }
        public BigDecimal(string i,int r)
        {
            this.IntPart = i;
            this.Rank = r;
        }
        private int AcquireRank()
        {
            return Rank;
        }
        private string AcquireIntPart()
        {
            return IntPart;
        }
        
        public static BigDecimal Add(BigDecimal Num1, BigDecimal Num2)
        {
            int Rank1 = Num1.AcquireRank();
            int Rank2 = Num2.AcquireRank();
            string IntPart1 = Num1.AcquireIntPart();
            string IntPart2 = Num2.AcquireIntPart();
            if (Rank1 > Rank2)
            {
                //第一个数字小数点需往右边移动
                StringBuilder Str = new StringBuilder();
                for (int i = 0; i < Rank1 - Rank2; i++)
                    Str.Append("0");
                Str.Append(IntPart1);
                BigInteger x = BigInteger.Parse(Str.ToString()) + BigInteger.Parse(IntPart2);
                return new BigDecimal(x.ToString(), Rank2);
            }
            else if (Rank1 < Rank2)
            {
                //第二个数字小数点需往右边移动
                StringBuilder Str = new StringBuilder();
                for (int i = 0; i < Rank2 - Rank1; i++)
                    Str.Append("0");
                Str.Append(IntPart2);
                BigInteger x = BigInteger.Parse(Str.ToString()) + BigInteger.Parse(IntPart1);
                return new BigDecimal(x.ToString(), Rank1);
            }
            else
            {
                //无需调整
                BigInteger x = BigInteger.Parse(IntPart1) + BigInteger.Parse(IntPart2);
                return new BigDecimal(x.ToString(), Rank1);
            }
        }
        
       /* public static BigDecimal Add(BigDecimal Num1, BigDecimal Num2)
        {
            int Rank1 = Num1.AcquireRank();
            int Rank2 = Num2.AcquireRank();
            if (Rank1 == Rank2)
            {
                BigInteger x = BigInteger.Parse(Num1.AcquireIntPart()) + BigInteger.Parse(Num2.AcquireIntPart());
                if (Rank1 < 0)
                {
                    //小数
                    //6*10^(-7) 
                    string Result = x.ToString().Insert(x.ToString().Length + Rank1, ".");
                    return new BigDecimal(Result);
                }
                else if (Rank1 > 0)
                {
                    //整数 3×10^2
                    string AddZero = new string('0', -Rank1);
                    string Result = x.ToString()+AddZero;

                    return new BigDecimal(Result);
                }
                else
                {
                    return new BigDecimal(x.ToString());
                }
            }
            else if (Rank1 > Rank2)
            {
                if (Rank2 < 0)
                {
                    string AdjIntPart;
                    string AddZero = new string('0', Rank1 - Rank2);
                    AdjIntPart = Num1.AcquireIntPart() + AddZero;
                    BigInteger x = BigInteger.Parse(AdjIntPart) + BigInteger.Parse(Num2.AcquireIntPart());
                    string Result = x.ToString().Insert(x.ToString().Length + Rank2, ".");
                    return new BigDecimal(Result);
                }
                else
                {
                    //Rank2 >= 0
                    string AdjIntPart;
                    string AddZero = new string('0', Rank1 - Rank2);
                    AdjIntPart = Num1.AcquireIntPart() + AddZero;
                    BigInteger x = BigInteger.Parse(Num1.AcquireIntPart()) + BigInteger.Parse(AdjIntPart);
                    string Result = x.ToString()+new string ('0',Rank2);
                    return new BigDecimal(Result);
                }
                }
            else
            {
                return Add(Num2,Num1);
            }
           
        }*/
        public override string ToString()
        {
            if (Rank == 0)
            {
                return IntPart;
            }
            else if (Rank < 0)
            {
                int len = IntPart.Length;
                //求出整数部分的长度
                if (len <= -Rank)
                {
                    //需要补0
                    //string AddZero = new string('0', -Rank - len);
                    StringBuilder Str = new StringBuilder();
                    Str.Append("0.");
                    for (int i = 0; i < -Rank - len; i++)
                        Str.Append("0");
                    Str.Append(IntPart);
                    return Str.ToString();
                }
                else
                {
                    //len > -Rank
                    return IntPart.Insert(len + Rank, ".");
                    //插入小数点
                }
            }
            else
            {
                //Rank > 0
                //string  AddZero = new string('0', -Rank );
                StringBuilder Str = new StringBuilder();
                Str.Append(IntPart);
                for (int i = 0; i < -Rank;i++)
                    Str.Append("0");
                return Str.ToString();
            }
            
        }
        

        
    }
}

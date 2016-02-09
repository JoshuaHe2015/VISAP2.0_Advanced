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
            StringBuilder Numbers = new StringBuilder(Str);
            int DecimalPoint = Str.IndexOf('.');
            if (DecimalPoint == -1)
            {
                Rank = 0;
                IntPart = Str;
            }
            else
            {
                Rank = -(Str.Length - DecimalPoint - 1);
                //IntPart = Str.Remove(DecimalPoint, 1);
                IntPart = Numbers.Remove(DecimalPoint, 1).ToString();
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
        static BigInteger TenEight = 100000000;
        public static BigInteger Transform(string Numbers)
        {
            BigInteger x = 0;
            int len = Numbers.Length;
            for (int Start = 0;Start < len;Start = Start + 8){
                if (Start + 8 <= len){
                    x = Convert.ToInt32(Numbers.Substring(Start, 8)) + x * TenEight;
                }
                else{
                    x = Convert.ToInt32(Numbers.Substring(Start,Numbers.Length - Start)) + x * BigInteger.Pow(10,len - Start);
                }
            }
           
            return x;
            
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
                Str.Append(IntPart1);
                string AddZero = new string('0', Rank1 - Rank2);
                Str.Append(AddZero);
                //BigInteger x = BigInteger.Parse(Str.ToString()) + BigInteger.Parse(IntPart2);
                BigInteger x = Transform(Str.ToString()) + Transform(IntPart2);
                return new BigDecimal(x.ToString(), Rank2);
            }
            else if (Rank1 < Rank2)
            {
                //第二个数字小数点需往右边移动
                StringBuilder Str = new StringBuilder();
                Str.Append(IntPart2);
                string AddZero = new string('0', Rank2 - Rank1);
                Str.Append(AddZero);
                //BigInteger x = BigInteger.Parse(Str.ToString()) + BigInteger.Parse(IntPart1);
                BigInteger x = Transform(Str.ToString()) + Transform(IntPart1);
                return new BigDecimal(x.ToString(), Rank1);
            }
            else
            {
                //无需调整
                //BigInteger x = BigInteger.Parse(IntPart1) + BigInteger.Parse(IntPart2);
                BigInteger x = Transform(IntPart1) + Transform(IntPart2);
                return new BigDecimal(x.ToString(), Rank1);
            }
        }
        
       
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
                    string AddZero = new string('0', -Rank - len);
                    Str.Append(AddZero);
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
                string AddZero = new string('0', -Rank);
                Str.Append(AddZero);
                return Str.ToString();
            }
            
        }
        

        
    }
}

using System;
using System.Text;

namespace Skyiah.Way
{
    public static class TextUtility
    {
        // hexidecimal numbers
        static readonly char[] HEX = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f'};

        static readonly char[] HEXU = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

        public static string ToHex(ulong v)
        {
            char[] buf = new char[16];
            for (int i = 0; i < 16; i++)
            {
                buf[i] = HEX[(v >> (i * 4)) & 0x0fL];
            }

            return new string(buf);
        }

        // days of week
        static readonly string[] DOW = {"Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"};

        // sexagesimal numbers
        static readonly string[] SEX =
        {
            "00", "01", "02", "03", "04", "05", "06", "07", "08", "09",
            "10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
            "20", "21", "22", "23", "24", "25", "26", "27", "28", "29",
            "30", "31", "32", "33", "34", "35", "36", "37", "38", "39",
            "40", "41", "42", "43", "44", "45", "46", "47", "48", "49",
            "50", "51", "52", "53", "54", "55", "56", "57", "58", "59"
        };

        // months
        static readonly string[] MON =
        {
            "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
        };

        // HTTP date format
        public static string FormatDate(DateTime v)
        {
            v = v.ToUniversalTime();

            StringBuilder sb = new StringBuilder();
            int yr = v.Year;

            if (yr < 1000) sb.Append('0');
            if (yr < 100) sb.Append('0');
            if (yr < 10) sb.Append('0');
            sb.Append(v.Year);
            sb.Append('-');
            sb.Append(SEX[v.Month - 1]);
            sb.Append('-');
            sb.Append(SEX[v.Day]);

            sb.Append(SEX[v.Hour]);
            sb.Append(':');
            sb.Append(SEX[v.Minute]);
            sb.Append(':');
            sb.Append(SEX[v.Second]);

            return sb.ToString();
        }

        // HTTP date format
        public static string FormatUtcDate(DateTime v)
        {
            v = v.ToUniversalTime();

            StringBuilder gmt = new StringBuilder();
            gmt.Append(DOW[(int) v.DayOfWeek]);
            gmt.Append(", ");

            gmt.Append(SEX[v.Day]);
            gmt.Append(' ');
            gmt.Append(MON[v.Month - 1]);
            gmt.Append(' ');
            gmt.Append(v.Year);
            gmt.Append(' ');

            gmt.Append(SEX[v.Hour]);
            gmt.Append(':');
            gmt.Append(SEX[v.Minute]);
            gmt.Append(':');
            gmt.Append(SEX[v.Second]);

            gmt.Append(" GMT");

            return gmt.ToString();
        }

        public static bool TryParseDate(string str, out DateTime v)
        {
            int year = ParseInt(str, 0, 4, 1000);
            int month = ParseInt(str, 5, 2, 10);
            int day = ParseInt(str, 8, 2, 10);
            int len = str.Length;

            int hour = 0, minute = 0, second = 0; // optional time part
            if (len >= 19)
            {
                hour = ParseInt(str, 11, 2, 10);
                minute = ParseInt(str, 14, 2, 10);
                second = ParseInt(str, 17, 2, 10);
            }

            try
            {
                v = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Unspecified);
                return true;
            }
            catch
            {
                v = default;
                return false;
            }
        }

        public static bool TryParseUtcDate(string utcstr, out DateTime v)
        {
            int day = ParseInt(utcstr, 5, 2, 10);
            int month = ParseMonth(utcstr, 8);
            int year = ParseInt(utcstr, 12, 4, 1000);
            int hour = ParseInt(utcstr, 17, 2, 10);
            int minute = ParseInt(utcstr, 20, 2, 10);
            int second = ParseInt(utcstr, 23, 2, 10);
            try
            {
                v = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
                return true;
            }
            catch
            {
                v = default;
                return false;
            }
        }

        static int ParseInt(string str, int start, int count, int @base)
        {
            int num = 0;
            for (int i = 0; i < count; i++, @base /= 10)
            {
                char c = str[start + i];
                int digit = c - '0';
                if (digit < 0 || digit > 9) digit = 0;
                num += digit * @base;
            }

            return num;
        }

        static int ParseMonth(string str, int start)
        {
            char a = str[start], b = str[start + 1], c = str[start + 2];
            for (int i = 0; i < MON.Length; i++)
            {
                string m = MON[i];
                if (a == m[0] && b == m[1] && c == m[2])
                {
                    return i + 1;
                }
            }

            return 0;
        }

        //
        // DIGEST
        //

        public static string MD5(string src, bool uppercase = false)
        {
            if (src == null) return null;

            var arr = uppercase ? HEXU : HEX;

            byte[] raw = Encoding.UTF8.GetBytes(src);

            // digest and transform
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var hash = md5.ComputeHash(raw);
                var str = new StringBuilder(32);
                for (int i = 0; i < 16; i++)
                {
                    byte b = hash[i];
                    str.Append(arr[b >> 4]);
                    str.Append(arr[b & 0x0f]);
                }

                return str.ToString();
            }
        }

        public static string SHA1(string src)
        {
            if (src == null) return null;

            byte[] raw = Encoding.UTF8.GetBytes(src);

            // digest and transform
            using (var sha1 = System.Security.Cryptography.SHA1.Create())
            {
                byte[] hash = sha1.ComputeHash(raw);
                StringBuilder str = new StringBuilder(32);
                for (int i = 0; i < 20; i++)
                {
                    byte b = hash[i];
                    str.Append(HEX[b >> 4]);
                    str.Append(HEX[b & 0x0f]);
                }

                return str.ToString();
            }
        }

        /// <summary>
        /// Returns the central 8 bytes of the hash result of the input string.
        /// </summary>
        public static bool EqualsCredential(this string credential, string id, string pass)
        {
            if (credential == null || credential.Length != 32) return false;

            // convert to bytea, assume ascii 
            int idlen = id.Length;
            int passlen = pass.Length;
            int len = idlen + passlen + 1;
            byte[] raw = new byte[len];
            int p = 0;
            for (int i = 0; i < idlen; i++)
            {
                raw[p++] = (byte) id[i];
            }

            raw[p++] = (byte) ':';
            for (int i = 0; i < passlen; i++)
            {
                raw[p++] = (byte) pass[i];
            }

            // digest and transform
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] hash = md5.ComputeHash(raw);
                for (int i = 0; i < 16; i++)
                {
                    byte b = hash[i];
                    if (credential[i * 2] != HEX[b >> 4] || credential[i * 2 + 1] != HEX[b & 0x0f])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static string BytesToHex(byte[] bytes, int count)
        {
            char[] buf = new char[count * 2];
            for (int i = 0; i < count; i++)
            {
                int b = bytes[i];
                buf[i * 2] = HEX[(b & 0xf0) >> 4];
                buf[i * 2 + 1] = HEX[b & 0x0f];
            }

            return new string(buf);
        }

        public static byte[] HexToBytes(string str)
        {
            int strlen = str.Length;
            byte[] buf = new byte[strlen / 2];
            for (int i = 0; i < buf.Length; i++)
            {
                byte b = (byte) ((Dv(str[i * 2]) << 4) | Dv(str[i * 2 + 1]));
                buf[i] = b;
            }

            return buf;
        }

        static int Dv(char hex)
        {
            int num = hex - 'a';
            if (num >= 0 && num <= 5)
            {
                return num + 10;
            }
            num = hex - '0';
            if (num >= 0 && num <= 9)
            {
                return num;
            }

            return 0;
        }

        //
        // CONVERTION
        //

        public static bool ToBool(this string str)
        {
            return str == "true" || str == "1";
        }

        public static char ToChar(this string str)
        {
            return string.IsNullOrEmpty(str) ? '\0' : str[0];
        }

        public static short ToShort(this string str)
        {
            if (str == null) return 0;
            return (short) str.ToInt(0, str.Length);
        }

        public static int ToInt(this string str, int start, int end = -1)
        {
            if (str == null) return 0;
            if (end == -1)
            {
                end = str.Length;
            }

            int sum = 0;
            for (int i = start; i < end; i++)
            {
                char c = str[i];
                int n = c - '0';
                if (n >= 0 && n <= 9)
                {
                    sum = sum * 10 + n;
                }
            }

            return sum;
        }

        public static int ToInt(this string str)
        {
            if (str == null) return 0;
            return str.ToInt(0, str.Length);
        }

        public static long ToLong(this string str, int start, int end)
        {
            long sum = 0;
            for (int i = start; i < end; i++)
            {
                char c = str[i];
                int n = c - '0';
                if (n > 0 && n <= 10)
                {
                    sum = sum * 10 + n;
                }
            }

            return sum;
        }

        public static long ToLong(this string str)
        {
            if (str == null) return 0;
            return str.ToLong(0, str.Length);
        }

        public static DateTime ToDateTime(this string str)
        {
            TryParseDate(str, out var v);
            return v;
        }

        public static (string, string) ToStringString(this string str, char sep = '-')
        {
            int pos = 0;
            var a = str.ParseString(ref pos, sep);
            var b = str.ParseString(ref pos, sep);
            return (a, b);
        }

        public static (string, short) ToStringShort(this string str, char sep = '-')
        {
            int pos = 0;
            var a = str.ParseString(ref pos, sep);
            var b = str.ParseShort(ref pos, sep);
            return (a, b);
        }

        public static (string, int) ToStringInt(this string str, char sep = '-')
        {
            int pos = 0;
            var a = str.ParseString(ref pos, sep);
            var b = str.ParseInt(ref pos, sep);
            return (a, b);
        }

        public static (string, long) ToStringLong(this string str, char sep = '-')
        {
            int pos = 0;
            var a = str.ParseString(ref pos, sep);
            var b = str.ParseLong(ref pos, sep);
            return (a, b);
        }

        public static (short, string) ToShortString(this string str, char sep = '-')
        {
            int pos = 0;
            var a = str.ParseShort(ref pos, sep);
            var b = str.ParseString(ref pos, sep);
            return (a, b);
        }

        public static (short, short) ToShortShort(this string str, char sep = '-')
        {
            int pos = 0;
            var a = str.ParseShort(ref pos, sep);
            var b = str.ParseShort(ref pos, sep);
            return (a, b);
        }

        public static (short, int) ToShortInt(this string str, char sep = '-')
        {
            int pos = 0;
            var a = str.ParseShort(ref pos, sep);
            var b = str.ParseInt(ref pos, sep);
            return (a, b);
        }

        public static (short, long) ToShortLong(this string str, char sep = '-')
        {
            int pos = 0;
            var a = str.ParseShort(ref pos, sep);
            var b = str.ParseLong(ref pos, sep);
            return (a, b);
        }

        public static (int, string) ToIntString(this string str, char sep = '-')
        {
            int pos = 0;
            var a = str.ParseInt(ref pos, sep);
            var b = str.ParseString(ref pos, sep);
            return (a, b);
        }

        public static (int, short) ToIntShort(this string str, char sep = '-')
        {
            int pos = 0;
            var a = str.ParseInt(ref pos, sep);
            var b = str.ParseShort(ref pos, sep);
            return (a, b);
        }

        public static (int, int) ToIntInt(this string str, char sep = '-')
        {
            int pos = 0;
            var a = str.ParseInt(ref pos, sep);
            var b = str.ParseInt(ref pos, sep);
            return (a, b);
        }

        public static (int, long) ToIntLong(this string str, char sep = '-')
        {
            int pos = 0;
            var a = str.ParseInt(ref pos, sep);
            var b = str.ParseLong(ref pos, sep);
            return (a, b);
        }

        public static (long, string) ToLongString(this string str, char sep = '-')
        {
            int pos = 0;
            var a = str.ParseLong(ref pos, sep);
            var b = str.ParseString(ref pos, sep);
            return (a, b);
        }

        public static (long, short) ToLongShort(this string str, char sep = '-')
        {
            int pos = 0;
            var a = str.ParseLong(ref pos, sep);
            var b = str.ParseShort(ref pos, sep);
            return (a, b);
        }

        public static (long, int) ToLongInt(this string str, char sep = '-')
        {
            int pos = 0;
            var a = str.ParseLong(ref pos, sep);
            var b = str.ParseInt(ref pos, sep);
            return (a, b);
        }

        public static (long, long) ToLongLong(this string str, char sep = '-')
        {
            int pos = 0;
            var a = str.ParseLong(ref pos, sep);
            var b = str.ParseLong(ref pos, sep);
            return (a, b);
        }

        public static string ParseString(this string str, ref int pos, char sep = '-')
        {
            int len = str.Length;
            if (pos >= len)
            {
                return null;
            }

            if (str[pos] == sep) pos++; // skip sep

            int p = pos;
            while (p < len && str[p] != sep) p++;
            string ret = p == len ? str : str.Substring(pos, p);
            pos = p;
            return ret;
        }

        public static short ParseShort(this string str, ref int pos, char sep = '-')
        {
            int len = str.Length;
            if (pos >= len)
            {
                return 0;
            }

            if (str[pos] == sep) pos++; // skip sep

            int sum = 0;
            int p = pos;
            while (p < len && str[p] != sep)
            {
                char c = str[p];
                int n = c - '0';
                if (n >= 0 && n <= 9)
                {
                    sum = sum * 10 + n;
                }

                p++;
            }
            pos = p;
            return (short) sum;
        }

        public static int ParseInt(this string str, ref int pos, char sep = '-')
        {
            int len = str.Length;
            if (pos >= len)
            {
                return 0;
            }

            if (str[pos] == sep) pos++; // skip sep

            int sum = 0;
            int p = pos;
            while (p < len && str[p] != sep)
            {
                char c = str[p];
                int n = c - '0';
                if (n >= 0 && n <= 9)
                {
                    sum = sum * 10 + n;
                }

                p++;
            }

            pos = p;
            return sum;
        }

        public static long ParseLong(this string str, ref int pos, char sep = '-')
        {
            int len = str.Length;
            if (pos >= len)
            {
                return 0;
            }

            if (str[pos] == sep) pos++; // skip sep

            long sum = 0;
            int p = pos;
            while (p < len && str[p] != sep)
            {
                char c = str[p];
                int n = c - '0';
                if (n >= 0 && n <= 9)
                {
                    sum = sum * 10 + n;
                }

                p++;
            }

            pos = p;
            return sum;
        }

        public static decimal ParseDecimal(this string str, ref int pos, char sep = '-')
        {
            int len = str.Length;
            if (pos >= len)
            {
                return 0;
            }

            if (str[pos] == sep) pos++; // skip sep

            JNumber num = new JNumber(str[pos]);
            int p = pos;
            for (;;)
            {
                if (p >= len - 1) break;
                int b = str[++p];
                if (b == '.')
                {
                    num.Pt = true;
                }
                else if (b >= '0' && b <= '9')
                {
                    num.Add(b);
                }
                else
                {
                    pos = p - 1;
                    return num;
                }
            }

            pos = p;
            return num.Decimal;
        }


        public static bool Compare(string a, string b, int num)
        {
            if (a.Length < num || b.Length < num) return false;
            for (int i = 0; i < num; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
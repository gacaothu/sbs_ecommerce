using SBS_Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SBS_Ecommerce.Framework.Utilities
{
    public sealed class GeneratorUtil
    {
        private const int Length = 10;
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private static Random random = new Random();
        private static SBS_Entities db = new SBS_Entities();

        public static string GenerateMemberNo()
        {
            string key = new string(Enumerable.Repeat(Chars, Length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            if (CheckExist(key))
            {
                GenerateMemberNo();
            }
            return key;
        }

        private static bool CheckExist(string s)
        {
            List<string> lstMemberNo = db.GetUsers.Where(m => m.MemberNo != null).Select(o => o.MemberNo).ToList();
            return lstMemberNo.Where(item => lstMemberNo.Contains(s)).FirstOrDefault() != null;
        }
    }
}
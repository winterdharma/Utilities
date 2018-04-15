﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Chinese
{
    /// <summary>
    /// This class provides an ordered array of the radicals traditionally used to organize
    /// Chinese dictionaries. It also provides a few handy methods to fetch a radical's string
    /// or number or validate whether a string is one of the 214 radicals.
    /// </summary>
    public static class Radical
    {
        private static List<string> _radicals = new List<string> {
                "??", "一", "丨", "丶", "丿", "乙", "亅", "二", "亠", "人", "儿",
                "入", "八", "冂", "冖", "冫", "几", "凵", "刀", "力", "勹",
                "匕", "匚", "匸", "十", "卜", "卩", "厂", "厶", "又", "口",
                "囗", "土", "士", "夂", "夊", "夕", "大", "女", "子", "宀",
                "寸", "小", "尢", "尸", "屮", "山", "川", "工", "己", "巾",
                "干", "幺", "广", "廴", "廾", "弋", "弓", "彑", "彡", "彳",
                "心", "戈", "戸", "手", "支", "攴", "文", "斗", "斤", "方",
                "无", "日", "曰", "月", "木", "欠", "止", "歹", "殳", "毋",
                "比", "毛", "氏", "气", "水", "火", "爫", "父", "爻", "爿",
                "片", "牙", "牛", "犬", "玄", "玉", "瓜", "瓦", "甘", "生",
                "用", "田", "疋", "疒", "癶", "白", "皮", "皿", "目", "矛",
                "矢", "石", "示", "禸", "禾", "穴", "立", "竹", "米", "糸",
                "缶", "网", "羊", "羽", "老", "而", "耒", "耳", "聿", "肉",
                "臣", "自", "至", "臼", "舌", "舛", "舟", "艮", "色", "艸",
                "虍", "虫", "血", "行", "衣", "西", "見", "角", "言", "谷",
                "豆", "豕", "豸", "貝", "赤", "走", "足", "身", "車", "辛",
                "辰", "辵", "邑", "酉", "釆", "里", "金", "長", "門", "阜",
                "隶", "隹", "雨", "青", "非", "面", "革", "韋", "韭", "音",
                "頁", "風", "飛", "食", "首", "香", "馬", "骨", "高", "髟",
                "鬥", "鬯", "鬲", "鬼", "魚", "鳥", "鹵", "鹿", "麥", "麻",
                "黄", "黍", "黒", "黹", "黽", "鼎", "鼓", "鼠", "鼻", "齊",
                "齒", "龍", "龜", "龠"
            };

        public static List<string> Radicals { get => _radicals; }

        public static bool IsRadical(string str)
        {
            if (str.Equals("??"))
                return false;

            return Radicals.Contains(str);
        }

        public static string GetRadical(int radicalNumber)
        {
            return Radicals[radicalNumber];
        }

        public static int GetNumber(string radical)
        {
            return Radicals.ToList().FindIndex(element => element.Equals(radical));
        }
    }
}

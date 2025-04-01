using System.Text;

namespace GalgameSearchFor.ConsoleStyle.ANSI;

public static class ToStings
{
    public static string TargetPlatform(string platform)
    {
        return platform.ToLower() switch
        {
            "pc" or "电脑" or "wim" or "windows" => $"\e[38;2;90;200;200m{platform}\e[0m", // 天空蓝
            "tyranor" => "\e[38;2;255;176;228mTyranor\e[0m", // 偏粉色
            "krkr" => "\e[38;2;100;149;237mK\e[38;2;147;112;219mr\e[38;2;218;112;214mk\e[38;2;255;105;180mr\e[0m", // 蓝→紫→粉渐变
            "ons" => "\x1b[38;2;173;216;230mONS\x1b[0m", // 淡蓝色
            "ai翻译" => "\x1b[38;2;255;215;0mAI\x1b[38;2;144;238;144m翻译\x1b[0m", // 金色AI+亮绿翻译
            "ios" => "\x1b[38;2;255;255;255m\x1b[48;2;0;0;0miOS\x1b[0m", // 白字黑底
            "android" or "mobile" => $"\x1b[38;2;164;198;57m{platform}\x1b[0m", // 安卓绿
            _ => $"\x1b[4m{platform}\x1b[0m" // 下划线
        };
    }

    public static string LanguageColor(string language)
    {
        return language.ToLower() switch
        {
            // 中文（红+金配色）
            "中文" or "zh-hans" or "zh" or "汉化" or "简体" =>
                "\x1b[38;2;220;50;50m中\x1b[38;2;255;215;0m文\x1b[0m",

            // 日语（日出配色：红+白）
            "日语" or "ja" or "jp" or "日本語" =>
                "\x1b[38;2;220;50;50m日\x1b[38;2;255;255;255m語\x1b[0m",

            // 英语（蓝+红配色）
            "英语" or "en" or "eng" or "english" =>
                "\x1b[38;2;0;100;255mE\x1b[38;2;255;50;50mn\x1b[38;2;0;100;255mg\x1b[0m",

            // 其他语言
            "法语" or "fr" => "\x1b[38;2;0;114;187mF\x1b[38;2;239;65;53mr\x1b[0m", // 法国国旗蓝+红
            "韩语" or "ko" => "\x1b[38;2;0;71;160m한\x1b[38;2;205;45;45m국\x1b[38;2;0;71;160m어\x1b[0m", // 韩国国旗色
            "俄语" or "ru" => "\x1b[38;2;0;57;166mР\x1b[38;2;213;43;30mу\x1b[38;2;0;57;166mс\x1b[0m", // 俄罗斯国旗色

            _ => $"\x1b[4m{language}\x1b[0m" // 默认下划线
        };
    }

    public static string Gradient((int r, int g, int b) start, (int r, int g, int b) end, string text)
    {
        if (string.IsNullOrEmpty(text)) return text;

        var gradientText = new StringBuilder();
        var length = text.Length;

        for (var i = 0; i < length; i++)
        {
            // 计算当前字符的过渡颜色
            var ratio = (double)i / (length - 1);
            var r = (int)(start.r + (end.r - start.r) * ratio);
            var g = (int)(start.g + (end.g - start.g) * ratio);
            var b = (int)(start.b + (end.b - start.b) * ratio);

            // 添加ANSI颜色代码
            gradientText.Append($"\x1b[38;2;{r};{g};{b}m{text[i]}");
        }

        gradientText.Append("\x1b[0m"); // 重置样式
        return gradientText.ToString();
    }
}
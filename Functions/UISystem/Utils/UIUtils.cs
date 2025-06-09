using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReLogic.Graphics;

namespace Luxcinder.Functions.UISystem.Utils;
public static class UIUtils
{
    public static List<string> WrapText(string text, DynamicSpriteFont font, float maxWidth)
    {
        List<string> lines = new();
        StringBuilder currentLine = new();

        int i = 0;
        while (i < text.Length)
        {
            // 判断当前字符是否为中文
            bool isChinese = IsChinese(text[i]);

            if (isChinese)
            {
                // 中文按字符断行
                string testLine = currentLine.ToString() + text[i];
                float lineWidth = font.MeasureString(testLine).X;
                if (lineWidth > maxWidth)
                {
                    if (currentLine.Length > 0)
                    {
                        lines.Add(currentLine.ToString());
                        currentLine.Clear();
                    }
                }
                currentLine.Append(text[i]);
                i++;
            }
            else if (char.IsWhiteSpace(text[i]))
            {
                // 保留空格
                currentLine.Append(text[i]);
                i++;
            }
            else
            {
                // 英文按单词断行
                int wordStart = i;
                while (i < text.Length && !char.IsWhiteSpace(text[i]) && !IsChinese(text[i]))
                    i++;
                string word = text.Substring(wordStart, i - wordStart);
                string testLine = currentLine.Length == 0 ? word : currentLine + word;
                float lineWidth = font.MeasureString(testLine).X;
                if (lineWidth > maxWidth)
                {
                    if (currentLine.Length > 0)
                    {
                        lines.Add(currentLine.ToString());
                        currentLine.Clear();
                    }
                    // 单个英文单词超长，强制断行
                    if (font.MeasureString(word).X > maxWidth)
                    {
                        string part = "";
                        foreach (char c in word)
                        {
                            if (font.MeasureString(part + c).X > maxWidth)
                            {
                                lines.Add(part);
                                part = "";
                            }
                            part += c;
                        }
                        if (part.Length > 0)
                            currentLine.Append(part);
                    }
                    else
                    {
                        currentLine.Append(word);
                    }
                }
                else
                {
                    currentLine.Append(word);
                }
            }
        }
        if (currentLine.Length > 0)
            lines.Add(currentLine.ToString());
        return lines;
    }

    // 判断字符是否为中文
    private static bool IsChinese(char c)
    {
        return c >= 0x4e00 && c <= 0x9fff;
    }
}

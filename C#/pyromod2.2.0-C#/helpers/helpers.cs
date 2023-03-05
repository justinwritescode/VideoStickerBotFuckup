namespace Pyromod;

using System.Collections.Generic;
using System.Linq;

public static class helpers
{

    public static InlineKeyboardMarkup ikb(List<List<object>> rows)
    {
        List<List<InlineKeyboardButton>> lines = new List<List<InlineKeyboardButton>>();
        foreach (var row in rows)
        {
            List<InlineKeyboardButton> line = new List<InlineKeyboardButton>();
            foreach (var button in row)
            {
                InlineKeyboardButton button = button.GetType() == typeof(string) ? btn(button, button) : btn((string)((object[])button)[0], (string)((object[])button)[1]);
                line.Add(button);
            }
            lines.Add(line);
        }
        return new InlineKeyboardMarkup(lines);
        // return {'inline_keyboard': lines}
    }


    public InlineKeyboardButton btn(string text, string value, string type = "callback_data")
    {
        return new InlineKeyboardButton(text, new Dictionary<string, string> { { type, value } });
    }

    /// <sumary>The inverse of above</summry>
    public static List<List<InlineKeyboardButton>> bki(InlineKeyboardMarkup keyboard)
    {
        List<List<InlineKeyboardButton>> lines = new List<List<InlineKeyboardButton>>();
        foreach (InlineKeyboardButton[] row in keyboard.inline_keyboard)
        {
            List<InlineKeyboardButton> line = new List<InlineKeyboardButton>();
            foreach (InlineKeyboardButton button in row)
            {
                button = ntb(button);  // btn() format
                line.Add(button);
            }
            lines.Add(line);
        }
        return lines;
        // return ikb() format
    }

    static void ntb(Button button)
    {
        var btnType = new List<string>
    {
        "callback_data",
        "url",
        "switch_inline_query",
        "switch_inline_query_current_chat",
        "callback_game",
    };

        foreach (var type in btnType)
        {
            var value = button.GetType().GetProperty(type).GetValue(button);
            if (value != null)
                break;
        }

        var btn = new List<object> { button.text, value };
        if (btnType != "callback_data")
            btn.Add(btnType);
        return btn;
    }

    public static ReplyKeyboardMarkup kb(ReplyKeyboardMarkup rows =[], ** kwargs)
    {
        List<List<KeyboardButton>> lines = new List<List<KeyboardButton>>();
        foreach (var row in rows)
        {
            List<KeyboardButton> line = new List<KeyboardButton>();
            foreach (var button in row)
            {
                var button_type = button.GetType();
                if (button_type == typeof(string))
                {
                    button = new KeyboardButton(button);
                }
                else if (button_type == typeof(Dictionary<string, string>))
                {
                    button = new KeyboardButton(**button);
                }

                line.Add(button);
            }
            lines.Add(line);
        }
        return new ReplyKeyboardMarkup(lines, **kwargs);
    }


    kbtn = new KeyboardButton();


    public static ForceReply force_reply(bool selective = true)
    {
        return new ForceReply(selective);
    }


    public static List<List<T>> array_chunk<T>(List<T> input, int size)
    {
        return Enumerable.Range(0, input.Count / size).Select(i => input.GetRange(i * size, size)).ToList();
    }
}

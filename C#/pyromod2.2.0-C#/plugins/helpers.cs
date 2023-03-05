namespace VideoStickerBot;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using asyncio

using database;// import database
using emoji

using os

using plugins.bot_api;// import BotAPI
using pyrogram.types;// import InlineKeyboardMarkup, InlineKeyboardButton, Message
using pystark;//import filters


class Helpers : BotAPI
{

    private string ffprobe = """ffprobe -v error -select_streams v -show_entries {} -of csv=p=0:s=x {}""";

    public helpers() : base() { }

    publib helpers(message: Message, status: Message): : base(message, status)
    {
    }

public async Task<(bool, bool)> user_settings()
{
    var data = await database.get('users', this.user_id);
    if (!data)
        return (false, false);
    string tick = '✔';
    string cross = '✖️ ';
    string ask_emojis = 'Ask for Emojis ';
    string text = $"**Settings** \n\n";
    bool ask_emojis_db = data['ask_emojis'];
    if (ask_emojis_db)
    {
        ask_emojis += tick;
        text += $"**Ask For Emojis** : True";
    }
    else
    {
        ask_emojis += cross;
        text += $"**Ask For Emojis** : False";
    }
    var markup = new InlineKeyboardMarkup(new InlineKeyboardButton[][]
    {
            new InlineKeyboardButton[] { new InlineKeyboardButton(ask_emojis, callback_data = "emojis") }
    });
    return (text, markup);
}

public static async Task<string> extract_emojis(dynamic text)
{
    if (text is Message)
    {
        text = text.text;
    }
    string emojis = string.Concat(char for char in text if char in emoji.EMOJI_DATA);
    return emojis;
}



apublic async Task<string> subshell(string cmd = "")
{
    if (string.IsNullOrEmpty(cmd))
    {
        cmd = await get_ffmpeg_cmd();
    }
    var proc = await Process.StartAsync(new ProcessStartInfo
    {
        FileName = "cmd",
        Arguments = $"/c {cmd}",
        RedirectStandardError = true,
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true,
    });
    var stdout = await proc.StandardOutput.ReadToEndAsync();
    var stderr = await proc.StandardError.ReadToEndAsync();
    return !string.IsNullOrEmpty(stderr) ? stderr : stdout;
}

public async Task<string> get_ffmpeg_cmd()
{
    var cmd = @"ffmpeg -ss 00:00:00 -to 00:00:03 -t 3 -i {0} -fs 256000 -c:v libvpx-vp9 -b:v 0 -vf ""scale=512:-2"" -an {1}";
    // -metadata:s:v:0 alpha_mode=""1"" a -> ALPHA CHANNEL
    // -b:a 128k -c:a libopus -> AUDIO
    var dim = await get_dimensions();
    if (string.IsNullOrEmpty(dim))
    {
        cmd = cmd.Replace("512:-2", "-2:512");
    }
    return string.Format(cmd, input_file, output_file);
}

public async Task<string> get_dimensions()
{
    var cmd = ffprobe.format("stream=width,height", input_file);
    var info = await subshell(cmd);
    var dim = info.Split('x');
    dim = dim.Select(int.Parse).ToArray();
    if (dim[0] < dim[1])
    {
        return "";
    }
    else
    {
        return info;
    }
}


public async Task<(bool, string, string)> get_default_pack()
{
    var packs = await database.get('users', this.user_id, 'packs');
    if (packs <= 1)
    {
        if (!packs)
            var boo = false;
        else
            var boo = true;
        return (boo, this.PACK_NAME.format(this.user_id), this.PACK_TITLE);
    }
    else
        return (true, this.NEW_PACK_NAME.format(packs, this.user_id), this.NEW_PACK_TITLE.format(packs));
}

public static async Task send_webm(Message message)
{
    await message.reply_chat_action('upload_video');
    var file = f"downloads/{message.from_user.id}_{message.message_id}.webm";
    await message.download(file);
    await message.reply_document(file, quote = true);
    if (os.path.exists(file))
        os.remove(file);
}

public async Task<(bool, string, string)> GetDefaultPack()
{
    var packs = await Database.Get("users", this.UserId, "packs");
    if (packs <= 1)
    {
        if (!packs)
        {
            var boo = false;
        }
        else
        {
            var boo = true;
        }
        return (boo, this.PackName.format(this.UserId), this.PackTitle);
    }
    else
    {
        return (true, this.NewPackName.format(packs, this.UserId), this.NewPackTitle.format(packs));
    }
}

public static async Task SendWebm(Message message)
{
    await message.ReplyChatAction("upload_video");
    var file = $"downloads/{message.FromUser.Id}_{message.MessageId}.webm";
    await message.Download(file);
    await message.ReplyDocument(file, quote: true);
    if (System.IO.File.Exists(file))
    {
        System.IO.File.Delete(file);
    }
}

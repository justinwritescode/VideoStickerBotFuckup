
namespace VideoStickerBot; ;

using httpx;
using typing;
using requests;
using Telegram.Bot.Types;// import Message
using database;// import database
using pystark.config;// import ENV
using plugins.exceptions;// import TooManyRequests, AlreadyOccupied, UnknownException, StickersTooMuch, StickerPackInvalid


public class BotAPI
{
    private const string base_url = $"https://api.telegram.org/bot{ENV.BOT_TOKEN}/";
    private static readonly string new_pack_url = $"{base_url}createNewStickerSet";
    private static readonly string get_pack_url = $"{base_url}getStickerSet";
    private static readonly string add_to_pack_url = $"{base_url}addStickerToSet";
    private static readonly string ERROR = "Oops, an error occurred. My owner has been notified. \n\nFor queries visit @PhoenixXGuardians";
    private static readonly string username = requests.get(base_url + "getMe").json()["result"]["username"].title();
    private static readonly string PACK_NAME = "fpack_{}_by_" + username;
    private static readonly string NEW_PACK_NAME = "fpack{}_{}_by_" + username;
    private static readonly string PACK_TITLE = 'Pack By @' + username;
    private static readonly string NEW_PACK_TITLE = 'Pack {} By @' + username;
    private static readonly string LOG_CHAT = ENV["LOG_CHAT"];


    public BotAPI(Message message, Message status)
    {
        session = httpx.AsyncClient();
        user_id = message.from_user.id;
        message = message;
        directory = 'downloads'; w
        input_file = $"{this.directory}/{this.user_id}_{message.message_id}";
        output_file = $"{this.input_file}_output.webm";
        client = message._client;
        thid.status = status;
    }

    public async Task<Dictionary<string, object>> Params(string pack_name, string emojis, string title)
    {
        //if (!pack_name)
        //{
        //    pack_name = this.PACK_NAME.format(this.user_id);
        //}
        //if (!title)
        //{
        //    title = this.PACK_TITLE;
        //}
        var params = new Dictionary<string, object>
        {
            {"user_id", this.user_id},
            {"name", pack_name},
            {"emojis", emojis},
            {"title", title}
        };
        return params;
    }


    async Task<string> NewPack(Dictionary<string, string> parameters, Stream file)
    {
        return await this.Interact("new", parameters, file);
    }

    async Task<string> AddToPack(Dictionary<string, string> parameters, Stream file)
    {
        return await this.Interact("add", parameters, file);
    }

    async Task<string> GetPack(Dictionary<string, string> parameters, Stream file)
    {
        return await this.Interact("get", parameters, file);
    }

    public async Task interact(string method, Dictionary<string, string> params, FileStream file)
    {
        string url;
        if (method == "get")
        {
            url = this.get_pack_url;
        }
        else if (method == "new")
        {
            url = this.new_pack_url;
        }
        else
        {
            url = this.add_to_pack_url;
        }
        var data = await this.session.Post(url: url, params: params, files: new Dictionary<string, Stream> {
            {
                "webm_sticker",
                file
            }
        });
        var resp = data.Json();
        try
        {
            await this.error(resp, params["name"]);
        }
        catch (TooManyRequests e)
        {
            var msg = this.message;
            var err = $"Error from Telegram \n\n{e.desc} \n\nFor queries visit @StarkBotsChat";
            await msg.Reply(err, quote: true);
            await this.client.SendMessage(this.LOG_CHAT, $"'#TooManyRequests \n\n**Info** : {resp} \n\n**User** : {msg.from_user.Mention()} [`{msg.from_user.id}`] \n\n**Supposed Pack** : t.me/addstickers/{e.pack}");
        }
        catch (AlreadyOccupied)
        {
            await this.add_to_pack(params, file);
        }
        catch (StickerPackInvalid)
        {
            await this.new_pack(params, file);
        }
        catch (StickersTooMuch)
        {
            var total_packs = await database.Get("users", this.user_id, "packs");
            if (!total_packs)
            { // Just in Case
                total_packs = 1;
            }
            await this.status.Edit("Oh. Your pack {} is full. Lemme create a new one for you :)", total_packs);
            total_packs += 1;
            var pack_name = this.NEW_PACK_NAME.Format(total_packs, this.user_id);
            params.Update({
                "name",
                pack_name
            });
            params.Update({
                "title",
                this.NEW_PACK_TITLE.Format(total_packs)
            });
            await this.new_pack(params, file);
            await database.Set("users", this.user_id, {
                {
                    "packs",
                    total_packs
                }
            });
        }
        catch (UnknownException e)
        {
            var msg = this.message;
            await msg.Reply(this.ERROR);
            var err = await this.client.SendMessage(this.LOG_CHAT, $"'#ERROR \n\n**Info** : {resp} \n\n**User** : {msg.from_user.Mention()} [`{msg.from_user.id}`] \n\n**Supposed Pack** : t.me/addstickers/{e.pack}");
            await err.ReplyDocument(this.output_file);
            await err.Reply($"PARAMS : {params} \n\nMETHOD : {method}");
            await msg.Forward(this.LOG_CHAT, disable_notification: true);
            return false;
        }
        finally
        {
            if (method == "get")
            {
                if (resp["ok"])
                {
                    return resp["result"]["stickers"][-1]["file_id"];
                }
                else
                {
                    return false;
                }
            }
        }
        return true;
    }


    public static async Task error(dynamic resp, string pack_name)
    {
        if (!resp.ok)
        {
            string desc = resp.description;
            if (desc.Contains("Too Many Requests"))
            {
                // print('Raise TooManyRequests')
                throw new TooManyRequests(desc, pack_name);
            }
            else if (desc.Contains("STICKERS_TOO_MUCH"))
            {
                // print('Raise StickersTooMuch')
                throw new StickersTooMuch(desc, pack_name);
            }
            else if (desc.Contains("STICKERSET_INVALID"))
            {
                // print('Raise StickerPackInvalid')
                throw new StickerPackInvalid(desc, pack_name);
            }
            else if (desc.Contains("name is already occupied"))
            {
                // print('Raise AlreadyOccupied')
                throw new AlreadyOccupied(desc, pack_name);
            }
            else
            {
                // print('Raise UnknownException')
                throw new UnknownException(desc, pack_name);
            }
        }
    }

    async Task ffmpeg_error(string stderr)
    {
        Print(stderr);
        await msg.reply(ERROR);
        await client.send_message(LOG_CHAT, $"#ERROR #FFMPEG \n\n{stderr} \n\n**User** : {msg.from_user.mention} [`{msg.from_user.id}`]");
        await msg.forward(LOG_CHAT, disable_notification: true);
    }
}

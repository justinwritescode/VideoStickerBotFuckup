using System;
using System.Collections.Generic;
using System.Threading.Tasks;


var loop = asyncio.get_event_loop();

public class ListenerStopped : Exception
{
}

public class ListenerTimeout : Exception
{
}


public enum ListenerTypes
{
    MESSAGE,
    CALLBACK_QUERY
}

namespace Pyrogram
{
    public partial class Client : global::Telegram.Bot.Client
    {
        public async Task<Message> ListenAsync(
            string identifier,
            Func<Message, bool> filters,
            ListenerTypes listenerType,
            int timeout = 0,
            bool unallowedClickAlert = true)
        {
            if (listenerType != ListenerTypes.Message)
            {
                throw new ArgumentException("Parameter listenerType should be a value from pyromod.listen.ListenerTypes");
            }

            var future = new TaskCompletionSource<Message>();

            var listenerData = new Dictionary<string, object>
            {
                { "future", future },
                { "filters", filters },
                { "unallowedClickAlert", unallowedClickAlert }
            };

            Listeners[listenerType][identifier] = listenerData;

            try
            {
                if (timeout > 0)
                {
                    return await future.Task.TimeoutAfter(timeout);
                }
                else
                {
                    return await future.Task;
                }
            }
            finally
            {
                StopListening(identifier, listenerType);
            }
        }
    }

    [Patchable]
    public virtual async Task<Message> Ask(
            string text,
            Tuple<string, string> identifier,
            Filter filters = null,
            ListenerTypes listenerType = ListenerTypes.MESSAGE,
            double? timeout = null,
            params object[] args
        )
    {
        var request = await this.SendMessage(identifier.Item1, text, args);
        var response = await this.Listen(identifier, filters, listenerType, timeout);

        if (response != null)
        {
            response.Request = request;
        }

        return response;
    }

    [Patchable]
    public (Func<Update, Task> listener, object identifier) match_listener(
        object data = null,
        ListenerTypes listenerType = ListenerTypes.MESSAGE,
        object identifierPattern = null
    )
    {
        if (data != null)
        {
            var listeners = this.listeners[listenerType];
            // case with 3 args on identifier
            // most probably waiting for a specific user
            // to click a button in a specific message
            if (listeners.ContainsKey(data))
                return (listeners[data], data);
            // cases with 2 args on identifier
            // (None, user, message) does not make
            // sense since the message_id is not unique
            else if (listeners.ContainsKey(new object[] { data[0], data[1], null }))
                matched = new object[] { data[0], data[1], null };
            else if (listeners.ContainsKey(new object[] { data[0], null, data[2] }))
                matched = new object[] { data[0], null, data[2] };
            // cases with 1 arg on identifier
            // (None, None, message) does not make sense as well
            else if (listeners.ContainsKey(new object[] { data[0], null, null }))
                matched = new object[] { data[0], null, null };
            else if (listeners.ContainsKey(new object[] { null, data[1], null }))
                matched = new object[] { null, data[1], null };
            else
                return (null, null);
            return (listeners[matched], matched);
        }
        else if (identifierPattern != null)
        {
            foreach (var (identifier, listener) in this.listeners[listenerType])
            {
                if (match_identifier(identifierPattern, identifier))
                    return (listener, identifier);
            }
            return (null, null);
        }
        return (null, null);
    }

    public void StopListening(object data = null, ListenerTypes listener_type = ListenerTypes.MESSAGE, object identifier_pattern = null)
    {
        var listener = MatchListener(data, listener_type, identifier_pattern);
        if (listener == null)
        {
            return;
        }
        else if (listener["future"].IsCompleted)
        {
            this.Listeners[listener_type].Remove(identifier);
            return;
        }
        if (PyromodConfig.StoppedHandler != null)
        {
            PyromodConfig.StoppedHandler(identifier, listener);
        }
        else if (PyromodConfig.ThrowExceptions)
        {
            listener["future"].SetException(new ListenerStopped());
        }
        this.Listeners[listener_type].Remove(identifier);
    }


    namespace pyrogram.handlers.message_handler
    {
        public partial class MessageHandler
        {
            private Func<pyrogram.Client, pyrogram.types.Message, Task> registered_handler;

            public MessageHandler(Func<pyrogram.Client, pyrogram.types.Message, Task> callback, object filters = null) : base(this.resolve_future, filters)
            {
                this.registered_handler = callback;
                this.old__init__(this.resolve_future, filters);

                public async Task<bool> check(pyrogram.Client client, pyrogram.types.Message message)
                {
                    (pyrogram.types.Chat, pyrogram.types.User, long) listener = client.match_listener(
                        (message.chat.id, message.from_user.id, message.id),
                        ListenerTypes.MESSAGE,




                    )[0];

                    bool listener_does_match = false;
                    bool handler_does_match = false;

                    if (listener != null)
                    {
                        object filters = listener["filters"];
                        listener_does_match = (
                            await filters(client, message) if callable(filters) else true
                    );
            }
            handler_does_match = (
                await this.filters(client, message)
                    if callable(this.filters)
                else true
                );

                // let handler get the chance to handle if listener
                // exists but its filters doesn't match
                return listener_does_match || handler_does_match;
            }
        }
    }

    [Patchable]
    async Task resolve_future(client, message, * args)
    {
        listener_type = ListenerTypes.MESSAGE;
        listener, identifier = client.match_listener(
            (message.chat.id, message.from_user.id, message.id),
            listener_type,


        );
        listener_does_match = false;
        if (listener)
        {
            filters = listener["filters"];
            listener_does_match = (
                await filters(client, message) if callable(filters) else true
            );
        }

        if (listener_does_match)
        {
            if (!listener["future"].done())
            {
                listener["future"].set_result(message);
                del client.listeners[listener_type][identifier];
                throw pyrogram.StopPropagation;
            }
        }
        else
        {
            await this.registered_handler(client, message, *args);
        }
    }



    public class CallbackQueryHandler
    {
        private Func<Pyrogram.Client, Pyrogram.Types.CallbackQuery, Task<bool>> Check;
        private Func<Pyrogram.Client, Pyrogram.Types.CallbackQuery, Task> RegisteredHandler;
        private Func<Pyrogram.Client, Pyrogram.Types.CallbackQuery, Task<bool>> OldCheck;
        private Action<Pyrogram.Client, Func<Pyrogram.Client, Pyrogram.Types.CallbackQuery, Task<bool>>> OldInit;
        private Action<Pyrogram.Client, Func<Pyrogram.Client, Pyrogram.Types.CallbackQuery, Task<bool>>> Init;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "<Pending>")]
        private Func<Pyrogram.Client, Pyrogram.Types.CallbackQuery, Task> old__init__;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "<Pending>")]
        private Func<Pyrogram.Client, Pyrogram.Types.CallbackQuery, Task> __init__;
        private Func<Pyrogram.Client, Pyrogram.Types.CallbackQuery, Task<bool>> resolve_future;
        private Func<Pyrogram.Client, Pyrogram.Types.CallbackQuery, Task<bool>> filters;
        private Func<Pyrogram.Client, Pyrogram.Types.CallbackQuery, Task<bool>> listener;
        private Func<Pyrogram.Client, Pyrogram.Types.CallbackQuery, Task<bool>> permissive_listener;
        private Func<Pyrogram.Client, Pyrogram.Types.CallbackQuery, Task<bool>> alert;
        private Func<Pyrogram.Client, Pyrogram.Types.CallbackQuery, Task<bool>> unallowed_click_alert;
        private Func<Pyrogram.Client, Pyrogram.Types.CallbackQuery, Task<bool>> type;
        private Func<Pyrogram.Client, Pyrogram.Types.CallbackQuery, Task<bool>> unallowed_click_alert_text;
        private Func<Pyrogram.Client, Pyrogram.Types.CallbackQuery, Task<bool>> identifier_pattern;
        private Func<Pyrogram.Client, Pyrogram.Types.CallbackQuery, Task<bool>> listener_type;
        public CallbackQueryHandler(Func<Pyrogram.Client, Pyrogram.Types.CallbackQuery, Task<bool>> callback, Func<Pyrogram.Client, Pyrogram.Types.CallbackQuery, Task<bool>> filters = null)
        {
            RegisteredHandler = callback;
            OldInit = this.resolve_future;
            Filters = filters;
        }
        public async Task<bool> Check(Pyrogram.Client client, Pyrogram.Types.CallbackQuery query)
        {
            await listener = client.MatchListener((query.Message.Chat.Id, query.FromUser.Id, query.Message.Id), ListenerTypes.CALLBACK_QUERY)[0];
            if (PyromodConfig.UnallowedClickAlert)
            {
                permissive_listener = client.MatchListener(identifier_pattern: (query.Message.Chat.Id, null, query.Message.Id), listener_type: ListenerTypes.CALLBACK_QUERY)[0];
                if ((permissive_listener && !listener) && permissive_listener["unallowed_click_alert"])
                {
                    alert = permissive_listener["unallowed_click_alert"] if type(permissive_listener["unallowed_click_alert"]) == string ? PyromodConfig.UnallowedClickAlertText : alert;
                    await query.Answer(alert);
                    return false;
                }
            }
            filters = listener ? listener["filters"] : Filters;
            return await filters(client, query) if callable(filters);
        }
        public async Task RegisteredHandler(Pyrogram.Client client, Pyrogram.Types.CallbackQuery query)
        {
            await Check(client, query);
            await OldCheck(client, query);
        }
        public async Task OldCheck(Pyrogram.Client client, Pyrogram.Types.CallbackQuery query)
        {
            await RegisteredHandler(client, query);
        }
        public void Init(Pyrogram.Client client, Func<Pyrogram.Client, Pyrogram.Types.CallbackQuery, Task<bool>> resolve_future)
        {
            __init__ = this.resolve_future;
            OldInit(client, resolve_future);
        }
        public void OldInit(Pyrogram.Client client, Func<Pyrogram.Client, Pyrogram.Types.CallbackQuery, Task<bool>> resolve_future)
        {
            __init__ = this.resolve_future;
            Init(client, resolve_future);
        }
    }

    [Patchable]
    async Task resolve_future(TelegramBotClient client, CallbackQuery query, params object[] args)
    {
        string listener_type = ListenerTypes.CALLBACK_QUERY;
        Tuple<TelegramBotClient, object, object, object> identifier = client.match_listener(new object[] { query.message.chat.id, query.from_user.id, query.message.id }, listener_type);
        Dictionary<string, object> listener = (Dictionary<string, object>)client.listeners[listener_type][identifier];
        if (listener && listener["future"].done() == false)
        {
            listener["future"].set_result(query);
            del client.listeners[listener_type][identifier];
        }
        else
        {
            await this.registered_handler(client, query, args);
        }
    }

    [pstch(pyrogram.types.messages_and_media.message.Message)]
    class Message(pyrogram.types.messages_and_media.message.Message)
{
    @patchable
    async Task wait_for_click(
        Optional<int> from_user_id = null,
        Optional<int> timeout = null,
        filters = null,
        Union<string, bool> alert = true,
    )
    {
        return await this._client.listen(
            (this.chat.id, from_user_id, this.id),
            listener_type = ListenerTypes.CALLBACK_QUERY,
            timeout = timeout,
            filters = filters,
            unallowed_click_alert = alert,
        );
    }
}


public class Chat
{
    public Chat Listen(params object[] args)
    {
        return this._client.Listen(args, this.id, null, null);
    }
    public Chat Ask(string text, params object[] args)
    {
        return this._client.Ask(text, args, this.id, null, null);
    }
    public Chat StopListening(params object[] args)
    {
        return this._client.StopListening(args, this.id, null, null);
    }
}

public partial class User
{
    public async Task<Update> Listen(params object[] args)
    {
        return await _client.Listen((null, this.Id, null), args);
    }

    public async Task<Update> Ask(string text, params object[] args)
    {
        return await _client.Ask(
          text, (this.Id, this.Id, null), args
        );
    }

    public async Task StopListening(params object[] args)
    {
        return await _client.StopListening(
          args, identifierPattern: (null, this.Id, null)
        );
    }
}

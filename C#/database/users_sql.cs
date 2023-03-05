using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace database.users_sql
{
    public class Users
    {
        public BigInteger user_id { get; set; }
        public int packs { get; set; }
        public bool ask_emojis { get; set; }
        public bool get_webm { get; set; }
        public bool kang_mode { get; set; }
        public string default_emojis { get; set; }

        public Users(BigInteger user_id, int packs, bool ask_emojis)
        {
            this.user_id = user_id;
            this.packs = packs;
            this.ask_emojis = ask_emojis;
        }
    }
}

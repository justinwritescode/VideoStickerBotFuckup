using database.users_sql;
using pyromod;
using pystark;


bot = Stark();
Users.__table__.create(checkfirst = True);

// if __name__ == "__main__":
bot.activate();

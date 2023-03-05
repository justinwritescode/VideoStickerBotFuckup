namespace VideoStickerBot;

using System;

public class CustomException : Exception
{
    public string desc;
    public string pack;

    public CustomException(string desc, string pack)
    {
        this.desc = desc;
        this.pack = pack;
    }
}
public class AlreadyOccupied : CustomException
{
    public AlreadyOccupied(string desc, string pack) : base(desc, pack) { }
}
public class TooManyRequests : CustomException
{
    public TooManyRequests(string desc, string pack) : base(desc, pack) { }
}
public class UnknownException : CustomException
{
    public UnknownException(string desc, string pack) : base(desc, pack) { }
}
public class StickersTooMuch : CustomException
{
    public StickersTooMuch(string desc, string pack) : base(desc, pack) { }
}
public class StickerPackInvalid : CustomException
{
    public StickerPackInvalid(string desc, string pack) : base(desc, pack) { }
}

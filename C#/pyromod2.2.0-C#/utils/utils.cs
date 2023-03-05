using System;

/**<summary>
pyromod - A monkeypatcher add-on for Pyrogram
Copyright (C) 2020 Cezar H. <https://github.com/usernein>

This file is part of pyromod.

pyromod is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

pyromod is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with pyromod.  If not, see <https://www.gnu.org/licenses/>.
</summary>*/


public class PyromodConfig
{
    public static Action timeout_handler = null;
    public static Action stopped_handler = null;
    public static bool throw_exceptions = true;
    public static bool unallowed_click_alert = true;
    public static string unallowed_click_alert_text = "[pyromod] You're not expected to click this button.";

    public static dynamic patch(obj)
    {
        public static bool is_patchable(item)
        {
            return getattr(item[1], "patchable", false);
        }

        public static wrapper(container)
        {
            for (name, func in filter(is_patchable, container.__dict__.items()))
            {
                old = getattr(obj, name, null);
                setattr(obj, "old" + name, old);
                setattr(obj, name, func);
            }
            return container;
        }

        return wrapper;
    }

    public static dynamic patchable(dynamic func)
    {
        func.patchable = true;
        return func;
    }
}

/** <summary>
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
</summary> */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using import_helpers;



public class Paginator
{

    private List<object> objects;
    private Func<object, string> pageData;
    private Func<object, int, string> itemData;
    private Func<object, int, string> itemTitle;

    public Paginator(List<object> objects, Func<object, string> pageData = null, Func<object, int, string> itemData = null, Func<object, int, string> itemTitle = null)
    {
        Func<object, string> defaultPageCallback = x => x.ToString();
        Func<object, int, string> defaultItemCallback = (i, pg) => $"[{pg}] {i}";
        this.objects = objects;
        this.pageData = pageData ?? defaultPageCallback;
        this.itemData = itemData ?? defaultItemCallback;
        this.itemTitle = itemTitle ?? defaultItemCallback;
    }

    public List<List<(string, string)>> Create(int page, int lines = 5, int columns = 1)
    {
        int quantPerPage = lines * columns;
        page = page <= 0 ? 1 : page;
        int offset = (page - 1) * quantPerPage;
        int stop = offset + quantPerPage;
        List<object> cutted = objects.Skip(offset).Take(stop).ToList();

        int total = objects.Count;
        List<int> pagesRange = Enumerable.Range(1, (int)Math.Ceiling((double)total / quantPerPage)).ToList();
        int lastPage = pagesRange.Count;

        List<(string, string)> nav = new List<(string, string)>();
        if (page <= 3)
        {
            for (int n = 1; n <= 3; n++)
            {
                if (!pagesRange.Contains(n)) continue;
                string text = n == page ? $"· {n} ·" : n.ToString();
                nav.Add((text, pageData(n)));
            }
            if (lastPage >= 4)
            {
                nav.Add((lastPage > 5 ? "4 ›" : "4", pageData(4)));
            }
            if (lastPage > 4)
            {
                nav.Add((lastPage > 5 ? $"{lastPage} »" : lastPage.ToString(), pageData(lastPage)));
            }
        }
        else if (page >= lastPage - 2)
        {
            nav.Add((lastPage - 4 > 1 ? "« 1" : "1", pageData(1)));
            nav.Add((lastPage - 4 > 1 ? $"‹ {lastPage - 3}" : $"{lastPage - 3}", pageData(lastPage - 3)));
            for (int n = lastPage - 2; n <= lastPage; n++)
            {
                string text = n == page ? $"· {n} ·" : n.ToString();
                nav.Add((text, pageData(n)));
            }
        }
        else
        {
            nav = new List<(string, string)>()
            {
                ("« 1", pageData(1)),
                ($"‹ {page - 1}", pageData(page - 1)),
                ($"· {page} ·", "noop"),
                ($"{page + 1} ›", pageData(page + 1)),
                ($"{lastPage} »", pageData(lastPage))
            };
        }

        List<(string, string)> buttons = new List<(string, string)>();
        foreach (object item in cutted)
        {
            buttons.Add((itemTitle(item, page), itemData(item, page)));
        }
        List<List<(string, string)>> kbLines = buttons.Chunk(columns).ToList().Select(x => x.ToList()).ToList();
        if (lastPage > 1)
        {
            kbLines.Add(nav);
        }

        return kbLines;
    }
}

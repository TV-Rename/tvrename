//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//

using Alphaleonis.Win32.Filesystem;
using System.Collections.Generic;
using System.Linq;

namespace TVRename;

internal class DownloadIdentifiersController
{
    private readonly List<DownloadIdentifier> identifiers;

    public DownloadIdentifiersController()
    {
        identifiers = new List<DownloadIdentifier>
        {
            new DownloadFolderJpg(),
            new DownloadEpisodeJpg(),
            new DownloadFanartJpg(),
            new DownloadMede8erMetaData(),
            new DownloadpyTivoMetaData(),
            new DownloadWdtvMetaData(),
            new DownloadSeriesJpg(),
            new DownloadKodiMetaData(),
            new DownloadKodiImages(),
            new IncorrectFileDates(),
        };
    }

    public void NotifyComplete(FileInfo file)
    {
        foreach (DownloadIdentifier di in identifiers)
        {
            di.NotifyComplete(file);
        }
    }

    public ItemList ProcessMovie(MovieConfiguration? si, FileInfo filo)
    {
        if (si is null)
        {
            return new ItemList();
        }

        ItemList theActionList = new();

        foreach (DownloadIdentifier di in identifiers)
        {
            theActionList.Add(di.ProcessMovie(si, filo));
        }
        return theActionList;
    }

    public ItemList ProcessShow(ShowConfiguration? si)
    {
        ItemList theActionList = new();
        if (si is null)
        {
            return theActionList;
        }

        foreach (DownloadIdentifier di in identifiers)
        {
            theActionList.Add(di.ProcessShow(si));
        }
        return theActionList;
    }

    public ItemList ProcessSeason(ShowConfiguration? si, string folder, int snum)
    {
        ItemList theActionList = new();
        if (si is null)
        {
            return theActionList;
        }

        foreach (DownloadIdentifier di in identifiers)
        {
            theActionList.Add(di.ProcessSeason(si, folder, snum));
        }
        return theActionList;
    }

    public ItemList? ProcessEpisode(ProcessedEpisode? episode, FileInfo filo)
    {
        if (episode is null)
        {
            return null;
        }

        ItemList theActionList = new();
        foreach (DownloadIdentifier di in identifiers)
        {
            theActionList.Add(di.ProcessEpisode(episode, filo));
        }
        return theActionList;
    }

    public void Reset()
    {
        foreach (DownloadIdentifier di in identifiers)
        {
            di.Reset();
        }
    }

    public ItemList ForceUpdateMovie(DownloadIdentifier.DownloadType dt, MovieConfiguration? si, FileInfo filo)
    {
        ItemList theActionList = new();
        if (si is null)
        {
            return theActionList;
        }

        foreach (DownloadIdentifier di in identifiers.Where(di => dt == di.GetDownloadType()))
        {
            theActionList.Add(di.ProcessMovie(si, filo, true));
        }
        return theActionList;
    }

    public ItemList ForceUpdateShow(DownloadIdentifier.DownloadType dt, ShowConfiguration? si)
    {
        ItemList theActionList = new();
        if (si is null)
        {
            return theActionList;
        }

        foreach (DownloadIdentifier di in identifiers.Where(di => dt == di.GetDownloadType()))
        {
            theActionList.Add(di.ProcessShow(si, true));
        }
        return theActionList;
    }

    public ItemList ForceUpdateSeason(DownloadIdentifier.DownloadType dt, ShowConfiguration? si, string folder, int snum)
    {
        ItemList theActionList = new();
        if (si is null)
        {
            return theActionList;
        }

        foreach (DownloadIdentifier di in identifiers.Where(di => dt == di.GetDownloadType()))
        {
            theActionList.Add(di.ProcessSeason(si, folder, snum, true));
        }
        return theActionList;
    }

    public ItemList ForceUpdateEpisode(DownloadIdentifier.DownloadType dt, ProcessedEpisode episode, FileInfo filo)
    {
        ItemList theActionList = new();
        foreach (DownloadIdentifier di in identifiers.Where(di => dt == di.GetDownloadType()))
        {
            theActionList.Add(di.ProcessEpisode(episode, filo, true));
        }
        return theActionList;
    }
}

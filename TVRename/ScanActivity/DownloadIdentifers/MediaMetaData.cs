using System.Collections.Immutable;
using System.Linq;
using Alphaleonis.Win32.Filesystem;
using WinCopies.Util;

namespace TVRename;

internal sealed class MediaMetaData : DownloadIdentifier
{
    public override DownloadType GetDownloadType() => DownloadType.downloadMetaData;

    public override ItemList ProcessEpisode(ProcessedEpisode episode, FileInfo file, bool forceRefresh)
    {
        ItemList returnActions = [];

        TagLib.File tfile = TagLib.File.Create(file.FullName);

        if (tfile.Tag.Title != file.Name)
        {
            returnActions.Add(new UpdateMediaFileTitle(file,episode,file.Name));
        }

        if (episode.Overview.HasValue() && tfile.Tag.Description != episode.Overview)
        {
            returnActions.Add(new UpdateMediaFileDescription(file, episode, episode.Overview));
        }

        if (episode.Overview.HasValue() &&  tfile.Tag.Comment != episode.Overview)
        {
            returnActions.Add(new UpdateMediaFileComment(file, episode, episode.Overview));
        }

        if (episode.Year.HasValue && tfile.Tag.Year != episode.Year)
        {
            returnActions.Add(new UpdateMediaFileYear(file, episode, (uint)episode.Year.Value));
        }

        return returnActions;
    }

    public override ItemList? ProcessMovie(MovieConfiguration movie, FileInfo file, bool forceRefresh)
    {
        CachedMovieInfo? data = movie.CachedMovie;

        if (data == null)
        {
            return null;
        }

        ItemList returnActions = [];
        try
        {
            TagLib.File tfile = TagLib.File.Create(file.FullName);

            if (data.Name.HasValue() && tfile.Tag.Title != movie.Name)
            {
                returnActions.Add(new UpdateMediaFileTitle(file, movie, data.Name));
            }

            if (data.Overview.HasValue() && tfile.Tag.Description != data.Overview)
            {
                returnActions.Add(new UpdateMediaFileDescription(file, movie, data.Overview));
            }

            if (data.Overview.HasValue() && tfile.Tag.Comment != data.Overview)
            {
                returnActions.Add(new UpdateMediaFileComment(file, movie, data.Overview));
            }

            if (data.TagLine.HasValue() && tfile.Tag.Subtitle != data.TagLine)
            {
                returnActions.Add(new UpdateMediaFileSubtitle(file, movie, data.TagLine));
            }

            if (data.Genres.HasAny() && NotEqual(tfile.Tag.Genres, data.Genres))
            {
                returnActions.Add(new UpdateMediaFileGenres(file, movie, [.. data.Genres.ToEnumerable()]));
            }

            if (data.Year.HasValue && tfile.Tag.Year != data.Year)
            {
                returnActions.Add(new UpdateMediaFileYear(file, movie, (uint)data.Year.Value));
            }
        }
        catch (System.IO.FileNotFoundException fnfe)
        {
            LOGGER.Warn(fnfe, $"Failed to find file ({file.FullName})");
        }
        return returnActions;
    }

    private bool NotEqual(string[] tagGenres, SafeList<string> dataGenres)
    {
        if ( tagGenres.Length != dataGenres.Count)
        {
            return true;
        }
        return !string.Equals(tagGenres.ToImmutableSortedSet().ToCsv(),dataGenres.ToImmutableSortedSet().ToCsv());
    }
}

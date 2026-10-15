//
// Main website for TVRename is http://tvrename.com
//
// Source code available at https://github.com/TV-Rename/tvrename
//
// Copyright (c) TV Rename. This code is released under GPLv3 https://github.com/TV-Rename/tvrename/blob/master/LICENSE.md
//


using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TVRename;
// "PossibleNewMovie" represents a folder found by doing a Check in the 'Bulk Add Movie' dialog

public class PossibleNewMovie : ISeriesSpecifier, INotifyPropertyChanged
{
    public readonly string movieStub;
    public readonly FileInfo movieFile;
    // ReSharper disable once InconsistentNaming
    public string RefinedHint;
    public int? PossibleYear;
    internal int ProviderCode;
    internal TVDoc.ProviderType SourceProvider;

    public virtual string Folder => Directory.FullName;
    public virtual string Movie => CodeKnown ? CachedMovie?.Name ?? string.Empty : RefinedHint;
    public virtual string Year => CodeKnown ? CachedMovie?.FirstAired?.Year.ToString() ?? string.Empty : PossibleYear?.ToString() ?? string.Empty;
    public virtual string SourceCode => CodeKnown ? CodeString : string.Empty;
    public virtual int ImageTypeName => CodeKnown && HasStub ? 1 : 0;

    public DirectoryInfo Directory => movieFile.Directory;
    public CachedMovieInfo? CachedMovie => TVDoc.GetMovieCache(SourceProvider).GetMovie(ProviderCode);
    public bool CodeKnown => !CodeUnknown;
    public bool CodeUnknown => ProviderCode <= 0;
    public string CodeString => CodeKnown ? $"{ProviderCode} ({SourceProvider.PrettyPrint()})" : "<Unknown>";

    public PossibleNewMovie(FileInfo possibleMovieFile, bool showErrorMsgBox)
    {
        movieStub = possibleMovieFile.MovieFileNameBase();
        movieFile = possibleMovieFile;

        (string? directoryRefinedHint, int? directoryPossibleYear) = FileHelper.GuessShowName(possibleMovieFile.Directory.Name);
        (string? fileRefinedHint, int? filePossibleYear) = FileHelper.GuessShowName(possibleMovieFile.MovieFileNameBase());

        RefinedHint = fileRefinedHint.HasValue() ? fileRefinedHint : directoryRefinedHint;
        PossibleYear = filePossibleYear ?? directoryPossibleYear;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    // This method is called by the Set accessor of each property.
    // The CallerMemberName attribute that is applied to the optional propertyName
    // parameter causes the property name of the caller to be substituted as an argument.
    protected void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString()
    {
        return $"Possible New Movie: {movieFile.FullName} Estimated:{movieStub} Hint:{RefinedHint} Year:{PossibleYear} Provider:{SourceProvider.PrettyPrint()}:{ProviderCode}";
    }

    public TVDoc.ProviderType Provider => SourceProvider;

    public int TvdbId => Provider == TVDoc.ProviderType.TheTVDB && CodeKnown ? ProviderCode : -1;

    public string Name => RefinedHint;

    public MediaConfiguration.MediaType Media => MediaConfiguration.MediaType.tv;

    public int TvMazeId => Provider == TVDoc.ProviderType.TVmaze && CodeKnown ? ProviderCode : -1;

    public int TmdbId => Provider == TVDoc.ProviderType.TMDB && CodeKnown ? ProviderCode : -1;

    public string? ImdbCode { get; private set; }

    public Locale TargetLocale => new();

    public ProcessedSeason.SeasonType SeasonOrder => throw new InvalidOperationException();

    public bool HasStub => !string.IsNullOrWhiteSpace(movieStub);

    public void UpdateId(int id, TVDoc.ProviderType source)
    {
        SourceProvider = source;
        ProviderCode = id;

        NotifyPropertyChanged(nameof(Movie));
        NotifyPropertyChanged(nameof(Year));
        NotifyPropertyChanged(nameof(SourceCode));
        NotifyPropertyChanged(nameof(ImageTypeName));
    }

    public bool Matches(PossibleNewMovie ai) => movieStub.Equals(ai.movieStub, StringComparison.CurrentCultureIgnoreCase);
}

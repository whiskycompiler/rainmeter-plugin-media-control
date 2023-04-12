/* -----------------------------------------------------------------------
    Copyright (C) 2023 whiskycompiler

    This file is part of "MediaControlPlugin".

    This program is free software: you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation, either version 3
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
    See the GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program. If not, see <https://www.gnu.org/licenses/>.
--------------------------------------------------------------------------*/

using Windows.Media.Control;

using MediaControlPlugin.Models;

using Microsoft.Extensions.Logging;

namespace MediaControlPlugin.Services;

public class MediaControlService
{
    private readonly ILogger<MediaControlService> _logger;
    private readonly MediaThumbnailService _mediaThumbnailService;

    private GlobalSystemMediaTransportControlsSessionManager? _mediaSessionManager;

    public MediaInfo MediaInfo { get; private set; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MediaControlService"/> class.
    /// </summary>
    public MediaControlService(
        ILogger<MediaControlService> logger,
        MediaThumbnailService mediaThumbnailService)
    {
        _logger = logger;
        _mediaThumbnailService = mediaThumbnailService;
    }

    public async Task Initialize()
    {
        _mediaSessionManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
        _mediaSessionManager.CurrentSessionChanged += MediaSessionManager_CurrentSessionChanged;

        await UpdateMediaInfo(_mediaSessionManager);
    }

    private async void MediaSessionManager_CurrentSessionChanged(
        GlobalSystemMediaTransportControlsSessionManager sender,
        CurrentSessionChangedEventArgs args)
    {
        try
        {
            await Task.Delay(200); // we react too fast for the windows API so we need to wait until all info is really available
            await UpdateMediaInfo(sender);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error ocurred on media session change!");
        }
    }

    private async Task UpdateMediaInfo(GlobalSystemMediaTransportControlsSessionManager sessionManager)
    {
        var session = sessionManager.GetCurrentSession();
        if (session == null)
        {
            _logger.LogDebug("Session is null!");
            MediaInfo = new();
            return;
        }

        var mediaProperties = await session.TryGetMediaPropertiesAsync();
        var thumbnailPath = await _mediaThumbnailService.GetThumbnailPath(mediaProperties.Thumbnail);

        MediaInfo = new MediaInfo(
            mediaProperties.Title,
            mediaProperties.Artist,
            mediaProperties.AlbumTitle,
            mediaProperties.AlbumArtist,
            mediaProperties.AlbumTrackCount,
            mediaProperties.TrackNumber,
            string.Join(", ", mediaProperties.Genres),
            mediaProperties.PlaybackType?.ToString() ?? string.Empty,
            mediaProperties.Subtitle,
            thumbnailPath ?? string.Empty,
            session.SourceAppUserModelId);
    }

    ~MediaControlService()
    {
        if (_mediaSessionManager != null)
        {
            _mediaSessionManager.CurrentSessionChanged -= MediaSessionManager_CurrentSessionChanged;
        }
    }
}
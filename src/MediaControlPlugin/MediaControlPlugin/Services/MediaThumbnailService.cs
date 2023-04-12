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

using Windows.Storage.Streams;

using Microsoft.Extensions.Logging;

namespace MediaControlPlugin.Services;

/// <summary>
/// Service to handle thumbnails for media.
/// </summary>
/// <remarks>
/// Singleton
/// </remarks>
public class MediaThumbnailService
{
    private readonly ILogger<MediaThumbnailService> _logger;
    private readonly string _tempFolderPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="MediaThumbnailService"/> class.
    /// </summary>
    public MediaThumbnailService(ILogger<MediaThumbnailService> logger)
    {
        _logger = logger;
        _tempFolderPath = Path.Combine(Path.GetTempPath(), "Rainmeter.MediaControlPlugin");

        DeleteAllCachedThumbnails();
    }

    /// <summary>
    /// Gets the path to a thumbnail file from a stream reference.
    /// </summary>
    public async Task<string?> GetThumbnailPath(IRandomAccessStreamReference? streamReference)
    {
        if (streamReference == null)
        {
            _logger.LogDebug("Thumbnail stream ref is null!");
            return null;
        }

        try
        {
            Directory.CreateDirectory(_tempFolderPath);
            var tempFilePath = Path.Combine(_tempFolderPath, $"{Guid.NewGuid()}.jpg");
            _logger.LogInformation(tempFilePath);

            var thumbnailStream = (await streamReference.OpenReadAsync()).AsStreamForRead();
            await using var fileStream = new FileStream(tempFilePath, FileMode.CreateNew);

            thumbnailStream.Seek(0, SeekOrigin.Begin);
            await thumbnailStream.CopyToAsync(fileStream);

            return tempFilePath;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occurred trying to get media thumbnail!");
            return null;
        }
    }

    /// <summary>
    /// Deletes all cached media thumbnails.
    /// </summary>
    public void DeleteAllCachedThumbnails()
    {
        try
        {
            if (Directory.Exists(_tempFolderPath))
            {
                Directory.Delete(_tempFolderPath, true);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error occurred trying to delete cache folder!");
        }
    }
}
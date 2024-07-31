import { DownloadTaskType, FolderType, PlexMediaType } from '@dto';
import ButtonType from '@enums/buttonType';

export const Convert = {
	buttonTypeToIcon(type: ButtonType): string {
		switch (type) {
			case ButtonType.Download:
				return 'mdi-download';
			case ButtonType.Error:
				return 'mdi-alert-circle';
			case ButtonType.ExternalLink:
				return 'mdi-open-in-new';
			case ButtonType.Start:
			case ButtonType.Resume:
				return 'mdi-play';
			case ButtonType.Restart:
				return 'mdi-refresh';
			case ButtonType.Pause:
				return 'mdi-pause';
			case ButtonType.Stop:
				return 'mdi-stop';
			case ButtonType.Clear:
				return 'mdi-notification-clear-all';
			case ButtonType.Details:
				return 'mdi-chart-box-outline';
			case ButtonType.Delete:
				return 'mdi-delete';
			default:
				return '';
		}
	},

	mediaTypeToIcon(mediaType: PlexMediaType): string {
		switch (mediaType) {
			case PlexMediaType.TvShow:
				return 'mdi-television-classic';
			case PlexMediaType.Season:
				return 'mdi-play-box-multiple';
			case PlexMediaType.Episode:
				return 'mdi-movie-open';
			case PlexMediaType.Movie:
				return 'mdi-filmstrip';
			case PlexMediaType.Music:
				return 'mdi-music';
			default:
				return 'mdi-help-circle-outline';
		}
	},

	mediaTypeToFolderType(mediaType: PlexMediaType): FolderType {
		switch (mediaType) {
			case PlexMediaType.TvShow:
				return FolderType.TvShowFolder;
			case PlexMediaType.Season:
				return FolderType.TvShowFolder;
			case PlexMediaType.Episode:
				return FolderType.TvShowFolder;
			case PlexMediaType.Movie:
				return FolderType.MovieFolder;
			case PlexMediaType.OtherVideos:
				return FolderType.OtherVideosFolder;
			case PlexMediaType.Music:
				return FolderType.MusicFolder;
			case PlexMediaType.Album:
				return FolderType.MusicFolder;
			case PlexMediaType.Song:
				return FolderType.MusicFolder;
			default:
				return FolderType.Unknown;
		}
	},
	toPlexMediaType(downloadType: DownloadTaskType) {
		switch (downloadType) {
			case DownloadTaskType.Movie:
				return PlexMediaType.Movie;
			case DownloadTaskType.TvShow:
				return PlexMediaType.TvShow;
			case DownloadTaskType.Season:
				return PlexMediaType.Season;
			case DownloadTaskType.Episode:
				return PlexMediaType.Episode;
			default:
				return PlexMediaType.Unknown;
		}
	},
	toDownloadTaskType(mediaType: PlexMediaType) {
		switch (mediaType) {
			case PlexMediaType.Movie:
				return DownloadTaskType.Movie;
			case PlexMediaType.TvShow:
				return DownloadTaskType.TvShow;
			case PlexMediaType.Season:
				return DownloadTaskType.Season;
			case PlexMediaType.Episode:
				return DownloadTaskType.Episode;
			default:
				return DownloadTaskType.None;
		}
	},
};

export default Convert;

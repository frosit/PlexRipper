import type { DownloadMediaDTO, PlexMediaDTO, PlexMediaSlimDTO } from '@dto';

export function toDownloadMedia(mediaItem: PlexMediaDTO | PlexMediaSlimDTO): DownloadMediaDTO[] {
	return [
		{
			mediaIds: [mediaItem.id],
			type: mediaItem.type,
			plexServerId: mediaItem.plexServerId,
			plexLibraryId: mediaItem.plexLibraryId,
		},
	];
}

export function toFullThumbUrl({
	connectionUrl,
	mediaKey,
	MetaDataKey,
	token,
	width,
	height,
}: {
	connectionUrl: string;
	mediaKey: number;
	MetaDataKey: number;
	token: string;
	width?: number;
	height?: number;
}): string {
	return `${connectionUrl}/photo/:/transcode?url=/library/metadata/${mediaKey}/thumb/${MetaDataKey}&minSize=1&upscale=1&width=${width}&height=${height}&X-Plex-Token=${token}`;
}

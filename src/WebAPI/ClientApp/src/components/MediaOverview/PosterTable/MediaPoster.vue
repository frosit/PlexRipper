<template>
	<q-card flat>
		<q-card-section>
			<MediaPosterImage
				:media-item="mediaItem"
				:all-media-mode="mediaOverviewStore.allMediaMode"
				overlay
				actions
				@action="onAction" />
			<!--	Poster bar	-->
			<div
				v-if="qualities.length"
				class="media-poster-quality-bar">
				<q-chip
					v-for="(quality, j) in qualities"
					:key="j"
					:color="getQualityColor(quality.quality)"
					size="md">
					{{ quality.displayQuality }}
				</q-chip>
			</div>
		</q-card-section>
		<QLoadingOverlay :loading="loading" />
	</q-card>
</template>

<script setup lang="ts">
import Log from 'consola';
import { get } from '@vueuse/core';
import { type DownloadMediaDTO, type PlexMediaSlimDTO, PlexMediaType } from '@dto';
import { useMediaOverviewStore } from '#imports';

const mediaOverviewStore = useMediaOverviewStore();

const props = defineProps<{
	mediaItem: PlexMediaSlimDTO;
	index: number;

}>();

const emit = defineEmits<{
	(e: 'download', downloadMediaCommands: DownloadMediaDTO[]): void;
	(e: 'open-media-details', payload: PlexMediaSlimDTO): void;
}>();

const loading = ref(false);
const mediaType = computed(() => props.mediaItem?.type ?? PlexMediaType.Unknown);
const qualities = computed(() => props.mediaItem?.qualities ?? []);

const getQualityColor = (quality: string): string => {
	switch (quality) {
		case 'sd':
			return 'brown darken-4';
		case '480':
			return 'deep-orange';
		case '576':
			return 'yellow darken-1';
		case '720':
			return 'lime accent-4';
		case '1080':
			return 'blue accent-3';
		case '4k':
			return 'red darken-4';
		default:
			Log.debug('Missing quality color option', quality, props.mediaItem);
			return 'black';
	}
};

function onAction(event: 'download' | 'open-media-details') {
	const downloadCommand: DownloadMediaDTO = {
		type: get(mediaType),
		mediaIds: [props.mediaItem.id],
		plexLibraryId: props.mediaItem.plexLibraryId,
		plexServerId: props.mediaItem.plexServerId,
	};

	switch (event) {
		case 'download':
			emit('download', [downloadCommand]);
			break;
		case 'open-media-details':
			emit('open-media-details', props.mediaItem);
			break;
		default:
			Log.error('Unknown action event', event);
			break;
	}
}
</script>

<style lang="scss">
@import '@/assets/scss/_mixins.scss';

.media-poster-quality-bar {
  @extend .background-sm;

  padding: 0;
  text-align: center;
}
</style>

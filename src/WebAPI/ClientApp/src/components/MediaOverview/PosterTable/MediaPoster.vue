<template>
	<q-card
		flat
		class="media-poster highlight-border-box">
		<q-card-section class="media-poster--image">
			<QHover v-if="imageUrl">
				<template #default="{ hover }">
					<q-img
						loading="eager"
						:src="imageUrl"
						fit="fill"
						no-spinner
						class="media-poster--image"
						:alt="mediaItem.title">
						<template #default>
							<!--	Overlay	-->
							<div :class="['media-poster--overlay', hover ? 'on-hover' : '', 'white--text']">
								<MediaPosterImage
									:media-item="mediaItem"
									:all-media-mode="mediaOverviewStore.allMediaMode"
									@download="downloadMedia" />
							</div>
						</template>
						<template #error>
							<!--	Show fallback image	-->
							<MediaPosterImage
								fallback
								:media-item="mediaItem"
								:all-media-mode="mediaOverviewStore.allMediaMode" />
						</template>
					</q-img>
				</template>
			</QHover>
			<!--	Show fallback image	-->
			<MediaPosterImage
				v-else
				fallback
				:media-item="mediaItem"
				:all-media-mode="mediaOverviewStore.allMediaMode" />
		</q-card-section>
		<!--	Poster bar	-->
		<q-card-section
			v-if="qualities.length"
			class="media-poster--quality-bar">
			<q-chip
				v-for="(quality, j) in qualities"
				:key="j"
				:color="getQualityColor(quality.quality)"
				size="md">
				{{ quality.displayQuality }}
			</q-chip>
		</q-card-section>
		<QLoadingOverlay :loading="loading" />
		<!--	Highlight animation effect	-->
		<svg class="glow-container">
			<!-- suppress HtmlUnknownAttribute -->
			<rect
				pathLength="100"
				height="5"
				width="5"
				stroke-linecap="round"
				class="glow-blur" />
			<!-- suppress HtmlUnknownAttribute -->
			<rect
				pathLength="100"
				height="5"
				width="5"
				stroke-linecap="round"
				class="glow-line" />
		</svg>
	</q-card>
</template>

<script setup lang="ts">
import Log from 'consola';
import { get } from '@vueuse/core';
import { type DownloadMediaDTO, type PlexMediaSlimDTO, PlexMediaType } from '@dto';
import { toFullThumbUrl } from '@composables/conversion';
import { useMediaOverviewStore, useServerConnectionStore } from '#imports';

const connectionStore = useServerConnectionStore();
const mediaOverviewStore = useMediaOverviewStore();

const props = defineProps<{
	mediaItem: PlexMediaSlimDTO;
	index: number;
}>();

const emit = defineEmits<{
	(e: 'download', downloadMediaCommands: DownloadMediaDTO[]): void;
	(e: 'open-media-details', payload: PlexMediaSlimDTO): void;
}>();

const thumbWidth = ref(200);
const thumbHeight = ref(300);

const loading = ref(false);
const mediaType = computed(() => props.mediaItem?.type ?? PlexMediaType.Unknown);
const qualities = computed(() => props.mediaItem?.qualities ?? []);

const imageUrl = computed((): string => {
	if (!props.mediaItem?.hasThumb) {
		return '';
	}

	const connection = connectionStore.chooseServerConnection(props.mediaItem.plexServerId);
	if (!connection) {
		return '';
	}

	return toFullThumbUrl({
		connectionUrl: connection.url,
		mediaKey: props.mediaItem.key,
		MetaDataKey: props.mediaItem.metaDataKey,
		token: props.mediaItem.plexToken,
		width: get(thumbWidth),
		height: get(thumbHeight),
	});
});

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

function downloadMedia() {
	const downloadCommand: DownloadMediaDTO = {
		type: get(mediaType),
		mediaIds: [props.mediaItem.id],
		plexLibraryId: props.mediaItem.plexLibraryId,
		plexServerId: props.mediaItem.plexServerId,
	};

	emit('download', [downloadCommand]);
}
</script>

<style lang="scss">
@import '@/assets/scss/_mixins.scss';

.q-img__content > div {
  padding: 0;
}

.media-poster {
  @extend .background-sm;

  width: 200px;
  margin: 32px;

  &--image {
    height: 300px;
    padding: 0;
  }

  &--quality-bar {
    height: 40px;
    padding: 0;
    text-align: center;
  }

  &--overlay {
    @extend .background-xl;
    width: 100%;
    height: 100%;
    opacity: 0;
    margin: 0;
    transition: opacity 0.2s ease-in-out;

    &.on-hover {
      opacity: 0.8;

      .q-btn {
        opacity: 1;
      }
    }
  }

}
</style>
